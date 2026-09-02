using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class IdleState : InputState
    {
        public IdleState(InputStateMachine machine, BattleController controller)
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
