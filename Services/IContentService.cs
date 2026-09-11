using NailsApi.Models;

namespace NailsApi.Services;

public interface IContentService
{
    Task<SiteContentDto> GetAsync(CancellationToken cancellationToken = default);
    Task<SiteContentDto> PublishAsync(SiteContentDto content, CancellationToken cancellationToken = default);
}
