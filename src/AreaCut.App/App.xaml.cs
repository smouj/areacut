using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.ProjectModel;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
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
    private MainWindow? _window;
    private CancellationTokenSource? _handoffCancellation;

    public MediaFoundationRuntime MfRuntime => _mfRuntime ??= new();
    public UndoRedoStack UndoRedo => _undoRedo ??= new();
    public AreaCutProject? CurrentProject => _currentProject;

    /// <summary>
    /// Video handed over on the command line by AreaRec. Applied when the window exists.
    /// </summary>
    public string? PendingVideoPath { get; init; }

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

        // Create the main window and give it a project, so the shell opens in a
        // usable state instead of empty.
        _window = new MainWindow();
        var project = PendingVideoPath is not null
            ? OpenVideoDirectly(PendingVideoPath)
            : NewProject("Untitled");

        _window.Initialize(project, UndoRedo);
        _window.Activate();

        StartHandoffListener();
    }

    /// <summary>
    /// Listens for files forwarded by later launches. A second AreaCut process never
    /// opens its own window: it sends the path here and exits.
    /// </summary>
    private void StartHandoffListener()
    {
        _handoffCancellation = new CancellationTokenSource();
        var token = _handoffCancellation.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using var server = new NamedPipeServerStream(
                        HandoffChannel.PipeName,
                        PipeDirection.In,
                        1,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    await server.WaitForConnectionAsync(token).ConfigureAwait(false);

                    using var reader = new StreamReader(server);
                    var path = await reader.ReadLineAsync().ConfigureAwait(false);

                    if (HandoffChannel.IsSupportedVideo(path))
                    {
                        var forwarded = path!;
                        _window?.DispatcherQueue.TryEnqueue(() => ApplyHandoff(forwarded));
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // A broken connection must not kill the listener: keep serving.
                }
            }
        }, token);
    }

    /// <summary>Opens a forwarded file in the window that is already open.</summary>
    private void ApplyHandoff(string videoPath)
    {
        _currentProject = OpenVideoDirectly(videoPath);
        _window?.Initialize(_currentProject, UndoRedo);
        _window?.Activate();
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
