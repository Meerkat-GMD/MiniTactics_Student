using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace MiniTactics.Lesson06
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private LayerMask _unitLayerMask = ~0;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private List<Unit> _combatUnits = new List<Unit>();

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

        public bool CanAcceptPlayerInput => _gameManager != null && _gameManager.IsPlaying &&
            _turnManager != null && _turnManager.Phase == BattlePhase.Player;

        public bool CanPlayerAct(Unit unit) => CanAcceptPlayerInput && _turnManager.CanPlayerAct(unit);

        public void BindCombatUnits(IEnumerable<Unit> units)
        {
            _combatUnits = units == null ? new List<Unit>() : new List<Unit>(units);
        }

        public bool TryAttack(Unit attacker, Unit defender)
        {
            // 학습 과제: 대상 검증, 피해 적용, 명령 이력의 경계를 완성한다.
            return false;
        }

        public void CompletePlayerAction(Unit unit)
        {
            // 학습 과제: 보류한 공격 행동을 한 번 완료하고 다음 상태로 연결한다.
        }

        internal void ShowAttackTargets(Unit unit)
        {
            if (unit == null || _board == null || _overlayRoot == null) return;
            SelectedUnit = unit;
            GetHighlighter().ShowAttackTargets(AttackTargeting.GetAdjacentEnemies(unit, _combatUnits, _board));
        }

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
            _turnManager.BindCommandHistory(_history);
            _history.Execute(new PlayerMoveCommand(unit, _board, targetCell));
            // 현재는 완성된 5차시 이동 경로다. 전투 구현 후 공격 선택 흐름을 연결한다.
            if (!_turnManager.CheckGoal(unit))
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
