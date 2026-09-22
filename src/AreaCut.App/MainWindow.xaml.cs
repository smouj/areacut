using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.ProjectModel;
using System;
using System.Collections.ObjectModel;
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
    }

    /// <summary>Initialize the window with a project.</summary>
    public void Initialize(AreaCutProject project, UndoRedoStack undoRedo)
    {
        _project = project;
        _undoRedo = undoRedo;
        _clock = new PreviewClock();

        UpdateProjectInfo();
    }

    private void UpdateProjectInfo()
    {
        if (_project == null) return;
        ProjectInfoText.Text = $"{_project.Name} · {_project.Canvas.Width}×{_project.Canvas.Height} · {_project.Canvas.Fps}fps";
    }

    // Keyboard shortcut handling
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
                    e.Handled = true;
                    break;
                case VirtualKey.Y:
                    _undoRedo.Redo();
                    e.Handled = true;
                    break;
                case VirtualKey.S when !shift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down):
                    // Save
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
                case VirtualKey.J:
                    // Reverse playback
                    e.Handled = true;
                    break;
                case VirtualKey.K:
                    PausePlayback();
                    e.Handled = true;
                    break;
                case VirtualKey.L:
                    // Forward playback
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
    }

    private void PausePlayback() => _clock?.Pause();

    private void SplitAtPlayhead()
    {
        if (_project == null || _undoRedo == null || _clock == null) return;
        var selectedClip = GetSelectedClip();
        if (selectedClip == null) return;

        var command = new SplitClipCommand(_project, selectedClip.Id, _clock.Position);
        _undoRedo.Execute(command);
    }

    private void DeleteSelectedClip()
    {
        if (_project == null || _undoRedo == null) return;
        var selectedClip = GetSelectedClip();
        if (selectedClip == null) return;

        var command = new DeleteClipCommand(_project, selectedClip.Id);
        _undoRedo.Execute(command);
    }

    private void StepForward() => _clock?.Seek(_clock.Position.Add(TimeStamp.FromSeconds(1.0 / 30)));
    private void StepBackward() => _clock?.Seek(_clock.Position.Subtract(TimeStamp.FromSeconds(1.0 / 30)));

    private Clip? GetSelectedClip() => _project?.Clips.Find(c => c.Id == _selectedClipId);
}
