namespace AreaCut.Core.Models;

/// <summary>
/// Canvas/output presets for social platforms.
/// </summary>
public sealed record SocialPreset(
    string Name,
    int Width,
    int Height,
    int Fps,
    string Codec,
    string AudioCodec,
    string? SafeAreaProfile = null
)
{
    public double AspectRatio => (double)Width / Height;

    public static readonly SocialPreset Vertical = new("Vertical / 9:16", 1080, 1920, 30, "H264", "AAC", "tiktok");
    public static readonly SocialPreset InstagramPortrait = new("Instagram Portrait / 4:5", 1080, 1350, 30, "H264", "AAC", "instagram");
    public static readonly SocialPreset Square = new("Square / 1:1", 1080, 1080, 30, "H264", "AAC");
    public static readonly SocialPreset YouTube = new("YouTube / 16:9", 1920, 1080, 30, "H264", "AAC", "youtube");
    public static readonly SocialPreset Shorts = new("YouTube Shorts / 9:16", 1080, 1920, 30, "H264", "AAC", "youtube-shorts");
    public static readonly SocialPreset Discord = new("Discord / 16:9", 1280, 720, 30, "H264", "AAC");

    public static SocialPreset Original(int width, int height, int fps) => new("Original", width, height, fps, "H264", "AAC");
}
