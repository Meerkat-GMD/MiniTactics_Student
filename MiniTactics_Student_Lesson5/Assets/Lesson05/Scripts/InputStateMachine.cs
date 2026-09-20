using UnityEngine;

namespace MiniTactics.Lesson05
{
    public sealed class InputStateMachine
    {
        public InputState Current { get; private set; }

        public void ChangeState(InputState next)
        {
            Current?.Exit();
            Current = next;
            Current.Enter();
        }

        public void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            Current.HandleBoardClick(clickedUnit, cell);
        }

        public void HandleUndo()
        {
            Current.HandleUndo();
        }
    }
}
