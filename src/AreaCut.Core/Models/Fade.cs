using AreaCut.Core.Time;

namespace AreaCut.Core.Models;

/// <summary>
/// Fade in/out effect for a clip (video or audio).
/// </summary>
public sealed record Fade
{
    public TimeStamp? FadeInDuration { get; init; }
    public TimeStamp? FadeOutDuration { get; init; }

    public static Fade None => new();
}
