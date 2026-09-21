using AreaCut.Core.Models;

namespace AreaCut.Export.Presets;

/// <summary>
/// Export presets for common social platforms and resolutions.
/// </summary>
public sealed record ExportPreset(
    string Name,
    int Width,
    int Height,
    int Fps,
    string VideoCodec,
    string AudioCodec,
    int VideoBitrateKbps,
    int AudioBitrateKbps,
    bool UseHardwareEncoding,
    string? Category = null
)
{
    public static readonly ExportPreset TikTok = new("TikTok", 1080, 1920, 30, "H264", "AAC", 8000, 128, true, "Social");
    public static readonly ExportPreset InstagramReel = new("Instagram Reel", 1080, 1920, 30, "H264", "AAC", 8000, 128, true, "Social");
    public static readonly ExportPreset YouTubeShorts = new("YouTube Shorts", 1080, 1920, 30, "H264", "AAC", 10000, 128, true, "Social");
    public static readonly ExportPreset YouTube1080 = new("YouTube 1080p", 1920, 1080, 30, "H264", "AAC", 12000, 192, true, "YouTube");
    public static readonly ExportPreset Discord = new("Discord", 1280, 720, 30, "H264", "AAC", 4000, 128, true, "Social");
    public static readonly ExportPreset Original720 = new("720p", 1280, 720, 30, "H264", "AAC", 5000, 128, true, "Resolution");
    public static readonly ExportPreset Original1080 = new("1080p", 1920, 1080, 30, "H264", "AAC", 12000, 192, true, "Resolution");
    public static readonly ExportPreset Original1440 = new("1440p", 2560, 1440, 30, "H264", "AAC", 20000, 192, true, "Resolution");
    public static readonly ExportPreset Original4K = new("4K", 3840, 2160, 30, "H264", "AAC", 40000, 192, true, "Resolution");

    public static ExportPreset Custom(int width, int height, int fps, int videoBitrateKbps) =>
        new("Custom", width, height, fps, "H264", "AAC", videoBitrateKbps, 192, true, "Custom");

    public static IReadOnlyList<ExportPreset> AllPresets => new[]
    {
        TikTok, InstagramReel, YouTubeShorts, YouTube1080, Discord,
        Original720, Original1080, Original1440, Original4K
    };
}
