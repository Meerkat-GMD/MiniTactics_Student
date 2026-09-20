using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson05
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _overlayPrefab;

        private readonly CommandHistory _history = new CommandHistory();
        private readonly InputStateMachine _stateMachine = new InputStateMachine();
        private RangeHighlighter _highlighter;

        private void Awake()
        {
            _highlighter = new RangeHighlighter(transform, _overlayPrefab, _board);
            _stateMachine.ChangeState(new IdleState(_stateMachine, this));
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

            _stateMachine.HandleBoardClick(clickedUnit, _board.WorldToCell(worldPosition));
        }

        public void OnUndoClicked()
        {
            _stateMachine.HandleUndo();
        }

        public void ShowSelection(Unit unit)
        {
            _highlighter.Show(_board.GetMovementRange(unit), unit);
        }

        public void ClearSelection()
        {
            _highlighter.Hide();
        }

        public bool ExecuteMove(Unit unit, Vector2Int targetCell)
        {
            if (!_board.CanMoveTo(unit, targetCell))
            {
                return false;
            }

            _history.Execute(new MoveUnitCommand(unit, _board, targetCell));
            return true;
        }

        public void UndoLast()
        {
            _history.UndoLast();
        }
    }
}
