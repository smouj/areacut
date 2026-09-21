#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Install AreaCut from a portable build.
.DESCRIPTION
    Installs AreaCut to %LOCALAPPDATA%\Programs\AreaCut with Start Menu shortcut.
#>
param(
    [switch]$NoDesktopShortcut
)

$ErrorActionPreference = "Stop"
$InstallDir = Join-Path $env:LOCALAPPDATA "Programs\AreaCut"
$StartMenuDir = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs"

Write-Host "=== AreaCut Installation ===" -ForegroundColor Cyan

# Find the publish directory
$PublishDir = Join-Path $PSScriptRoot "..\artifacts\AreaCut-0.1.0-win-x64"
if (-not (Test-Path $PublishDir)) {
    # Try current directory
    $PublishDir = $PSScriptRoot
}

if (-not (Test-Path (Join-Path $PublishDir "AreaCut.exe"))) {
    Write-Host "Error: AreaCut.exe not found. Run PUBLISH_PORTABLE.ps1 first." -ForegroundColor Red
    exit 1
}

# Create install directory
if (-not (Test-Path $InstallDir)) {
    New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null
}

# Copy files
Write-Host "Installing to $InstallDir..." -ForegroundColor Yellow
Copy-Item (Join-Path $PublishDir "*") $InstallDir -Recurse -Force

# Create Start Menu shortcut
$WScript = New-Object -ComObject WScript.Shell
$Shortcut = $WScript.CreateShortcut((Join-Path $StartMenuDir "AreaCut.lnk"))
$Shortcut.TargetPath = Join-Path $InstallDir "AreaCut.exe"
$Shortcut.WorkingDirectory = $InstallDir
$Shortcut.Description = "AreaCut - Local-first video editor"
$Shortcut.Save()

# Create Desktop shortcut (optional)
if (-not $NoDesktopShortcut) {
    $DesktopShortcut = Join-Path ([Environment]::GetFolderPath("Desktop")) "AreaCut.lnk"
    $DesktopLnk = $WScript.CreateShortcut($DesktopShortcut)
    $DesktopLnk.TargetPath = Join-Path $InstallDir "AreaCut.exe"
    $DesktopLnk.WorkingDirectory = $InstallDir
    $DesktopLnk.Description = "AreaCut - Local-first video editor"
    $DesktopLnk.Save()
}

Write-Host "AreaCut installed successfully!" -ForegroundColor Green
Write-Host "Location: $InstallDir" -ForegroundColor Green
Write-Host "Run 'AreaCut.exe' or use the Start Menu shortcut." -ForegroundColor Green
