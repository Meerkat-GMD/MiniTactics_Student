using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class TurnManagerTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private Tile _testTile;

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject gameObject in _objects)
            {
                Object.DestroyImmediate(gameObject);
            }

            Object.DestroyImmediate(_testTile);
        }

        [Test]
        public void BeginPlayerPhase_ResetsOnlyActivePlayerTurnUnitsAndClearsHistory()
        {
            TurnManager manager = CreateManager();
            Unit activePlayer = CreateUnit("Active Player", Team.Player, canMove: true, moved: true);
            Unit fixedPlayer = CreateUnit("Fixed Player", Team.Player, canMove: false, moved: true);
            Unit inactivePlayer = CreateUnit("Inactive Player", Team.Player, canMove: true, moved: true);
            inactivePlayer.gameObject.SetActive(false);
            Unit enemy = CreateUnit("Enemy", Team.Enemy, canMove: true, moved: true);
            CommandHistory history = new CommandHistory();
            history.Execute(new TestCommand());
            manager.Bind(
                new[] { activePlayer, fixedPlayer, inactivePlayer, enemy },
                Array.Empty<EnemyAI>(),
                Vector2Int.zero,
                commandHistory: history);

            manager.BeginPlayerPhase();

            Assert.That(activePlayer.HasMoved, Is.False);
            Assert.That(fixedPlayer.HasMoved, Is.True);
            Assert.That(inactivePlayer.HasMoved, Is.True);
            Assert.That(enemy.HasMoved, Is.True);
            Assert.That(history.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanPlayerAct_RejectsEnemyMovedAndFixedUnits()
        {
            TurnManager manager = CreateManager();
            Unit readyPlayer = CreateUnit("Ready Player", Team.Player, canMove: true, moved: false);
            Unit movedPlayer = CreateUnit("Moved Player", Team.Player, canMove: true, moved: true);
            Unit fixedPlayer = CreateUnit("Fixed Player", Team.Player, canMove: false, moved: false);
            Unit enemy = CreateUnit("Enemy", Team.Enemy, canMove: true, moved: false);
            manager.Bind(new[] { readyPlayer, movedPlayer, fixedPlayer, enemy }, Array.Empty<EnemyAI>(), Vector2Int.zero);
            manager.BeginPlayerPhase();
            movedPlayer.SetMoved(true);

            Assert.That(manager.CanPlayerAct(readyPlayer), Is.True);
            Assert.That(manager.CanPlayerAct(movedPlayer), Is.False);
            Assert.That(manager.CanPlayerAct(fixedPlayer), Is.False);
            Assert.That(manager.CanPlayerAct(enemy), Is.False);
            Assert.That(manager.CanPlayerAct(null), Is.False);
        }

        [Test]
        public void NotifyPlayerActionCompleted_StartsEnemyPhaseWhenEveryActivePlayerTurnUnitMoved()
        {
            TurnManager manager = CreateManager();
            Unit firstPlayer = CreateUnit("First Player", Team.Player, canMove: true, moved: false);
            Unit secondPlayer = CreateUnit("Second Player", Team.Player, canMove: true, moved: false);
            Unit inactivePlayer = CreateUnit("Inactive Player", Team.Player, canMove: true, moved: false);
            inactivePlayer.gameObject.SetActive(false);
            CommandHistory history = new CommandHistory();
            manager.Bind(
                new[] { firstPlayer, secondPlayer, inactivePlayer },
                Array.Empty<EnemyAI>(),
                Vector2Int.zero,
                commandHistory: history);
            manager.BeginPlayerPhase();
            history.Execute(new TestCommand());
            firstPlayer.SetMoved(true);

            manager.NotifyPlayerActionCompleted();

            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Player));
            Assert.That(history.Count, Is.EqualTo(1));
            secondPlayer.SetMoved(true);
            manager.NotifyPlayerActionCompleted();

            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Enemy));
            Assert.That(history.Count, Is.EqualTo(0));
        }

        [Test]
        public void RequestEndPlayerPhase_ClearsHistoryBeforePublishingEnemyPhase()
        {
            TurnManager manager = CreateManager();
            CommandHistory history = new CommandHistory();
            manager.Bind(Array.Empty<Unit>(), Array.Empty<EnemyAI>(), Vector2Int.zero, commandHistory: history);
            history.Execute(new TestCommand());
            int historyCountWhenEnemyPhasePublished = -1;
            manager.PhaseChanged += phase =>
            {
                if (phase == BattlePhase.Enemy)
                {
                    historyCountWhenEnemyPhasePublished = history.Count;
                }
            };

            manager.RequestEndPlayerPhase();

            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Enemy));
            Assert.That(history.Count, Is.EqualTo(0));
            Assert.That(historyCountWhenEnemyPhasePublished, Is.EqualTo(0));
        }

        [Test]
        public async Task RunEnemyPhaseAsync_UsesSerializedEnemyOrderAndReturnsToPlayer()
        {
            BoardView board = CreateBoard("PPPPPP");
            TurnManager manager = CreateManager();
            Unit firstEnemy = CreateUnit("First Enemy", Team.Enemy, canMove: true, moved: false);
            Unit secondEnemy = CreateUnit("Second Enemy", Team.Enemy, canMove: true, moved: false);
            firstEnemy.transform.position = board.CellToWorld(new Vector2Int(0, 0));
            secondEnemy.transform.position = board.CellToWorld(new Vector2Int(3, 0));
            EnemyAI firstAi = CreateEnemyAi(board);
            EnemyAI secondAi = CreateEnemyAi(board);
            var actionOrder = new List<Unit>();
            manager.EnemyTurnStarted += actionOrder.Add;
            manager.Bind(
                new[] { firstEnemy, secondEnemy },
                new[] { firstAi, secondAi },
                new Vector2Int(5, 0));
            manager.RequestEndPlayerPhase();

            await manager.RunEnemyPhaseAsync(CancellationToken.None);

            Assert.That(actionOrder, Is.EqualTo(new[] { firstEnemy, secondEnemy }));
            Assert.That(firstEnemy.HasMoved, Is.True);
            Assert.That(secondEnemy.HasMoved, Is.True);
            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Player));
        }

        [Test]
        public async Task RunEnemyPhaseAsync_EntersEnemyOnceAndWaitsForTheFirstEnemyBeforeTheSecond()
        {
            BoardView board = CreateBoard("PPPPPP");
            TurnManager manager = CreateManager();
            Unit firstEnemy = CreateUnit("First Enemy", Team.Enemy, canMove: true, moved: false);
            Unit secondEnemy = CreateUnit("Second Enemy", Team.Enemy, canMove: true, moved: false);
            firstEnemy.transform.position = board.CellToWorld(new Vector2Int(0, 0));
            secondEnemy.transform.position = board.CellToWorld(new Vector2Int(3, 0));
            EnemyAI firstAi = CreateEnemyAi(board);
            EnemyAI secondAi = CreateEnemyAi(board);
            firstAi.TurnDelayMilliseconds = 10000;
            var actionOrder = new List<Unit>();
            manager.EnemyTurnStarted += actionOrder.Add;
            manager.Bind(
                new[] { firstEnemy, secondEnemy },
                new[] { firstAi, secondAi },
                new Vector2Int(5, 0));
            using var cancellation = new CancellationTokenSource();

            UniTask firstRun = manager.RunEnemyPhaseAsync(cancellation.Token);
            UniTask duplicateRun = manager.RunEnemyPhaseAsync(cancellation.Token);
            await UniTask.Yield();
            bool enteredEnemyPhase = manager.Phase == BattlePhase.Enemy;
            bool onlyFirstEnemyStarted = actionOrder.Count == 1 && actionOrder[0] == firstEnemy;
            cancellation.Cancel();
            await IgnoreCancellation(firstRun);
            await IgnoreCancellation(duplicateRun);

            Assert.That(enteredEnemyPhase, Is.True);
            Assert.That(onlyFirstEnemyStarted, Is.True);
        }

        [Test]
        public async Task RunEnemyPhaseAsync_CancellationReleasesTheInFlightGuardForANextRun()
        {
            BoardView board = CreateBoard("PPP");
            TurnManager manager = CreateManager();
            Unit enemy = CreateUnit("Enemy", Team.Enemy, canMove: true, moved: false);
            enemy.transform.position = board.CellToWorld(Vector2Int.zero);
            EnemyAI enemyAi = CreateEnemyAi(board);
            enemyAi.TurnDelayMilliseconds = 10000;
            manager.Bind(new[] { enemy }, new[] { enemyAi }, new Vector2Int(2, 0));
            using var cancellation = new CancellationTokenSource();

            UniTask cancelledRun = manager.RunEnemyPhaseAsync(cancellation.Token);
            await UniTask.Yield();
            cancellation.Cancel();
            await IgnoreCancellation(cancelledRun);
            enemyAi.TurnDelayMilliseconds = 0;

            await manager.RunEnemyPhaseAsync(CancellationToken.None);

            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Player));
        }

        [Test]
        public void RunEnemyPhaseAsync_PropagatesAnUnrelatedCancellation()
        {
            BoardView board = CreateBoard("PPP");
            TurnManager manager = CreateManager();
            Unit enemy = CreateUnit("Enemy", Team.Enemy, canMove: true, moved: false);
            enemy.transform.position = board.CellToWorld(Vector2Int.zero);
            EnemyAI enemyAi = CreateEnemyAi(board);
            enemyAi.TurnDelayMilliseconds = 1;
            manager.Bind(new[] { enemy }, new[] { enemyAi }, new Vector2Int(2, 0));
            manager.RequestEndPlayerPhase();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            Assert.That(
                async () => await manager.RunEnemyPhaseAsync(cancellation.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public async Task RunEnemyPhaseAsync_StaysFinishedWhenGameManagerHasEnded()
        {
            TurnManager manager = CreateManager();
            GameObject gameManagerObject = Track(new GameObject("Game Manager"));
            GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
            gameManager.TryFinish(BattleResult.PlayerWon);
            manager.Bind(Array.Empty<Unit>(), Array.Empty<EnemyAI>(), Vector2Int.zero, gameManager);
            manager.RequestEndPlayerPhase();

            await manager.RunEnemyPhaseAsync(CancellationToken.None);

            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Finished));
        }

        [Test]
        public void MarkFinished_ForcesFinishedAndDoesNotRestartThePhaseLoop()
        {
            TurnManager manager = CreateManager();
            Unit player = CreateUnit("Player", Team.Player, canMove: true, moved: true);
            manager.Bind(new[] { player }, Array.Empty<EnemyAI>(), Vector2Int.zero);
            manager.BeginPlayerPhase();

            manager.MarkFinished(BattleResult.PlayerWon);
            manager.NotifyPlayerActionCompleted();
            manager.RequestEndPlayerPhase();

            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Finished));
            Assert.That(manager.Result, Is.EqualTo(BattleResult.PlayerWon));
        }

        [Test]
        public async Task EnemyPhase_EveryRoundResetsOnlyActiveEnemiesBeforePublishingOrActing()
        {
            BoardView board = CreateBoard("PPPPPP\nPPPPPP");
            TurnManager manager = CreateManager();
            Unit first = CreateUnit("First", Team.Enemy, true, true);
            Unit second = CreateUnit("Second", Team.Enemy, true, true);
            Unit fixedEnemy = CreateUnit("Fixed", Team.Enemy, false, true);
            Unit inactiveEnemy = CreateUnit("Inactive", Team.Enemy, true, true);
            inactiveEnemy.gameObject.SetActive(false);
            first.MoveTo(board, Vector2Int.zero);
            second.MoveTo(board, new Vector2Int(3, 0));
            fixedEnemy.MoveTo(board, new Vector2Int(5, 1));
            manager.Bind(new[] { first, second, fixedEnemy, inactiveEnemy },
                new[] { CreateEnemyAi(board), CreateEnemyAi(board) }, new Vector2Int(5, 0));
            int entries = 0;
            int actions = 0;
            manager.PhaseChanged += phase =>
            {
                if (phase != BattlePhase.Enemy) return;
                entries++;
                Assert.That(first.HasMoved, Is.False, "Reset before publishing Enemy.");
                Assert.That(second.HasMoved, Is.False);
                Assert.That(fixedEnemy.HasMoved, Is.True);
                Assert.That(inactiveEnemy.HasMoved, Is.True);
            };
            manager.EnemyTurnStarted += unit =>
            {
                Assert.That(unit.HasMoved, Is.False, "A new enemy action starts unspent.");
                if (unit == second) Assert.That(first.HasMoved, Is.True);
                actions++;
            };

            for (int round = 0; round < 2; round++)
            {
                manager.RequestEndPlayerPhase();
                await manager.RunEnemyPhaseAsync(CancellationToken.None);
                Assert.That(first.HasMoved, Is.True);
                Assert.That(second.HasMoved, Is.True);
                Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Player));
            }
            Assert.That(entries, Is.EqualTo(2));
            Assert.That(actions, Is.EqualTo(4));
        }


        private TurnManager CreateManager()
        {
            return Track(new GameObject("Turn Manager")).AddComponent<TurnManager>();
        }

        [Test]
        public async Task EnemyEnteringPlayerGoal_FinishesBeforeSecondEnemyActs()
        {
            BoardView board = CreateBoard("PPPPP\nPPPPP");
            TurnManager manager = CreateManager();
            GameManager game = Track(new GameObject("Game")).AddComponent<GameManager>();
            Unit first = CreateUnit("First", Team.Enemy, true, false);
            Unit second = CreateUnit("Second", Team.Enemy, true, false);
            first.MoveTo(board, new Vector2Int(2, 0));
            second.MoveTo(board, new Vector2Int(4, 1));
            ObjectiveZone goal = Track(new GameObject("Player Goal")).AddComponent<ObjectiveZone>();
            var objective = new SerializedObject(goal);
            objective.FindProperty("_owner").enumValueIndex = (int)Team.Player;
            objective.FindProperty("_cell").vector2IntValue = Vector2Int.zero;
            objective.ApplyModifiedPropertiesWithoutUndo();
            manager.Bind(new[] { first, second }, new[] { CreateEnemyAi(board), CreateEnemyAi(board) }, Vector2Int.zero, game);
            var serialized = new SerializedObject(manager);
            if (serialized.FindProperty("_board") != null)
            {
                serialized.FindProperty("_board").objectReferenceValue = board;
                var goals = serialized.FindProperty("_objectives");
                goals.arraySize = 1;
                goals.GetArrayElementAtIndex(0).objectReferenceValue = goal;
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
            await manager.RunEnemyPhaseAsync(CancellationToken.None);
            Assert.That(first.transform.position, Is.EqualTo(board.CellToWorld(Vector2Int.zero)));
            Assert.That(game.Result, Is.EqualTo(BattleResult.PlayerLost));
            Assert.That(manager.Phase, Is.EqualTo(BattlePhase.Finished));
            Assert.That(second.HasMoved, Is.False);
            Assert.That(second.transform.position, Is.EqualTo(board.CellToWorld(new Vector2Int(4, 1))));
        }

        private BoardView CreateBoard(string map)
        {
            GameObject gridObject = Track(new GameObject("Grid", typeof(Grid)));
            gridObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
            GameObject tilemapObject = Track(
                new GameObject("Ground Tilemap", typeof(Tilemap), typeof(TilemapRenderer)));
            tilemapObject.transform.SetParent(gridObject.transform, false);
            BoardView board = Track(new GameObject("Board")).AddComponent<BoardView>();
            _testTile = ScriptableObject.CreateInstance<Tile>();
            TextAsset mapText = new TextAsset(map);

            SerializedObject serializedBoard = new SerializedObject(board);
            serializedBoard.FindProperty("_tilemap").objectReferenceValue =
                tilemapObject.GetComponent<Tilemap>();
            serializedBoard.FindProperty("_plainTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_forestTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_mountainTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_riverTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_mapText").objectReferenceValue = mapText;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();
            board.LoadBoard();
            return board;
        }

        private EnemyAI CreateEnemyAi(BoardView board)
        {
            EnemyAI enemyAi = Track(new GameObject("Enemy AI")).AddComponent<EnemyAI>();
            enemyAi.TurnDelayMilliseconds = 0;
            SerializedObject serializedAi = new SerializedObject(enemyAi);
            serializedAi.FindProperty("_board").objectReferenceValue = board;
            serializedAi.ApplyModifiedPropertiesWithoutUndo();
            return enemyAi;
        }

        private Unit CreateUnit(string name, Team team, bool canMove, bool moved)
        {
            GameObject unitObject = Track(new GameObject(name));
            unitObject.SetActive(false);
            Unit unit = unitObject.AddComponent<Unit>();
            SerializedObject serializedUnit = new SerializedObject(unit);
            serializedUnit.FindProperty("_team").enumValueIndex = (int)team;
            serializedUnit.FindProperty("_canMove").boolValue = canMove;
            serializedUnit.FindProperty("_hasMoved").boolValue = moved;
            serializedUnit.ApplyModifiedPropertiesWithoutUndo();
            unitObject.SetActive(true);
            return unit;
        }

        private GameObject Track(GameObject gameObject)
        {
            _objects.Add(gameObject);
            return gameObject;
        }

        private static async Task IgnoreCancellation(UniTask task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private sealed class TestCommand : IUndoableCommand
        {
            public void Execute() { }
            public void Undo() { }
        }
    }
}
