using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson02
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private SpriteRenderer _overlayPrefab;
        [SerializeField] private LayerMask _unitLayerMask = ~0;
        private MovementOverlayPool _overlayPool;

        public Unit SelectedUnit { get; private set; }

        private void Awake()
        {
            _overlayPool = new MovementOverlayPool(_overlayRoot, _overlayPrefab);
        }

        public void Select(Unit unit)
        {
            SelectedUnit = unit != null && unit.CanMove ? unit : null;
            _overlayPool.Hide();
            if (SelectedUnit == null)
            {
                return;
            }

            List<Vector3> positions = new List<Vector3>();
            List<bool> occupied = new List<bool>();
            foreach (MovementCell cell in _board.GetMovementRange(SelectedUnit))
            {
                positions.Add(_board.CellToWorld(cell.Cell));
                occupied.Add(cell.IsOccupied);
            }

            _overlayPool.Show(positions, occupied);
        }

        public bool TryMoveSelectedUnit(Vector2Int targetCell)
        {
            if (SelectedUnit == null || !_board.CanMoveTo(SelectedUnit, targetCell))
            {
                return false;
            }

            SelectedUnit.MoveTo(_board, targetCell);
            SelectedUnit = null;
            _overlayPool.Hide();
            return true;
        }

        private void Update()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPosition, _unitLayerMask);
            Unit clickedUnit = hit != null ? hit.GetComponentInParent<Unit>() : null;
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
    }
}
