using System;
using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.Models;

namespace AreaCut.Core.Commands;

/// <summary>
/// Change a clip's transform (position, scale, rotation, opacity).
/// Undo restores the original transform.
/// </summary>
public sealed class TransformClipCommand : ICommand
{
    private readonly AreaCutProject _project;
    private readonly string _clipId;
    private readonly Transform _newTransform;
    private Transform _oldTransform = Transform.Identity;

    public string Description => $"Transform clip '{_clipId}'";

    public TransformClipCommand(AreaCutProject project, string clipId, Transform newTransform)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _clipId = clipId ?? throw new ArgumentNullException(nameof(clipId));
        _newTransform = newTransform ?? throw new ArgumentNullException(nameof(newTransform));
    }

    public void Execute()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        _oldTransform = clip.Transform with { };
        clip.Transform = _newTransform;
    }

    public void Undo()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        clip.Transform = _oldTransform;
    }
}
