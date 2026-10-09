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

        public bool CanAct(Unit unit)
        {
            return GameManager.Instance.IsPlaying
                && Phase == BattlePhase.Player
                && unit.IsAlive
                && unit.CanMove
                && unit.Team == Team.Player
                && !unit.HasMoved;
        }

        public bool AreAllPlayersMoved()
        {
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team == Team.Player && unit.IsAlive && !unit.HasMoved)
                {
                    return false;
                }
            }

            return true;
        }

        public List<Unit> GetAttackTargets(Unit attacker)
        {
            List<Unit> targets = new List<Unit>();
            Vector2Int origin = _board.WorldToCell(attacker.transform.position);
            foreach (Unit unit in _spawner.Units)
            {
                Vector2Int cell = _board.WorldToCell(unit.transform.position);
                int distance = Mathf.Abs(cell.x - origin.x) + Mathf.Abs(cell.y - origin.y);
                if (unit.IsAlive && unit.Team != attacker.Team
                    && distance >= 1 && distance <= attacker.AttackRange)
                {
                    targets.Add(unit);
                }
            }

            return targets;
        }

        public List<Unit> GetUnitsInRange(Unit user, int range, Team team)
        {
            List<Unit> targets = new List<Unit>();
            Vector2Int origin = _board.WorldToCell(user.transform.position);
            foreach (Unit unit in _spawner.Units)
            {
                Vector2Int cell = _board.WorldToCell(unit.transform.position);
                int distance = Mathf.Abs(cell.x - origin.x) + Mathf.Abs(cell.y - origin.y);
                if (unit.IsAlive && unit.Team == team && distance <= range)
                {
                    targets.Add(unit);
                }
            }

            return targets;
        }

        public void Attack(Unit attacker, Unit defender)
        {
            Attack(attacker, defender, 0);
        }

        public void Attack(Unit attacker, Unit defender, int bonusDamage)
        {
            attacker.GetComponent<UnitAnimator>().PlayAttack(defender);
            Vector2Int cell = _board.WorldToCell(defender.transform.position);
            int terrainDefense = _board.GetDefenseBonus(cell);
            int damage = CombatCalculator.CalculateDamage(attacker.AttackPower, defender.Defense, terrainDefense);
            defender.TakeDamage(damage + bonusDamage);
        }

        public void PushAway(Unit user, Unit target)
        {
            if (!target.IsAlive) return;
            Vector2Int origin = _board.WorldToCell(user.transform.position);
            Vector2Int cell = _board.WorldToCell(target.transform.position);
            Vector2Int delta = cell - origin;
            Vector2Int direction = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y)
                ? new Vector2Int(System.Math.Sign(delta.x), 0)
                : new Vector2Int(0, System.Math.Sign(delta.y));
            Vector2Int destination = cell + direction;
            if (!_board.IsInside(destination)) return;
            if (!_board.Board.GetTerrain(destination).IsWalkable) return;
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
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team != Team.Enemy || !unit.IsAlive)
                {
                    continue;
                }

                if (!GameManager.Instance.IsPlaying) return;
                if (GetAttackTargets(unit).Count == 0)
                {
                    Unit target = _enemyAi.ChooseNearestAlly(unit, _spawner.Units);
                    if (target == null) return;
                    Vector2Int goalCell = _board.WorldToCell(target.transform.position);
                    await _enemyAi.TakeTurnAsync(unit, goalCell, token);
                }

                if (!GameManager.Instance.IsPlaying) return;

                List<Unit> targets = GetAttackTargets(unit);
                if (targets.Count > 0)
                {
                    Attack(unit, _enemyAi.ChooseTarget(targets));
                }

                if (!GameManager.Instance.IsPlaying)
                {
                    return;
                }
            }

            BeginPlayerPhase();
        }

        private void BeginPlayerPhase()
        {
            foreach (Unit unit in _spawner.Units)
            {
                unit.HasMoved = false;
            }

            Phase = BattlePhase.Player;
        }
    }
}
