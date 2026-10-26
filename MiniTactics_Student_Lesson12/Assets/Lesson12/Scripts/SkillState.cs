using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class SkillState : InputState
    {
        private readonly BattleController _controller;
        private readonly Unit _user;

        public SkillState(BattleController controller, Unit user)
        {
            _controller = controller;
            _user = user;
        }

        public override void Enter() => _controller.ShowSkillTargets(_user);
        public override void Exit() => _controller.ClearSelection();

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (!_controller.CanAcceptInput() || !_controller.IsCurrentState(this)) return;
            if (!_controller.CanUseSkill(_user, clickedUnit))
            {
                _controller.RequestAction(_user, false);
                return;
            }
            _controller.UseSkill(_user, clickedUnit);
            _controller.ReturnToIdle();
            _controller.CompleteAction(_user);
        }

        public override void HandleUndo()
        {
            if (!_controller.CanAcceptInput() || !_controller.IsCurrentState(this)) return;
            if (_user.HasMoved) _controller.UndoLast();
            _controller.ReturnToIdle();
        }
    }
}
