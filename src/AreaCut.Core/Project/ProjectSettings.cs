namespace AreaCut.Core.Project;

/// <summary>
/// Project-level settings that affect export and behavior.
/// </summary>
public sealed record ProjectSettings
{
    public string ExportPath { get; init; } = "";
    public string ExportFileName { get; init; } = "export";
    public bool UseHardwareEncoding { get; init; } = true;
    public int ExportQuality { get; init; } = 80; // 1-100
    public bool AutoSaveEnabled { get; init; } = true;
    public int AutoSaveIntervalSeconds { get; init; } = 120;
    public long MaxCacheSizeMb { get; init; } = 2048;
    public string CachePath { get; init; } = ".cache";
}
