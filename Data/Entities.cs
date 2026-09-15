namespace NailsApi.Data;

public sealed class SiteText
{
    public string Language { get; set; } = "en";
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}

public sealed class SiteSetting { public string Key { get; set; } = ""; public string Value { get; set; } = ""; }
public sealed class SiteMedia { public string Key { get; set; } = ""; public string Url { get; set; } = ""; }

public sealed class ServiceEntity
{
    public string Id { get; set; } = "";
    public Dictionary<string, string> Name { get; set; } = [];
    public Dictionary<string, string> Description { get; set; } = [];
    public decimal Price { get; set; }
    public int SortOrder { get; set; }
    /// <summary>Empty = shown in the flat homepage service list. Set = grouped under a CategoryEntity's accordion page.</summary>
    public string CategoryId { get; set; } = "";
    /// <summary>Top-level accordion group within a category, e.g. "Маникюр" / "Пилочный".</summary>
    public Dictionary<string, string> GroupLabel { get; set; } = [];
    /// <summary>Expandable row label within a group, e.g. "С покрытием".</summary>
    public Dictionary<string, string> SubgroupLabel { get; set; } = [];
}

public sealed class CategoryEntity
{
    public string Id { get; set; } = "";
    public Dictionary<string, string> Name { get; set; } = [];
    public string ImageUrl { get; set; } = "";
    public int SortOrder { get; set; }
}

public sealed class GalleryItemEntity
{
    public string Id { get; set; } = "";
    public Dictionary<string, string> Title { get; set; } = [];
    public string Category { get; set; } = "manicure";
    public string ImageUrl { get; set; } = "";
    public string Position { get; set; } = "50% 50%";
    public int SortOrder { get; set; }
}

public sealed class LocationEntity
{
    public string Id { get; set; } = "";
    public string Area { get; set; } = "";
    public Dictionary<string, string> Address { get; set; } = [];
    public string Phone { get; set; } = "";
    public string Coordinates { get; set; } = "";
    public int SortOrder { get; set; }
}

public sealed class AdminUser
{
    public int Id { get; set; }
    public string PasswordHash { get; set; } = "";
    public string PasswordSalt { get; set; } = "";
    public int Iterations { get; set; } = 100_000;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class SiteState
{
    public int Id { get; set; } = 1;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class BookingEntity
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
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
