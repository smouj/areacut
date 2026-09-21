using System;
using AreaCut.Core.Models;
using AreaCut.Core.Project;

namespace AreaCut.Core.Commands;

/// <summary>
/// Change a clip's volume.
/// Undo restores original volume.
/// </summary>
public sealed class ChangeVolumeCommand : ICommand
{
    private readonly Project _project;
    private readonly string _clipId;
    private readonly double _newVolume;
    private double _oldVolume;

    public string Description => $"Change volume of clip '{_clipId}'";

    public ChangeVolumeCommand(Project project, string clipId, double newVolume)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _clipId = clipId ?? throw new ArgumentNullException(nameof(clipId));
        _newVolume = Math.Clamp(newVolume, 0, 2);
    }

    public void Execute()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        _oldVolume = clip.Volume;
        clip.Volume = _newVolume;
    }

    public void Undo()
    {
        var clip = _project.FindClip(_clipId) ?? throw new InvalidOperationException($"Clip '{_clipId}' not found");
        clip.Volume = _oldVolume;
    }
}
