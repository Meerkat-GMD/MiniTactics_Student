using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class UnitSelectedState : InputState
    {
        public UnitSelectedState(
            InputStateMachine machine,
            BattleController controller,
            Unit unit)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
        }

        public override void HandleUndo()
        {
        }
    }
}
