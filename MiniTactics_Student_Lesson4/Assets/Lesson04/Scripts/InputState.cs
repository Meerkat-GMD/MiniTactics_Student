using UnityEngine;

namespace MiniTactics.Lesson04
{
    public abstract class InputState
    {
        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public abstract void HandleBoardClick(Unit clickedUnit, Vector2Int cell);
        public abstract void HandleUndo();
    }
}
