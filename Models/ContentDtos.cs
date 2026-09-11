namespace NailsApi.Models;

public sealed class SiteContentDto
{
    public Dictionary<string, Dictionary<string, string>> Translations { get; set; } = [];
    public Dictionary<string, string> Media { get; set; } = [];
    public Dictionary<string, string> Settings { get; set; } = [];
    public List<ServiceDto> Services { get; set; } = [];
    public List<GalleryItemDto> Gallery { get; set; } = [];
    public List<LocationDto> Locations { get; set; } = [];
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class ServiceDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public Dictionary<string, string> Name { get; set; } = [];
    public Dictionary<string, string> Description { get; set; } = [];
    public decimal Price { get; set; }
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
