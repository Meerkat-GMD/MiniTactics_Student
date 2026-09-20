using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson04
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _overlayPrefab;

        private readonly CommandHistory _history = new CommandHistory();
        private RangeHighlighter _highlighter;

        public Unit SelectedUnit { get; private set; }

        private void Awake()
        {
            _highlighter = new RangeHighlighter(transform, _overlayPrefab, _board);
        }

        private void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
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
            _highlighter.Show(_board.GetMovementRange(unit), unit);
        }

        public bool TryMoveSelectedUnit(Vector2Int targetCell)
        {
            if (!_board.CanMoveTo(SelectedUnit, targetCell))
            {
                return false;
            }

            _history.Execute(new MoveUnitCommand(SelectedUnit, _board, targetCell));
            SelectedUnit = null;
            _highlighter.Hide();
            return true;
        }

        public void OnUndoClicked()
        {
            _history.UndoLast();
            SelectedUnit = null;
            _highlighter.Hide();
        }
    }
}
