using System.Collections.Generic;

namespace MiniTactics.Lesson03
{
    public sealed class CommandHistory
    {
        private readonly Stack<IUndoableCommand> _commands = new Stack<IUndoableCommand>();

        public void Execute(IUndoableCommand command)
        {
            command.Execute();
            _commands.Push(command);
        }

        public bool UndoLast()
        {
            if (_commands.Count == 0)
            {
                return false;
            }

            _commands.Pop().Undo();
            return true;
        }
    }
}
