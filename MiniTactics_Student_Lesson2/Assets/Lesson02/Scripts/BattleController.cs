using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson02
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private LayerMask _unitLayerMask = ~0;

        private MovementOverlayPool _overlayPool;

        public Unit SelectedUnit { get; private set; }

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (_board == null || _camera == null)
            {
                Debug.LogError("BattleController requires a BoardView and Camera.", this);
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

        public void Select(Unit unit)
        {
            SelectedUnit = unit != null && unit.CanMove ? unit : null;

            if (_board == null || _overlayRoot == null)
            {
                Debug.LogError("BattleController requires a BoardView and MovementOverlayRoot.", this);
                return;
            }

            if (SelectedUnit == null)
            {
                _overlayPool?.Hide();
                return;
            }

            GetOverlayPool().Show(_board.GetMovementRange(SelectedUnit));
        }

        public bool TryMoveSelectedUnit(Vector2Int targetCell)
        {
            if (SelectedUnit == null || _board == null || !_board.CanMoveTo(SelectedUnit, targetCell))
            {
                return false;
            }

            SelectedUnit.MoveTo(_board, targetCell);
            SelectedUnit = null;
            _overlayPool?.Hide();
            return true;
        }

        private MovementOverlayPool GetOverlayPool()
        {
            return _overlayPool ??= new MovementOverlayPool(_overlayRoot, _board);
        }
    }
}
