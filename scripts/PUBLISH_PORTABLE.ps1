#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publish AreaCut as a self-contained portable win-x64 build.
.DESCRIPTION
    Creates a self-contained AreaCut build that doesn't require .NET runtime.
    Output goes to artifacts/AreaCut-win-x64/ with a ZIP archive.
#>
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Version = "0.1.0"
)

$ErrorActionPreference = "Stop"
$RootDir = Split-Path -Parent $PSScriptRoot
$ArtifactsDir = Join-Path $RootDir "artifacts"
$PublishDir = Join-Path $ArtifactsDir "AreaCut-$Version-$Runtime"

Write-Host "=== AreaCut Portable Publish ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration"
Write-Host "Runtime: $Runtime"
Write-Host "Version: $Version"
Write-Host ""

# Build first
& "$PSScriptRoot/BUILD.ps1" -Configuration $Configuration -Runtime $Runtime
if ($LASTEXITCODE -ne 0) { exit 1 }

# Publish
Write-Host "Publishing..." -ForegroundColor Yellow
dotnet publish "$RootDir/src/AreaCut.App/AreaCut.App.csproj" `
    --configuration $Configuration `
    --runtime $Runtime `
    --self-contained true `
    --output $PublishDir `
    -p:Version=$Version `
    -p:Platform=x64 `
    -p:PublishTrimmed=false

if ($LASTEXITCODE -ne 0) { Write-Host "Publish failed!" -ForegroundColor Red; exit 1 }

# Copy assets
$AssetsDir = Join-Path $RootDir "assets"
if (Test-Path $AssetsDir) {
    Copy-Item (Join-Path $AssetsDir "AreaCut.ico") $PublishDir -ErrorAction SilentlyContinue
}

# Create ZIP
Write-Host "Creating archive..." -ForegroundColor Yellow
$ZipPath = Join-Path $ArtifactsDir "AreaCut-$Version-$Runtime.zip"
if (Test-Path $ZipPath) { Remove-Item $ZipPath }
Compress-Archive -Path $PublishDir -DestinationPath $ZipPath

# SHA-256
$Hash = (Get-FileHash -Algorithm SHA256 $ZipPath).Hash
$HashPath = "$ZipPath.sha256"
Set-Content $HashPath $Hash

Write-Host ""
Write-Host "Published to: $PublishDir" -ForegroundColor Green
Write-Host "Archive: $ZipPath" -ForegroundColor Green
Write-Host "SHA-256: $Hash" -ForegroundColor Green
