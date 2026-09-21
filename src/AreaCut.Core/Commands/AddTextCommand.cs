using System;
using AreaCut.Core.Models;
using AreaCut.Core.Project;

namespace AreaCut.Core.Commands;

/// <summary>
/// Add a text clip to the project timeline.
/// Undo removes it.
/// </summary>
public sealed class AddTextCommand : ICommand
{
    private readonly Project _project;
    private readonly TextClip _textClip;

    public string Description => $"Add text clip '{_textClip.Id}'";

    public AddTextCommand(Project project, TextClip textClip)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _textClip = textClip ?? throw new ArgumentNullException(nameof(textClip));
    }

    public void Execute() => _project.TextClips.Add(_textClip);
    public void Undo() => _project.TextClips.Remove(_textClip);
}
