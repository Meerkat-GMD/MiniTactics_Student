using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class IdleState : InputState
    {
        private readonly InputStateMachine _machine;
        private readonly BattleController _controller;

        public IdleState(InputStateMachine machine, BattleController controller)
        {
            _machine = machine;
            _controller = controller;
        }

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (clickedUnit != null && _controller.CanAct(clickedUnit))
            {
                _machine.ChangeState(new UnitSelectedState(_machine, _controller, clickedUnit));
            }
        }

        public override void HandleUndo()
        {
            _controller.UndoLast();
        }
    }
}
