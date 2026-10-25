using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson11
{
    public sealed class EnemyAI : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private int _delayMilliseconds = 350;

        public Unit ChooseTarget(System.Collections.Generic.IReadOnlyList<Unit> targets)
        {
            Unit weakest = targets[0];
            foreach (Unit unit in targets)
            {
                if (unit.CurrentHp < weakest.CurrentHp) weakest = unit;
            }
            return weakest;
        }

        public Vector2Int ChooseDestination(Unit enemy, Vector2Int goalCell)
        {
            Vector2Int best = _board.WorldToCell(enemy.transform.position);
            int bestDistance = Distance(best, goalCell);
            foreach (Vector2Int cell in _board.CalculateMovementRange(enemy).ReachableCells)
            {
                if (!_board.CanMoveTo(enemy, cell)) continue;
                int distance = Distance(cell, goalCell);
                if (distance >= bestDistance) continue;
                best = cell;
                bestDistance = distance;
            }
            return best;
        }

        private static int Distance(Vector2Int a, Vector2Int b) =>
            Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        public async UniTask TakeTurnAsync(Unit enemy, Vector2Int goalCell, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Vector2Int current = _board.WorldToCell(enemy.transform.position);
            Vector2Int destination = ChooseDestination(enemy, goalCell);
            if (destination != current && _board.CanMoveTo(enemy, destination))
                enemy.MoveTo(_board, destination);
            enemy.HasMoved = true;
            await UniTask.Delay(_delayMilliseconds, cancellationToken: token);
            token.ThrowIfCancellationRequested();
        }
    }
}
