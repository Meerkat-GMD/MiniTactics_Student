using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class RangeHighlighter
    {
        private static readonly Color WalkableColor = new Color(0.2f, 1f, 0.35f, 0.55f);
        private static readonly Color OccupiedColor = new Color(1f, 0.2f, 0.2f, 0.65f);

        private readonly Transform _root;
        private readonly SpriteRenderer _overlayPrefab;
        private readonly BoardView _board;
        private readonly List<SpriteRenderer> _overlays = new List<SpriteRenderer>();

        public RangeHighlighter(Transform root, SpriteRenderer overlayPrefab, BoardView board)
        {
            _root = root;
            _overlayPrefab = overlayPrefab;
            _board = board;
        }

        public void Show(MovementRangeResult range, Unit unit)
        {
            Hide();
            foreach (Vector2Int cell in range.Costs.Keys)
            {
                SpriteRenderer overlay = Object.Instantiate(_overlayPrefab, _root);
                overlay.transform.position = _board.CellToWorld(cell);
                overlay.color = _board.IsOccupiedByOther(unit, cell) ? OccupiedColor : WalkableColor;
                _overlays.Add(overlay);
            }
        }

        public void Hide()
        {
            foreach (SpriteRenderer overlay in _overlays)
            {
                Object.Destroy(overlay.gameObject);
            }

            _overlays.Clear();
        }
    }
}
