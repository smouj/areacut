using System;
using AreaCut.Core.Models;
using AreaCut.Core.Project;
using AreaCut.Core.Time;

namespace AreaCut.Rendering.Composition;

/// <summary>
/// GPU compositor that renders a frame from the timeline at a given timestamp.
/// Pipeline: Source decode → D3D11 texture → GPU composition → D2D/DWrite overlays → Preview/Export
/// Preview and export share the same composition rules to avoid discrepancies.
/// </summary>
public sealed class CompositionEngine : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Render a frame at the given timestamp for the given canvas.
    /// Returns a composited frame description (GPU textures in production).
    /// </summary>
    public CompositedFrame Compose(Project project, TimeStamp timestamp)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var frame = new CompositedFrame
        {
            Timestamp = timestamp,
            CanvasWidth = project.Canvas.Width,
            CanvasHeight = project.Canvas.Height
        };

        // Compose video clips (back to front by Z-order)
        foreach (var track in project.Tracks)
        {
            if (!track.IsVisible) continue;

            foreach (var clip in project.GetClipsOnTrack(track.Id))
            {
                if (timestamp < clip.TimelineStart || timestamp >= clip.TimelineEnd) continue;

                // Calculate source time for this clip at the current timestamp
                var timelineOffset = timestamp.Subtract(clip.TimelineStart);
                var sourceTime = clip.SourceIn.Add(timelineOffset.Multiply(clip.PlaybackRate));

                frame.VideoLayers.Add(new VideoLayer
                {
                    ClipId = clip.Id,
                    SourceMediaId = clip.SourceMediaId,
                    SourceTime = sourceTime,
                    Transform = clip.Transform,
                    Crop = clip.Crop,
                    Opacity = clip.Opacity,
                    ZOrder = track.ZOrder
                });
            }
        }

        // Compose text overlays
        foreach (var textClip in project.TextClips)
        {
            if (timestamp < textClip.TimelineStart || timestamp >= textClip.TimelineEnd) continue;

            var textOpacity = textClip.Opacity;

            // Apply fade in/out
            if (textClip.FadeInDuration != null)
            {
                var fadeInDuration = textClip.FadeInDuration.Value;
                var fadeInEnd = textClip.TimelineStart.Add(fadeInDuration);
                if (timestamp < fadeInEnd)
                {
                    var progress = timestamp.Subtract(textClip.TimelineStart).TotalSeconds / fadeInDuration.TotalSeconds;
                    textOpacity *= Math.Clamp(progress, 0, 1);
                }
            }
            if (textClip.FadeOutDuration != null)
            {
                var fadeOutDuration = textClip.FadeOutDuration.Value;
                var fadeOutStart = textClip.TimelineEnd.Subtract(fadeOutDuration);
                if (timestamp > fadeOutStart)
                {
                    var progress = 1.0 - (timestamp.Subtract(fadeOutStart).TotalSeconds / fadeOutDuration.TotalSeconds);
                    textOpacity *= Math.Clamp(progress, 0, 1);
                }
            }

            frame.TextLayers.Add(new TextLayer
            {
                TextClipId = textClip.Id,
                Content = textClip.Content,
                FontFamily = textClip.FontFamily,
                FontSize = textClip.FontSize,
                FontWeight = textClip.FontWeight,
                Alignment = textClip.Alignment,
                Foreground = textClip.Foreground,
                Background = textClip.Background,
                Opacity = textOpacity,
                OffsetX = textClip.OffsetX,
                OffsetY = textClip.OffsetY,
                Scale = textClip.Scale,
                RotationDegrees = textClip.RotationDegrees,
                HasShadow = textClip.HasShadow,
                HasOutline = textClip.HasOutline,
                OutlineColor = textClip.OutlineColor,
                OutlineThickness = textClip.OutlineThickness
            });
        }

        // Compose captions
        foreach (var caption in project.Captions)
        {
            if (timestamp < caption.Start || timestamp >= caption.End) continue;

            frame.CaptionLayers.Add(new CaptionLayer
            {
                CaptionId = caption.Id,
                Text = caption.Text,
                Style = caption.Style
            });
        }

        // Apply safe areas (visual guides only, not rendered in export)
        return frame;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}
