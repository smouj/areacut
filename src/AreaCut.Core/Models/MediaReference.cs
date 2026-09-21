using System;

namespace AreaCut.Core.Models;

/// <summary>
/// Reference to a source media file on disk. The original file is never modified.
/// </summary>
public sealed class MediaReference
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public string FilePath { get; set; }
    public string? FileName { get; set; }
    public MediaKind Kind { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public double? Fps { get; set; }
    public string? Codec { get; set; }
    public string? AudioCodec { get; set; }
    public int? AudioChannels { get; set; }
    public int? AudioSampleRate { get; set; }
    public long? Bitrate { get; set; }
    public bool IsOffline { get; set; }

    public MediaReference(string filePath, MediaKind kind)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        Kind = kind;
        FileName = System.IO.Path.GetFileName(filePath);
    }

    /// <summary>Check if the file still exists on disk.</summary>
    public bool FileExists => System.IO.File.Exists(FilePath);
}

public enum MediaKind
{
    Video,
    Audio,
    Image
}
