using System;
using System.Collections.Generic;
using AreaCut.Core.Commands;

namespace AreaCut.Core.Undo;

/// <summary>
/// Undo/redo stack built on the Command Pattern.
/// Every editable operation goes through this stack.
/// Ctrl+Z = Undo, Ctrl+Y = Redo.
/// </summary>
public sealed class UndoRedoStack
{
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();
    private readonly int _maxDepth;

    public event EventHandler<string>? CommandExecuted;
    public event EventHandler<string>? CommandUndone;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public int UndoCount => _undoStack.Count;
    public int RedoCount => _redoStack.Count;

    public UndoRedoStack(int maxDepth = 200)
    {
        _maxDepth = maxDepth;
    }

    /// <summary>Execute a command and push it onto the undo stack. Clears the redo stack.</summary>
    public void Execute(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);

        // Clear redo stack on new action
        _redoStack.Clear();

        // Trim undo stack if too deep
        while (_undoStack.Count > _maxDepth)
        {
            var items = _undoStack.ToArray();
            _undoStack.Clear();
            for (var i = items.Length - 1; i > 0; i--)
                _undoStack.Push(items[i]);
        }

        CommandExecuted?.Invoke(this, command.Description);
    }

    /// <summary>Undo the last command. Moves it to the redo stack.</summary>
    public void Undo()
    {
        if (!CanUndo) throw new InvalidOperationException("Nothing to undo");
        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
        CommandUndone?.Invoke(this, command.Description);
    }

    /// <summary>Redo the last undone command.</summary>
    public void Redo()
    {
        if (!CanRedo) throw new InvalidOperationException("Nothing to redo");
        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        CommandExecuted?.Invoke(this, command.Description);
    }

    /// <summary>Clear both stacks.</summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}
