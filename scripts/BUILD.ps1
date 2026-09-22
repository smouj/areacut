#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build AreaCut solution.
.DESCRIPTION
    Restores dependencies and builds the AreaCut solution in Release configuration.
#>
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$RootDir = Split-Path -Parent $PSScriptRoot

Write-Host "=== AreaCut Build ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration"
Write-Host "Runtime: $Runtime"
Write-Host ""

# Restore
Write-Host "[1/3] Restoring dependencies..." -ForegroundColor Yellow
dotnet restore "$RootDir/AreaCut.sln" --runtime $Runtime
if ($LASTEXITCODE -ne 0) { Write-Host "Restore failed!" -ForegroundColor Red; exit 1 }

# Build
Write-Host "[2/3] Building solution..." -ForegroundColor Yellow
dotnet build "$RootDir/AreaCut.sln" --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { Write-Host "Build failed!" -ForegroundColor Red; exit 1 }

# Verify
Write-Host "[3/3] Verifying build..." -ForegroundColor Yellow
$appDll = Join-Path $RootDir "src/AreaCut.App/bin/x64/$Configuration/net8.0-windows10.0.19041.0/AreaCut.dll"
if (Test-Path $appDll) {
    Write-Host "Build succeeded!" -ForegroundColor Green
} else {
    Write-Host "Warning: Expected output not found at $appDll" -ForegroundColor Yellow
}
