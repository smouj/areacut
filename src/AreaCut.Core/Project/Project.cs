using System;
using System.Collections.Generic;
using AreaCut.Core.Models;
using AreaCut.Core.Time;

namespace AreaCut.Core.ProjectModel;

/// <summary>
/// The complete non-destructive project state.
/// Serialized to .areacut JSON files.
/// Original media files are never modified.
/// </summary>
public sealed class Project
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Untitled";
    public CanvasSpec Canvas { get; set; } = CanvasSpec.Vertical1080;
    public ProjectSettings Settings { get; set; } = new();

    public List<Track> Tracks { get; } = new();
    public List<Clip> Clips { get; } = new();
    public List<TextClip> TextClips { get; } = new();
    public List<MediaReference> Media { get; } = new();
    public List<CaptionItem> Captions { get; } = new();
    public List<Transition> Transitions { get; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Find a media reference by ID.</summary>
    public MediaReference? FindMedia(string mediaId) => Media.Find(m => m.Id == mediaId);

    /// <summary>Find a clip by ID.</summary>
    public Clip? FindClip(string clipId) => Clips.Find(c => c.Id == clipId);

    /// <summary>Find a track by ID.</summary>
    public Track? FindTrack(string trackId) => Tracks.Find(t => t.Id == trackId);

    /// <summary>Get all clips on a track.</summary>
    public IEnumerable<Clip> GetClipsOnTrack(string trackId) => Clips.FindAll(c => c.TrackId == trackId);

    /// <summary>Get the total timeline duration.</summary>
    public TimeStamp TimelineDuration
    {
        get
        {
            var maxEnd = TimeStamp.Zero;
            foreach (var clip in Clips)
            {
                var end = clip.TimelineEnd;
                if (end > maxEnd) maxEnd = end;
            }
            foreach (var text in TextClips)
            {
                var end = text.TimelineEnd;
                if (end > maxEnd) maxEnd = end;
            }
            return maxEnd;
        }
    }
}
