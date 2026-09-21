using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AreaCut.Core.Serialization;

namespace AreaCut.Core.ProjectModel;

/// <summary>
/// Periodic auto-save with crash recovery.
/// Saves to a recovery file alongside the main project.
/// On next launch, if a recovery file exists, offers to recover.
/// </summary>
public sealed class AutoSave : IDisposable
{
    private readonly ProjectSerializer _serializer = new();
    private Timer? _timer;
    private AreaCutProject? _currentProject;
    private string? _projectPath;
    private bool _disposed;

    public event EventHandler<string>? Saved;
    public event EventHandler<string>? RecoveryCreated;

    /// <summary>Start auto-saving a project at the given interval.</summary>
    public void Start(AreaCutProject project, string projectPath, int intervalSeconds = 120)
    {
        Stop();
        _currentProject = project;
        _projectPath = projectPath;
        _timer = new Timer(async _ => await SaveRecoveryAsync(), null,
            TimeSpan.FromSeconds(intervalSeconds), TimeSpan.FromSeconds(intervalSeconds));
    }

    /// <summary>Stop auto-saving.</summary>
    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    /// <summary>Save a recovery snapshot.</summary>
    public async Task SaveRecoveryAsync()
    {
        if (_currentProject == null || _projectPath == null || _disposed) return;

        try
        {
            var recoveryPath = GetRecoveryPath(_projectPath);
            await _serializer.SaveAsync(_currentProject, recoveryPath).ConfigureAwait(false);
            RecoveryCreated?.Invoke(this, recoveryPath);
        }
        catch
        {
            // Auto-save failures are non-fatal; log but don't crash
        }
    }

    /// <summary>Save the project to its main path.</summary>
    public async Task SaveAsync()
    {
        if (_currentProject == null || _projectPath == null || _disposed) return;
        await _serializer.SaveAsync(_currentProject, _projectPath).ConfigureAwait(false);
        Saved?.Invoke(this, _projectPath);

        // Remove recovery file after successful save
        var recoveryPath = GetRecoveryPath(_projectPath);
        if (File.Exists(recoveryPath))
            File.Delete(recoveryPath);
    }

    /// <summary>Check if a recovery file exists for a project path.</summary>
    public static bool HasRecovery(string projectPath)
        => File.Exists(GetRecoveryPath(projectPath));

    /// <summary>Load from recovery file if it exists, otherwise from main file.</summary>
    public async Task<AreaCutProject> LoadWithRecoveryAsync(string projectPath)
    {
        var recoveryPath = GetRecoveryPath(projectPath);
        if (File.Exists(recoveryPath))
            return await _serializer.LoadAsync(recoveryPath).ConfigureAwait(false);

        return await _serializer.LoadAsync(projectPath).ConfigureAwait(false);
    }

    /// <summary>Discard recovery file.</summary>
    public static void DiscardRecovery(string projectPath)
    {
        var recoveryPath = GetRecoveryPath(projectPath);
        if (File.Exists(recoveryPath))
            File.Delete(recoveryPath);
    }

    private static string GetRecoveryPath(string projectPath) => projectPath + ".recovery";

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
    }
}
