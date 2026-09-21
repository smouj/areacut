# Project Format (.areacut)

AreaCut uses a versioned JSON format stored in `.areacut` files.

## Version 1 Schema

```json
{
  "version": 1,
  "name": "My Video",
  "canvas": {
    "width": 1080,
    "height": 1920,
    "fps": 30
  },
  "tracks": [
    { "id": "...", "name": "Video 1", "kind": "Video", "isMuted": false, "isLocked": false, "isVisible": true, "zOrder": 0 }
  ],
  "clips": [
    {
      "id": "...", "sourceMediaId": "...", "sourceInTicks": 0, "sourceOutTicks": 300000000,
      "timelineStartTicks": 0, "trackId": "...",
      "offsetX": 0, "offsetY": 0, "scaleX": 1, "scaleY": 1, "rotationDegrees": 0, "opacity": 1,
      "playbackRate": 1, "volume": 1,
      "cropLeft": 0, "cropTop": 0, "cropRight": 1, "cropBottom": 1
    }
  ],
  "textClips": [...],
  "media": [
    {
      "id": "...", "filePath": "C:\\Videos\\clip.mp4", "fileName": "clip.mp4",
      "kind": "Video", "durationSeconds": 60, "width": 1920, "height": 1080, "fps": 30,
      "codec": "H264", "audioCodec": "AAC", "audioChannels": 2, "audioSampleRate": 48000
    }
  ],
  "captions": [...],
  "transitions": [...],
  "settings": {
    "exportPath": "", "exportFileName": "export", "useHardwareEncoding": true,
    "exportQuality": 80, "autoSaveEnabled": true, "autoSaveIntervalSeconds": 120,
    "maxCacheSizeMb": 2048, "cachePath": ".cache"
  },
  "createdAt": "2025-01-01T00:00:00Z",
  "modifiedAt": "2025-01-01T00:00:00Z"
}
```

## Key Design Points

- **Timestamps use ticks** (100ns units) for precision
- **Original files are never modified** — only references are stored
- **Atomic saves** via temp file + rename
- **Crash recovery** via `.areacut.recovery` sidecar
- **Migration** — older versions are automatically upgraded on load
- **Moved files** — AreaCut detects offline media and offers relink

## File Detection

When a media reference is offline, AreaCut:
1. Marks `IsOffline = true`
2. Searches in the project directory and common locations
3. Offers the user a relink dialog
