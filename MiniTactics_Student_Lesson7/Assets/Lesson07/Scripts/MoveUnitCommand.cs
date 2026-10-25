using UnityEngine;

namespace MiniTactics.Lesson07
{
    public sealed class MoveUnitCommand : IUndoableCommand
    {
        private readonly Unit _unit;
        private readonly BoardView _board;
        private readonly Vector2Int _fromCell;
        private readonly Vector2Int _toCell;
        private readonly bool _wasMoved;

        public MoveUnitCommand(Unit unit, BoardView board, Vector2Int toCell)
        {
            _unit = unit;
            _board = board;
            _fromCell = board.WorldToCell(unit.transform.position);
            _toCell = toCell;
            _wasMoved = unit.HasMoved;
        }

        public void Execute()
        {
            _unit.MoveTo(_board, _toCell);
            _unit.HasMoved = true;
        }

        public void Undo()
        {
            _unit.MoveTo(_board, _fromCell);
            _unit.HasMoved = _wasMoved;
        }
    }
}
