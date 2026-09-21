# Testing

## Unit Tests

Run: `dotnet test AreaCut.sln`

### Core Tests (platform-independent)
- `TimeStamp` — creation, arithmetic, comparison, round-trips
- `FrameRate` — FPS calculation, frame↔time conversion, presets
- `TimeRange` — duration, contains, overlap, intersection
- `Clip` — timeline duration with playback rate, source duration
- `Transform` — identity, opacity clamping
- `Crop` — defaults, width/height calculation
- `SocialPreset` — dimensions, aspect ratios
- `UndoRedo` — execute, undo, redo, new-action-clears-redo, split-undo-merge
- `SnapEngine` — snap to clip boundaries, playhead, clear
- `Project` — track management, clip lookup, timeline duration
- `CanvasSpec` — presets, aspect ratios

## Integration Tests (Windows-only)
- Media Foundation initialization and probing
- Video decode and seek
- Audio playback sync
- Export short video and verify output

## Smoke Test

Manual checklist before release:
1. Launch AreaCut
2. Import an MP4
3. Play the video
4. Seek to different positions
5. Split at playhead (S)
6. Trim clip edges
7. Add text overlay
8. Change audio volume
9. Save project
10. Close and reopen project
11. Export to MP4
12. Play exported file and verify sync
