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
            Services = await db.Services.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => new ServiceDto { Id=x.Id, Name=x.Name, Description=x.Description, Price=x.Price, ImageUrl=x.ImageUrl, CategoryId=x.CategoryId, GroupLabel=x.GroupLabel, SubgroupLabel=x.SubgroupLabel }).ToListAsync(cancellationToken),
            Gallery = await db.GalleryItems.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => new GalleryItemDto { Id=x.Id, Title=x.Title, Category=x.Category, ImageUrl=x.ImageUrl, Position=x.Position }).ToListAsync(cancellationToken),
            Locations = await db.Locations.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => new LocationDto { Id=x.Id, Area=x.Area, Address=x.Address, Phone=x.Phone, Coordinates=x.Coordinates }).ToListAsync(cancellationToken),
            Categories = await db.Categories.AsNoTracking().OrderBy(x => x.SortOrder).Select(x => new CategoryDto { Id=x.Id, Name=x.Name, ImageUrl=x.ImageUrl }).ToListAsync(cancellationToken),
            UpdatedAt = (await db.SiteStates.AsNoTracking().SingleAsync(x => x.Id == 1, cancellationToken)).UpdatedAt
        };
    }

    public async Task<SiteContentDto> PublishAsync(SiteContentDto content, bool force = false, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        // FOR UPDATE takes a row lock held until commit/rollback, so a second concurrent Publish
        // blocks here until this one finishes, then sees the fresh UpdatedAt — closing the race a
        // plain read-then-write would leave open between two saves landing at nearly the same time.
        var state = await db.SiteStates
            .FromSqlRaw("SELECT * FROM \"SiteStates\" WHERE \"Id\" = 1 FOR UPDATE")
            .SingleAsync(cancellationToken);
        // UpdatedAt default(DateTimeOffset) means "no baseline sent" — nothing to conflict against.
        if (!force && content.UpdatedAt != default && content.UpdatedAt != state.UpdatedAt)
            throw new ContentConflictException(state.UpdatedAt);

        // SiteTexts/SiteMedia/SiteSettings are plain key -> value stores with no "remove" button
        // anywhere in the admin UI — every key that has ever existed should simply keep whatever
        // value it last had. Upserting (instead of delete-then-reinsert-what-this-payload-knows-
        // about) means a browser tab running an OLDER build — one from before a key like a newly
        // added translation string existed — can no longer wipe that key out just by publishing;
        // it only touches the keys it actually sent.
        var existingTexts = await db.SiteTexts.ToDictionaryAsync(x => (x.Language, x.Key), cancellationToken);
        foreach (var language in content.Translations)
            foreach (var item in language.Value)
            {
                var value = item.Value ?? "";
                if (existingTexts.TryGetValue((language.Key, item.Key), out var row)) row.Value = value;
                else db.SiteTexts.Add(new SiteText { Language = language.Key, Key = item.Key, Value = value });
            }

        var existingMedia = await db.SiteMedia.ToDictionaryAsync(x => x.Key, cancellationToken);
        foreach (var (key, url) in content.Media)
        {
            if (existingMedia.TryGetValue(key, out var row)) row.Url = url ?? "";
            else db.SiteMedia.Add(new SiteMedia { Key = key, Url = url ?? "" });
        }

        var existingSettings = await db.SiteSettings.ToDictionaryAsync(x => x.Key, cancellationToken);
        foreach (var (key, value) in content.Settings)
        {
            if (existingSettings.TryGetValue(key, out var row)) row.Value = value ?? "";
            else db.SiteSettings.Add(new SiteSetting { Key = key, Value = value ?? "" });
        }

        // Services/Gallery/Locations/Categories are ordered lists with an explicit Remove button in
        // admin, so the payload's list IS the intended full list — replacing them wholesale is correct.
        await db.Services.ExecuteDeleteAsync(cancellationToken);
        await db.GalleryItems.ExecuteDeleteAsync(cancellationToken);
        await db.Locations.ExecuteDeleteAsync(cancellationToken);
        await db.Categories.ExecuteDeleteAsync(cancellationToken);

        db.Services.AddRange(content.Services.Select((x,i) => new ServiceEntity { Id=x.Id, Name=x.Name, Description=x.Description, Price=x.Price, ImageUrl=x.ImageUrl, SortOrder=i, CategoryId=x.CategoryId, GroupLabel=x.GroupLabel, SubgroupLabel=x.SubgroupLabel }));
        db.GalleryItems.AddRange(content.Gallery.Select((x,i) => new GalleryItemEntity { Id=x.Id, Title=x.Title, Category=x.Category, ImageUrl=x.ImageUrl, Position=x.Position, SortOrder=i }));
        db.Locations.AddRange(content.Locations.Select((x,i) => new LocationEntity { Id=x.Id, Area=x.Area, Address=x.Address, Phone=x.Phone, Coordinates=x.Coordinates, SortOrder=i }));
        db.Categories.AddRange(content.Categories.Select((x,i) => new CategoryEntity { Id=x.Id, Name=x.Name, ImageUrl=x.ImageUrl, SortOrder=i }));
        content.UpdatedAt = state.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return content;
    }
}
