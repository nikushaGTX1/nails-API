namespace NailsApi.Services;

public sealed class MediaStorageService(IWebHostEnvironment environment)
{
    private static readonly HashSet<string> AllowedTypes = ["image/jpeg","image/png","image/webp","image/gif","video/mp4","video/webm"];
    public string UploadDirectory { get; } = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath,"wwwroot"),"uploads");
    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0) throw new InvalidDataException("The file is empty.");
        if (file.Length > 100L*1024*1024) throw new InvalidDataException("Maximum file size is 100 MB.");
        if (!AllowedTypes.Contains(file.ContentType.ToLowerInvariant())) throw new InvalidDataException("Unsupported file type.");
        Directory.CreateDirectory(UploadDirectory);
        var extension=Path.GetExtension(file.FileName).ToLowerInvariant();
        var name=$"{Guid.NewGuid():N}{extension}";
        await using var stream=File.Create(Path.Combine(UploadDirectory,name));
        await file.CopyToAsync(stream,cancellationToken);
        return $"/uploads/{name}";
    }
}
