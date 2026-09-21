# AreaCut Architecture

AreaCut is a Windows-native video editor built with C# / .NET 8 / WinUI 3.

## Dependency Direction

```
AreaCut.App (WinUI 3 shell, views, view models, keyboard, drag-drop)
      |
      +--> AreaCut.Core (project, timeline, clips, commands, undo, serialization)
      |         |
      |         + (no dependency direction — Core is standalone)
      |
      +--> AreaCut.Media (Media Foundation decode, metadata, thumbnails, waveforms)
      +--> AreaCut.Rendering (D3D11/D2D composition, preview, text rendering)
      +--> AreaCut.Audio (WASAPI playback, mixing, waveform cache)
      +--> AreaCut.Export (H.264/AAC encoder pipeline, presets, progress)
      +--> AreaCut.Transcription (SRT parsing, caption presets, local Whisper)
```

**Core has no upward dependencies.** The UI never contains core editor logic. Each library has a clear boundary.

## Project Structure

| Project | Responsibility |
|---------|---------------|
| `AreaCut.App` | WinUI 3 shell, MainWindow, keyboard shortcuts. Drag-drop, functional clip selection and single-instance redirection are not implemented yet. |
| `AreaCut.Core` | Project model, timeline, clips, tracks, commands, undo/redo, serialization, time math |
| `AreaCut.Media` | Media Foundation runtime, capabilities, source reader, metadata probing, thumbnails, waveforms |
| `AreaCut.Rendering` | D3D11 device, composition engine, preview clock, text/caption rendering |
| `AreaCut.Audio` | WASAPI playback, audio mixer, waveform cache |
| `AreaCut.Export` | Render pipeline, MF encoder, export presets, progress reporting |
| `AreaCut.Transcription` | SRT/WebVTT parsing, caption presets, local transcription (optional Whisper) |

## Key Design Decisions

### Non-destructive Editing
All editing operations modify project state, never source files. The `.areacut` format stores references, positions, cuts, transforms — not pixel data.

### Command Pattern (Undo/Redo)
Every editable operation implements `ICommand` with `Execute()` and `Undo()`. `UndoRedoStack` manages history. New actions clear the redo stack.

### Tick-Based Time
`TimeStamp` uses 100ns ticks internally, avoiding floating-point drift in timeline calculations. `FrameRate` is rational (numerator/denominator) to handle 29.97fps exactly.

### GPU Preview Pipeline
```
Media Foundation decoder → D3D11 texture → GPU composition → D2D/DWrite overlays → preview
```
Preview and export share the same `CompositionEngine` rules to avoid discrepancies.

### Reliable Playback Clock
`PreviewClock` uses `Stopwatch`, not UI timers, for drift-free A/V sync.

### Media Foundation Throughout
No FFmpeg dependency. Media Foundation handles decode, encode, and container operations. Hardware encoding is used when available, with software fallback.

## Project Format (.areacut)

Versioned JSON:

```json
{
  "version": 1,
  "name": "Untitled",
  "canvas": { "width": 1080, "height": 1920, "fps": 30 },
  "tracks": [...],
  "clips": [...],
  "textClips": [...],
  "media": [...],
  "captions": [...],
  "transitions": [...],
  "settings": { ... }
}
```

Atomic saves via temp file + rename. Crash recovery via `.recovery` sidecar.

## AreaRec Integration

**Status: not wired up yet.** The intended contract is a command-line handover:

```
AreaCut.exe "C:\path\to\recording.mp4"
```

`Program.cs` already validates the extension and calls `App.OpenVideoDirectly`,
which builds a project, adds a `MediaReference` and places a clip on the first
video track. Two pieces are missing: the `MainWindow` created in `OnLaunched`
never receives that project, and running AreaCut while another instance is open
exits instead of redirecting the file to the existing window. Tracked for v0.4 in
[roadmap.md](roadmap.md).

No shared code between the two applications — the integration is a process
boundary, by design.
