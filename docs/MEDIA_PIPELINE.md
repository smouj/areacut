# Media Pipeline

## Import Flow

```
User drops file → MediaProber.Probe() → MediaReference created → Added to project
```

`MediaProber` uses `MediaFoundationReader` to extract:
- Duration, resolution, FPS, codec, bitrate
- Audio channels, sample rate, orientation

Unsupported formats get a clear error message, not a crash.

## Supported Formats

| Format | Container | Video Codec | Audio Codec | Notes |
|--------|-----------|-------------|-------------|-------|
| MP4 | ✅ | H.264 | AAC | Primary format |
| M4V | ✅ | H.264 | AAC | |
| MOV | ⚠️ | H.264 | AAC | MF-dependent |
| AVI | ⚠️ | Varies | Varies | Limited |
| PNG | ✅ | — | — | Image clip |
| JPEG | ✅ | — | — | Image clip |
| WebP | ⚠️ | — | — | MF-dependent |
| MP3 | ✅ | — | MP3 | Audio-only |
| WAV | ✅ | — | PCM | Audio-only |
| M4A | ✅ | — | AAC | Audio-only |

Formats marked ⚠️ may not work on all Windows installations. AreaCut checks `MediaFoundationCapabilities` and reports clear errors.

## Thumbnail Generation

- `ThumbnailGenerator` creates 160×90 thumbnails progressively
- Cached in `.cache/thumbnails/`
- Never blocks the UI thread
- Cancelable via `CancellationToken`

## Waveform Generation

- `WaveformGenerator` extracts RMS amplitude data
- Cached in `.cache/waveforms/`
- 1000 samples per clip by default
- Displayed on timeline tracks

## Proxy Workflow

For 4K+ content:
1. Generate 720p proxy from original
2. Edit using proxy
3. Export reads from original (never from proxy unless user explicitly chooses)
4. Proxy generation is cancelable and shows progress
