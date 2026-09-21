using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AreaCut.Media.Waveforms;

/// <summary>
/// Generates audio waveform data for timeline visualization.
/// Caches waveform data locally.
/// </summary>
public sealed class WaveformGenerator : IDisposable
{
    private readonly string _cachePath;
    private bool _disposed;

    public WaveformGenerator(string? cachePath = null)
    {
        _cachePath = cachePath ?? Path.Combine(".cache", "waveforms");
        Directory.CreateDirectory(_cachePath);
    }

    /// <summary>Generate waveform samples from an audio/video file.</summary>
    /// <param name="filePath">Path to the media file.</param>
    /// <param name="sampleCount">Number of amplitude samples to generate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Array of normalized amplitude values (0-1).</returns>
    public async Task<float[]> GenerateAsync(string filePath, int sampleCount = 1000, CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var cacheKey = $"{Path.GetFileName(filePath)}_{sampleCount}.wave";
        var cacheFile = Path.Combine(_cachePath, cacheKey);

        // Check cache
        if (File.Exists(cacheFile))
        {
            var cached = await File.ReadAllBytesAsync(cacheFile, ct);
            var samples = new float[sampleCount];
            Buffer.BlockCopy(cached, 0, samples, 0, Math.Min(cached.Length, sampleCount * 4));
            return samples;
        }

        // In production: use Media Foundation Source Reader to extract audio samples,
        // downsample to the requested count, and compute RMS amplitude per segment
        // For now, return an empty waveform
        return new float[sampleCount];
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}
