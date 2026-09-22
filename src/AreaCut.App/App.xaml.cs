using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.ProjectModel;
using System;
using Microsoft.UI.Xaml;
using AreaCut.Core.Serialization;
using Models = AreaCut.Core.Models;
using AreaCut.Core.Undo;
using AreaCut.Media.MediaFoundation;

namespace AreaCut.App;

/// <summary>
/// Application entry point. Initializes Media Foundation and manages app-wide services.
/// </summary>
public sealed partial class App : Microsoft.UI.Xaml.Application
{
    private MediaFoundationRuntime? _mfRuntime;
    private UndoRedoStack? _undoRedo;
    private AreaCutProject? _currentProject;
    private AutoSave? _autoSave;

    public MediaFoundationRuntime MfRuntime => _mfRuntime ??= new();
    public UndoRedoStack UndoRedo => _undoRedo ??= new();
    public AreaCutProject? CurrentProject => _currentProject;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Initialize Media Foundation on startup
        try
        {
            MfRuntime.Initialize();
        }
        catch
        {
            // Non-fatal: some MF features may not work
        }

        // Create main window
        var window = new MainWindow();
        window.Activate();
    }

    /// <summary>Create a new project with the given canvas specification.</summary>
    public AreaCutProject NewProject(string name, CanvasSpec? canvas = null)
    {
        _currentProject = new AreaCutProject
        {
            Name = name,
            Canvas = canvas ?? CanvasSpec.Vertical1080
        };
        _undoRedo?.Clear();

        // Add default tracks
        _currentProject.Tracks.Add(new Models.Track(Models.TrackKind.Video, "Video 1"));
        _currentProject.Tracks.Add(new Models.Track(Models.TrackKind.Audio, "Audio 1"));
        _currentProject.Tracks.Add(new Models.Track(Models.TrackKind.Text, "Text"));
        _currentProject.Tracks.Add(new Models.Track(Models.TrackKind.Caption, "Captions"));

        return _currentProject;
    }

    /// <summary>Open an existing project file.</summary>
    public async System.Threading.Tasks.Task<AreaCutProject> OpenProjectAsync(string filePath)
    {
        var serializer = new ProjectSerializer();
        _currentProject = await serializer.LoadAsync(filePath);
        _undoRedo?.Clear();

        // Start auto-save
        _autoSave?.Stop();
        _autoSave = new AutoSave();
        _autoSave.Start(_currentProject, filePath);

        return _currentProject;
    }

    /// <summary>Open a video file directly (from AreaRec integration or drag-drop).</summary>
    public AreaCutProject OpenVideoDirectly(string videoPath)
    {
        var project = NewProject("Untitled");
        // Import the video and add it to the first video track
        var media = new AreaCut.Core.Models.MediaReference(videoPath, AreaCut.Core.Models.MediaKind.Video);
        project.Media.Add(media);

        var videoTrack = project.Tracks.Find(t => t.Kind == Models.TrackKind.Video);
        if (videoTrack != null)
        {
            var clip = new AreaCut.Core.Models.Clip(media.Id, videoTrack.Id)
            {
                SourceIn = AreaCut.Core.Time.TimeStamp.Zero,
                SourceOut = media.Duration.HasValue
                    ? AreaCut.Core.Time.TimeStamp.FromTimeSpan(media.Duration.Value)
                    : AreaCut.Core.Time.TimeStamp.FromSeconds(60), // default 60s if unknown
                TimelineStart = AreaCut.Core.Time.TimeStamp.Zero,
            };
            project.Clips.Add(clip);
        }

        return project;
    }
}
