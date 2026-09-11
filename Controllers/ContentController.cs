using Microsoft.AspNetCore.Mvc;
using NailsApi.Models;
using NailsApi.Services;

namespace NailsApi.Controllers;

[ApiController, Route("api/content")]
public sealed class ContentController(IContentService contentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<SiteContentDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SiteContentDto>> Get(CancellationToken cancellationToken) => Ok(await contentService.GetAsync(cancellationToken));
}
