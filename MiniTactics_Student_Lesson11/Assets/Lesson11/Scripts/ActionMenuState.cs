using UnityEngine;

namespace MiniTactics.Lesson11
{
    public sealed class ActionMenuState : InputState
    {
        private readonly InputStateMachine _machine;
        private readonly BattleController _controller;
        private readonly Unit _unit;

        public ActionMenuState(InputStateMachine machine, BattleController controller, Unit unit)
        {
            _machine = machine;
            _controller = controller;
            _unit = unit;
        }

        public override void Enter()
        {
            _controller.ActionMenu.AttackClicked += OnAttackClicked;
            _controller.ActionMenu.SkillClicked += OnSkillClicked;
            _controller.ActionMenu.WaitClicked += OnWaitClicked;
            _controller.ActionMenu.Show(
                _controller.HasAttackTargets(_unit),
                _controller.HasSkillTargets(_unit),
                _controller.SkillName(_unit));
        }

        public override void Exit()
        {
            _controller.ActionMenu.AttackClicked -= OnAttackClicked;
            _controller.ActionMenu.SkillClicked -= OnSkillClicked;
            _controller.ActionMenu.WaitClicked -= OnWaitClicked;
            _controller.ActionMenu.Hide();
        }

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (!_controller.CanAcceptInput()) return;
        }

        public override void HandleUndo()
        {
            if (!_controller.CanAcceptInput()) return;
            if (_unit.HasMoved)
            {
                _controller.UndoLast();
            }

            _machine.ChangeState(new IdleState(_machine, _controller));
        }

        private void OnAttackClicked()
        {
            if (!_controller.CanAcceptInput()) return;
            _machine.ChangeState(new AttackState(_machine, _controller, _unit));
        }

        private void OnSkillClicked()
        {
            if (!_controller.CanAcceptInput()) return;
            _machine.ChangeState(new SkillState(_machine, _controller, _unit));
        }

        private void OnWaitClicked()
        {
            if (!_controller.CanAcceptInput()) return;
            _machine.ChangeState(new IdleState(_machine, _controller));
            _controller.CompleteAction(_unit);
        }
    }
}
