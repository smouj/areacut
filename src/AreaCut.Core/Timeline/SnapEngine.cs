using System.Collections.Generic;
using AreaCut.Core.Time;

namespace AreaCut.Core.Timeline;

/// <summary>
/// Snapping engine for timeline editing.
/// Snaps clip edges to playhead, other clip edges, and grid positions.
/// </summary>
public sealed class SnapEngine
{
    private readonly double _thresholdSeconds;
    private readonly List<TimeStamp> _snapPoints = new();

    public SnapEngine(double thresholdSeconds = 0.01)
    {
        _thresholdSeconds = thresholdSeconds;
    }

    /// <summary>Add snap points from clip boundaries on the timeline.</summary>
    public void AddClipBoundaries(IEnumerable<Models.Clip> clips)
    {
        foreach (var clip in clips)
        {
            _snapPoints.Add(clip.TimelineStart);
            _snapPoints.Add(clip.TimelineEnd);
        }
    }

    /// <summary>Add the playhead position as a snap point.</summary>
    public void AddPlayhead(TimeStamp playhead) => _snapPoints.Add(playhead);

    /// <summary>Add a custom snap point.</summary>
    public void AddSnapPoint(TimeStamp point) => _snapPoints.Add(point);

    /// <summary>Add grid snap points at the given frame rate interval.</summary>
    public void AddGrid(FrameRate frameRate, TimeRange range)
    {
        var frameDuration = frameRate.FrameDuration;
        var current = range.Start;
        while (current < range.End)
        {
            _snapPoints.Add(current);
            current = current.Add(frameDuration);
        }
    }

    /// <summary>
    /// Try to snap the given timestamp to the nearest snap point.
    /// Returns the snapped timestamp if within threshold, otherwise the original.
    /// </summary>
    public TimeStamp Snap(TimeStamp value)
    {
        if (_snapPoints.Count == 0) return value;

        var threshold = TimeStamp.FromSeconds(_thresholdSeconds);
        TimeStamp? closest = null;
        var closestDistance = long.MaxValue;

        foreach (var point in _snapPoints)
        {
            var distance = Math.Abs((value - point).Ticks);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = point;
            }
        }

        if (closest.HasValue && closestDistance <= threshold.Ticks)
            return closest.Value;

        return value;
    }

    /// <summary>Clear all snap points.</summary>
    public void Clear() => _snapPoints.Clear();
}
