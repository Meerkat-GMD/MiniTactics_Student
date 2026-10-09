using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class SkillState : InputState
    {
        private readonly InputStateMachine _machine;
        private readonly BattleController _controller;
        private readonly Unit _user;

        public SkillState(InputStateMachine machine, BattleController controller, Unit user)
        {
            _machine = machine;
            _controller = controller;
            _user = user;
        }

        public override void Enter()
        {
            _controller.ShowSkillTargets(_user);
        }

        public override void Exit()
        {
            _controller.ClearSelection();
        }

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (!_controller.CanAcceptInput()) return;
            if (clickedUnit == null || !_controller.CanUseSkill(_user, clickedUnit))
            {
                _machine.ChangeState(new ActionMenuState(_machine, _controller, _user));
                return;
            }

            _controller.UseSkill(_user, clickedUnit);
            _machine.ChangeState(new IdleState(_machine, _controller));
            _controller.CompleteAction(_user);
        }

        public override void HandleUndo()
        {
            if (!_controller.CanAcceptInput()) return;
            if (_user.HasMoved)
            {
                _controller.UndoLast();
            }

            _machine.ChangeState(new IdleState(_machine, _controller));
        }
    }
}
