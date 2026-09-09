using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace MiniTactics.Lesson05
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private LayerMask _unitLayerMask = ~0;
        [SerializeField] private GameManager _gameManager;
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
            if (!CanAcceptPlayerInput) return;
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame))
            {
                OnEndTurnRequested();
                return;
            }
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
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
            if (CanPlayerAct(unit))
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

        public bool CanAcceptPlayerInput => true;

        public bool CanPlayerAct(Unit unit) => unit != null && unit.CanMove;

        private void Start() => _turnManager?.BindCommandHistory(_history);

        public void OnEndTurnRequested()
        {
            if (!CanAcceptPlayerInput) return;
            StateMachine.ChangeState(new IdleState(StateMachine, this));
            ClearSelection();
            _turnManager?.BindCommandHistory(_history);
            _turnManager?.RequestEndPlayerPhase();
        }

        public void OnUndoClicked()
        {
            if (!CanAcceptPlayerInput) return;
            StateMachine.HandleUndo();
        }

        internal void ShowSelection(Unit unit)
        {
            if (!CanPlayerAct(unit))
            {
                ClearSelection();
                return;
            }
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
            if (!CanPlayerAct(unit) || _board == null || !_board.CanMoveTo(unit, targetCell)) return false;
            _turnManager?.BindCommandHistory(_history);
            _history.Execute(new PlayerMoveCommand(unit, _board, targetCell));
            if (_turnManager != null && !_turnManager.CheckGoal(unit))
                _turnManager.NotifyPlayerActionCompleted();
            return true;
        }

        internal void UndoLast()
        {
            if (!CanAcceptPlayerInput) return;
            _history.UndoLast();
        }

        private RangeHighlighter GetHighlighter()
        {
            return _highlighter ??= new RangeHighlighter(_overlayRoot, _board);
        }
    }
}
