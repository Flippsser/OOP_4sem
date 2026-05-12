namespace Lab_04_05
{
    public interface IUndoRedoCommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }
}
