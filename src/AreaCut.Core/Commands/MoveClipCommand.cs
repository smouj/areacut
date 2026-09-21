using System;
using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.Time;
using AreaCut.Core.Models;

namespace AreaCut.Core.Commands;

/// <summary>
/// Move a clip on the timeline (change TimelineStart and optionally TrackId).
/// Undo restores original position.
/// </summary>
public sealed class MoveClipCommand : ICommand
{
    private readonly AreaCutProject _project;
    private readonly string _clipId;
    private readonly TimeStamp _newStart;
    private readonly string? _newTrackId;
    private TimeStamp _oldStart;
    private string? _oldTrackId;

    public string Description => $"Move clip '{_clipId}'";

    public MoveClipCommand(AreaCutProject project, string clipId, TimeStamp newStart, string? newTrackId = null)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _clipId = clipId ?? throw new ArgumentNullException(nameof(clipId));
        _newStart = newStart;
        _newTrackId = newTrackId;
    }

    public void Execute()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        _oldStart = clip.TimelineStart;
        _oldTrackId = clip.TrackId;
        clip.TimelineStart = _newStart;
        if (_newTrackId != null) clip.TrackId = _newTrackId;
    }

    public void Undo()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        clip.TimelineStart = _oldStart;
        if (_oldTrackId != null) clip.TrackId = _oldTrackId;
    }
}
