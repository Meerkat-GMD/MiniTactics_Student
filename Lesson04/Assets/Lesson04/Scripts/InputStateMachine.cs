using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class InputStateMachine
    {
        public InputState Current { get; private set; }

        public void ChangeState(InputState next)
        {
        }

        public void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
        }

        public void HandleUndo()
        {
        }
    }
}
