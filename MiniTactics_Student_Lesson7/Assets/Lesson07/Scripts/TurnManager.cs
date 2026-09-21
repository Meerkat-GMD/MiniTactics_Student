using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson07
{
    public sealed class TurnManager : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _spawner;
        [SerializeField] private BoardView _board;
        [SerializeField] private EnemyAI _enemyAi;
        [SerializeField] private Transform _playerBase;
        [SerializeField] private Transform _enemyBase;

        public BattlePhase Phase { get; private set; } = BattlePhase.Player;

        public bool CanAct(Unit unit)
        {
            return GameManager.Instance.IsPlaying
                && Phase == BattlePhase.Player
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

        public bool CheckBase(Unit unit)
        {
            Transform targetBase = unit.Team == Team.Player ? _enemyBase : _playerBase;
            if (_board.WorldToCell(unit.transform.position) != _board.WorldToCell(targetBase.position))
            {
                return false;
            }

            BattleResult result = unit.Team == Team.Player ? BattleResult.PlayerWon : BattleResult.PlayerLost;
            GameManager.Instance.Finish(result);
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
                if (unit.IsAlive && unit.Team != attacker.Team && distance == 1)
                {
                    targets.Add(unit);
                }
            }

            return targets;
        }

        public void Attack(Unit attacker, Unit defender)
        {
            Vector2Int cell = _board.WorldToCell(defender.transform.position);
            int terrainDefense = _board.GetDefenseBonus(cell);
            int damage = CombatCalculator.CalculateDamage(attacker.AttackPower, defender.Defense, terrainDefense);
            defender.TakeDamage(damage);
        }

        public void EndPlayerPhase()
        {
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team == Team.Player && unit.IsAlive && CheckBase(unit))
                {
                    return;
                }
            }

            Phase = BattlePhase.Enemy;
            RunEnemyPhaseAsync(destroyCancellationToken).Forget();
        }

        private async UniTask RunEnemyPhaseAsync(CancellationToken token)
        {
            Vector2Int goalCell = _board.WorldToCell(_playerBase.position);
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team != Team.Enemy || !unit.IsAlive)
                {
                    continue;
                }

                await _enemyAi.TakeTurnAsync(unit, goalCell, token);

                List<Unit> targets = GetAttackTargets(unit);
                if (targets.Count > 0)
                {
                    Attack(unit, _enemyAi.ChooseTarget(targets));
                }

                if (CheckBase(unit) || !GameManager.Instance.IsPlaying)
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
