using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.ProjectModel;
using System;
using AreaCut.Core.Commands;
using AreaCut.Core.Models;
using AreaCut.Core.Time;
using AreaCut.Core.Undo;
using AreaCut.Rendering.Preview;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace AreaCut.App;

/// <summary>
/// Main application window with the standard video editor layout:
/// Menu bar | Media bin | Preview | Properties | Timeline | Status bar
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly App _app;
    private AreaCutProject? _project;
    private UndoRedoStack? _undoRedo;
    private PreviewClock? _clock;
    private DispatcherTimer? _timecodeTimer;
    private string? _selectedClipId;

    public MainWindow()
    {
        _app = (App)App.Current;
        InitializeComponent();
        Title = "AreaCut";

        // Set minimum window size appropriate for video editing
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        if (appWindow != null)
        {
            appWindow.Resize(new Windows.Graphics.SizeInt32(1440, 900));
        }

        // WinUI 3's Window is not a UIElement and exposes no key surface of its
        // own, so shortcuts are handled on the root element.
        if (Content is UIElement root)
        {
            root.KeyDown += OnRootKeyDown;
        }

        PlayPauseButton.Click += (_, _) => TogglePlayPause();
        PreviousFrameButton.Click += (_, _) => StepBackward();
        NextFrameButton.Click += (_, _) => StepForward();
        SplitButton.Click += (_, _) => SplitAtPlayhead();

        // The PreviewClock is real and monotonic, but nothing renders frames yet,
        // so the timecode reports that clock and nothing else.
        _timecodeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _timecodeTimer.Tick += (_, _) => UpdateTimecode();
        _timecodeTimer.Start();

        Closed += (_, _) => _timecodeTimer?.Stop();

        UpdateTimecode();
        UpdateCommandStates();
    }

    /// <summary>Initialize the window with a project.</summary>
    public void Initialize(AreaCutProject project, UndoRedoStack undoRedo)
    {
        _project = project;
        _undoRedo = undoRedo;
        _clock = new PreviewClock();
        _selectedClipId = null;

        UpdateProjectInfo();
        UpdateTimecode();
        UpdateMediaBin();
        UpdateCommandStates();
    }

    /// <summary>
    /// Shows the media the project references. This is also how an incoming
    /// handover becomes visible: without it a forwarded file would load silently.
    /// </summary>
    private void UpdateMediaBin()
    {
        MediaListView.Items.Clear();
        if (_project == null) return;

        foreach (var media in _project.Media)
        {
            MediaListView.Items.Add(media.IsOffline
                ? $"{media.FileName}  (missing)"
                : media.FileName);
        }

        // A freshly handed-over file arrives as a single clip; make it the
        // selection so the editing shortcuts have something to act on.
        _selectedClipId = _project.Clips.Count > 0 ? _project.Clips[0].Id : null;
    }

    private void UpdateProjectInfo()
    {
        ProjectInfoText.Text = _project == null
            ? string.Empty
            : $"{_project.Name} · {_project.Canvas.Width}×{_project.Canvas.Height} · {_project.Canvas.Fps}fps";
    }

    private void UpdateTimecode()
    {
        TimecodeDisplay.Text = (_clock?.Position ?? TimeStamp.Zero).ToString();
    }

    /// <summary>Keeps the menu honest: a command is enabled only when it can act.</summary>
    private void UpdateCommandStates()
    {
        UndoMenuItem.IsEnabled = _undoRedo?.CanUndo == true;
        RedoMenuItem.IsEnabled = _undoRedo?.CanRedo == true;
    }

    // ----- menu commands -----

    private void OnNewProjectClick(object sender, RoutedEventArgs e)
    {
        var project = _app.NewProject("Untitled");
        Initialize(project, _app.UndoRedo);
    }

    private void OnUndoClick(object sender, RoutedEventArgs e)
    {
        _undoRedo?.Undo();
        UpdateCommandStates();
    }

    private void OnRedoClick(object sender, RoutedEventArgs e)
    {
        _undoRedo?.Redo();
        UpdateCommandStates();
    }

    private void OnExitClick(object sender, RoutedEventArgs e) => Close();

    // ----- keyboard -----

    private void OnRootKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (_project == null || _undoRedo == null) return;

        var ctrl = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
        var shift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);

        if (ctrl.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
        {
            switch (e.Key)
            {
                case VirtualKey.Z when !shift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down):
                    _undoRedo.Undo();
                    UpdateCommandStates();
                    e.Handled = true;
                    break;
                case VirtualKey.Y:
                    _undoRedo.Redo();
                    UpdateCommandStates();
                    e.Handled = true;
                    break;
            }
        }
        else
        {
            switch (e.Key)
            {
                case VirtualKey.Space:
                    TogglePlayPause();
                    e.Handled = true;
                    break;
                case VirtualKey.S:
                    SplitAtPlayhead();
                    e.Handled = true;
                    break;
                case VirtualKey.Delete:
                    DeleteSelectedClip();
                    e.Handled = true;
                    break;
                case VirtualKey.Left:
                    StepBackward();
                    e.Handled = true;
                    break;
                case VirtualKey.Right:
                    StepForward();
                    e.Handled = true;
                    break;
                case VirtualKey.K:
                    PausePlayback();
                    e.Handled = true;
                    break;
            }
        }
    }

    private void TogglePlayPause()
    {
        if (_clock == null) return;
        if (_clock.IsPlaying) _clock.Pause();
        else _clock.Play();
        UpdateTimecode();
    }

    private void PausePlayback() => _clock?.Pause();

    private void SplitAtPlayhead()
    {
        if (_project == null || _undoRedo == null || _clock == null) return;
        var selectedClip = GetSelectedClip();
        if (selectedClip == null) return;

        var command = new SplitClipCommand(_project, selectedClip.Id, _clock.Position);
        _undoRedo.Execute(command);
        UpdateCommandStates();
    }

    private void DeleteSelectedClip()
    {
        if (_project == null || _undoRedo == null) return;
        var selectedClip = GetSelectedClip();
        if (selectedClip == null) return;

        var command = new DeleteClipCommand(_project, selectedClip.Id);
        _undoRedo.Execute(command);
        UpdateCommandStates();
    }

    private void StepForward() => _clock?.Seek(_clock.Position.Add(TimeStamp.FromSeconds(1.0 / 30)));
    private void StepBackward() => _clock?.Seek(_clock.Position.Subtract(TimeStamp.FromSeconds(1.0 / 30)));

    private Clip? GetSelectedClip() => _project?.Clips.Find(c => c.Id == _selectedClipId);
}
