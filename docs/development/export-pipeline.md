# Export Pipeline

## Pipeline

```
Timeline → Frame scheduler → Decode sources → GPU composition → Text/effects → Audio mix → MF encoder → MP4
```

## Export Presets

| Preset | Resolution | FPS | Video | Audio | Bitrate |
|--------|-----------|-----|-------|-------|---------|
| TikTok | 1080×1920 | 30 | H.264 | AAC | 8 Mbps |
| Instagram Reel | 1080×1920 | 30 | H.264 | AAC | 8 Mbps |
| YouTube Shorts | 1080×1920 | 30 | H.264 | AAC | 10 Mbps |
| YouTube 1080p | 1920×1080 | 30 | H.264 | AAC | 12 Mbps |
| Discord | 1280×720 | 30 | H.264 | AAC | 4 Mbps |
| 720p | 1280×720 | 30 | H.264 | AAC | 5 Mbps |
| 1080p | 1920×1080 | 30 | H.264 | AAC | 12 Mbps |
| 1440p | 2560×1440 | 30 | H.264 | AAC | 20 Mbps |
| 4K | 3840×2160 | 30 | H.264 | AAC | 40 Mbps |

## Hardware Encoding

- NVENC (NVIDIA), AMF (AMD), QSV (Intel) are used when available
- Software H.264 fallback via Media Foundation
- Detected at export time, not build time

## Export UX

- Progress percentage
- Frames rendered / total frames
- Time elapsed
- Estimated time remaining
- Cancel button
- "Open video" and "Open folder" on completion
