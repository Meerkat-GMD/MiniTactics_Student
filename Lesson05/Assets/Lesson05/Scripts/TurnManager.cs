using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiniTactics.Lesson05
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
            return false;
        }

        public void BeginPlayerPhase()
        {
        }

        public bool CanPlayerAct(Unit unit)
        {
            return unit != null && unit.CanMove;
        }

        public void NotifyPlayerActionCompleted()
        {
        }

        public void RequestEndPlayerPhase()
        {
            // 학습 과제: Enter와 전체 행동 완료를 같은 턴 전환으로 연결한다.
        }

        public UniTask RunEnemyPhaseAsync(CancellationToken cancellationToken)
        {
            // 학습 과제: 진입 시 활성 적의 행동 표시를 초기화하고 순차 행동을 연결한다.
            return UniTask.CompletedTask;
        }

        public void MarkFinished(BattleResult result)
        {
        }
    }
}
