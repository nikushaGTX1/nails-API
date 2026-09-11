using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsApi.Data;
using NailsApi.Models;
using NailsApi.Services;

namespace NailsApi.Controllers;

[ApiController]
public sealed class BookingsController(NailsDbContext db, AdminSessionService sessions) : ControllerBase
{
    [HttpPost("api/bookings")]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.StudioId) || string.IsNullOrWhiteSpace(request.Studio) ||
            string.IsNullOrWhiteSpace(request.ServiceId) || string.IsNullOrWhiteSpace(request.Service) ||
            request.Date == default || request.Time == default)
            return BadRequest(new { message = "Complete every booking field." });

        if (request.Name.Length > 100 || request.Phone.Length > 40 ||
            request.Studio.Length > 150 || request.Service.Length > 150)
            return BadRequest(new { message = "One or more booking fields are too long." });

        if (request.Date < DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest(new { message = "Choose a future booking date." });

        var booking = new BookingEntity
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            StudioId = request.StudioId.Trim(),
            Studio = request.Studio.Trim(),
            ServiceId = request.ServiceId.Trim(),
            Service = request.Service.Trim(),
            Date = request.Date,
            Time = request.Time
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(booking));
    }

    [HttpGet("api/admin/bookings")]
    public async Task<ActionResult<List<BookingDto>>> List(CancellationToken cancellationToken)
    {
        if (!sessions.IsValid(Request)) return Unauthorized();
        return Ok(await db.Bookings.AsNoTracking()
            .OrderByDescending(x => x.Date).ThenByDescending(x => x.Time)
            .Select(x => new BookingDto
            {
                Id = x.Id, Name = x.Name, Phone = x.Phone, StudioId = x.StudioId,
                Studio = x.Studio, ServiceId = x.ServiceId, Service = x.Service,
                Date = x.Date, Time = x.Time, Status = x.Status, CreatedAt = x.CreatedAt
            }).ToListAsync(cancellationToken));
    }

    [HttpPut("api/admin/bookings/{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(long id, UpdateBookingStatusRequest request, CancellationToken cancellationToken)
    {
        if (!sessions.IsValid(Request)) return Unauthorized();
        var status = request.Status?.Trim().ToLowerInvariant();
        if (status is not ("new" or "confirmed" or "completed" or "cancelled"))
            return BadRequest(new { message = "Invalid booking status." });
        var updated = await db.Bookings.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(b => b.Status, status), cancellationToken);
        return updated == 0 ? NotFound() : NoContent();
    }

    [HttpDelete("api/admin/bookings/{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        if (!sessions.IsValid(Request)) return Unauthorized();
        var deleted = await db.Bookings.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deleted == 0 ? NotFound() : NoContent();
    }

    private static BookingDto ToDto(BookingEntity x) => new()
    {
        Id = x.Id, Name = x.Name, Phone = x.Phone, StudioId = x.StudioId,
        Studio = x.Studio, ServiceId = x.ServiceId, Service = x.Service,
        Date = x.Date, Time = x.Time, Status = x.Status, CreatedAt = x.CreatedAt
    };
}
