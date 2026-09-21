using System;
using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.Models;

namespace AreaCut.Core.Commands;

/// <summary>
/// Delete a clip from the project timeline.
/// Undo restores it.
/// </summary>
public sealed class DeleteClipCommand : ICommand
{
    private readonly AreaCutProject _project;
    private readonly string _clipId;
    private Clip? _removedClip;

    public string Description => $"Delete clip '{_clipId}'";

    public DeleteClipCommand(AreaCutProject project, string clipId)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _clipId = clipId ?? throw new ArgumentNullException(nameof(clipId));
    }

    public void Execute()
    {
        _removedClip = _project.FindClip(_clipId);
        if (_removedClip != null)
            _project.Clips.Remove(_removedClip);
    }

    public void Undo()
    {
        if (_removedClip != null)
            _project.Clips.Add(_removedClip);
    }
}
