# Changelog

All notable changes to AreaCut are documented here.

## [0.1.0] - 2025-09-21

### Added
- Solution scaffold with 7 projects (App, Core, Media, Rendering, Audio, Export, Transcription)
- Core models: Clip, Track, Transform, Crop, TextClip, CaptionItem, MediaReference, Transition, Fade
- Time math: TimeStamp (tick-based), FrameRate (rational), TimeRange (half-open intervals)
- Command Pattern: AddClip, DeleteClip, MoveClip, TrimClip, SplitClip, TransformClip, ChangeVolume, AddText, DeleteText
- UndoRedoStack with execute/undo/redo and new-action-clears-redo
- Project serialization to .areacut (versioned JSON)
- Project migration framework
- Autosave with crash recovery
- Moved file detection and relink
- Social presets: Vertical, Instagram Portrait, Square, YouTube, Shorts, Discord
- Safe area profiles: TikTok, Instagram Reels, YouTube Shorts
- SRT parser and exporter
- Caption presets: Minimal, Classic, Bold, Retro, Subtitle
- Local transcription module (stub, Whisper-compatible)
- Media Foundation runtime, capabilities, and reader stubs
- Media probing and import pipeline
- Thumbnail and waveform cache generators
- D3D11 device manager
- Composition engine with video/text/caption layer composition
- Preview clock (Stopwatch-based, not UI timers)
- Audio playback engine (WASAPI stub)
- Audio mixer with track volume, mute, and normalization
- Waveform cache
- H.264/AAC export pipeline with presets
- Render pipeline with frame-by-frame composition
- WinUI 3 App shell with standard editor layout
- MainWindow with menu bar, media bin, preview, properties, timeline
- Keyboard shortcuts (Space, S, Delete, Ctrl+Z/Y, arrows, J/K/L)
- Single-instance enforcement
- Command-line video import (AreaRec integration path)
- Unit tests for Time, Clip, Transform, Crop, SocialPreset, UndoRedo, Snapping, Project
- Build, Test, Publish, Install PowerShell scripts
- README, LICENSE, .editorconfig, .gitignore
- Architecture, Media Pipeline, Timeline, Project Format, Export Pipeline, Performance, Testing, Privacy, Roadmap docs
