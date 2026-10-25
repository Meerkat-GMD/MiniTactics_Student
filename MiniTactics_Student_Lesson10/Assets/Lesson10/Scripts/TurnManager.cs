using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson10
{
    public sealed class TurnManager : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _spawner;
        [SerializeField] private BoardView _board;
        [SerializeField] private EnemyAI _enemyAi;
        public BattlePhase Phase { get; private set; } = BattlePhase.Player;

        public bool CanAct(Unit unit) => unit != null && unit.IsAlive && unit.CanMove &&
            unit.Team == Team.Player && Phase == BattlePhase.Player && !unit.HasMoved &&
            GameManager.Instance.IsPlaying;

        public bool AreAllPlayersMoved()
        {
            bool found = false;
            foreach (Unit unit in _spawner.Units)
            {
                if (!unit.IsAlive || unit.Team != Team.Player || !unit.CanMove) continue;
                found = true;
                if (!unit.HasMoved) return false;
            }
            return found;
        }

        public List<Unit> GetAttackTargets(Unit attacker)
        {
            List<Unit> targets = new List<Unit>();
            if (attacker == null || !attacker.IsAlive) return targets;
            Vector2Int origin = _board.WorldToCell(attacker.transform.position);
            foreach (Unit unit in _spawner.Units)
            {
                if (!unit.IsAlive || unit.Team == attacker.Team) continue;
                Vector2Int cell = _board.WorldToCell(unit.transform.position);
                int distance = Mathf.Abs(origin.x - cell.x) + Mathf.Abs(origin.y - cell.y);
                if (distance >= 1 && distance <= attacker.AttackRange) targets.Add(unit);
            }
            return targets;
        }

        public List<Unit> GetUnitsInRange(Unit origin, int range, Team team)
        {
            List<Unit> targets = new List<Unit>();
            Vector2Int center = _board.WorldToCell(origin.transform.position);
            foreach (Unit unit in _spawner.Units)
            {
                if (!unit.IsAlive || unit.Team != team) continue;
                Vector2Int cell = _board.WorldToCell(unit.transform.position);
                int distance = Mathf.Abs(center.x - cell.x) + Mathf.Abs(center.y - cell.y);
                if (distance <= range) targets.Add(unit);
            }
            return targets;
        }

        public bool CanAttack(Unit attacker, Unit defender) =>
            GameManager.Instance.IsPlaying && GetAttackTargets(attacker).Contains(defender);

        public void Attack(Unit attacker, Unit defender)
        {
            if (!CanAttack(attacker, defender)) return;
            Attack(attacker, defender, 0);
        }

        public void Attack(Unit attacker, Unit defender, int bonusDamage)
        {
            if (!GameManager.Instance.IsPlaying || !attacker.IsAlive || !defender.IsAlive ||
                attacker.Team == defender.Team) return;
            Vector2Int cell = _board.WorldToCell(defender.transform.position);
            int damage = CombatCalculator.CalculateDamage(attacker.AttackPower, defender.Defense,
                _board.GetDefenseBonus(cell)) + bonusDamage;
            attacker.GetComponent<UnitAnimator>().PlayAttack(defender.transform.position);
            defender.TakeDamage(damage);
        }

        public void PushAway(Unit user, Unit target)
        {
            if (!target.IsAlive) return;
            Vector2Int origin = _board.WorldToCell(user.transform.position);
            Vector2Int cell = _board.WorldToCell(target.transform.position);
            Vector2Int delta = cell - origin;
            Vector2Int direction = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y)
                ? new Vector2Int(Math.Sign(delta.x), 0)
                : new Vector2Int(0, Math.Sign(delta.y));
            Vector2Int destination = cell + direction;
            if (!_board.IsInside(destination)) return;
            if (!_board.Board.GetTile(destination).IsWalkable) return;
            if (_board.IsOccupiedByOther(target, destination)) return;
            target.MoveTo(_board, destination);
        }

        public void EndPlayerPhase()
        {
            if (!GameManager.Instance.IsPlaying || Phase != BattlePhase.Player) return;
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
                    if (!GameManager.Instance.IsPlaying) return;
                    if (!unit.IsAlive || unit.Team != Team.Enemy || !unit.CanMove) continue;
                    Unit target = FindNearestPlayer(unit);
                    if (target == null) continue;
                    Vector2Int goal = _board.WorldToCell(target.transform.position);
                    await _enemyAi.TakeTurnAsync(unit, goal, token);
                    token.ThrowIfCancellationRequested();
                    if (!GameManager.Instance.IsPlaying) return;
                    List<Unit> targets = GetAttackTargets(unit);
                    if (targets.Count > 0) Attack(unit, _enemyAi.ChooseTarget(targets));
                    if (!GameManager.Instance.IsPlaying) return;
                }
                token.ThrowIfCancellationRequested();
                if (GameManager.Instance.IsPlaying) BeginPlayerPhase();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        }

        private void BeginPlayerPhase()
        {
            if (!GameManager.Instance.IsPlaying) return;
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.IsAlive) unit.HasMoved = false;
            }
            Phase = BattlePhase.Player;
        }

        private Unit FindNearestPlayer(Unit enemy)
        {
            Unit nearest = null;
            int bestDistance = int.MaxValue;
            Vector2Int origin = _board.WorldToCell(enemy.transform.position);
            foreach (Unit unit in _spawner.Units)
            {
                if (!unit.IsAlive || unit.Team != Team.Player) continue;
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
