using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson03
{
    public sealed class RangeHighlighter
    {
        private readonly BoardView _board;
        private readonly MovementOverlayPool _overlayPool;

        public RangeHighlighter(Transform root, SpriteRenderer prefab, BoardView board)
        {
            _board = board;
            _overlayPool = new MovementOverlayPool(root, prefab);
        }

        public void Show(MovementRangeResult range, Unit selectedUnit)
        {
            List<Vector3> positions = new List<Vector3>();
            List<bool> occupied = new List<bool>();
            foreach (Vector2Int cell in range.ReachableCells)
            {
                positions.Add(_board.CellToWorld(cell));
                occupied.Add(_board.IsOccupiedByOther(selectedUnit, cell));
            }

            _overlayPool.Show(positions, occupied);
        }

        public void Hide()
        {
            _overlayPool.Hide();
        }
    }
}
