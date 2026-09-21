# Building AreaCut from source

## Prerequisites

| | |
| --- | --- |
| System | Windows 10 (2004 / build 19041) or Windows 11, x64 |
| SDK | [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) |
| UI workload | Windows App SDK / WinUI 3 tooling (installed with the .NET SDK; Visual Studio 2022 with the *Windows App SDK* workload also works) |
| PowerShell | Windows PowerShell 5.1 or PowerShell 7+ |

## The WSL constraint

`AreaCut.App` is a WinUI 3 project. The XAML compiler **cannot run from a
`\\wsl.localhost\...` UNC path**, so the application project must be built from a
native Windows path. Clone the repository on the Windows side, or copy it to a
local Windows folder before building the app.

The six library projects (`Core`, `Media`, `Rendering`, `Audio`, `Export`,
`Transcription`) and the test projects are plain .NET 8 and build anywhere,
including WSL.

## Build

From Windows PowerShell, in the repository root:

```powershell
.\scripts\BUILD.ps1
```

The script restores with `--runtime win-x64`, builds the whole solution in
`Release`, and checks that `AreaCut.dll` was produced. Options:

```powershell
.\scripts\BUILD.ps1 -Configuration Debug
.\scripts\BUILD.ps1 -Runtime win-x64
```

## Run

```powershell
dotnet run --project src\AreaCut.App\AreaCut.App.csproj
```

## Test

```powershell
.\scripts\TEST.ps1
```

This runs `tests/AreaCut.Core.Tests`, which is platform-independent. The
Windows-only integration tests described in [testing.md](testing.md) are not
part of this script yet. To run a single project directly:

```powershell
dotnet test tests\AreaCut.Core.Tests\AreaCut.Core.Tests.csproj -c Debug
```

## Package a portable build

```powershell
.\scripts\PUBLISH_PORTABLE.ps1 -Version 0.1.0
```

The script builds first, then publishes `src\AreaCut.App` as a self-contained
(`--self-contained true`, `PublishTrimmed=false`) `win-x64` app into
`artifacts/AreaCut-<version>-win-x64/`, copies `assets/AreaCut.ico` next to the
executable, and produces:

- `artifacts/AreaCut-<version>-win-x64.zip`
- `artifacts/AreaCut-<version>-win-x64.zip.sha256`

## Install a portable build locally

```powershell
.\scripts\INSTALL_PORTABLE.ps1
```

Copies the publish output into `%LOCALAPPDATA%\Programs\AreaCut`, creates a Start
Menu shortcut and, unless you pass `-NoDesktopShortcut`, a desktop shortcut.

## Regenerate the application icon

```powershell
.\scripts\BUILD_APP_ICON.ps1
```

Renders `assets/mark.png` onto the 256×256 `#0F0F12` canvas and writes
`assets/AreaCut.ico`. Pass `-SourcePath` / `-OutputPath` to override either end.

## Solution layout

```
AreaCut.sln
├── src/AreaCut.App            WinUI 3 shell            (Windows-only build)
├── src/AreaCut.Core           project model, timeline, commands
├── src/AreaCut.Media          Media Foundation decode, probing, caches
├── src/AreaCut.Rendering      D3D11 / D2D composition, preview clock
├── src/AreaCut.Audio          WASAPI playback, mixing, waveform cache
├── src/AreaCut.Export         encoder pipeline, presets, progress
├── src/AreaCut.Transcription  SRT parsing, caption presets, local Whisper
└── tests/                     Core, Media and Export test projects
```

See [architecture.md](architecture.md) for the dependency direction and the
decisions behind it.
