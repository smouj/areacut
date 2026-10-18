<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="assets/logo-dark.png">
    <source media="(prefers-color-scheme: light)" srcset="assets/logo.png">
    <img src="assets/logo.png" alt="Voryda Cut" width="220">
  </picture>
</p>

<h1 align="center">Voryda Cut</h1>

<p align="center">
  <strong>Cut. Reframe. Caption. Export.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/platform-Windows%2010%20%7C%2011-0078D4?style=flat-square" alt="Windows 10 and 11">
  <img src="https://img.shields.io/badge/.NET-8-512BD4?style=flat-square" alt=".NET 8">
  <img src="https://img.shields.io/badge/UI-WinUI%203-0078D4?style=flat-square" alt="WinUI 3">
  <img src="https://img.shields.io/badge/project-.areacut-555?style=flat-square" alt=".areacut project format">
  <img src="https://img.shields.io/badge/status-pre--alpha-D97706?style=flat-square" alt="Pre-alpha">
  <img src="https://img.shields.io/badge/license-MIT-2EA44F?style=flat-square" alt="MIT License">
</p>

<p align="center">
  <a href="#overview">Overview</a> ·
  <a href="#current-status">Status</a> ·
  <a href="#implemented-foundation">Foundation</a> ·
  <a href="#architecture">Architecture</a> ·
  <a href="#build-and-test">Build</a> ·
  <a href="#privacy">Privacy</a>
</p>

## Overview

**Voryda Cut** is a Windows-native video editor under development for short-form and standard social video workflows. Its project model, editing commands, time math, non-destructive project serialization, social presets, safe-area definitions and caption parsing are already implemented. The editor application itself is **not yet usable for real video editing**.

The current WinUI 3 window is a shell around the editing foundation. Import, functional timeline editing, preview, audio playback and final H.264/AAC export are not complete. There is no public build or GitHub Release.

## Current status

The repository is **pre-alpha**. The distinction between implemented foundation code and user-facing functionality is important:

| Area | State | Current repository state |
| --- | --- | --- |
| Core project model | **Implemented** | Clips, tracks, transforms, crop, text, captions, transitions and fades. |
| Time math | **Implemented** | Tick-based timestamps, rational frame rates and half-open ranges. |
| Editing commands | **Implemented** | Add, delete, move, trim, split, transform, volume and text commands. |
| Undo / redo | **Implemented** | Command-based undo/redo stack. |
| `.areacut` project format | **Implemented** | Versioned JSON, migration, atomic save and crash-recovery groundwork. |
| Social presets and safe areas | **Implemented** | Preset data and safe-area profiles exist in Core. |
| SRT and caption presets | **Implemented** | Parser/exporter and caption presets are present. |
| WinUI 3 editor shell | **Partial** | Window and layout exist; several menu actions are intentionally disabled. |
| Media decode / probing | **Partial / stubbed** | Media Foundation layer exists but is not a completed editing workflow. |
| GPU preview | **Partial / stubbed** | D3D11/D2D composition scaffolding exists; usable preview is not complete. |
| Functional timeline UI | **Not implemented** | No complete import/selection/trim/split workflow from the interface. |
| H.264/AAC MP4 export | **Not implemented** | Export pipeline is not usable from the product. |
| Audio playback / mixing | **Not implemented** | WASAPI-facing groundwork exists, but the product workflow is incomplete. |
| Voryda Rec handoff | **Not implemented** | Planned for a later milestone. |
| GitHub Release | **Not published** | GitHub Releases is currently empty. |

The active roadmap is maintained in [docs/development/roadmap.md](docs/development/roadmap.md).

> **Do not use the current repository as a production video editor.** The application shell is intentionally ahead of several runtime implementations.

## Implemented foundation

### Non-destructive project model

Voryda Cut stores editing decisions rather than modifying source media. The `.areacut` project model tracks media references, clip positions, transforms, crops, captions and other project state.

Implemented Core areas include:

- clips and tracks;
- transforms and crops;
- text clips and caption items;
- transitions and fades;
- frame-rate and timestamp math;
- snapping;
- command-based editing operations;
- undo and redo;
- project serialization and migration;
- autosave and crash-recovery groundwork;
- social output presets;
- safe-area profiles;
- SRT parsing and export.

### Current application shell

The WinUI 3 shell already contains the intended editor regions:

| Region | Present today |
| --- | --- |
| Menu bar | Yes; incomplete actions are disabled and labelled as not implemented. |
| Media panel | Present as a shell. |
| Preview panel | Present with placeholder content and controls. |
| Properties panel | Present as a shell. |
| Timeline area | Present visually, but not yet a functional editing timeline. |
| Status bar | Present. |

