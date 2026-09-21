using System;
using System.Threading;
using AreaCut.Core.Time;
using AreaCut.Rendering.Preview;

namespace AreaCut.Audio.Playback;

/// <summary>
/// Audio playback engine using WASAPI for low-latency output.
/// Synchronized with PreviewClock for A/V sync.
/// </summary>
public sealed class AudioPlaybackEngine : IDisposable
{
    private bool _disposed;
    private bool _isPlaying;
    private double _volume = 1.0;

    public bool IsPlaying => _isPlaying;
    public double Volume => _volume;

    /// <summary>Start audio playback synchronized with the given clock.</summary>
    public void Play(PreviewClock clock)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _isPlaying = true;
        // In production: initialize WASAPI client, start rendering endpoint
    }

    /// <summary>Pause audio playback.</summary>
    public void Pause()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _isPlaying = false;
    }

    /// <summary>Seek audio to match video position.</summary>
    public void Seek(TimeStamp position)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        // Seek audio buffers to match video position
    }

    /// <summary>Set master volume (0-1).</summary>
    public void SetVolume(double volume)
    {
        _volume = Math.Clamp(volume, 0, 1);
    }

    /// <summary>Mute/unmute.</summary>
    public void Mute(bool muted)
    {
        // In production: set WASAPI session volume
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _isPlaying = false;
    }
}
