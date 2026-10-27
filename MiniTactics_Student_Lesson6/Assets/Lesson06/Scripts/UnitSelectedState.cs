using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class UnitSelectedState : InputState
    {
        private readonly BattleController _controller;
        private readonly Unit _unit;

        public UnitSelectedState(BattleController controller, Unit unit)
        {
            _controller = controller;
            _unit = unit;
        }

        public override void Enter() => _controller.ShowSelection(_unit);
        public override void Exit() => _controller.ClearSelection();

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (_controller.CanSelect(clickedUnit))
            {
                _controller.Select(clickedUnit);
                return;
            }

            if (_controller.ExecuteMove(_unit, cell))
            {
                _controller.Select(null);
                _controller.RequestAction(_unit, true);
            }
        }

        public override void HandleUndo()
        {
            if (_controller.UndoLast())
            {
                _controller.ShowSelection(_unit);
            }
        }
    }
}
