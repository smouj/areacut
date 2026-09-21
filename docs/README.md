# AreaCut documentation

AreaCut is a local-first Windows video editor. The documents here are split by
audience: what a user needs, what a contributor needs, and how the thing is
built inside.

> **Status:** the editing engine is implemented and unit-tested; the WinUI 3
> interface is still a shell and cannot be used for real editing yet. See the
> [roadmap](development/roadmap.md).

## For users

| Document | What it covers |
| --- | --- |
| [install.md](user/install.md) | How to get AreaCut — today and once the first release ships |
| [supported-formats.md](user/supported-formats.md) | Video, audio and image files AreaCut can import |
| [shortcuts.md](user/shortcuts.md) | Keyboard and mouse reference, with implementation status |
| [privacy.md](user/privacy.md) | What AreaCut does, and does not do, with your data |

## For contributors

| Document | What it covers |
| --- | --- |
| [../CONTRIBUTING.md](../CONTRIBUTING.md) | Bug reports, code style, commit messages, pull requests |
| [building.md](development/building.md) | Build, run, test and package from source |
| [testing.md](development/testing.md) | Test layers, coverage and the manual smoke test |
| [roadmap.md](development/roadmap.md) | What is done, in progress and planned |

## Architecture and internals

| Document | What it covers |
| --- | --- |
| [architecture.md](development/architecture.md) | Projects, dependency direction, key design decisions |
| [project-format.md](development/project-format.md) | The `.areacut` file format |
| [timeline.md](development/timeline.md) | Track types, clip model, snapping, composition |
| [media-pipeline.md](development/media-pipeline.md) | Import, probing, thumbnails, waveforms, proxies |
| [export-pipeline.md](development/export-pipeline.md) | Render pipeline, export presets, hardware encoding |
| [performance.md](development/performance.md) | Performance targets and the strategies behind them |

Also relevant: [assets/BRAND.md](../assets/BRAND.md) for the visual identity and
[THIRD_PARTY.md](../THIRD_PARTY.md) for dependency licences.
