using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class MovementRangeResult
    {
        private readonly Vector2Int _start;
        private readonly Dictionary<Vector2Int, int> _costs;
        private readonly Dictionary<Vector2Int, Vector2Int> _previous;

        internal MovementRangeResult(
            Vector2Int start,
            Dictionary<Vector2Int, int> costs,
            Dictionary<Vector2Int, Vector2Int> previous)
        {
            _start = start;
            _costs = costs;
            _previous = previous;
        }

        public IReadOnlyDictionary<Vector2Int, int> Costs => _costs;
        public IReadOnlyCollection<Vector2Int> ReachableCells => _costs.Keys;

        public static MovementRangeResult Empty(Vector2Int start)
        {
            return new MovementRangeResult(
                start,
                new Dictionary<Vector2Int, int>(),
                new Dictionary<Vector2Int, Vector2Int>());
        }

        public bool TryGetCost(Vector2Int cell, out int cost)
        {
            return _costs.TryGetValue(cell, out cost);
        }

        public IReadOnlyList<Vector2Int> GetPathTo(Vector2Int destination)
        {
            if (!_costs.ContainsKey(destination))
            {
                return Array.Empty<Vector2Int>();
            }

            List<Vector2Int> path = new List<Vector2Int> { destination };
            Vector2Int current = destination;

            while (current != _start)
            {
                if (!_previous.TryGetValue(current, out current))
                {
                    return Array.Empty<Vector2Int>();
                }

                path.Add(current);
            }

            path.Reverse();
            return path;
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
            Board board,
            Vector2Int start,
            int budget)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (budget < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(budget));
            }

            Dictionary<Vector2Int, int> costs = new Dictionary<Vector2Int, int>();
            Dictionary<Vector2Int, Vector2Int> previous =
                new Dictionary<Vector2Int, Vector2Int>();

            if (!board.Contains(start) || !board.GetTerrain(start).IsWalkable)
            {
                return new MovementRangeResult(start, costs, previous);
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
                    if (!board.Contains(next))
                    {
                        continue;
                    }

                    TerrainType terrain = board.GetTerrain(next);
                    if (!terrain.IsWalkable)
                    {
                        continue;
                    }

                    int candidateCost = costs[current] + terrain.MovementCost;
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
                    previous[next] = current;
                    frontier.Add(next);
                }
            }

            return new MovementRangeResult(start, costs, previous);
        }

        private static Vector2Int FindLowestCost(
            IEnumerable<Vector2Int> frontier,
            IReadOnlyDictionary<Vector2Int, int> costs)
        {
            bool found = false;
            Vector2Int best = default;
            int bestCost = int.MaxValue;

            foreach (Vector2Int cell in frontier)
            {
                int cost = costs[cell];
                if (!found || cost < bestCost)
                {
                    found = true;
                    best = cell;
                    bestCost = cost;
                }
            }

            return best;
        }
    }
}
