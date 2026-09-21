using System;
using AreaCut.Core.Time;

namespace AreaCut.Core.Models;

/// <summary>
/// Text/caption overlay clip on the timeline.
/// </summary>
public sealed class TextClip
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public string Content { get; set; } = string.Empty;
    public string TrackId { get; set; }

    public TimeStamp TimelineStart { get; set; }
    public TimeStamp TimelineEnd { get; set; }
    public TimeStamp Duration => TimelineEnd.Subtract(TimelineStart);

    // Font properties
    public string FontFamily { get; set; } = "Segoe UI";
    public double FontSize { get; set; } = 48.0;
    public string FontWeight { get; set; } = "Bold";
    public string Alignment { get; set; } = "Center";

    // Color/opacity
    public string Foreground { get; set; } = "#FFFFFF";
    public string Background { get; set; } = "transparent";
    public double Opacity { get; set; } = 1.0;

    // Transform
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double Scale { get; set; } = 1.0;
    public double RotationDegrees { get; set; }

    // Effects
    public bool HasShadow { get; set; }
    public bool HasOutline { get; set; }
    public string? OutlineColor { get; set; }
    public double OutlineThickness { get; set; }

    // Fade
    public TimeStamp? FadeInDuration { get; set; }
    public TimeStamp? FadeOutDuration { get; set; }

    public TextClip(string trackId)
    {
        TrackId = trackId ?? throw new ArgumentNullException(nameof(trackId));
    }
}
