using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson02
{
    public sealed class BattleController : MonoBehaviour
    {
        private static readonly Color WalkableColor = new Color(0.2f, 1f, 0.35f, 0.55f);
        private static readonly Color OccupiedColor = new Color(1f, 0.2f, 0.2f, 0.65f);

        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _overlayPrefab;

        private readonly List<SpriteRenderer> _overlays = new List<SpriteRenderer>();

        public Unit SelectedUnit { get; private set; }

        private void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPosition);
            Unit clickedUnit = hit != null ? hit.GetComponent<Unit>() : null;

            if (clickedUnit != null && clickedUnit.CanMove)
            {
                Select(clickedUnit);
                return;
            }

            if (SelectedUnit != null)
            {
                TryMoveSelectedUnit(_board.WorldToCell(worldPosition));
            }
        }

        public void Select(Unit unit)
        {
            SelectedUnit = unit;
            HideRange();
            ShowRange(unit);
        }

        public bool TryMoveSelectedUnit(Vector2Int targetCell)
        {
            if (!_board.CanMoveTo(SelectedUnit, targetCell))
            {
                return false;
            }

            SelectedUnit.MoveTo(_board, targetCell);
            SelectedUnit = null;
            HideRange();
            return true;
        }

        private void ShowRange(Unit unit)
        {
            foreach (Vector2Int cell in _board.GetMovementRange(unit))
            {
                SpriteRenderer overlay = Instantiate(_overlayPrefab, transform);
                overlay.transform.position = _board.CellToWorld(cell);
                overlay.color = _board.IsOccupiedByOther(unit, cell) ? OccupiedColor : WalkableColor;
                _overlays.Add(overlay);
            }
        }

        private void HideRange()
        {
            foreach (SpriteRenderer overlay in _overlays)
            {
                Destroy(overlay.gameObject);
            }

            _overlays.Clear();
        }
    }
}
