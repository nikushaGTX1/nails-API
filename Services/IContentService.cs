using NailsApi.Models;

namespace NailsApi.Services;

public interface IContentService
{
    Task<SiteContentDto> GetAsync(CancellationToken cancellationToken = default);
    /// <summary>Throws ContentConflictException when content.UpdatedAt doesn't match the live version and force is false.</summary>
    Task<SiteContentDto> PublishAsync(SiteContentDto content, bool force = false, CancellationToken cancellationToken = default);
}
