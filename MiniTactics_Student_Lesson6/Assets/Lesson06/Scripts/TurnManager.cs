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
                if (unit.Team == Team.Player && !unit.HasMoved)
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

        public void EndPlayerPhase()
        {
            Phase = BattlePhase.Enemy;
            RunEnemyPhaseAsync(destroyCancellationToken).Forget();
        }

        private async UniTask RunEnemyPhaseAsync(CancellationToken token)
        {
            Vector2Int goalCell = _board.WorldToCell(_playerBase.position);
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team != Team.Enemy)
                {
                    continue;
                }

                await _enemyAi.TakeTurnAsync(unit, goalCell, token);
                if (CheckBase(unit))
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
