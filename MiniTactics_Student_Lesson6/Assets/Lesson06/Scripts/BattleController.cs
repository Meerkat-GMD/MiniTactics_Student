using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson06
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _overlayPrefab;
        [SerializeField] private TurnManager _turnManager;

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
            if (!CanAcceptInput())
            {
                return;
            }

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                EndTurn();
                return;
            }

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
            if (CanAcceptInput())
            {
                _stateMachine.HandleUndo();
            }
        }

        public bool CanAct(Unit unit)
        {
            return _turnManager.CanAct(unit);
        }

        public void ShowSelection(Unit unit)
        {
            _highlighter.Show(_board.GetMovementRange(unit));
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
            if (!_turnManager.CheckBase(unit) && _turnManager.AreAllPlayersMoved())
            {
                EndTurn();
            }

            return true;
        }

        public void UndoLast()
        {
            _history.UndoLast();
        }

        private bool CanAcceptInput()
        {
            return GameManager.Instance.IsPlaying && _turnManager.Phase == BattlePhase.Player;
        }

        private void EndTurn()
        {
            _stateMachine.ChangeState(new IdleState(_stateMachine, this));
            _history.Clear();
            _turnManager.EndPlayerPhase();
        }
    }
}
