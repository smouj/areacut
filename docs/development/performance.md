# Performance

## Targets

| Metric | Target |
|--------|--------|
| UI refresh rate | 60 FPS when reasonable |
| Timeline scrolling | Immediate |
| Scrub responsiveness | <100ms for common codecs |
| Thumbnail generation | Progressive, non-blocking |
| Memory usage | Controlled, no full-video preload |
| Startup time | <3 seconds |
| Export (1080p 30fps) | <2× real-time |

## Strategies

- **All decode/render off UI thread** — `CancellationToken` throughout
- **GPU-accelerated preview** — D3D11 composition, no CPU round-trips
- **Progressive thumbnails** — generated on demand, cached
- **Proxy workflow** — edit on 720p, export from original
- **Tick-based timing** — no floating-point drift
- **COM/D3D cleanup** — explicit `Dispose` patterns
- **No full-video preload** — decode on demand
