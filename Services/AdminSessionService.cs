using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace NailsApi.Services;

public sealed class AdminSessionService
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _tokens = new();
    public string Create() { var token=Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); _tokens[token]=DateTimeOffset.UtcNow.AddHours(8); return token; }
    public bool IsValid(HttpRequest request)
    {
        var header=request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return false;
        return _tokens.TryGetValue(header[7..], out var expiry) && expiry > DateTimeOffset.UtcNow;
    }
    public void RevokeAll() => _tokens.Clear();
}
