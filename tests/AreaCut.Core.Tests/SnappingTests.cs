using AreaCut.Core.Models;
using AreaCut.Core.Time;
using AreaCut.Core.Timeline;
using Xunit;

namespace AreaCut.Core.Tests;

public class SnappingTests
{
    [Fact]
    public void Snap_SnapsToClipBoundaries()
    {
        var engine = new SnapEngine(thresholdSeconds: 0.05);
        var clip = new Clip("m1", "t1")
        {
            SourceIn = TimeStamp.Zero,
            SourceOut = TimeStamp.FromSeconds(10),
            TimelineStart = TimeStamp.Zero,
        };

        engine.AddClipBoundaries(new[] { clip });

        // Value within threshold of clip start
        var result = engine.Snap(TimeStamp.FromSeconds(0.01));
        Assert.Equal(0.0, result.TotalSeconds, 6);

        // Value outside threshold
        var result2 = engine.Snap(TimeStamp.FromSeconds(0.1));
        Assert.Equal(0.1, result2.TotalSeconds, 6);
    }

    [Fact]
    public void Snap_SnapsToPlayhead()
    {
        var engine = new SnapEngine(thresholdSeconds: 0.05);
        engine.AddPlayhead(TimeStamp.FromSeconds(5));

        var result = engine.Snap(TimeStamp.FromSeconds(5.02));
        Assert.Equal(5.0, result.TotalSeconds, 6);
    }

    [Fact]
    public void Snap_Clear_RemovesAllPoints()
    {
        var engine = new SnapEngine();
        engine.AddPlayhead(TimeStamp.FromSeconds(5));
        engine.Clear();

        // After clear, no snap points
        var result = engine.Snap(TimeStamp.FromSeconds(5.01));
        Assert.Equal(5.01, result.TotalSeconds, 6); // Not snapped
    }
}
