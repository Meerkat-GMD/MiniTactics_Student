using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class TurnManager : MonoBehaviour
    {
        [SerializeField] private List<Unit> _units = new List<Unit>();
        [SerializeField] private List<EnemyAI> _enemyAis = new List<EnemyAI>();
        [SerializeField] private Vector2Int _enemyGoalCell;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private BoardView _board;
        [SerializeField] private List<ObjectiveZone> _objectives = new List<ObjectiveZone>();

        private CommandHistory _commandHistory;
        private BattleResult _result = BattleResult.Playing;
        private bool _isEnemyPhaseRunning;

        public BattlePhase Phase { get; private set; } = BattlePhase.Player;
        public BattleResult Result => _gameManager != null ? _gameManager.Result : _result;
        public event Action<BattlePhase> PhaseChanged;
        public event Action<Unit> EnemyTurnStarted;

        private void Start()
        {
            BeginPlayerPhase();
        }

        public void Bind(
            IReadOnlyList<Unit> units,
            IReadOnlyList<EnemyAI> enemyAis,
            Vector2Int enemyGoalCell,
            GameManager gameManager = null,
            CommandHistory commandHistory = null)
        {
            _units = units == null ? new List<Unit>() : new List<Unit>(units);
            _enemyAis = enemyAis == null ? new List<EnemyAI>() : new List<EnemyAI>(enemyAis);
            _enemyGoalCell = enemyGoalCell;
            _gameManager = gameManager;
            _commandHistory = commandHistory;
        }

        public void BindCommandHistory(CommandHistory commandHistory)
        {
            _commandHistory = commandHistory;
        }

        public void BindObjectives(BoardView board, IReadOnlyList<ObjectiveZone> objectives)
        {
            _board = board;
            _objectives = objectives == null ? new List<ObjectiveZone>() : new List<ObjectiveZone>(objectives);
        }

        public bool CheckGoal(Unit unit)
        {
            if (!IsGamePlaying() || unit == null || _board == null) return false;
            Vector2Int cell = _board.WorldToCell(unit.transform.position);
            foreach (ObjectiveZone objective in _objectives)
            {
                if (objective == null || objective.Cell != cell) continue;
                BattleResult result = objective.GetResultFor(unit);
                if (result == BattleResult.Playing) continue;
                MarkFinished(result);
                return true;
            }
            return false;
        }

        public void BeginPlayerPhase()
        {
            if (Phase == BattlePhase.Finished)
            {
                return;
            }

            if (!IsGamePlaying())
            {
                FinishFromGameManager();
                return;
            }

            foreach (Unit unit in _units)
            {
                if (IsActivePlayerTurnUnit(unit))
                {
                    unit.SetMoved(false);
                }
            }

            _commandHistory?.Clear();
            SetPhase(BattlePhase.Player);
        }

        public bool CanPlayerAct(Unit unit)
        {
            return Phase == BattlePhase.Player
                && IsGamePlaying()
                && unit != null
                && unit.isActiveAndEnabled
                && unit.IsAlive
                && unit.Team == Team.Player
                && unit.IsTurnUnit
                && !unit.HasMoved;
        }

        public void NotifyPlayerActionCompleted()
        {
            if (Phase != BattlePhase.Player || !AreAllActivePlayerTurnUnitsMoved())
            {
                return;
            }

            RequestEndPlayerPhase();
        }

        public void RequestEndPlayerPhase()
        {
            if (Phase != BattlePhase.Player)
            {
                return;
            }

            if (!TryEnterEnemyPhase())
            {
                return;
            }

            if (Application.isPlaying)
            {
                CancellationToken lifetimeToken = destroyCancellationToken;
                RunEnemyPhaseForLifetimeAsync(lifetimeToken).Forget();
            }
        }

        public async UniTask RunEnemyPhaseAsync(CancellationToken cancellationToken)
        {
            if (!TryEnterEnemyPhase() || _isEnemyPhaseRunning)
            {
                return;
            }

            _isEnemyPhaseRunning = true;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                int enemyAiIndex = 0;
                foreach (Unit unit in _units)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!IsGamePlaying())
                    {
                        FinishFromGameManager();
                        return;
                    }

                    if (unit == null || unit.Team != Team.Enemy)
                    {
                        continue;
                    }

                    EnemyAI enemyAi = enemyAiIndex < _enemyAis.Count
                        ? _enemyAis[enemyAiIndex]
                        : null;
                    enemyAiIndex++;

                    if (!IsActiveEnemyTurnUnit(unit) || enemyAi == null)
                    {
                        continue;
                    }

                    EnemyTurnStarted?.Invoke(unit);
                    // 5차시 이동을 보존한다. 전투 본문 완성 후 전투 행동 API를 연결한다.
                    await enemyAi.TakeTurnAsync(unit, _enemyGoalCell, cancellationToken);

                    if (!IsGamePlaying())
                    {
                        FinishFromGameManager();
                        return;
                    }

                    if (CheckGoal(unit)) return;
                }

                if (IsGamePlaying())
                {
                    BeginPlayerPhase();
                }
                else
                {
                    FinishFromGameManager();
                }
            }
            finally
            {
                _isEnemyPhaseRunning = false;
            }
        }

        public void MarkFinished(BattleResult result)
        {
            _result = result;
            _gameManager?.TryFinish(result);
            SetPhase(BattlePhase.Finished);
        }

        private async UniTask RunEnemyPhaseForLifetimeAsync(CancellationToken lifetimeToken)
        {
            try
            {
                await RunEnemyPhaseAsync(lifetimeToken);
            }
            catch (OperationCanceledException) when (lifetimeToken.IsCancellationRequested)
            {
            }
        }

        private bool TryEnterEnemyPhase()
        {
            if (Phase == BattlePhase.Finished)
            {
                return false;
            }

            if (!IsGamePlaying())
            {
                FinishFromGameManager();
                return false;
            }

            if (Phase == BattlePhase.Player)
            {
                _commandHistory?.Clear();
                SetPhase(BattlePhase.Enemy);
            }

            return Phase == BattlePhase.Enemy;
        }

        private bool AreAllActivePlayerTurnUnitsMoved()
        {
            foreach (Unit unit in _units)
            {
                if (IsActivePlayerTurnUnit(unit) && !unit.HasMoved)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsGamePlaying()
        {
            return (_gameManager == null && _result == BattleResult.Playing) ||
                (_gameManager != null && _gameManager.IsPlaying);
        }

        private static bool IsActivePlayerTurnUnit(Unit unit)
        {
            return unit != null && unit.isActiveAndEnabled && unit.IsAlive &&
                unit.Team == Team.Player && unit.IsTurnUnit;
        }

        private static bool IsActiveEnemyTurnUnit(Unit unit)
        {
            return unit != null && unit.isActiveAndEnabled && unit.IsAlive &&
                unit.Team == Team.Enemy && unit.IsTurnUnit;
        }

        private void FinishFromGameManager()
        {
            if (_gameManager != null)
            {
                _result = _gameManager.Result;
            }

            SetPhase(BattlePhase.Finished);
        }

        private void SetPhase(BattlePhase phase)
        {
            if (Phase == phase)
            {
                return;
            }

            Phase = phase;
            PhaseChanged?.Invoke(phase);
        }
    }
}
