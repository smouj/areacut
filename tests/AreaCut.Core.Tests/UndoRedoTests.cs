using AreaCutProject = AreaCut.Core.ProjectModel.AreaCutProject;
using AreaCut.Core.ProjectModel;
using AreaCut.Core.Commands;
using AreaCut.Core.Models;
using AreaCut.Core.Time;
using AreaCut.Core.Undo;
using Xunit;

namespace AreaCut.Core.Tests;

public class UndoRedoTests
{
    [Fact]
    public void Execute_AndUndo_Works()
    {
        var project = new AreaCutProject();
        var undoRedo = new UndoRedoStack();
        var clip = new Clip("media1", "track1")
        {
            SourceIn = TimeStamp.Zero,
            SourceOut = TimeStamp.FromSeconds(10),
            TimelineStart = TimeStamp.Zero,
        };

        var command = new AddClipCommand(project, clip);
        undoRedo.Execute(command);

        Assert.Single(project.Clips);
        Assert.True(undoRedo.CanUndo);
        Assert.False(undoRedo.CanRedo);

        undoRedo.Undo();
        Assert.Empty(project.Clips);
        Assert.False(undoRedo.CanUndo);
        Assert.True(undoRedo.CanRedo);
    }

    [Fact]
    public void Redo_AfterUndo_Works()
    {
        var project = new AreaCutProject();
        var undoRedo = new UndoRedoStack();
        var clip = new Clip("media1", "track1")
        {
            SourceIn = TimeStamp.Zero,
            SourceOut = TimeStamp.FromSeconds(10),
            TimelineStart = TimeStamp.Zero,
        };

        undoRedo.Execute(new AddClipCommand(project, clip));
        undoRedo.Undo();
        undoRedo.Redo();

        Assert.Single(project.Clips);
        Assert.True(undoRedo.CanUndo);
        Assert.False(undoRedo.CanRedo);
    }

    [Fact]
    public void NewAction_ClearsRedoStack()
    {
        var project = new AreaCutProject();
        var undoRedo = new UndoRedoStack();

        var clip1 = new Clip("m1", "t1") { SourceIn = TimeStamp.Zero, SourceOut = TimeStamp.FromSeconds(10), TimelineStart = TimeStamp.Zero };
        var clip2 = new Clip("m1", "t1") { SourceIn = TimeStamp.Zero, SourceOut = TimeStamp.FromSeconds(5), TimelineStart = TimeStamp.FromSeconds(10) };

        undoRedo.Execute(new AddClipCommand(project, clip1));
        undoRedo.Execute(new AddClipCommand(project, clip2));
        undoRedo.Undo(); // Undo clip2
        Assert.Single(project.Clips);

        // New action should clear redo stack
        var clip3 = new Clip("m1", "t1") { SourceIn = TimeStamp.Zero, SourceOut = TimeStamp.FromSeconds(8), TimelineStart = TimeStamp.FromSeconds(20) };
        undoRedo.Execute(new AddClipCommand(project, clip3));

        Assert.False(undoRedo.CanRedo);
        Assert.Equal(2, project.Clips.Count);
    }

    [Fact]
    public void DeleteClip_UndoRestoresClip()
    {
        var project = new AreaCutProject();
        var undoRedo = new UndoRedoStack();
        var clip = new Clip("m1", "t1") { SourceIn = TimeStamp.Zero, SourceOut = TimeStamp.FromSeconds(10), TimelineStart = TimeStamp.Zero };

        undoRedo.Execute(new AddClipCommand(project, clip));
        Assert.Single(project.Clips);

        undoRedo.Execute(new DeleteClipCommand(project, clip.Id));
        Assert.Empty(project.Clips);

        undoRedo.Undo(); // Undo delete
        Assert.Single(project.Clips);
    }

    [Fact]
    public void MoveClip_UndoRestoresPosition()
    {
        var project = new AreaCutProject();
        var undoRedo = new UndoRedoStack();
        var clip = new Clip("m1", "t1") { SourceIn = TimeStamp.Zero, SourceOut = TimeStamp.FromSeconds(10), TimelineStart = TimeStamp.Zero };

        undoRedo.Execute(new AddClipCommand(project, clip));
        undoRedo.Execute(new MoveClipCommand(project, clip.Id, TimeStamp.FromSeconds(5)));

        Assert.Equal(5.0, project.Clips[0].TimelineStart.TotalSeconds, 6);

        undoRedo.Undo();
        Assert.Equal(0.0, project.Clips[0].TimelineStart.TotalSeconds, 6);
    }

    [Fact]
    public void SplitClip_UndoMerges()
    {
        var project = new AreaCutProject();
        var undoRedo = new UndoRedoStack();
        var clip = new Clip("m1", "t1") { SourceIn = TimeStamp.Zero, SourceOut = TimeStamp.FromSeconds(10), TimelineStart = TimeStamp.Zero };

        undoRedo.Execute(new AddClipCommand(project, clip));
        undoRedo.Execute(new SplitClipCommand(project, clip.Id, TimeStamp.FromSeconds(5)));

        Assert.Equal(2, project.Clips.Count);

        undoRedo.Undo();
        Assert.Single(project.Clips);
        Assert.Equal(10.0, project.Clips[0].SourceOut.TotalSeconds, 6);
    }
}
