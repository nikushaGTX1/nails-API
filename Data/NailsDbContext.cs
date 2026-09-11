using Microsoft.EntityFrameworkCore;

namespace NailsApi.Data;

public sealed class NailsDbContext(DbContextOptions<NailsDbContext> options) : DbContext(options)
{
    public DbSet<SiteText> SiteTexts => Set<SiteText>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<SiteMedia> SiteMedia => Set<SiteMedia>();
    public DbSet<ServiceEntity> Services => Set<ServiceEntity>();
    public DbSet<GalleryItemEntity> GalleryItems => Set<GalleryItemEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<SiteState> SiteStates => Set<SiteState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SiteText>().HasKey(x => new { x.Language, x.Key });
        modelBuilder.Entity<SiteSetting>().HasKey(x => x.Key);
        modelBuilder.Entity<SiteMedia>().HasKey(x => x.Key);
        modelBuilder.Entity<ServiceEntity>().Property(x => x.Name).HasColumnType("jsonb");
        modelBuilder.Entity<ServiceEntity>().Property(x => x.Description).HasColumnType("jsonb");
        modelBuilder.Entity<GalleryItemEntity>().Property(x => x.Title).HasColumnType("jsonb");
        modelBuilder.Entity<LocationEntity>().Property(x => x.Address).HasColumnType("jsonb");
        modelBuilder.Entity<ServiceEntity>().Property(x => x.Price).HasPrecision(10, 2);
        modelBuilder.Entity<SiteState>().HasData(new SiteState { Id = 1 });
    }
}
