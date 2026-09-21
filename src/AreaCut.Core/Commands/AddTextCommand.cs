using System;
using AreaCut.Core.Models;
using ProjectModel = AreaCut.Core.Project.Project;

namespace AreaCut.Core.Commands;

/// <summary>
/// Add a text clip to the project timeline.
/// Undo removes it.
/// </summary>
public sealed class AddTextCommand : ICommand
{
    private readonly ProjectModel _project;
    private readonly TextClip _textClip;

    public string Description => $"Add text clip '{_textClip.Id}'";

    public AddTextCommand(ProjectModel project, TextClip textClip)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _textClip = textClip ?? throw new ArgumentNullException(nameof(textClip));
    }

    public void Execute() => _project.TextClips.Add(_textClip);
    public void Undo() => _project.TextClips.Remove(_textClip);
}
