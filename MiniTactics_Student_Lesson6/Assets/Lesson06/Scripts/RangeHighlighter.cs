using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class RangeHighlighter
    {
        private static readonly Color WalkableColor = new Color(0.2f, 1f, 0.35f, 0.55f);

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

        public void Show(MovementRangeResult range)
        {
            Hide();
            foreach (Vector2Int cell in range.Costs.Keys)
            {
                SpriteRenderer overlay = Object.Instantiate(_overlayPrefab, _root);
                overlay.transform.position = _board.CellToWorld(cell);
                overlay.color = WalkableColor;
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
