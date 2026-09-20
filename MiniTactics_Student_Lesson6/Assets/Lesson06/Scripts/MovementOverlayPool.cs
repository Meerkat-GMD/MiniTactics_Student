using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    internal sealed class MovementOverlayPool
    {
        private static readonly Color WalkableColor = new Color(0.2f, 1f, 0.35f, 0.55f);
        private static readonly Color OccupiedColor = new Color(1f, 0.2f, 0.2f, 0.65f);

        private readonly Transform _root;
        private readonly BoardView _board;
        private readonly List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
        private Sprite _sprite;

        public MovementOverlayPool(Transform root, BoardView board)
        {
            _root = root;
            _board = board;
        }

        public void Show(IReadOnlyList<MovementCell> cells)
        {
            for (int index = 0; index < cells.Count; index++)
            {
                MovementCell cell = cells[index];
                SpriteRenderer renderer = GetOrCreateRenderer(index);
                renderer.transform.position = _board.CellToWorld(cell.Cell);
                renderer.color = cell.IsOccupied ? OccupiedColor : WalkableColor;
                renderer.gameObject.SetActive(true);
            }

            for (int index = cells.Count; index < _renderers.Count; index++)
            {
                _renderers[index].gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            foreach (SpriteRenderer renderer in _renderers)
            {
                renderer.gameObject.SetActive(false);
            }
        }

        private SpriteRenderer GetOrCreateRenderer(int index)
        {
            if (index < _renderers.Count)
            {
                return _renderers[index];
            }

            _sprite ??= MovementOverlaySpriteFactory.Create();
            GameObject overlayObject = new GameObject("MovementOverlay");
            overlayObject.transform.SetParent(_root, false);

            SpriteRenderer renderer = overlayObject.AddComponent<SpriteRenderer>();
            renderer.sprite = _sprite;
            renderer.sortingOrder = 8;
            _renderers.Add(renderer);
            return renderer;
        }
    }
}
