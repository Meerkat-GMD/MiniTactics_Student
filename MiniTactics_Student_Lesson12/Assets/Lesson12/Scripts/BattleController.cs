using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson12
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private LayerMask _unitLayerMask = ~0;
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private ActionMenu _actionMenu;

        private RangeHighlighter _highlighter;
        private readonly CommandHistory _history = new CommandHistory();
        private readonly InputStateMachine _stateMachine = new InputStateMachine();

        public Unit SelectedUnit { get; private set; }
        internal ActionMenu ActionMenu => _actionMenu;
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
            if (!CanAcceptInput()) return;
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
            Services.Audio.Play(SoundCue.Selection);
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
            if (!CanAcceptInput()) return false;
            if (unit == null || _board == null || !_board.CanMoveTo(unit, targetCell)) return false;
            _history.Execute(new MoveUnitCommand(unit, _board, targetCell));
            return true;
        }

        internal bool HasAttackTargets(Unit attacker)
        {
            return _turnManager.GetAttackTargets(attacker).Count > 0;
        }

        internal bool HasSkillTargets(Unit user)
        {
            Skill skill = user.UnitClass.Skill;
            return skill != null && skill.GetTargets(user, _turnManager).Count > 0;
        }

        internal void ShowAttackTargets(Unit attacker)
        {
            GetHighlighter().ShowTargets(_turnManager.GetAttackTargets(attacker));
        }

        internal void ShowSkillTargets(Unit user)
        {
            GetHighlighter().ShowTargets(user.UnitClass.Skill.GetTargets(user, _turnManager));
        }

        internal bool CanAttack(Unit attacker, Unit defender)
        {
            return _turnManager.GetAttackTargets(attacker).Contains(defender);
        }

        internal bool CanUseSkill(Unit user, Unit target)
        {
            return user.UnitClass.Skill.GetTargets(user, _turnManager).Contains(target);
        }

        internal void Attack(Unit attacker, Unit defender)
        {
            _turnManager.Attack(attacker, defender);
            _history.Clear();
        }

        internal void UseSkill(Unit user, Unit target)
        {
            _turnManager.UseSkill(user, target);
            _history.Clear();
        }

        internal string SkillName(Unit unit)
        {
            Skill skill = unit.UnitClass.Skill;
            return skill == null ? "스킬" : skill.DisplayName;
        }

        internal void CompleteAction(Unit unit)
        {
            unit.HasMoved = true;
            if (!GameManager.Instance.IsPlaying)
            {
                return;
            }

            if (_turnManager.AreAllPlayersMoved())
            {
                EndTurn();
            }
        }

        internal void UndoLast()
        {
            if (CanAcceptInput()) _history.UndoLast();
        }

        internal bool CanAcceptInput()
        {
            return GameManager.Instance.IsPlaying && !GameManager.Instance.InputBlocked && !GameManager.Instance.IsPresenting
                && _turnManager.Phase == BattlePhase.Player;
        }

        public void OnEndTurnClicked()
        {
            if (CanAcceptInput()) EndTurn();
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
