using System.Collections.Generic;

namespace AreaCut.Core.Models;

public enum TrackKind
{
    Video,
    Audio,
    Text,
    Caption
}

/// <summary>
/// A track on the timeline containing clips of the same kind.
/// </summary>
public sealed class Track
{
    public string Id { get; } = System.Guid.NewGuid().ToString("N");
    public string Name { get; set; }
    public TrackKind Kind { get; }
    public bool IsMuted { get; set; }
    public bool IsLocked { get; set; }
    public bool IsVisible { get; set; } = true;
    public int ZOrder { get; set; }

    public Track(TrackKind kind, string? name = null)
    {
        Kind = kind;
        Name = name ?? kind.ToString();
    }
}
