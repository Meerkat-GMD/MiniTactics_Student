using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson03
{
    public sealed class RangeHighlighter
    {
        private readonly BoardView _board;
        private readonly MovementOverlayPool _overlayPool;

        public RangeHighlighter(Transform root, BoardView board)
        {
            _board = board;
            _overlayPool = new MovementOverlayPool(root, board);
        }

        public void Show(MovementRangeResult range, Unit selectedUnit)
        {
            List<MovementCell> cells = new List<MovementCell>();
            foreach (Vector2Int cell in range.ReachableCells)
            {
                cells.Add(new MovementCell(
                    cell,
                    IsOccupiedByOther(selectedUnit, cell)));
            }

            _overlayPool.Show(cells);
        }

        public void Hide()
        {
            _overlayPool.Hide();
        }

        private bool IsOccupiedByOther(Unit selectedUnit, Vector2Int cell)
        {
            foreach (Unit unit in Object.FindObjectsByType<Unit>(FindObjectsInactive.Exclude))
            {
                if (unit != selectedUnit && _board.WorldToCell(unit.transform.position) == cell)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
