using System.Collections.Generic;

namespace Lab_04_05
{
    public class UndoRedoManager
    {
        private readonly Stack<IUndoRedoCommand> _undoStack = new();
        private readonly Stack<IUndoRedoCommand> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public string UndoDescription => _undoStack.Count > 0 ? _undoStack.Peek().Description : "";
        public string RedoDescription => _redoStack.Count > 0 ? _redoStack.Peek().Description : "";

        public void ExecuteCommand(IUndoRedoCommand cmd)
        {
            cmd.Execute();
            _undoStack.Push(cmd);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count == 0) return;
            var cmd = _undoStack.Pop();
            cmd.Undo();
            _redoStack.Push(cmd);
        }

        public void Redo()
        {
            if (_redoStack.Count == 0) return;
            var cmd = _redoStack.Pop();
            cmd.Execute();
            _undoStack.Push(cmd);
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
