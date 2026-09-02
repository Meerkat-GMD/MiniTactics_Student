using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson03
{
    public sealed class MovementRangeResult
    {
        public IReadOnlyCollection<Vector2Int> ReachableCells =>
            Array.Empty<Vector2Int>();

        public static MovementRangeResult Empty(Vector2Int start)
        {
            return new MovementRangeResult();
        }

        public bool TryGetCost(Vector2Int cell, out int cost)
        {
            cost = 0;
            return false;
        }

        public IReadOnlyList<Vector2Int> GetPathTo(Vector2Int destination)
        {
            return Array.Empty<Vector2Int>();
        }
    }

    public static class MovementRange
    {
        public static MovementRangeResult Calculate(
            Board board,
            Vector2Int start,
            int budget)
        {
            return MovementRangeResult.Empty(start);
        }
    }
}
