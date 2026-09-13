using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson05
{
    public sealed class EnemyAI : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField, Min(0)] private int _turnDelayMilliseconds = 350;

        public int TurnDelayMilliseconds
        {
            get => _turnDelayMilliseconds;
            set => _turnDelayMilliseconds = Mathf.Max(0, value);
        }

        public Vector2Int ChooseDestination(Unit enemy, Vector2Int goalCell)
        {
            return enemy != null && _board != null
                ? _board.WorldToCell(enemy.transform.position)
                : Vector2Int.zero;
        }

        public UniTask TakeTurnAsync(
            Unit enemy,
            Vector2Int goalCell,
            CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
