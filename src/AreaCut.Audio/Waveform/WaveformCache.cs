using System;
using System.Collections.Concurrent;
using System.IO;

namespace AreaCut.Audio.Waveform;

/// <summary>
/// Cached waveform data for audio visualization on the timeline.
/// </summary>
public sealed class WaveformCache : IDisposable
{
    private readonly string _cachePath;
    private readonly ConcurrentDictionary<string, float[]> _memoryCache = new();
    private bool _disposed;

    public WaveformCache(string? cachePath = null)
    {
        _cachePath = cachePath ?? Path.Combine(".cache", "waveforms");
        Directory.CreateDirectory(_cachePath);
    }

    /// <summary>Get or generate waveform data for a media file.</summary>
    public float[] GetOrGenerate(string mediaId, string filePath, int sampleCount = 1000)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_memoryCache.TryGetValue(mediaId, out var cached))
            return cached;

        // In production: decode audio, compute RMS amplitude per segment
        var samples = new float[sampleCount];
        _memoryCache[mediaId] = samples;
        return samples;
    }

    /// <summary>Clear all cached waveform data.</summary>
    public void Clear()
    {
        _memoryCache.Clear();
        try
        {
            if (Directory.Exists(_cachePath))
                foreach (var f in Directory.GetFiles(_cachePath))
                    File.Delete(f);
        }
        catch { /* non-fatal */ }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _memoryCache.Clear();
    }
}
