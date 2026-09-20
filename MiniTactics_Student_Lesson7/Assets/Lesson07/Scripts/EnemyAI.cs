using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson07
{
    public sealed class EnemyAI : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField, Min(0)] private int _delayMilliseconds = 350;

        public Vector2Int ChooseDestination(Unit enemy, Vector2Int goalCell)
        {
            Vector2Int best = _board.WorldToCell(enemy.transform.position);
            foreach (Vector2Int cell in _board.GetMovementRange(enemy).Costs.Keys)
            {
                if (Distance(cell, goalCell) < Distance(best, goalCell))
                {
                    best = cell;
                }
            }

            return best;
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
