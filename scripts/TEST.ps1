#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Run AreaCut tests.
#>
param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$RootDir = Split-Path -Parent $PSScriptRoot

Write-Host "=== AreaCut Tests ===" -ForegroundColor Cyan

# Core tests (platform-independent)
Write-Host "Running AreaCut.Core.Tests..." -ForegroundColor Yellow
dotnet test "$RootDir/tests/AreaCut.Core.Tests/AreaCut.Core.Tests.csproj" --configuration $Configuration --no-restore --verbosity normal
if ($LASTEXITCODE -ne 0) { Write-Host "Core tests failed!" -ForegroundColor Red; exit 1 }

Write-Host "All tests passed!" -ForegroundColor Green
