using System;
using System.Collections.Generic;
using System.Linq;
using AreaCut.Core.Models;

namespace AreaCut.Core.Timeline;

/// <summary>
/// Manages tracks on the timeline: add, remove, reorder.
/// Clips are stored in the Project, not in TrackManager directly.
/// </summary>
public sealed class TrackManager
{
    private readonly List<Track> _tracks = new();

    public IReadOnlyList<Track> Tracks => _tracks.AsReadOnly();

    /// <summary>Add a new track.</summary>
    public Track AddTrack(TrackKind kind, string? name = null)
    {
        var track = new Track(kind, name);
        _tracks.Add(track);
        return track;
    }

    /// <summary>Remove a track by ID.</summary>
    public bool RemoveTrack(string trackId)
    {
        var index = _tracks.FindIndex(t => t.Id == trackId);
        if (index < 0) return false;
        _tracks.RemoveAt(index);
        return true;
    }

    /// <summary>Move a track to a new position.</summary>
    public void MoveTrack(string trackId, int newIndex)
    {
        var index = _tracks.FindIndex(t => t.Id == trackId);
        if (index < 0) throw new ArgumentException("Track not found", nameof(trackId));
        var track = _tracks[index];
        _tracks.RemoveAt(index);
        _tracks.Insert(Math.Clamp(newIndex, 0, _tracks.Count), track);
    }

    /// <summary>Get tracks of a specific kind.</summary>
    public IEnumerable<Track> GetTracksByKind(TrackKind kind) => _tracks.Where(t => t.Kind == kind);

    /// <summary>Get video tracks.</summary>
    public IEnumerable<Track> VideoTracks => GetTracksByKind(TrackKind.Video);

    /// <summary>Get audio tracks.</summary>
    public IEnumerable<Track> AudioTracks => GetTracksByKind(TrackKind.Audio);

    /// <summary>Get text tracks.</summary>
    public IEnumerable<Track> TextTracks => GetTracksByKind(TrackKind.Text);

    /// <summary>Find a track by ID.</summary>
    public Track? FindTrack(string trackId) => _tracks.FirstOrDefault(t => t.Id == trackId);
}
