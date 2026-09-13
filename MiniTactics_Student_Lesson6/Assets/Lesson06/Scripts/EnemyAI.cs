using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson06
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
            if (enemy == null || _board == null)
            {
                return Vector2Int.zero;
            }

            Vector2Int start = _board.WorldToCell(enemy.transform.position);
            MovementRangeResult movementRange = _board.GetMovementRange(enemy);
            var orderedCandidates = movementRange.Costs
                .Where(pair => pair.Key != start)
                .OrderBy(pair => ManhattanDistance(pair.Key, goalCell))
                .ThenBy(pair => pair.Value)
                .ThenBy(pair => pair.Key.y)
                .ThenBy(pair => pair.Key.x);

            if (!orderedCandidates.Any())
            {
                return start;
            }

            var bestCandidate = orderedCandidates.First();
            if (ManhattanDistance(bestCandidate.Key, goalCell) >=
                ManhattanDistance(start, goalCell))
            {
                return start;
            }

            return bestCandidate.Key;
        }

        public Unit ChooseAttackTarget(Unit enemy, IEnumerable<Unit> units)
        {
            // 학습 과제: 공격 가능한 아군 후보와 동률 선택 규칙을 완성한다.
            return null;
        }

        public UniTask TakeCombatTurnAsync(
            Unit enemy,
            IReadOnlyList<Unit> units,
            CancellationToken cancellationToken)
        {
            // 학습 과제: 공격 우선 및 이동 후 공격을 완성한다.
            // 그 전에는 실제 장면의 아군 목표를 향한 5차시 이동을 사용한다.
            if (!CanTakeCombatTurn(enemy)) return UniTask.CompletedTask;
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return UniTask.CompletedTask;
            Vector2Int goalCell = _board.WorldToCell(enemy.transform.position);
            ObjectiveZone playerGoal = UnityEngine.Object.FindObjectsByType<ObjectiveZone>()
                .FirstOrDefault(goal => goal.Owner == Team.Player);
            if (playerGoal != null) goalCell = playerGoal.Cell;
            return TakeTurnAsync(enemy, goalCell, cancellationToken);
        }

        public async UniTask TakeTurnAsync(
            Unit enemy,
            Vector2Int goalCell,
            CancellationToken cancellationToken)
        {
            if (!CanTakeCombatTurn(enemy))
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            Vector2Int start = _board.WorldToCell(enemy.transform.position);
            Vector2Int destination = ChooseDestination(enemy, goalCell);
            if (destination != start)
            {
                new MoveUnitCommand(enemy, _board, destination).Execute();
            }

            enemy.SetMoved(true);
            await UniTask.Delay(
                TimeSpan.FromMilliseconds(_turnDelayMilliseconds),
                cancellationToken: cancellationToken);
        }

        private bool CanTakeCombatTurn(Unit enemy)
        {
            return enemy != null && enemy.isActiveAndEnabled && enemy.IsAlive &&
                enemy.Team == Team.Enemy && enemy.IsTurnUnit && _board != null &&
                _board.Board.Cells.Count > 0 && _board.Board.Contains(_board.WorldToCell(enemy.transform.position));
        }

        private static int ManhattanDistance(Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        }
    }
}
