using System;
using System.IO;
using AreaCut.Core.Models;
using AreaCut.Media.MediaFoundation;

namespace AreaCut.Media.Metadata;

/// <summary>
/// High-level media probing: takes a file path, returns metadata.
/// Uses Media Foundation for format detection.
/// Returns understandable errors for unsupported formats.
/// </summary>
public static class MediaProber
{
    /// <summary>Probe a media file for metadata.</summary>
    public static MediaProbeResult Probe(string filePath)
    {
        if (!File.Exists(filePath))
            return new MediaProbeResult { FilePath = filePath, Error = $"File not found: {filePath}" };

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (!IsSupportedExtension(extension))
            return new MediaProbeResult { FilePath = filePath, Error = $"Unsupported format: {extension}" };

        try
        {
            using var reader = new MediaFoundationReader(filePath);
            reader.Open();
            return reader.Probe();
        }
        catch (Exception ex)
        {
            return new MediaProbeResult
            {
                FilePath = filePath,
                Error = $"Cannot open video: {ex.Message}"
            };
        }
    }

    /// <summary>Import a media file and create a MediaReference.</summary>
    public static (MediaReference? reference, string? error) ImportMedia(string filePath)
    {
        var probeResult = Probe(filePath);
        if (!probeResult.IsSuccess)
            return (null, probeResult.Error);

        var mediaRef = probeResult.ToMediaReference();
        return (mediaRef, null);
    }

    private static bool IsSupportedExtension(string ext) => ext switch
    {
        ".mp4" or ".m4v" or ".mov" or ".avi" or ".wmv" or ".asf" => true,
        ".png" or ".jpg" or ".jpeg" or ".webp" or ".bmp" => true,
        ".mp3" or ".wav" or ".m4a" or ".aac" or ".wma" => true,
        _ => false
    };
}
