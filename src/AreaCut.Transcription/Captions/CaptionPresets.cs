using AreaCut.Core.Models;

namespace AreaCut.Transcription.Captions;

/// <summary>
/// Visual presets for caption/subtitle rendering.
/// </summary>
public sealed record CaptionPreset(
    string Id,
    string Name,
    string FontFamily,
    double FontSize,
    string FontWeight,
    string Foreground,
    string? Background,
    string Alignment,
    bool HasOutline,
    string? OutlineColor,
    double OutlineThickness,
    bool HasShadow
)
{
    public static readonly CaptionPreset Minimal = new("minimal", "Minimal",
        "Segoe UI", 42, "SemiBold", "#FFFFFF", null, "Center",
        true, "#000000", 2, false);

    public static readonly CaptionPreset Classic = new("classic", "Classic",
        "Segoe UI", 48, "Bold", "#FFFFFF", "#000000CC", "Center",
        true, "#000000", 3, true);

    public static readonly CaptionPreset Bold = new("bold", "Bold",
        "Impact", 56, "Bold", "#FFFFFF", null, "Center",
        true, "#000000", 4, true);

    public static readonly CaptionPreset Retro = new("retro", "Retro",
        "Courier New", 44, "Bold", "#FFFF00", null, "Center",
        true, "#000000", 3, false);

    public static readonly CaptionPreset Subtitle = new("subtitle", "Subtitle",
        "Segoe UI", 40, "Regular", "#FFFFFF", "#000000AA", "Center",
        false, null, 0, false);

    public static IReadOnlyList<CaptionPreset> AllPresets => new[]
    {
        Minimal, Classic, Bold, Retro, Subtitle
    };
}
