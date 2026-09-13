using System;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class PlayerMoveCommand : IUndoableCommand
    {
        private readonly Unit _unit;
        private readonly MoveUnitCommand _movement;

        public PlayerMoveCommand(Unit unit, BoardView board, Vector2Int toCell)
        {
            _unit = unit != null ? unit : throw new ArgumentNullException(nameof(unit));
            _movement = new MoveUnitCommand(unit, board, toCell);
        }

        public void Execute()
        {
            _movement.Execute();
            _unit.SetMoved(true);
        }

        public void Undo()
        {
            _movement.Undo();
            _unit.SetMoved(false);
        }
    }
}
