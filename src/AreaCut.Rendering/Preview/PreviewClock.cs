using System;
using System.Diagnostics;
using AreaCut.Core.Time;

namespace AreaCut.Rendering.Preview;

/// <summary>
/// Reliable playback clock that drives preview timing.
/// Not based on UI timers — uses Stopwatch for precision.
/// Synchronizes video, audio, playhead and preview.
/// </summary>
public sealed class PreviewClock
{
    private readonly Stopwatch _stopwatch = new();
    private TimeStamp _basePosition;
    private double _playbackRate = 1.0;
    private bool _isPlaying;

    /// <summary>Current playback position.</summary>
    public TimeStamp Position => _isPlaying
        ? _basePosition.Add(TimeStamp.FromSeconds(_stopwatch.Elapsed.TotalSeconds * _playbackRate))
        : _basePosition;

    /// <summary>Whether playback is active.</summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>Current playback rate (0.5x, 1x, 1.5x, 2x).</summary>
    public double PlaybackRate => _playbackRate;

    /// <summary>Number of frames dropped due to slow composition.</summary>
    public long DroppedFrames { get; private set; }

    /// <summary>Start or resume playback from the current position.</summary>
    public void Play()
    {
        if (_isPlaying) return;
        _isPlaying = true;
        _stopwatch.Restart();
    }

    /// <summary>Pause playback, keeping the current position.</summary>
    public void Pause()
    {
        if (!_isPlaying) return;
        _basePosition = Position;
        _isPlaying = false;
        _stopwatch.Stop();
    }

    /// <summary>Seek to a specific position.</summary>
    public void Seek(TimeStamp position)
    {
        _basePosition = position;
        if (_isPlaying)
            _stopwatch.Restart();
    }

    /// <summary>Set the playback rate.</summary>
    public void SetPlaybackRate(double rate)
    {
        _basePosition = Position;
        _playbackRate = Math.Clamp(rate, 0.25, 4.0);
        if (_isPlaying)
            _stopwatch.Restart();
    }

    /// <summary>Report a dropped frame.</summary>
    public void ReportDroppedFrame() => DroppedFrames++;

    /// <summary>Reset dropped frame counter.</summary>
    public void ResetDroppedFrames() => DroppedFrames = 0;

    /// <summary>Stop playback and reset position to zero.</summary>
    public void Stop()
    {
        _isPlaying = false;
        _basePosition = TimeStamp.Zero;
        _stopwatch.Reset();
    }
}
