namespace AreaCut.Core.Models;

/// <summary>
/// Crop rectangle as normalized ratios (0-1) of the source dimensions.
/// </summary>
public sealed record Crop
{
    public double Left { get; init; }
    public double Top { get; init; }
    public double Right { get; init; } = 1.0;
    public double Bottom { get; init; } = 1.0;

    public static Crop None => new();

    public double Width => Right - Left;
    public double Height => Bottom - Top;

    public bool IsNone => Left == 0 && Top == 0 && Math.Abs(Right - 1) < double.Epsilon && Math.Abs(Bottom - 1) < double.Epsilon;
}
