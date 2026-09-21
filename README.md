<p align="center">
  <img src="assets/logo.png" alt="AreaCut" width="128"><br>
  <strong>AreaCut</strong>
</p>

<p align="center">
  <em>Cut. Reframe. Caption. Export.</em>
</p>

AreaCut is a minimal, local-first Windows video editor built for social content creators. It turns desktop recordings and videos into Instagram Reels, TikToks, YouTube Shorts, and standard videos — without accounts, cloud, telemetry, or complexity.

The visual identity is documented in [assets/BRAND.md](assets/BRAND.md). The repository ships the generated raster lockup, social banner, transparent mark, and Windows `.ico` application icon used by the app and release scripts.

Designed as the editing companion to [AreaRec](https://github.com/smouj/arearec):

```
AreaRec = capture
AreaCut = edit + compose + export
```

Both are independent applications. AreaCut opens a recording from AreaRec in one click, but neither depends on the other.

## Features

- **Non-destructive editing** — original files are never modified
- **Real-time preview** with GPU-accelerated compositing (D3D11/D2D)
- **Timeline** with video, audio, text, and caption tracks
- **Split, trim, move** clips with keyboard shortcuts (S, Delete, Ctrl+Z/Y)
- **Social presets** — Vertical 9:16, Instagram 4:5, Square 1:1, YouTube 16:9
- **Reframe** — visually adjust crop/scale for any aspect ratio
- **Safe areas** — TikTok, Instagram Reels, YouTube Shorts overlays
- **Auto Short** — one-click project setup for any platform
- **Captions** — import SRT, edit, style with presets
- **Local transcription** — optional Whisper-compatible runtime, no API
- **Text overlays** — fonts, colors, shadows, outlines via DirectWrite
- **Audio mixing** — video audio + music + additional track, with fades
- **Speed control** — 0.5×, 1×, 1.5×, 2× (custom values supported)
- **Transitions** — Cut, Crossfade, Fade to black
- **Proxy workflow** — edit on 720p proxies, export from originals
- **Export** — H.264/AAC MP4 via Media Foundation, hardware encoding when available
- **Undo/Redo** — Command Pattern from day one, Ctrl+Z / Ctrl+Y
- **Autosave** with crash recovery
- **Single instance** — reopens in existing window
- **No FFmpeg dependency** — native Windows APIs throughout
- **No Electron, no WebView** — pure WinUI 3

## Requirements

- Windows 10 version 19041+ or Windows 11
- .NET 8 SDK for development
- Direct3D 11 capable GPU (software fallback available)
- Media Foundation (included in Windows)

## Build

```powershell
.\scripts\BUILD.ps1
```

## Run

```powershell
dotnet run --project src/AreaCut.App/AreaCut.App.csproj
```

## Test

```powershell
.\scripts\TEST.ps1
```

## Publish

```powershell
.\scripts\PUBLISH_PORTABLE.ps1
```

Creates a self-contained `AreaCut-win-x64.zip` in `artifacts/`.

## Install

```powershell
.\scripts\INSTALL_PORTABLE.ps1
```

Installs to `%LOCALAPPDATA%\Programs\AreaCut` with Start Menu shortcut.

## AreaRec Integration

After recording with AreaRec:

```
AreaRec → Stop recording → "Open in AreaCut"
```

AreaCut accepts a video file via command line:

```
AreaCut.exe "C:\path\to\recording.mp4"
```

This creates a new project, imports the video, and places it on the timeline.

## Project Format

AreaCut saves projects as `.areacut` files — versioned JSON that stores:

- Canvas dimensions, FPS
- Track layout
- Clip references (source path, in/out, transforms)
- Text overlays and captions
- Transitions and effects
- Settings

Original media files are never modified.

## Architecture

```
AreaCut.App          WinUI 3 UI, views, view models, keyboard, drag-drop
AreaCut.Core         Project, timeline, clips, commands, undo, serialization
AreaCut.Media         Media Foundation decode, metadata, thumbnails, waveforms
AreaCut.Rendering     D3D11/D2D composition, preview clock, text rendering
AreaCut.Audio         WASAPI playback, mixing, waveform cache
AreaCut.Export        H.264/AAC encoder pipeline, presets, progress
AreaCut.Transcription SRT parsing, caption presets, local Whisper (optional)
```

Dependency direction: App → Core ← Media, Rendering, Audio, Export, Transcription

The UI never contains core editor logic. Each library has a clear boundary.

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| Space | Play / Pause |
| S | Split at playhead |
| Delete | Delete selected clip |
| Ctrl+Z | Undo |
| Ctrl+Y | Redo |
| Ctrl+S | Save |
| Ctrl+Shift+S | Save As |
| Ctrl+O | Open |
| Ctrl+E | Export |
| ← | Previous frame |
| → | Next frame |
| J | Reverse playback |
| K | Pause |
| L | Forward playback |

## Privacy

AreaCut does not make network requests. Project files and media stay on your machine. No telemetry, no analytics, no accounts.

## License

MIT — see [LICENSE](LICENSE).

## Related

- [AreaRec](https://github.com/smouj/arearec) — Select a region. Record it. Get an MP4.
