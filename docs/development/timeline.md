# Timeline

The timeline is a central piece of AreaCut. It must be fluid even with long projects.

## Track Types

- **Video** — video clips with transforms, crops, opacity
- **Audio** — audio clips with volume, fades
- **Text** — text overlays with font, color, position
- **Caption** — SRT/WebVTT caption tracks

## Supported Operations

| Operation | Shortcut | Notes |
|-----------|----------|-------|
| Select clip | Click | Single selection |
| Move clip | Drag | Snaps to grid |
| Trim left | Drag left edge | Changes SourceIn |
| Trim right | Drag right edge | Changes SourceOut |
| Split | S | At playhead |
| Delete | Delete | With undo |
| Duplicate | Ctrl+D | |
| Snap | Automatic | To clip edges, playhead |
| Zoom | Scroll wheel | Horizontal |
| Scroll | Shift+Scroll | Horizontal pan |

## Clip Model

Clips reference source media; they never copy pixel data:

```csharp
Clip {
    Id, SourceMediaId, SourceIn, SourceOut,
    TimelineStart, TrackId,
    Transform, Crop, Opacity, PlaybackRate, Volume
}
```

## Snapping

`SnapEngine` snaps clip edges to:
- Other clip boundaries
- Playhead position
- Frame grid at the project FPS

Threshold: 10ms by default.

## Composition

`CompositionEngine.Compose()` takes a project + timestamp and returns a `CompositedFrame` containing:
- Video layers (sorted by Z-order)
- Text layers (with fade calculations)
- Caption layers

Preview and export use the same composition rules.
