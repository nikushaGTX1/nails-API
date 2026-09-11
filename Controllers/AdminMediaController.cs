using Microsoft.AspNetCore.Mvc;
using NailsApi.Services;

namespace NailsApi.Controllers;

[ApiController, Route("api/admin/media")]
public sealed class AdminMediaController(MediaStorageService mediaStorage, AdminSessionService sessions) : ControllerBase
{
    [HttpPost, RequestSizeLimit(105_000_000)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (!sessions.IsValid(Request)) return Unauthorized();
        try { return Ok(new { url=await mediaStorage.SaveAsync(file,cancellationToken) }); }
        catch (InvalidDataException exception) { return BadRequest(new { message=exception.Message }); }
    }
}
