using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NailsApi.Models;
using NailsApi.Services;

namespace NailsApi.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(this WebApplication app)
    {
        await using var scope=app.Services.CreateAsyncScope();
        var db=scope.ServiceProvider.GetRequiredService<NailsDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.AdminUsers.AnyAsync())
        {
            var password=app.Configuration["Admin:InitialPassword"];
            if (string.IsNullOrWhiteSpace(password)) throw new InvalidOperationException("Set Admin__InitialPassword before the first database startup.");
            var user=new AdminUser(); AdminCredentialService.SetPassword(user,password);
            db.AdminUsers.Add(user); await db.SaveChangesAsync();
        }

        if (!await db.SiteTexts.AnyAsync() && !await db.Services.AnyAsync())
        {
            var legacyPath=Path.Combine(app.Environment.ContentRootPath,"Data","site-content.json");
            if (File.Exists(legacyPath))
            {
                await using var stream=File.OpenRead(legacyPath);
                var legacy=await JsonSerializer.DeserializeAsync<SiteContentDto>(stream,new JsonSerializerOptions(JsonSerializerDefaults.Web));
                if (legacy is not null) await scope.ServiceProvider.GetRequiredService<IContentService>().PublishAsync(legacy);
            }
        }
    }
}
