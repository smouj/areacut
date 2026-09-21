using System;
using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.Models;

namespace AreaCut.Core.Commands;

/// <summary>
/// Add a text clip to the project timeline.
/// Undo removes it.
/// </summary>
public sealed class AddTextCommand : ICommand
{
    private readonly AreaCutProject _project;
    private readonly TextClip _textClip;

    public string Description => $"Add text clip '{_textClip.Id}'";

    public AddTextCommand(AreaCutProject project, TextClip textClip)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _textClip = textClip ?? throw new ArgumentNullException(nameof(textClip));
    }

    public void Execute() => _project.TextClips.Add(_textClip);
    public void Undo() => _project.TextClips.Remove(_textClip);
}
