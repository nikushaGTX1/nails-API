namespace NailsApi.Services;

public sealed class MediaStorageService(IWebHostEnvironment environment)
{
    // Browsers don't agree on the Content-Type they report for every format (older Safari sends
    // "" for .mov, some send generic "application/octet-stream"), so a file is accepted if EITHER
    // its reported content type OR its extension is recognized, not only the content type.
    private static readonly HashSet<string> AllowedContentTypes = [
        "image/jpeg","image/png","image/webp","image/gif","image/heic","image/heif","image/avif",
        "video/mp4","video/webm","video/quicktime","video/ogg","video/x-m4v","video/3gpp",
    ];
    private static readonly HashSet<string> AllowedExtensions = [
        ".jpg",".jpeg",".png",".webp",".gif",".heic",".heif",".avif",
        ".mp4",".webm",".mov",".m4v",".ogv",".ogg",".3gp",
    ];
    public string UploadDirectory { get; } = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath,"wwwroot"),"uploads");
    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0) throw new InvalidDataException("The file is empty.");
        if (file.Length > 100L*1024*1024) throw new InvalidDataException("Maximum file size is 100 MB.");
        var extension=Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType=(file.ContentType ?? "").ToLowerInvariant();
        if (!AllowedContentTypes.Contains(contentType) && !AllowedExtensions.Contains(extension))
            throw new InvalidDataException(
                $"Unsupported file type ({(contentType.Length > 0 ? contentType : extension)}). " +
                "Use JPG, PNG, WEBP, GIF or HEIC for images, or MP4, WEBM or MOV for video.");
        Directory.CreateDirectory(UploadDirectory);
        var name=$"{Guid.NewGuid():N}{extension}";
        await using var stream=File.Create(Path.Combine(UploadDirectory,name));
        await file.CopyToAsync(stream,cancellationToken);
        return $"/uploads/{name}";
    }
}
