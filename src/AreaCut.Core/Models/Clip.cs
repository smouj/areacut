using System;
using AreaCut.Core.Time;

namespace AreaCut.Core.Models;

/// <summary>
/// Non-destructive clip: references source media with in/out points.
/// Never copies or modifies the original file.
/// </summary>
public sealed class Clip
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string SourceMediaId { get; set; }

    /// <summary>In point in the source media.</summary>
    public TimeStamp SourceIn { get; set; }

    /// <summary>Out point in the source media.</summary>
    public TimeStamp SourceOut { get; set; }

    /// <summary>Start position on the timeline.</summary>
    public TimeStamp TimelineStart { get; set; }

    public string TrackId { get; set; }
    public Transform Transform { get; set; } = Transform.Identity;
    public Crop Crop { get; set; } = Crop.None;
    public double Opacity { get; set; } = 1.0;
    public double PlaybackRate { get; set; } = 1.0;
    public double Volume { get; set; } = 1.0;

    /// <summary>Duration of source media used.</summary>
    public TimeStamp SourceDuration => SourceOut.Subtract(SourceIn);

    /// <summary>Duration on the timeline (affected by playback rate).</summary>
    public TimeStamp TimelineDuration => SourceDuration.Multiply(1.0 / PlaybackRate);

    /// <summary>End position on the timeline.</summary>
    public TimeStamp TimelineEnd => TimelineStart.Add(TimelineDuration);

    public Clip(string sourceMediaId, string trackId)
    {
        SourceMediaId = sourceMediaId ?? throw new ArgumentNullException(nameof(sourceMediaId));
        TrackId = trackId ?? throw new ArgumentNullException(nameof(trackId));
    }
}
