using AreaCut.Core.Models;
using AreaCut.Core.Project;
using AreaCut.Core.Time;
using Xunit;

namespace AreaCut.Core.Tests;

public class ProjectTests
{
    [Fact]
    public void NewProject_HasDefaultTracks()
    {
        var project = new Project { Name = "Test" };
        project.Tracks.Add(new Track(TrackKind.Video, "Video 1"));
        project.Tracks.Add(new Track(TrackKind.Audio, "Audio 1"));

        Assert.Equal(2, project.Tracks.Count);
        Assert.Equal(TrackKind.Video, project.Tracks[0].Kind);
    }

    [Fact]
    public void TimelineDuration_CalculatesFromClips()
    {
        var project = new Project { Name = "Test" };
        var track = new Track(TrackKind.Video, "Video 1");
        project.Tracks.Add(track);

        var clip = new Clip("m1", track.Id)
        {
            SourceIn = TimeStamp.Zero,
            SourceOut = TimeStamp.FromSeconds(30),
            TimelineStart = TimeStamp.FromSeconds(10),
        };
        project.Clips.Add(clip);

        // Timeline duration = clip end = 10 + 30 = 40s
        Assert.Equal(40.0, project.TimelineDuration.TotalSeconds, 6);
    }

    [Fact]
    public void FindClip_Works()
    {
        var project = new Project();
        var clip = new Clip("m1", "t1");
        project.Clips.Add(clip);

        var found = project.FindClip(clip.Id);
        Assert.NotNull(found);
        Assert.Equal(clip.Id, found.Id);
    }

    [Fact]
    public void FindMedia_Works()
    {
        var project = new Project();
        var media = new MediaReference("/path/to/video.mp4", MediaKind.Video);
        project.Media.Add(media);

        var found = project.FindMedia(media.Id);
        Assert.NotNull(found);
        Assert.Equal(media.Id, found.Id);
    }
}

public class CanvasSpecTests
{
    [Fact]
    public void Vertical_Is1080x1920()
    {
        var canvas = CanvasSpec.Vertical1080;
        Assert.Equal(1080, canvas.Width);
        Assert.Equal(1920, canvas.Height);
        Assert.True(canvas.IsVertical);
    }

    [Fact]
    public void YouTube_Is16x9()
    {
        var canvas = CanvasSpec.YouTube1080;
        Assert.Equal(16.0 / 9.0, canvas.AspectRatio, 4);
        Assert.False(canvas.IsVertical);
        Assert.False(canvas.IsSquare);
    }
}
