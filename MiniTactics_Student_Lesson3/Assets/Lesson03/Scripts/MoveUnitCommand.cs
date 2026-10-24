using UnityEngine;

namespace MiniTactics.Lesson03
{
    public sealed class MoveUnitCommand : IUndoableCommand
    {
        private readonly Unit _unit;
        private readonly BoardView _board;
        private readonly Vector2Int _fromCell;
        private readonly Vector2Int _toCell;

        public MoveUnitCommand(Unit unit, BoardView board, Vector2Int toCell)
        {
            _unit = unit;
            _board = board;
            _fromCell = board.WorldToCell(unit.transform.position);
            _toCell = toCell;
        }

        public void Execute()
        {
            _unit.MoveTo(_board, _toCell);
        }

        public void Undo()
        {
            _unit.MoveTo(_board, _fromCell);
        }
    }
}
