using UnityEngine;

namespace MiniTactics.Lesson11
{
    public sealed class ActionMenuState : InputState
    {
        private readonly BattleController _controller;
        private readonly Unit _unit;

        public ActionMenuState(BattleController controller, Unit unit)
        {
            _controller = controller;
            _unit = unit;
        }

        public override void Enter()
        {
            _controller.ActionMenu.AttackClicked += OnAttackClicked;
            _controller.ActionMenu.SkillClicked += OnSkillClicked;
            _controller.ActionMenu.WaitClicked += OnWaitClicked;
            _controller.ActionMenu.Show(_controller.HasAttackTargets(_unit),
                _controller.HasSkillTargets(_unit), _controller.SkillName(_unit));
        }

        public override void Exit()
        {
            _controller.ActionMenu.AttackClicked -= OnAttackClicked;
            _controller.ActionMenu.SkillClicked -= OnSkillClicked;
            _controller.ActionMenu.WaitClicked -= OnWaitClicked;
            _controller.ActionMenu.Hide();
        }

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell) { }

        private bool CanRespond() => _controller.CanAcceptInput() &&
            _controller.IsCurrentState(this) && _unit.IsAlive;

        public override void HandleUndo()
        {
            if (!CanRespond()) return;
            if (_unit.HasMoved) _controller.UndoLast();
            _controller.ReturnToIdle();
        }

        private void OnAttackClicked()
        {
            if (CanRespond() && _controller.HasAttackTargets(_unit)) _controller.RequestAttack(_unit);
        }

        private void OnSkillClicked()
        {
            if (CanRespond() && _controller.HasSkillTargets(_unit)) _controller.RequestSkill(_unit);
        }

        private void OnWaitClicked()
        {
            if (!CanRespond()) return;
            _controller.ReturnToIdle();
            _controller.CompleteAction(_unit);
        }
    }
}
