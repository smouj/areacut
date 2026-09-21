using System;
using AreaCut.Core.Models;
using AreaCut.Core.Project;

namespace AreaCut.Core.Commands;

/// <summary>
/// Add a clip to the project timeline.
/// Undo removes it.
/// </summary>
public sealed class AddClipCommand : ICommand
{
    private readonly Project _project;
    private readonly Clip _clip;

    public string Description => $"Add clip '{_clip.Id}'";

    public AddClipCommand(Project project, Clip clip)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _clip = clip ?? throw new ArgumentNullException(nameof(clip));
    }

    public void Execute() => _project.Clips.Add(_clip);
    public void Undo() => _project.Clips.Remove(_clip);
}
