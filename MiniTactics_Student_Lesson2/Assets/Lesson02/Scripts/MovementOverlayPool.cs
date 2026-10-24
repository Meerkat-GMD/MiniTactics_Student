using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson02
{
    // Provided drawing infrastructure: callers supply display data, never gameplay objects.
    internal sealed class MovementOverlayPool
    {
        private static readonly Color WalkableColor = new Color(0.2f, 1f, 0.35f, 0.55f);
        private static readonly Color OccupiedColor = new Color(1f, 0.2f, 0.2f, 0.65f);
        private readonly Transform _root;
        private readonly SpriteRenderer _prefab;
        private readonly List<SpriteRenderer> _renderers = new List<SpriteRenderer>();

        public MovementOverlayPool(Transform root, SpriteRenderer prefab)
        {
            _root = root;
            _prefab = prefab;
        }

        public void Show(IReadOnlyList<Vector3> worldPositions, IReadOnlyList<bool> occupied)
        {
            for (int index = 0; index < worldPositions.Count; index++)
            {
                if (index == _renderers.Count)
                {
                    _renderers.Add(Object.Instantiate(_prefab, _root));
                }

                SpriteRenderer renderer = _renderers[index];
                renderer.transform.position = worldPositions[index];
                renderer.color = occupied[index] ? OccupiedColor : WalkableColor;
                renderer.gameObject.SetActive(true);
            }

            for (int index = worldPositions.Count; index < _renderers.Count; index++)
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
    }
}
