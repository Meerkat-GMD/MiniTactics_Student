using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class TurnManager : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _spawner;
        [SerializeField] private BoardView _board;
        [SerializeField] private EnemyAI _enemyAi;
        public BattlePhase Phase { get; private set; } = BattlePhase.Player;

        public bool CanAct(Unit unit) => unit != null && unit.CanMove &&
            unit.Team == Team.Player && Phase == BattlePhase.Player && !unit.HasMoved;

        public bool AreAllPlayersMoved()
        {
            bool found = false;
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team != Team.Player || !unit.CanMove) continue;
                found = true;
                if (!unit.HasMoved) return false;
            }
            return found;
        }

        public void EndPlayerPhase()
        {
            if (Phase != BattlePhase.Player) return;
            Phase = BattlePhase.Enemy;
            RunEnemyPhaseAsync(destroyCancellationToken).Forget();
        }

        private async UniTask RunEnemyPhaseAsync(CancellationToken token)
        {
            try
            {
                foreach (Unit unit in _spawner.Units)
                {
                    token.ThrowIfCancellationRequested();
                    if (unit.Team != Team.Enemy || !unit.CanMove) continue;
                    Unit target = FindNearestPlayer(unit);
                    if (target == null) continue;
                    Vector2Int goal = _board.WorldToCell(target.transform.position);
                    await _enemyAi.TakeTurnAsync(unit, goal, token);
                }
                token.ThrowIfCancellationRequested();
                BeginPlayerPhase();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        }

        private void BeginPlayerPhase()
        {
            foreach (Unit unit in _spawner.Units) unit.HasMoved = false;
            Phase = BattlePhase.Player;
        }

        private Unit FindNearestPlayer(Unit enemy)
        {
            Unit nearest = null;
            int bestDistance = int.MaxValue;
            Vector2Int origin = _board.WorldToCell(enemy.transform.position);
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team != Team.Player) continue;
                Vector2Int cell = _board.WorldToCell(unit.transform.position);
                int distance = Mathf.Abs(origin.x - cell.x) + Mathf.Abs(origin.y - cell.y);
                if (distance >= bestDistance) continue;
                nearest = unit;
                bestDistance = distance;
            }
            return nearest;
        }
    }
}
