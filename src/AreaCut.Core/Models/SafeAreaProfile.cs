namespace AreaCut.Core.Models;

/// <summary>
/// Safe area geometry for a social platform, expressed as margins
/// from the canvas edges as fractions (0-1).
/// These regions show platform UI overlays that obscure content.
/// </summary>
public sealed record SafeAreaProfile(
    string Id,
    string Name,
    double TopMargin,
    double BottomMargin,
    double LeftMargin,
    double RightMargin
)
{
    public static readonly SafeAreaProfile TikTok = new("tiktok", "TikTok", 0.05, 0.15, 0.05, 0.05);
    public static readonly SafeAreaProfile InstagramReels = new("instagram", "Instagram Reels", 0.10, 0.15, 0.05, 0.05);
    public static readonly SafeAreaProfile YouTubeShorts = new("youtube-shorts", "YouTube Shorts", 0.08, 0.12, 0.03, 0.03);
}
