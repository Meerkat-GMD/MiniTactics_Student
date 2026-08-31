using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson01
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

            // TODO(학생) 1: 클릭한 대상에 따라 유닛 선택과 목적지 이동을 분기하세요.
            // 이동 가능한 유닛을 클릭하면 Select 후 즉시 반환합니다.
            // 선택된 유닛이 있을 때 빈 셀을 클릭하면 월드 좌표를 셀로 바꿔 이동을 시도합니다.
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