The UI currently exposes **New Project** and basic shell behavior. Open, Save, Save As, Export and Safe Areas are disabled in the present XAML because their product workflows are not ready.

## Planned editing workflow

The target workflow is:

```text
Import media
     │
     ▼
Timeline editing
 trim / split / move
     │
     ▼
Reframe and captions
     │
     ▼
Preview and audio
     │
     ▼
H.264 / AAC MP4 export
```

This describes the direction of the product, not the current functional state. See the roadmap for milestone ownership.

## Architecture

The solution is split into seven projects:

| Project | Responsibility | Current maturity |
| --- | --- | --- |
| `AreaCut.App` | WinUI 3 shell, window, menus and keyboard handling. | Shell |
| `AreaCut.Core` | Project model, timeline data, commands, undo/redo, serialization and time math. | Implemented / tested foundation |
| `AreaCut.Media` | Media Foundation runtime, metadata, thumbnails and waveforms. | Partial / stubbed |
| `AreaCut.Rendering` | D3D11 composition and preview clock. | Partial / stubbed |
| `AreaCut.Audio` | WASAPI-facing playback, mixing and waveform cache. | Groundwork |
| `AreaCut.Export` | Render pipeline, Media Foundation encoding and export presets. | Groundwork |
| `AreaCut.Transcription` | Caption presets, SRT handling and local transcription interface. | Partial |

The `AreaCut.*` namespaces, solution names and `.areacut` file extension are retained for compatibility while the public product name moves to **Voryda Cut**.

## Requirements

For development and the future desktop application:

- Windows 10 version 2004 (build 19041) or Windows 11, x64.
- Direct3D 11 capable graphics hardware.
- Windows Media Foundation.
- .NET 8 SDK.
- A native Windows filesystem path for the WinUI 3 application build.

## Build and test

Clone the repository:

```powershell
git clone https://github.com/smouj/areacut
cd areacut
```

Use the repository build script:

```powershell
.\scripts\BUILD.ps1
```

Run the application project:

```powershell
dotnet run --project src\AreaCut.App\AreaCut.App.csproj
```

Run the Core test suite:

```powershell
dotnet test tests\AreaCut.Core.Tests\AreaCut.Core.Tests.csproj -c Debug
```

The repository has a Windows GitHub Actions workflow at [`.github/workflows/ci.yml`](.github/workflows/ci.yml) that restores the solution, builds Debug and Release configurations, verifies required brand assets and runs the Core tests.

Packaging scripts are also present under [scripts/](scripts/), but there is currently **no public release build**.

## Documentation

| Topic | Documentation |
| --- | --- |
| Documentation index | [docs/README.md](docs/README.md) |
| Architecture | [docs/development/architecture.md](docs/development/architecture.md) |
| Building | [docs/development/building.md](docs/development/building.md) |
| Roadmap | [docs/development/roadmap.md](docs/development/roadmap.md) |
| Timeline design | [docs/development/timeline.md](docs/development/timeline.md) |
| Project format | [docs/development/project-format.md](docs/development/project-format.md) |
| Media pipeline | [docs/development/media-pipeline.md](docs/development/media-pipeline.md) |
| Export pipeline | [docs/development/export-pipeline.md](docs/development/export-pipeline.md) |
| Testing | [docs/development/testing.md](docs/development/testing.md) |
| Supported formats | [docs/user/supported-formats.md](docs/user/supported-formats.md) |
| Shortcuts | [docs/user/shortcuts.md](docs/user/shortcuts.md) |
| Privacy | [docs/user/privacy.md](docs/user/privacy.md) |

## Privacy

Voryda Cut is designed as a local desktop editor. The current application code does not require an account, analytics service or cloud storage. Project files and media remain on the user's disk, and the `.areacut` project format stores references to source files rather than uploading them.

See [docs/user/privacy.md](docs/user/privacy.md) for the repository's privacy notes.

## License

The source code is distributed under the **MIT License**. See [LICENSE](LICENSE). Third-party notices are documented in [THIRD_PARTY.md](THIRD_PARTY.md).

---

<p align="center">
  <strong>Voryda</strong><br>
  Focused Windows utilities.
</p>

<p align="center">
  <a href="https://github.com/smouj/areasnap">Snap</a>
  ·
  <a href="https://github.com/smouj/arearec">Rec</a>
  ·
  <a href="https://github.com/smouj/areadown">Down</a>
  ·
  <a href="https://github.com/smouj/areacut">Cut</a>
</p>
