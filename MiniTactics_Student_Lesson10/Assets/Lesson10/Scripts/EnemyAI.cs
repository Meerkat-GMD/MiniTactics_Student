using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson10
{
    public sealed class EnemyAI : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField, Min(0)] private int _delayMilliseconds = 350;

        public Vector2Int ChooseDestination(Unit enemy, Vector2Int goalCell)
        {
            Vector2Int start = _board.WorldToCell(enemy.transform.position);
            MovementRangeResult routes = GetRoutes(enemy, goalCell);
            MovementRangeResult reachable = _board.GetMovementRange(enemy);
            Vector2Int destination = start;
            foreach (Vector2Int cell in routes.GetPathTo(goalCell))
            {
                if (!reachable.TryGetCost(cell, out _)) break;
                destination = cell;
            }

            return destination;
        }

        private MovementRangeResult GetRoutes(Unit enemy, Vector2Int goalCell)
        {
            Vector2Int start = _board.WorldToCell(enemy.transform.position);
            HashSet<Vector2Int> blocked = GetRouteBlockers(enemy);
            blocked.Remove(goalCell);
            return MovementRange.Calculate(_board.Board, start, int.MaxValue, blocked);
        }

        private HashSet<Vector2Int> GetRouteBlockers(Unit enemy)
        {
            var blocked = new HashSet<Vector2Int>();
            foreach (Vector2Int cell in _board.Board.Cells.Keys)
            {
                if (_board.Board.GetTerrain(cell).MovementCost > enemy.MoveBudget
                    || _board.IsOccupiedByOther(enemy, cell))
                {
                    blocked.Add(cell);
                }
            }

            return blocked;
        }

        public Unit ChooseNearestAlly(Unit enemy, List<Unit> units)
        {
            Unit nearest = null;
            int bestDistance = int.MaxValue;
            Vector2Int origin = _board.WorldToCell(enemy.transform.position);
            foreach (Unit unit in units)
            {
                if (!unit.IsAlive || unit.Team == enemy.Team) continue;
                Vector2Int cell = _board.WorldToCell(unit.transform.position);
                int distance = Distance(origin, cell);
                if (!GetRoutes(enemy, cell).TryGetCost(cell, out _)) continue;
                if (distance < bestDistance)
                {
                    nearest = unit;
                    bestDistance = distance;
                }
            }

            return nearest;
        }

        public Unit ChooseTarget(List<Unit> targets)
        {
            Unit weakest = targets[0];
            foreach (Unit target in targets)
            {
                if (target.CurrentHp < weakest.CurrentHp)
                {
                    weakest = target;
                }
            }

            return weakest;
        }

        public async UniTask TakeTurnAsync(Unit enemy, Vector2Int goalCell, CancellationToken token)
        {
            enemy.MoveTo(_board, ChooseDestination(enemy, goalCell));
            await UniTask.Delay(_delayMilliseconds, cancellationToken: token);
        }

        private static int Distance(Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        }
    }
}
