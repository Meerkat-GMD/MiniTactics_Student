using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class MovementRangeResult
    {
        private readonly Dictionary<Vector2Int, int> _costs;

        public MovementRangeResult(Dictionary<Vector2Int, int> costs)
        {
            _costs = costs;
        }

        public IReadOnlyCollection<Vector2Int> ReachableCells => _costs.Keys;

        public static MovementRangeResult Empty()
        {
            return new MovementRangeResult(new Dictionary<Vector2Int, int>());
        }

        public bool TryGetCost(Vector2Int cell, out int cost)
        {
            return _costs.TryGetValue(cell, out cost);
        }
    }

    public static class MovementRange
    {
        private static readonly Vector2Int[] CardinalDirections =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        public static MovementRangeResult Calculate(
            BoardView board,
            Vector2Int start,
            int budget,
            ISet<Vector2Int> blockedCells)
        {
            Dictionary<Vector2Int, int> costs = new Dictionary<Vector2Int, int>();
            if (!board.TryGetMovementCost(start, out _))
            {
                return new MovementRangeResult(costs);
            }

            HashSet<Vector2Int> frontier = new HashSet<Vector2Int> { start };
            costs.Add(start, 0);

            while (frontier.Count > 0)
            {
                Vector2Int current = FindLowestCost(frontier, costs);
                frontier.Remove(current);

                foreach (Vector2Int direction in CardinalDirections)
                {
                    Vector2Int next = current + direction;
                    if (blockedCells.Contains(next) || !board.TryGetMovementCost(next, out int stepCost))
                    {
                        continue;
                    }

                    int candidateCost = costs[current] + stepCost;
                    if (candidateCost > budget)
                    {
                        continue;
                    }

                    if (costs.TryGetValue(next, out int knownCost) &&
                        knownCost <= candidateCost)
                    {
                        continue;
                    }

                    costs[next] = candidateCost;
                    frontier.Add(next);
                }
            }

            return new MovementRangeResult(costs);
        }

        private static Vector2Int FindLowestCost(
            IEnumerable<Vector2Int> frontier,
            IReadOnlyDictionary<Vector2Int, int> costs)
        {
            Vector2Int best = default;
            int bestCost = int.MaxValue;

            foreach (Vector2Int cell in frontier)
            {
                int cost = costs[cell];
                if (cost < bestCost)
                {
                    best = cell;
                    bestCost = cost;
                }
            }

            return best;
        }
    }
}
