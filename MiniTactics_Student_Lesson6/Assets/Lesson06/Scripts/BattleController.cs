using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson06
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private LayerMask _unitLayerMask = ~0;
        [SerializeField] private TurnManager _turnManager;

        private RangeHighlighter _highlighter;
        private readonly CommandHistory _history = new CommandHistory();
        private readonly InputStateMachine _stateMachine = new InputStateMachine();

        public Unit SelectedUnit { get; private set; }
        public InputStateMachine StateMachine
        {
            get
            {
                if (_stateMachine.Current == null)
                {
                    _stateMachine.ChangeState(new IdleState(_stateMachine, this));
                }

                return _stateMachine;
            }
        }
        public MovementRangeResult LastMovementRange { get; private set; } =
            MovementRangeResult.Empty(Vector2Int.zero);

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
            _ = StateMachine;
        }

        private void Update()
        {
            if (!CanAcceptInput()) return;
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                EndTurn();
                return;
            }

            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (_board == null || _camera == null)
            {
                Debug.LogError("BattleController requires a BoardView and Camera.", this);
                return;
            }

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPosition, _unitLayerMask);
            Unit clickedUnit = hit != null ? hit.GetComponentInParent<Unit>() : null;
            StateMachine.HandleBoardClick(clickedUnit, _board.WorldToCell(worldPosition));
        }

        public void Select(Unit unit)
        {
            if (unit != null && CanAct(unit))
            {
                StateMachine.ChangeState(new UnitSelectedState(StateMachine, this, unit));
                return;
            }

            StateMachine.ChangeState(new IdleState(StateMachine, this));
        }

        public bool TryMoveSelectedUnit(Vector2Int targetCell)
        {
            if (!ExecuteMove(SelectedUnit, targetCell)) return false;
            StateMachine.ChangeState(new IdleState(StateMachine, this));
            return true;
        }

        public void OnUndoClicked()
        {
            if (CanAcceptInput())
            {
                StateMachine.HandleUndo();
            }
        }

        public bool CanAct(Unit unit)
        {
            return _turnManager.CanAct(unit);
        }

        internal void ShowSelection(Unit unit)
        {
            SelectedUnit = unit;
            if (_board == null || _overlayRoot == null)
            {
                Debug.LogError("BattleController requires a BoardView and MovementOverlayRoot.", this);
                return;
            }

            LastMovementRange = _board.GetMovementRange(unit);
            GetHighlighter().Show(LastMovementRange, unit);
        }

        internal void ClearSelection()
        {
            SelectedUnit = null;
            _highlighter?.Hide();
        }

        internal bool ExecuteMove(Unit unit, Vector2Int targetCell)
        {
            if (unit == null || _board == null || !_board.CanMoveTo(unit, targetCell)) return false;
            _history.Execute(new MoveUnitCommand(unit, _board, targetCell));
            if (!_turnManager.CheckBase(unit) && _turnManager.AreAllPlayersMoved())
            {
                EndTurn();
            }

            return true;
        }

        internal void UndoLast() => _history.UndoLast();

        private bool CanAcceptInput()
        {
            return GameManager.Instance.IsPlaying && _turnManager.Phase == BattlePhase.Player;
        }

        private void EndTurn()
        {
            StateMachine.ChangeState(new IdleState(StateMachine, this));
            _history.Clear();
            _turnManager.EndPlayerPhase();
        }

        private RangeHighlighter GetHighlighter()
        {
            return _highlighter ??= new RangeHighlighter(_overlayRoot, _board);
        }
    }
}
