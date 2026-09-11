using Microsoft.AspNetCore.Mvc;
using NailsApi.Models;
using NailsApi.Services;

namespace NailsApi.Controllers;

[ApiController, Route("api/admin/content")]
public sealed class AdminContentController(IContentService contentService, AdminSessionService sessions) : ControllerBase
{
    [HttpPut]
    [RequestSizeLimit(110_000_000)]
    public async Task<ActionResult<SiteContentDto>> Publish(SiteContentDto content, CancellationToken cancellationToken)
    {
        if (!sessions.IsValid(Request)) return Unauthorized();
        return Ok(await contentService.PublishAsync(content,cancellationToken));
    }
}
