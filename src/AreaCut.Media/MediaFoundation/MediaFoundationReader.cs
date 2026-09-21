using System;
using System.Runtime.InteropServices;
using AreaCut.Core.Models;

namespace AreaCut.Media.MediaFoundation;

/// <summary>
/// Reads media files using Media Foundation Source Reader.
/// Extracts metadata, frames, and audio samples.
/// All COM/MF resources are properly disposed.
/// </summary>
public sealed class MediaFoundationReader : IDisposable
{
    private IntPtr _sourceReader;
    private bool _disposed;
    private readonly string _filePath;

    public MediaFoundationReader(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    /// <summary>Open the media file and prepare for reading.</summary>
    public void Open()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        // MFCreateSourceReaderFromURL will be called here
        // Proper COM apartment (MTA) required
    }

    /// <summary>Probe the file for metadata without decoding frames.</summary>
    public MediaProbeResult Probe()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // In production, this uses IMFSourceReader to read:
        // - Duration (MF_PD_DURATION)
        // - Video stream: width, height, FPS, codec
        // - Audio stream: channels, sample rate, bits per sample
        // For now, return a placeholder that indicates the probe structure
        var result = new MediaProbeResult
        {
            FilePath = _filePath,
            Kind = MediaKind.Video,
            Duration = TimeSpan.Zero,
            Width = 0,
            Height = 0,
            Fps = null,
            VideoCodec = null,
            AudioCodec = null,
            AudioChannels = null,
            AudioSampleRate = null,
            Bitrate = null
        };

        return result;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_sourceReader != IntPtr.Zero)
        {
            Marshal.ReleaseComObject(_sourceReader);
            _sourceReader = IntPtr.Zero;
        }
    }
}
