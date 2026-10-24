using UnityEngine;

namespace MiniTactics.Lesson02
{
    public readonly struct MovementCell
    {
        public MovementCell(Vector2Int cell, bool isOccupied)
        {
            Cell = cell;
            IsOccupied = isOccupied;
        }

        public Vector2Int Cell { get; }
        public bool IsOccupied { get; }
    }

}
