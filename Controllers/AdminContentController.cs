using Microsoft.AspNetCore.Mvc;
using NailsApi.Models;
using NailsApi.Services;

namespace NailsApi.Controllers;

[ApiController, Route("api/admin/content")]
public sealed class AdminContentController(IContentService contentService, AdminSessionService sessions) : ControllerBase
{
    [HttpPut]
    [RequestSizeLimit(110_000_000)]
    public async Task<ActionResult<SiteContentDto>> Publish(SiteContentDto content, [FromQuery] bool force, CancellationToken cancellationToken)
    {
        if (!sessions.IsValid(Request)) return Unauthorized();
        try
        {
            return Ok(await contentService.PublishAsync(content, force, cancellationToken));
        }
        catch (ContentConflictException ex)
        {
            return Conflict(new { message = ex.Message, latestUpdatedAt = ex.LatestUpdatedAt });
        }
    }
}
