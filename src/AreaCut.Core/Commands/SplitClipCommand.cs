using System;
using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.Time;
using AreaCut.Core.Models;

namespace AreaCut.Core.Commands;

/// <summary>
/// Split a clip at a given timeline position into two clips.
/// Undo merges them back.
/// </summary>
public sealed class SplitClipCommand : ICommand
{
    private readonly AreaCutProject _project;
    private readonly string _clipId;
    private readonly TimeStamp _splitAt;
    private Clip? _secondClip;

    public string Description => $"Split clip '{_clipId}'";

    public SplitClipCommand(AreaCutProject project, string clipId, TimeStamp splitAt)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _clipId = clipId ?? throw new ArgumentNullException(nameof(clipId));
        _splitAt = splitAt;
    }

    public void Execute()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");

        // splitAt is a timeline position, convert to source offset
        var offsetInSource = _splitAt.Subtract(clip.TimelineStart);
        var splitSourcePoint = clip.SourceIn.Add(offsetInSource);

        // Create the second half
        _secondClip = new Clip(clip.SourceMediaId, clip.TrackId)
        {
            SourceIn = splitSourcePoint,
            SourceOut = clip.SourceOut,
            TimelineStart = _splitAt,
            Transform = clip.Transform with { },
            Crop = clip.Crop with { },
            Opacity = clip.Opacity,
            PlaybackRate = clip.PlaybackRate,
            Volume = clip.Volume,
        };

        // Trim the first half
        clip.SourceOut = splitSourcePoint;

        _project.Clips.Add(_secondClip);
    }

    public void Undo()
    {
        var original = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        if (_secondClip == null) return;

        // Restore original source range
        original.SourceOut = _secondClip.SourceOut;

        // Remove the second half
        _project.Clips.Remove(_secondClip);
    }
}
