using Microsoft.AspNetCore.Mvc;
using NailsApi.Models;
using NailsApi.Services;

namespace NailsApi.Controllers;

[ApiController, Route("api/admin")]
public sealed class AdminAuthController(AdminCredentialService credentials, AdminSessionService sessions) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        if (!await credentials.VerifyAsync(request.Password ?? "",cancellationToken)) return Unauthorized();
        return Ok(new { token=sessions.Create(), expiresIn=28800 });
    }

    [HttpGet("session")]
    public IActionResult Session() => sessions.IsValid(Request) ? Ok(new { valid=true }) : Unauthorized();

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        if (!sessions.IsValid(Request)) return Unauthorized();
        if (!await credentials.VerifyAsync(request.CurrentPassword ?? "",cancellationToken)) return BadRequest(new { message="The current password is incorrect." });
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length<10) return BadRequest(new { message="The new password must be at least 10 characters." });
        await credentials.ChangeAsync(request.NewPassword,cancellationToken);
        sessions.RevokeAll();
        return Ok(new { token=sessions.Create(), message="Password changed successfully." });
    }
}
