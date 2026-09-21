namespace AreaCut.Core.ProjectModel;

/// <summary>
/// Canvas specification: dimensions, frame rate, and aspect ratio.
/// </summary>
public sealed record CanvasSpec
{
    public int Width { get; init; } = 1080;
    public int Height { get; init; } = 1920;
    public int Fps { get; init; } = 30;

    public double AspectRatio => (double)Width / Height;

    public bool IsVertical => Height > Width;
    public bool IsSquare => Width == Height;

    public static CanvasSpec Vertical1080 => new() { Width = 1080, Height = 1920, Fps = 30 };
    public static CanvasSpec InstagramPortrait => new() { Width = 1080, Height = 1350, Fps = 30 };
    public static CanvasSpec Square1080 => new() { Width = 1080, Height = 1080, Fps = 30 };
    public static CanvasSpec YouTube1080 => new() { Width = 1920, Height = 1080, Fps = 30 };
}
