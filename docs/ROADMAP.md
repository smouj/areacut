# Roadmap

## v0.1 — Foundation (Current)
- [x] Project structure and solution scaffold
- [x] Core models: Clip, Track, Transform, Crop, TextClip, CaptionItem
- [x] Time math: TimeStamp, FrameRate, TimeRange
- [x] Command Pattern: AddClip, DeleteClip, MoveClip, TrimClip, SplitClip, Transform, Volume, Text
- [x] Undo/Redo stack
- [x] Project serialization (.areacut format)
- [x] Social presets and safe areas
- [x] SRT parser and caption presets
- [x] WinUI 3 shell with layout and keyboard shortcuts
- [ ] Media Foundation video decode and metadata
- [ ] GPU-accelerated preview
- [ ] Functional timeline UI
- [ ] Split/trim/move in UI
- [ ] H.264/AAC MP4 export
- [ ] Audio playback and mixing

## v0.2 — Editing
- [ ] Timeline with thumbnails and waveforms
- [ ] Drag-and-drop from Explorer
- [ ] Reframe tool
- [ ] Speed control
- [ ] Fades (video and audio)
- [ ] Transitions (cut, crossfade, fade to black)
- [ ] Proxy workflow

## v0.3 — Social
- [ ] Auto Short workflow
- [ ] Caption editing UI
- [ ] Local transcription (optional)
- [ ] Safe area overlays

## v0.4 — Polish
- [ ] Performance profiling and optimization
- [ ] Crash handling and emergency autosave
- [ ] Settings UI
- [ ] AreaRec integration ("Open in AreaCut")

## v1.0 — Release
- [ ] All V1 criteria from spec met and verified
- [ ] CI green
- [ ] Smoke test completed
- [ ] Package published
- [ ] Documentation complete
