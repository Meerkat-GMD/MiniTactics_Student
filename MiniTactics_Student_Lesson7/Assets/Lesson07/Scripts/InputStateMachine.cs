using System;
using UnityEngine;

namespace MiniTactics.Lesson07
{
    public sealed class InputStateMachine
    {
        public InputState Current { get; private set; }

        public void ChangeState(InputState next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (ReferenceEquals(Current, next))
            {
                return;
            }

            Current?.Exit();
            Current = next;
            Current.Enter();
        }

        public void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            Current?.HandleBoardClick(clickedUnit, cell);
        }

        public void HandleUndo()
        {
            Current?.HandleUndo();
        }
    }
}
