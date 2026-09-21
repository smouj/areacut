using System;
using System.Collections.Generic;

namespace AreaCut.Audio.Mixer;

/// <summary>
/// Mixes multiple audio tracks: video audio + music + additional tracks.
/// Handles volume per track, fade in/out, and sample-rate alignment.
/// </summary>
public sealed class AudioMixer
{
    private readonly List<MixerTrack> _tracks = new();

    public IReadOnlyList<MixerTrack> Tracks => _tracks.AsReadOnly();

    /// <summary>Add a mixer track.</summary>
    public MixerTrack AddTrack(string name, double volume = 1.0, bool muted = false)
    {
        var track = new MixerTrack(name, _tracks.Count, volume, muted);
        _tracks.Add(track);
        return track;
    }

    /// <summary>Remove a mixer track by index.</summary>
    public void RemoveTrack(int index)
    {
        if (index >= 0 && index < _tracks.Count)
            _tracks.RemoveAt(index);
    }

    /// <summary>Mix all tracks into a single output buffer at the given timestamp.</summary>
    public void Mix(float[] outputBuffer, int sampleCount, double timeSeconds)
    {
        Array.Clear(outputBuffer, 0, sampleCount);

        foreach (var track in _tracks)
        {
            if (track.Muted) continue;

            // In production: decode audio samples from track source,
            // apply volume, fade, and mix into output buffer
            // For now, zero-fill (silence)
        }
    }

    /// <summary>Apply optional normalization to the mixed output.</summary>
    public void Normalize(float[] buffer, int sampleCount, double targetLevel = 0.95)
    {
        float peak = 0;
        for (var i = 0; i < sampleCount; i++)
        {
            var abs = Math.Abs(buffer[i]);
            if (abs > peak) peak = abs;
        }

        if (peak > 0.001f)
        {
            var gain = (float)(targetLevel / peak);
            for (var i = 0; i < sampleCount; i++)
                buffer[i] *= gain;
        }
    }
}

public sealed class MixerTrack
{
    public string Name { get; }
    public int Index { get; }
    public double Volume { get; set; }
    public bool Muted { get; set; }
    public double? FadeInDurationSeconds { get; set; }
    public double? FadeOutDurationSeconds { get; set; }

    internal MixerTrack(string name, int index, double volume, bool muted)
    {
        Name = name;
        Index = index;
        Volume = volume;
        Muted = muted;
    }
}
