using System.Collections.Generic;
using AreaCut.Core.Models;
using AreaCut.Core.Time;

namespace AreaCut.Rendering.Composition;

/// <summary>
/// A single composited frame ready for preview or export rendering.
/// </summary>
public sealed class CompositedFrame
{
    public TimeStamp Timestamp { get; set; }
    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
    public List<VideoLayer> VideoLayers { get; } = new();
    public List<TextLayer> TextLayers { get; } = new();
    public List<CaptionLayer> CaptionLayers { get; } = new();
}

public sealed class VideoLayer
{
    public string ClipId { get; set; } = "";
    public string SourceMediaId { get; set; } = "";
    public TimeStamp SourceTime { get; set; }
    public Transform Transform { get; set; } = Transform.Identity;
    public Crop Crop { get; set; } = Crop.None;
    public double Opacity { get; set; } = 1.0;
    public int ZOrder { get; set; }
}

public sealed class TextLayer
{
    public string TextClipId { get; set; } = "";
    public string Content { get; set; } = "";
    public string FontFamily { get; set; } = "Segoe UI";
    public double FontSize { get; set; } = 48;
    public string FontWeight { get; set; } = "Bold";
    public string Alignment { get; set; } = "Center";
    public string Foreground { get; set; } = "#FFFFFF";
    public string Background { get; set; } = "transparent";
    public double Opacity { get; set; } = 1.0;
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double Scale { get; set; } = 1.0;
    public double RotationDegrees { get; set; }
    public bool HasShadow { get; set; }
    public bool HasOutline { get; set; }
    public string? OutlineColor { get; set; }
    public double OutlineThickness { get; set; }
}

public sealed class CaptionLayer
{
    public string CaptionId { get; set; } = "";
    public string Text { get; set; } = "";
    public string? Style { get; set; }
}
