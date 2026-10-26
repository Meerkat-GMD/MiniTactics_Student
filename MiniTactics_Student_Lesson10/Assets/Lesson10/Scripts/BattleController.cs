using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace MiniTactics.Lesson10
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private SpriteRenderer _overlayPrefab;
        [SerializeField] private LayerMask _unitLayerMask = ~0;
        [SerializeField] private ActionMenu _actionMenu;
        public ActionMenu ActionMenu => _actionMenu;

        private RangeHighlighter _highlighter;
        private readonly CommandHistory _history = new CommandHistory();
        private InputStateMachine _stateMachine;

        public Unit SelectedUnit { get; private set; }

        private void Awake()
        {
            _highlighter = new RangeHighlighter(_overlayRoot, _overlayPrefab, _board);
            _stateMachine = new InputStateMachine();
            _stateMachine.ChangeState(new IdleState(this));
        }

        public void Select(Unit unit)
        {
            _stateMachine.ChangeState(CanSelect(unit)
                ? new UnitSelectedState(this, unit)
                : new IdleState(this));
        }

        internal bool IsCurrentState(InputState state) => _stateMachine.Current == state;

        internal bool CanSelect(Unit unit) => GameManager.Instance.Turns.CanAct(unit);

        internal void ShowSelection(Unit unit)
        {
            SelectedUnit = unit;
            _highlighter.Show(_board.CalculateMovementRange(unit), unit);
        }

        internal void ClearSelection()
        {
            SelectedUnit = null;
            _highlighter.Hide();
        }

        internal bool ExecuteMove(Unit unit, Vector2Int targetCell)
        {
            if (!CanSelect(unit) || !_board.CanMoveTo(unit, targetCell))
            {
                return false;
            }

            MoveUnitCommand command = new MoveUnitCommand(unit, _board, targetCell);
            _history.Execute(command);
            return true;
        }

        internal bool UndoLast() => _history.UndoLast();

        public void OnUndoClicked()
        {
            if (!CanAcceptInput()) return;
            _stateMachine.HandleUndo();
        }

        internal bool CanAcceptInput() => GameManager.Instance.IsPlaying &&
            GameManager.Instance.Turns.Phase == BattlePhase.Player;

        public void OnEndTurnClicked()
        {
            if (!CanAcceptInput()) return;
            Select(null);
            _history.Clear();
            GameManager.Instance.Turns.EndPlayerPhase();
        }

        internal void RequestAttack(Unit unit)
        {
            if (!CanAcceptInput()) return;
            _stateMachine.ChangeState(new AttackState(this, unit));
        }

        internal void RequestAction(Unit unit, bool afterMove)
        {
            if (!CanAcceptInput()) return;
            if (HasAttackTargets(unit) || HasSkillTargets(unit))
            {
                _stateMachine.ChangeState(new ActionMenuState(this, unit));
                return;
            }
            if (afterMove) CompleteAction(unit);
        }

        internal bool HasAttackTargets(Unit unit) =>
            GameManager.Instance.Turns.GetAttackTargets(unit).Count > 0;

        internal void ShowAttackTargets(Unit unit)
        {
            if (!CanAcceptInput()) return;
            SelectedUnit = unit;
            _highlighter.ShowTargets(GameManager.Instance.Turns.GetAttackTargets(unit));
        }

        internal bool CanAttack(Unit attacker, Unit defender) => CanAcceptInput() &&
            attacker.IsAlive && attacker.Team == Team.Player && attacker.CanMove &&
            GameManager.Instance.Turns.CanAttack(attacker, defender);

        internal void Attack(Unit attacker, Unit defender)
        {
            if (CanAttack(attacker, defender)) GameManager.Instance.Turns.Attack(attacker, defender);
        }

        internal bool HasSkillTargets(Unit unit) => unit.IsAlive && unit.UnitClass.Skill != null &&
            unit.UnitClass.Skill.GetTargets(unit, GameManager.Instance.Turns).Count > 0;

        internal string SkillName(Unit unit) =>
            unit.UnitClass.Skill != null ? unit.UnitClass.Skill.DisplayName : "스킬";

        internal void ShowSkillTargets(Unit unit)
        {
            if (!CanAcceptInput()) return;
            SelectedUnit = unit;
            _highlighter.ShowTargets(unit.UnitClass.Skill.GetTargets(unit, GameManager.Instance.Turns));
        }

        internal bool CanUseSkill(Unit user, Unit target) => CanAcceptInput() && user.IsAlive &&
            user.Team == Team.Player && user.CanMove && user.UnitClass.Skill != null &&
            user.UnitClass.Skill.GetTargets(user, GameManager.Instance.Turns).Contains(target);

        internal void UseSkill(Unit user, Unit target)
        {
            if (!CanUseSkill(user, target)) return;
            Vector3 position = target.transform.position;
            user.UnitClass.Skill.Use(user, target, GameManager.Instance.Turns);
            user.GetComponent<UnitAnimator>().PlaySkill(position);
            _history.Clear();
        }

        internal void RequestSkill(Unit unit)
        {
            if (!CanAcceptInput()) return;
            _stateMachine.ChangeState(new SkillState(this, unit));
        }

        internal void CompleteAction(Unit unit)
        {
            unit.HasMoved = true;
            _history.Clear();
            if (!GameManager.Instance.IsPlaying) return;
            if (GameManager.Instance.Turns.AreAllPlayersMoved()) OnEndTurnClicked();
        }

        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = screenPosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            return hits.Count > 0;
        }

        private void Update()
        {
            if (!CanAcceptInput()) return;
            if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            {
                OnEndTurnClicked();
                return;
            }
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            if (IsPointerOverUI(screenPosition))
            {
                return;
            }

            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPosition, _unitLayerMask);
            Unit clickedUnit = hit != null ? hit.GetComponentInParent<Unit>() : null;
            _stateMachine.HandleBoardClick(clickedUnit, _board.WorldToCell(worldPosition));
        }
    }
}
