using UnityEngine;

namespace MiniTactics.Lesson11
{
    public sealed class IdleState : InputState
    {
        private readonly BattleController _controller;

        public IdleState(BattleController controller)
        {
            _controller = controller;
        }

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (_controller.CanSelect(clickedUnit))
            {
                _controller.Select(clickedUnit);
            }
        }

        public override void HandleUndo() => _controller.UndoLast();
    }
}
