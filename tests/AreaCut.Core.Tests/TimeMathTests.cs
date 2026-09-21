using AreaCut.Core.Time;
using Xunit;

namespace AreaCut.Core.Tests;

public class TimeStampTests
{
    [Fact]
    public void FromSeconds_RoundTrips()
    {
        var ts = TimeStamp.FromSeconds(5.5);
        Assert.Equal(5.5, ts.TotalSeconds, 6);
    }

    [Fact]
    public void Add_Subtract_Works()
    {
        var a = TimeStamp.FromSeconds(10);
        var b = TimeStamp.FromSeconds(3);
        var result = a.Subtract(b);
        Assert.Equal(7.0, result.TotalSeconds, 6);
    }

    [Fact]
    public void Comparison_Works()
    {
        var a = TimeStamp.FromSeconds(5);
        var b = TimeStamp.FromSeconds(10);
        Assert.True(a < b);
        Assert.True(b > a);
        Assert.True(a <= b);
        Assert.False(a > b);
    }

    [Fact]
    public void Multiply_Scales()
    {
        var ts = TimeStamp.FromSeconds(10);
        var doubled = ts * 2.0;
        Assert.Equal(20.0, doubled.TotalSeconds, 6);
    }

    [Fact]
    public void Clamp_Works()
    {
        var val = TimeStamp.FromSeconds(15);
        var min = TimeStamp.FromSeconds(10);
        var max = TimeStamp.FromSeconds(20);
        Assert.Equal(15.0, val.Clamp(min, max).TotalSeconds, 6);
        Assert.Equal(10.0, TimeStamp.FromSeconds(5).Clamp(min, max).TotalSeconds, 6);
        Assert.Equal(20.0, TimeStamp.FromSeconds(25).Clamp(min, max).TotalSeconds, 6);
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var ts = TimeStamp.FromSeconds(3661.5);
        Assert.Contains("01", ts.ToString());
    }
}

public class FrameRateTests
{
    [Theory]
    [InlineData(24, 1, 24.0)]
    [InlineData(30, 1, 30.0)]
    [InlineData(30000, 1001, 29.97002997002999)]
    public void FramesPerSecond_CalculatesCorrectly(int num, int den, double expectedFps)
    {
        var fr = new FrameRate(num, den);
        Assert.Equal(expectedFps, fr.FramesPerSecond, 6);
    }

    [Fact]
    public void FrameToTimeStamp_RoundTrips()
    {
        var fps30 = FrameRate.Fps30;
        var ts = fps30.FrameToTimeStamp(90); // frame 90 at 30fps = 3 seconds
        Assert.Equal(3.0, ts.TotalSeconds, 6);
    }

    [Fact]
    public void TimeStampToFrame_RoundTrips()
    {
        var fps30 = FrameRate.Fps30;
        var ts = TimeStamp.FromSeconds(3.0);
        var frame = fps30.TimeStampToFrame(ts);
        Assert.Equal(90, frame);
    }

    [Fact]
    public void PresetFrameRates_AreCorrect()
    {
        Assert.Equal(30.0, FrameRate.Fps30.FramesPerSecond, 6);
        Assert.Equal(60.0, FrameRate.Fps60.FramesPerSecond, 6);
        Assert.Equal(24.0, FrameRate.Fps24.FramesPerSecond, 6);
    }
}

public class TimeRangeTests
{
    [Fact]
    public void Duration_CalculatesCorrectly()
    {
        var range = new TimeRange(TimeStamp.FromSeconds(5), TimeStamp.FromSeconds(10));
        Assert.Equal(5.0, range.Duration.TotalSeconds, 6);
    }

    [Fact]
    public void Contains_Works()
    {
        var range = new TimeRange(TimeStamp.FromSeconds(5), TimeStamp.FromSeconds(10));
        Assert.True(range.Contains(TimeStamp.FromSeconds(7)));
        Assert.False(range.Contains(TimeStamp.FromSeconds(4)));
        Assert.False(range.Contains(TimeStamp.FromSeconds(10))); // half-open
    }

    [Fact]
    public void Overlaps_DetectsOverlap()
    {
        var a = new TimeRange(TimeStamp.FromSeconds(5), TimeStamp.FromSeconds(15));
        var b = new TimeRange(TimeStamp.FromSeconds(10), TimeStamp.FromSeconds(20));
        Assert.True(a.Overlaps(b));
        Assert.True(b.Overlaps(a));
    }

    [Fact]
    public void Intersect_ReturnsOverlap()
    {
        var a = new TimeRange(TimeStamp.FromSeconds(5), TimeStamp.FromSeconds(15));
        var b = new TimeRange(TimeStamp.FromSeconds(10), TimeStamp.FromSeconds(20));
        var intersection = a.Intersect(b);
        Assert.Equal(10.0, intersection.Start.TotalSeconds, 6);
        Assert.Equal(15.0, intersection.End.TotalSeconds, 6);
    }

    [Fact]
    public void FromDuration_Works()
    {
        var range = TimeRange.FromDuration(TimeStamp.FromSeconds(5), TimeStamp.FromSeconds(10));
        Assert.Equal(5.0, range.Start.TotalSeconds, 6);
        Assert.Equal(15.0, range.End.TotalSeconds, 6);
    }
}
