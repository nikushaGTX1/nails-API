using Microsoft.EntityFrameworkCore;
using NailsApi.Data;
using NailsApi.Models;

namespace NailsApi.Services;

public sealed class ContentService(NailsDbContext db) : IContentService
{
    public async Task<SiteContentDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var texts = await db.SiteTexts.AsNoTracking().ToListAsync(cancellationToken);
        return new SiteContentDto
        {
            Translations = texts.GroupBy(x => x.Language).ToDictionary(g => g.Key, g => g.ToDictionary(x => x.Key, x => x.Value)),
            Media = await db.SiteMedia.AsNoTracking().ToDictionaryAsync(x => x.Key, x => x.Url, cancellationToken),
            Settings = await db.SiteSettings.AsNoTracking().ToDictionaryAsync(x => x.Key, x => x.Value, cancellationToken),
            Services = await db.Services.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => new ServiceDto { Id=x.Id, Name=x.Name, Description=x.Description, Price=x.Price }).ToListAsync(cancellationToken),
            Gallery = await db.GalleryItems.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => new GalleryItemDto { Id=x.Id, Title=x.Title, Category=x.Category, ImageUrl=x.ImageUrl, Position=x.Position }).ToListAsync(cancellationToken),
            Locations = await db.Locations.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => new LocationDto { Id=x.Id, Area=x.Area, Address=x.Address, Phone=x.Phone, Coordinates=x.Coordinates }).ToListAsync(cancellationToken),
            UpdatedAt = (await db.SiteStates.AsNoTracking().SingleAsync(x => x.Id == 1, cancellationToken)).UpdatedAt
        };
    }

    public async Task<SiteContentDto> PublishAsync(SiteContentDto content, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.SiteTexts.ExecuteDeleteAsync(cancellationToken);
        await db.SiteMedia.ExecuteDeleteAsync(cancellationToken);
        await db.SiteSettings.ExecuteDeleteAsync(cancellationToken);
        await db.Services.ExecuteDeleteAsync(cancellationToken);
        await db.GalleryItems.ExecuteDeleteAsync(cancellationToken);
        await db.Locations.ExecuteDeleteAsync(cancellationToken);

        db.SiteTexts.AddRange(content.Translations.SelectMany(language => language.Value.Select(item => new SiteText { Language=language.Key, Key=item.Key, Value=item.Value ?? "" })));
        db.SiteMedia.AddRange(content.Media.Select(x => new SiteMedia { Key=x.Key, Url=x.Value ?? "" }));
        db.SiteSettings.AddRange(content.Settings.Select(x => new SiteSetting { Key=x.Key, Value=x.Value ?? "" }));
        db.Services.AddRange(content.Services.Select((x,i) => new ServiceEntity { Id=x.Id, Name=x.Name, Description=x.Description, Price=x.Price, SortOrder=i }));
        db.GalleryItems.AddRange(content.Gallery.Select((x,i) => new GalleryItemEntity { Id=x.Id, Title=x.Title, Category=x.Category, ImageUrl=x.ImageUrl, Position=x.Position, SortOrder=i }));
        db.Locations.AddRange(content.Locations.Select((x,i) => new LocationEntity { Id=x.Id, Area=x.Area, Address=x.Address, Phone=x.Phone, Coordinates=x.Coordinates, SortOrder=i }));
        var state = await db.SiteStates.SingleAsync(x => x.Id == 1, cancellationToken);
        content.UpdatedAt = state.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return content;
    }
}
