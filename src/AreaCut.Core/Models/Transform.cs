using System;

namespace AreaCut.Core.Models;

/// <summary>
/// 2D transform: position, scale, rotation, opacity.
/// All values are relative to the canvas coordinate system.
/// </summary>
public sealed record Transform
{
    public double OffsetX { get; init; }
    public double OffsetY { get; init; }
    public double ScaleX { get; init; } = 1.0;
    public double ScaleY { get; init; } = 1.0;
    public double RotationDegrees { get; init; }
    public double Opacity { get; init; } = 1.0;

    public static Transform Identity => new();

    public Transform WithOffsetX(double x) => this with { OffsetX = x };
    public Transform WithOffsetY(double y) => this with { OffsetY = y };
    public Transform WithScaleX(double s) => this with { ScaleX = s };
    public Transform WithScaleY(double s) => this with { ScaleY = s };
    public Transform WithRotation(double degrees) => this with { RotationDegrees = degrees };
    public Transform WithOpacity(double o) => this with { Opacity = Math.Clamp(o, 0, 1) };
}
