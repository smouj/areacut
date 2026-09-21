namespace AreaCut.Core.Models;

/// <summary>
/// Transition between two adjacent clips.
/// </summary>
public enum TransitionKind
{
    Cut,
    Crossfade,
    FadeToBlack
}

public sealed record Transition
{
    public string Id { get; init; } = System.Guid.NewGuid().ToString("N");
    public TransitionKind Kind { get; init; } = TransitionKind.Cut;
    public double DurationSeconds { get; init; }
    public string FromClipId { get; init; } = string.Empty;
    public string ToClipId { get; init; } = string.Empty;
}
