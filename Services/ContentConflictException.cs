namespace NailsApi.Services;

/// <summary>
/// Thrown by ContentService.PublishAsync when the caller's UpdatedAt baseline doesn't match what's
/// currently live — someone else (another tab, device, or a left-open session) published in between.
/// Publishing anyway would silently discard their changes, so the caller must confirm with force=true.
/// </summary>
public sealed class ContentConflictException(DateTimeOffset latestUpdatedAt) : Exception(
    "Someone else published changes since this was loaded. Reload to see the latest version, or publish again with force to overwrite it.")
{
    public DateTimeOffset LatestUpdatedAt { get; } = latestUpdatedAt;
}
