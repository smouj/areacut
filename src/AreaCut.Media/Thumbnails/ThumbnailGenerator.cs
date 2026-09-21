using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AreaCut.Media.Thumbnails;

/// <summary>
/// Generates thumbnails for the timeline from video frames.
/// Uses Media Foundation to seek and decode specific frames.
/// Thumbnails are cached locally and generated progressively.
/// </summary>
public sealed class ThumbnailGenerator : IDisposable
{
    private readonly string _cachePath;
    private readonly ConcurrentDictionary<string, bool> _generating = new();
    private bool _disposed;

    public ThumbnailGenerator(string? cachePath = null)
    {
        _cachePath = cachePath ?? Path.Combine(".cache", "thumbnails");
        Directory.CreateDirectory(_cachePath);
    }

    /// <summary>Generate a thumbnail for a specific time position.</summary>
    public async Task<string?> GenerateAsync(string mediaId, string filePath, double timeSeconds,
        int width = 160, int height = 90, CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var cacheKey = $"{mediaId}_{timeSeconds:F2}_{width}x{height}.png";
        var cacheFile = Path.Combine(_cachePath, cacheKey);

        if (File.Exists(cacheFile))
            return cacheFile;

        // Avoid duplicate generation
        if (!_generating.TryAdd(cacheKey, true))
            return null;

        try
        {
            // In production: use IMFSourceReader to seek to timeSeconds,
            // decode the frame, scale to thumbnail size, and save as PNG
            // For now, return null to indicate no thumbnail available yet
            await Task.CompletedTask;
            return null;
        }
        finally
        {
            _generating.TryRemove(cacheKey, out _);
        }
    }

    /// <summary>Generate a set of thumbnails evenly spaced across the video duration.</summary>
    public async IAsyncEnumerable<(double timeSeconds, string? thumbnailPath)> GenerateRangeAsync(
        string mediaId, string filePath, double durationSeconds, int count,
        int width = 160, int height = 90,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var interval = durationSeconds / (count + 1);
        for (var i = 1; i <= count; i++)
        {
            ct.ThrowIfCancellationRequested();
            var time = interval * i;
            var path = await GenerateAsync(mediaId, filePath, time, width, height, ct);
            yield return (time, path);
        }
    }

    /// <summary>Clear the thumbnail cache.</summary>
    public void ClearCache()
    {
        try
        {
            if (Directory.Exists(_cachePath))
                foreach (var file in Directory.GetFiles(_cachePath, "*.png"))
                    File.Delete(file);
        }
        catch
        {
            // Non-fatal: cache cleanup failure
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}
