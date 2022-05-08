using System.Collections.Generic;

namespace ZoDream.Studio.Commands
{
    public class CommandManager : ICommandManager
    {
        private readonly Stack<IBackableCommand> undoItems = new();
        private readonly Stack<IBackableCommand> reverseItems = new();

        public event CommandStateChangedEventHandler? UndoStateChanged;
        public event CommandStateChangedEventHandler? ReverseUndoStateChanged;

        public void ExecuteCommand(ICommand command)
        {
            if (!command.Execute())
            {
                return;
            }
            reverseItems.Clear();
            if (command is IBackableCommand backableCommand)
            {
                undoItems.Push(backableCommand);
            }
            else
            {
                undoItems.Clear();
            }
            UndoStateChanged?.Invoke(undoItems.Count > 0);
        }

        public void ReverseUndo()
        {
            var command = reverseItems.Pop();
            if (command == null)
            {
                return;
            }
            command.Execute();
            undoItems.Push(command);
            UndoStateChanged?.Invoke(undoItems.Count > 0);
            ReverseUndoStateChanged?.Invoke(reverseItems.Count > 0);
        }

        public void Undo()
        {
            var command = undoItems.Pop();
            if (command == null)
            {
                return;
            }
            command.Undo();
            reverseItems.Push(command);
            UndoStateChanged?.Invoke(undoItems.Count > 0);
            ReverseUndoStateChanged?.Invoke(reverseItems.Count > 0);
        }
    }
}
