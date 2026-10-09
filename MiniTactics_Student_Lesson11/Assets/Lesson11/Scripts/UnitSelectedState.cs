using UnityEngine;

namespace MiniTactics.Lesson11
{
    public sealed class UnitSelectedState : InputState
    {
        private readonly InputStateMachine _machine;
        private readonly BattleController _controller;
        private readonly Unit _unit;

        public UnitSelectedState(InputStateMachine machine, BattleController controller, Unit unit)
        {
            _machine = machine;
            _controller = controller;
            _unit = unit;
        }

        public override void Enter() => _controller.ShowSelection(_unit);
        public override void Exit() => _controller.ClearSelection();

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (!_controller.CanAcceptInput()) return;
            if (clickedUnit == _unit)
            {
                if (_controller.HasAttackTargets(_unit) || _controller.HasSkillTargets(_unit))
                {
                    _machine.ChangeState(new ActionMenuState(_machine, _controller, _unit));
                }

                return;
            }

            if (clickedUnit != null && _controller.CanAct(clickedUnit))
            {
                _machine.ChangeState(new UnitSelectedState(_machine, _controller, clickedUnit));
                return;
            }

            if (!_controller.ExecuteMove(_unit, cell))
            {
                _machine.ChangeState(new IdleState(_machine, _controller));
                return;
            }

            if (_controller.HasAttackTargets(_unit) || _controller.HasSkillTargets(_unit))
            {
                _machine.ChangeState(new ActionMenuState(_machine, _controller, _unit));
                return;
            }

            _machine.ChangeState(new IdleState(_machine, _controller));
            _controller.CompleteAction(_unit);
        }

        public override void HandleUndo()
        {
            if (!_controller.CanAcceptInput()) return;
            _controller.UndoLast();
            _controller.ShowSelection(_unit);
        }
    }
}
