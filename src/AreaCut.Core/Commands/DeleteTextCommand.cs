using System;
using AreaCut.Core.Models;
using ProjectModel = AreaCut.Core.Project.Project;

namespace AreaCut.Core.Commands;

/// <summary>
/// Delete a text clip from the project timeline.
/// Undo restores it.
/// </summary>
public sealed class DeleteTextCommand : ICommand
{
    private readonly ProjectModel _project;
    private readonly string _textClipId;
    private TextClip? _removedTextClip;

    public string Description => $"Delete text clip '{_textClipId}'";

    public DeleteTextCommand(ProjectModel project, string textClipId)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _textClipId = textClipId ?? throw new ArgumentNullException(nameof(textClipId));
    }

    public void Execute()
    {
        _removedTextClip = _project.TextClips.Find(t => t.Id == _textClipId);
        if (_removedTextClip != null)
            _project.TextClips.Remove(_removedTextClip);
    }

    public void Undo()
    {
        if (_removedTextClip != null)
            _project.TextClips.Add(_removedTextClip);
    }
}
