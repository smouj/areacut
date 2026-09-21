# Installing AreaCut

> **There is no public build yet.** AreaCut is pre-alpha: the engine is written
> and tested, but the interface is still a shell and cannot be used for real
> editing. This page describes what installation will look like, and how to run
> the current source today.

## Requirements

| | |
| --- | --- |
| System | Windows 10 (2004 / build 19041) or Windows 11, x64 |
| Graphics | Direct3D 11 capable GPU — a software fallback exists |
| Media | Media Foundation, included in Windows |
| .NET | Not required for the packaged build; .NET 8 SDK only to build from source |

## The packaged release (planned)

When the first release is published it will be a portable, self-contained
package — no installer, no administrator rights and no separate .NET runtime to
install:

1. Download `AreaCut-win-x64.zip` from
   [Releases](https://github.com/smouj/areacut/releases).
2. Check it against the SHA-256 checksum published next to the archive.
3. Extract it anywhere and run `AreaCut.exe`.

An optional installer script (`scripts/INSTALL_PORTABLE.ps1`) will copy the
build into `%LOCALAPPDATA%\Programs\AreaCut` and add a Start Menu shortcut.

## Running the current source

The current build is not a product: expect a window shell, not a working editor.
If you want to see it anyway, follow
[../development/building.md](../development/building.md). The WinUI 3 project
must be built from a native Windows path — the XAML compiler cannot run from a
`\\wsl.localhost\...` UNC path.

## Uninstalling

The packaged build will be portable: delete the folder. If you used
`INSTALL_PORTABLE.ps1`, delete `%LOCALAPPDATA%\Programs\AreaCut` and the Start
Menu shortcut.

Projects (`.areacut`), autosave sidecars and caches live next to your project
files and in `.cache/` — deleting the application never touches your media.
