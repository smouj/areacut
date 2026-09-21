using AreaCut.Core.Models;
using AreaCut.Core.Time;
using Xunit;

namespace AreaCut.Core.Tests;

public class ClipTests
{
    [Fact]
    public void Clip_TimelineDuration_WithPlaybackRate()
    {
        var clip = new Clip("media1", "track1")
        {
            SourceIn = TimeStamp.Zero,
            SourceOut = TimeStamp.FromSeconds(10),
            TimelineStart = TimeStamp.Zero,
            PlaybackRate = 2.0 // 2x speed
        };
        // 10s source at 2x = 5s timeline duration
        Assert.Equal(5.0, clip.TimelineDuration.TotalSeconds, 6);
    }

    [Fact]
    public void Clip_TimelineEnd_WithOffset()
    {
        var clip = new Clip("media1", "track1")
        {
            SourceIn = TimeStamp.Zero,
            SourceOut = TimeStamp.FromSeconds(10),
            TimelineStart = TimeStamp.FromSeconds(5),
        };
        Assert.Equal(15.0, clip.TimelineEnd.TotalSeconds, 6);
    }

    [Fact]
    public void Clip_SourceDuration_Matches()
    {
        var clip = new Clip("media1", "track1")
        {
            SourceIn = TimeStamp.FromSeconds(2),
            SourceOut = TimeStamp.FromSeconds(8),
        };
        Assert.Equal(6.0, clip.SourceDuration.TotalSeconds, 6);
    }
}

public class TransformTests
{
    [Fact]
    public void Identity_IsDefault()
    {
        var t = Transform.Identity;
        Assert.Equal(0.0, t.OffsetX);
        Assert.Equal(0.0, t.OffsetY);
        Assert.Equal(1.0, t.ScaleX);
        Assert.Equal(1.0, t.ScaleY);
        Assert.Equal(0.0, t.RotationDegrees);
        Assert.Equal(1.0, t.Opacity);
    }

    [Fact]
    public void WithOpacity_Clamps()
    {
        var t = Transform.Identity.WithOpacity(2.0);
        Assert.Equal(1.0, t.Opacity);
        var t2 = Transform.Identity.WithOpacity(-0.5);
        Assert.Equal(0.0, t2.Opacity);
    }
}

public class CropTests
{
    [Fact]
    public void None_IsDefault()
    {
        var c = Crop.None;
        Assert.Equal(0.0, c.Left);
        Assert.Equal(0.0, c.Top);
        Assert.Equal(1.0, c.Right);
        Assert.Equal(1.0, c.Bottom);
        Assert.True(c.IsNone);
    }

    [Fact]
    public void Width_Height_Calculate()
    {
        var c = new Crop { Left = 0.1, Top = 0.2, Right = 0.9, Bottom = 0.8 };
        Assert.Equal(0.8, c.Width, 6);
        Assert.Equal(0.6, c.Height, 6);
    }
}

public class SocialPresetTests
{
    [Fact]
    public void Vertical_HasCorrectDimensions()
    {
        Assert.Equal(1080, SocialPreset.Vertical.Width);
        Assert.Equal(1920, SocialPreset.Vertical.Height);
        Assert.Equal(30, SocialPreset.Vertical.Fps);
    }

    [Fact]
    public void YouTube_Has16x9()
    {
        Assert.Equal(16.0 / 9.0, SocialPreset.YouTube.AspectRatio, 4);
    }
}
