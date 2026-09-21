using System;
using AreaCut.Core.Models;

namespace AreaCut.Media.Metadata;

/// <summary>
/// Result of probing a media file for metadata.
/// Contains all information needed for import without decoding frames.
/// </summary>
public sealed class MediaProbeResult
{
    public string FilePath { get; set; } = "";
    public MediaKind Kind { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public double? Fps { get; set; }
    public string? VideoCodec { get; set; }
    public string? AudioCodec { get; set; }
    public int? AudioChannels { get; set; }
    public int? AudioSampleRate { get; set; }
    public long? Bitrate { get; set; }
    public string? Orientation { get; set; }

    /// <summary>Human-readable error if probing failed.</summary>
    public string? Error { get; set; }
    public bool IsSuccess => string.IsNullOrEmpty(Error);

    /// <summary>Create a MediaReference from this probe result.</summary>
    public MediaReference ToMediaReference()
    {
        var mr = new MediaReference(FilePath, Kind)
        {
            Duration = Duration,
            Width = Width,
            Height = Height,
            Fps = Fps,
            Codec = VideoCodec,
            AudioCodec = AudioCodec,
            AudioChannels = AudioChannels,
            AudioSampleRate = AudioSampleRate,
            Bitrate = Bitrate
        };
        return mr;
    }
}
