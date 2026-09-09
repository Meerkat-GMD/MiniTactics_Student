using UnityEngine;

namespace MiniTactics.Lesson05
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
            if (_controller.CanPlayerAct(clickedUnit))
                _machine.ChangeState(new UnitSelectedState(_machine, _controller, clickedUnit));
        }

        public override void HandleUndo() => _controller.UndoLast();
    }
}
