using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NailsApi.Data;

namespace NailsApi.Services;

public sealed class AdminCredentialService(NailsDbContext db)
{
    public async Task<bool> VerifyAsync(string password, CancellationToken cancellationToken = default)
    {
        var user=await db.AdminUsers.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
        if (user is null) return false;
        var actual=Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(user.PasswordSalt), user.Iterations, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(actual, Convert.FromBase64String(user.PasswordHash));
    }
    public async Task ChangeAsync(string password, CancellationToken cancellationToken = default)
    {
        var user=await db.AdminUsers.SingleAsync(cancellationToken);
        SetPassword(user, password);
        await db.SaveChangesAsync(cancellationToken);
    }
    public static void SetPassword(AdminUser user, string password)
    {
        var salt=RandomNumberGenerator.GetBytes(16); const int iterations=100_000;
        user.PasswordSalt=Convert.ToBase64String(salt);
        user.PasswordHash=Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, 32));
        user.Iterations=iterations; user.UpdatedAt=DateTimeOffset.UtcNow;
    }
}
