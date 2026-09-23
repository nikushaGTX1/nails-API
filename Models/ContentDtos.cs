namespace NailsApi.Models;

public sealed class SiteContentDto
{
    public Dictionary<string, Dictionary<string, string>> Translations { get; set; } = [];
    public Dictionary<string, string> Media { get; set; } = [];
    public Dictionary<string, string> Settings { get; set; } = [];
    public List<ServiceDto> Services { get; set; } = [];
    public List<GalleryItemDto> Gallery { get; set; } = [];
    public List<LocationDto> Locations { get; set; } = [];
    public List<CategoryDto> Categories { get; set; } = [];
    /// <summary>
    /// The UpdatedAt this client last loaded. Left at its default (unset) means "no baseline" —
    /// PublishAsync then skips the conflict check, matching a fresh client that never loaded content.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class ServiceDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public Dictionary<string, string> Name { get; set; } = [];
    public Dictionary<string, string> Description { get; set; } = [];
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = "";
    public string CategoryId { get; set; } = "";
    public Dictionary<string, string> GroupLabel { get; set; } = [];
    public Dictionary<string, string> SubgroupLabel { get; set; } = [];
}

public sealed class CategoryDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public Dictionary<string, string> Name { get; set; } = [];
    public string ImageUrl { get; set; } = "";
}

public sealed class GalleryItemDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public Dictionary<string, string> Title { get; set; } = [];
    public string Category { get; set; } = "manicure";
    public string ImageUrl { get; set; } = "";
    public string Position { get; set; } = "50% 50%";
}

public sealed class LocationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Area { get; set; } = "";
    public Dictionary<string, string> Address { get; set; } = [];
    public string Phone { get; set; } = "";
    public string Coordinates { get; set; } = "";
}

public sealed record LoginRequest(string? Password);
public sealed record ChangePasswordRequest(string? CurrentPassword, string? NewPassword);

public sealed class BookingDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string StudioId { get; set; } = "";
    public string Studio { get; set; } = "";
    public string ServiceId { get; set; } = "";
    public string Service { get; set; } = "";
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Status { get; set; } = "new";
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class CreateBookingRequest
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string StudioId { get; set; } = "";
    public string Studio { get; set; } = "";
    public string ServiceId { get; set; } = "";
    public string Service { get; set; } = "";
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
}

public sealed record UpdateBookingStatusRequest(string? Status);
