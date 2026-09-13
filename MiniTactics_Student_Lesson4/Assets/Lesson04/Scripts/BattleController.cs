using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson04
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private LayerMask _unitLayerMask = ~0;

        private RangeHighlighter _highlighter;
        private readonly CommandHistory _history = new CommandHistory();

        public Unit SelectedUnit { get; private set; }
        public MovementRangeResult LastMovementRange { get; private set; } =
            MovementRangeResult.Empty(Vector2Int.zero);

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
                _highlighter?.Hide();
                return;
            }

            LastMovementRange = _board.GetMovementRange(SelectedUnit);
            GetHighlighter().Show(LastMovementRange, SelectedUnit);
        }

        public bool TryMoveSelectedUnit(Vector2Int targetCell)
        {
            if (SelectedUnit == null || _board == null || !_board.CanMoveTo(SelectedUnit, targetCell))
            {
                return false;
            }

            MoveUnitCommand command = new MoveUnitCommand(SelectedUnit, _board, targetCell);
            _history.Execute(command);
            SelectedUnit = null;
            _highlighter?.Hide();
            return true;
        }

        public void OnUndoClicked()
        {
            _history.UndoLast();
        }

        private RangeHighlighter GetHighlighter()
        {
            return _highlighter ??= new RangeHighlighter(_overlayRoot, _board);
        }
    }
}
