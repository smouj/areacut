# Roadmap

Status legend: ✅ done · 🚧 in progress · ⛔ not started

## v0.1 — Foundation (current)

The engine is real; the application around it is not.

| | Item | State |
| --- | --- | --- |
| 1 | Solution scaffold with 7 projects | ✅ |
| 2 | Core models — Clip, Track, Transform, Crop, TextClip, CaptionItem, Transition | ✅ |
| 3 | Time math — TimeStamp, FrameRate, TimeRange | ✅ |
| 4 | Command pattern — add, delete, move, trim, split, transform, volume, text | ✅ |
| 5 | Undo/redo stack | ✅ |
| 6 | `.areacut` serialization, migration, atomic saves, crash recovery | ✅ |
| 7 | Social presets and safe areas | ✅ |
| 8 | SRT parser, exporter and caption presets | ✅ |
| 9 | WinUI 3 shell — window, layout, keyboard handling | 🚧 shell only |
| 10 | Media Foundation decode and metadata probing | 🚧 stubbed |
| 11 | GPU-accelerated preview (D3D11 / D2D) | 🚧 stubbed |
| 12 | Functional timeline UI | ⛔ |
| 13 | Split, trim and move driven from the UI | ⛔ |
| 14 | H.264/AAC MP4 export | ⛔ |
| 15 | Audio playback and mixing (WASAPI) | ⛔ |

**v0.1 is not released and not usable.** Items 10–15 are what stand between the
current state and an editor you can actually cut a video with.

## v0.2 — Editing

- Timeline with thumbnails and waveforms
- Drag and drop from Explorer
- Reframe tool
- Speed control
- Fades, video and audio
- Transitions — cut, crossfade, fade to black
- Proxy workflow for 4K+ sources

## v0.3 — Social

- Auto Short workflow
- Caption editing UI
- Local transcription (optional, Whisper-compatible)
- Safe area overlays

## v0.4 — Polish and handover

- Performance profiling against the targets in [performance.md](performance.md)
- Crash handling and emergency autosave in the UI
- Settings UI
- **AreaRec → AreaCut handover**: "Open in AreaCut" in AreaRec, single-instance
  redirection in AreaCut, clean degradation when the sibling is not installed

## v1.0 — Release

- Every v1 criterion verified, not assumed
- CI green on `main`
- Smoke test completed from the checklist in [testing.md](testing.md)
- Portable package published with checksum and screenshots
- User documentation complete

## Known problems to clear before v1.0

- Clip selection in the UI is never assigned (`_selectedClipId` has no writer), so
  `S` and `Delete` cannot act on anything.
- The project created by `App.OpenVideoDirectly` is never handed to the
  `MainWindow`, so a command-line handover would open an empty shell.
- CI is failing on `main`.
