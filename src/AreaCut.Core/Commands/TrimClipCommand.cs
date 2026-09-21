using System;
using AreaCut.Core.Time;
using AreaCut.Core.Models;
using ProjectModel = AreaCut.Core.Project.Project;

namespace AreaCut.Core.Commands;

/// <summary>
/// Trim a clip's source in/out points or timeline start.
/// Undo restores original boundaries.
/// </summary>
public sealed class TrimClipCommand : ICommand
{
    private readonly ProjectModel _project;
    private readonly string _clipId;
    private readonly TimeStamp? _newSourceIn;
    private readonly TimeStamp? _newSourceOut;
    private readonly TimeStamp? _newTimelineStart;
    private TimeStamp _oldSourceIn;
    private TimeStamp _oldSourceOut;
    private TimeStamp _oldTimelineStart;

    public string Description => $"Trim clip '{_clipId}'";

    public TrimClipCommand(ProjectModel project, string clipId,
        TimeStamp? newSourceIn = null, TimeStamp? newSourceOut = null, TimeStamp? newTimelineStart = null)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _clipId = clipId ?? throw new ArgumentNullException(nameof(clipId));
        _newSourceIn = newSourceIn;
        _newSourceOut = newSourceOut;
        _newTimelineStart = newTimelineStart;
    }

    public void Execute()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        _oldSourceIn = clip.SourceIn;
        _oldSourceOut = clip.SourceOut;
        _oldTimelineStart = clip.TimelineStart;
        if (_newSourceIn != null) clip.SourceIn = _newSourceIn.Value;
        if (_newSourceOut != null) clip.SourceOut = _newSourceOut.Value;
        if (_newTimelineStart != null) clip.TimelineStart = _newTimelineStart.Value;
    }

    public void Undo()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        clip.SourceIn = _oldSourceIn;
        clip.SourceOut = _oldSourceOut;
        clip.TimelineStart = _oldTimelineStart;
    }
}
