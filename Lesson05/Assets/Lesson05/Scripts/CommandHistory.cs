using System;
using System.Collections.Generic;

namespace MiniTactics.Lesson05
{
    public sealed class CommandHistory
    {
        private readonly Stack<IUndoableCommand> _commands = new Stack<IUndoableCommand>();

        public int Count => _commands.Count;

        public void Clear()
        {
            _commands.Clear();
        }

        public void Execute(IUndoableCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

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
