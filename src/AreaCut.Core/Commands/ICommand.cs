namespace AreaCut.Core.Commands;

/// <summary>
/// Command pattern interface for undo/redo operations.
/// Every editable operation must implement this.
/// </summary>
public interface ICommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
