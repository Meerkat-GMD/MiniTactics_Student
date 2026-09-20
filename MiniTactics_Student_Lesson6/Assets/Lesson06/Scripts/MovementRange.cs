using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class MovementRangeResult
    {
        private readonly Vector2Int _start;
        private readonly Dictionary<Vector2Int, int> _costs;
        private readonly Dictionary<Vector2Int, Vector2Int> _previous;

        public MovementRangeResult(
            Vector2Int start,
            Dictionary<Vector2Int, int> costs,
            Dictionary<Vector2Int, Vector2Int> previous)
        {
            _start = start;
            _costs = costs;
            _previous = previous;
        }

        public IReadOnlyDictionary<Vector2Int, int> Costs => _costs;

        public bool Contains(Vector2Int cell)
        {
            return _costs.ContainsKey(cell);
        }

        public List<Vector2Int> GetPathTo(Vector2Int destination)
        {
            List<Vector2Int> path = new List<Vector2Int> { destination };
            Vector2Int current = destination;

            while (current != _start)
            {
                current = _previous[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }

    public static class MovementRange
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        public static MovementRangeResult Calculate(
            Board board,
            Vector2Int start,
            int budget,
            HashSet<Vector2Int> blockedCells)
        {
            Dictionary<Vector2Int, int> costs = new Dictionary<Vector2Int, int> { [start] = 0 };
            Dictionary<Vector2Int, Vector2Int> previous = new Dictionary<Vector2Int, Vector2Int>();
            List<Vector2Int> frontier = new List<Vector2Int> { start };

            while (frontier.Count > 0)
            {
                Vector2Int current = TakeLowestCost(frontier, costs);

                foreach (Vector2Int direction in Directions)
                {
                    Vector2Int next = current + direction;
                    if (!board.Contains(next) || !board.GetTile(next).IsWalkable)
                    {
                        continue;
                    }

                    if (blockedCells.Contains(next))
                    {
                        continue;
                    }

                    int nextCost = costs[current] + board.GetTile(next).MovementCost;
                    if (nextCost > budget)
                    {
                        continue;
                    }

                    if (costs.TryGetValue(next, out int knownCost) && knownCost <= nextCost)
                    {
                        continue;
                    }

                    costs[next] = nextCost;
                    previous[next] = current;
                    frontier.Add(next);
                }
            }

            return new MovementRangeResult(start, costs, previous);
        }

        private static Vector2Int TakeLowestCost(List<Vector2Int> frontier, Dictionary<Vector2Int, int> costs)
        {
            Vector2Int best = frontier[0];
            foreach (Vector2Int cell in frontier)
            {
                if (costs[cell] < costs[best])
                {
                    best = cell;
                }
            }

            frontier.Remove(best);
            return best;
        }
    }
}
