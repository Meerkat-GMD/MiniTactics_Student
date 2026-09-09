using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class EnemyAITests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private GameObject _gridObject;
        private GameObject _boardObject;
        private Tile _testTile;
        private TextAsset _mapText;
        private BoardView _board;
        private EnemyAI _enemyAI;

        [SetUp]
        public void SetUp()
        {
            _gridObject = Track(new GameObject("Grid", typeof(Grid)));
            _gridObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);

            GameObject tilemapObject = Track(
                new GameObject("Ground Tilemap", typeof(Tilemap), typeof(TilemapRenderer)));
            tilemapObject.transform.SetParent(_gridObject.transform, false);

            _boardObject = Track(new GameObject("Board"));
            _board = _boardObject.AddComponent<BoardView>();
            _testTile = ScriptableObject.CreateInstance<Tile>();

            GameObject aiObject = Track(new GameObject("Enemy AI"));
            _enemyAI = aiObject.AddComponent<EnemyAI>();
            _enemyAI.TurnDelayMilliseconds = 0;

            SerializedObject serializedAI = new SerializedObject(_enemyAI);
            serializedAI.FindProperty("_board").objectReferenceValue = _board;
            serializedAI.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject serializedBoard = new SerializedObject(_board);
            serializedBoard.FindProperty("_tilemap").objectReferenceValue =
                tilemapObject.GetComponent<Tilemap>();
            serializedBoard.FindProperty("_plainTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_forestTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_mountainTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_riverTile").objectReferenceValue = _testTile;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject gameObject in _objects)
            {
                Object.DestroyImmediate(gameObject);
            }

            Object.DestroyImmediate(_testTile);
            Object.DestroyImmediate(_mapText);
        }

        [Test]
        public void ChooseDestination_PrefersSmallerManhattanDistance()
        {
            ConfigureBoard("PPPPP");
            Unit enemy = CreateEnemy(new Vector2Int(0, 0), moveBudget: 3);

            Vector2Int destination = _enemyAI.ChooseDestination(enemy, new Vector2Int(4, 0));

            Assert.That(destination, Is.EqualTo(new Vector2Int(3, 0)));
        }

        [Test]
        public void ChooseDestination_BreaksEqualDistanceByLowerMovementCost()
        {
            ConfigureBoard("PPRPRPP\nPPPRPPP\nPPFRPPP\nPPPPPPP");
            Unit enemy = CreateEnemy(new Vector2Int(3, 0), moveBudget: 4);

            Vector2Int destination = _enemyAI.ChooseDestination(enemy, new Vector2Int(3, 3));

            Assert.That(destination, Is.EqualTo(new Vector2Int(4, 2)));
        }

        [Test]
        public void ChooseDestination_BreaksEqualDistanceAndCostByLowerY()
        {
            ConfigureBoard("PPPP\nPPPP\nPPPR\nPPPR");
            Unit enemy = CreateEnemy(new Vector2Int(0, 0), moveBudget: 3);

            Vector2Int destination = _enemyAI.ChooseDestination(enemy, new Vector2Int(3, 3));

            Assert.That(destination, Is.EqualTo(new Vector2Int(2, 1)));
        }

        [Test]
        public void ChooseDestination_BreaksEqualDistanceCostAndYByLowerX()
        {
            ConfigureBoard("PPPPP\nPPRPP\nPPRPP\nPPPPP");
            Unit enemy = CreateEnemy(new Vector2Int(2, 0), moveBudget: 3);

            Vector2Int destination = _enemyAI.ChooseDestination(enemy, new Vector2Int(2, 3));

            Assert.That(destination, Is.EqualTo(new Vector2Int(1, 2)));
        }

        [Test]
        public void ChooseDestination_ReturnsStartWhenNoReachableCellImprovesDistance()
        {
            ConfigureBoard("PPP");
            Unit enemy = CreateEnemy(new Vector2Int(1, 0), moveBudget: 1);

            Vector2Int destination = _enemyAI.ChooseDestination(enemy, new Vector2Int(1, 0));

            Assert.That(destination, Is.EqualTo(new Vector2Int(1, 0)));
        }

        [Test]
        public async Task TakeTurnAsync_MovesAndMarksEnemyMovedWithoutConfiguredDelay()
        {
            ConfigureBoard("PPP");
            Unit enemy = CreateEnemy(new Vector2Int(0, 0), moveBudget: 1);

            await _enemyAI.TakeTurnAsync(
                enemy,
                new Vector2Int(2, 0),
                CancellationToken.None);

            Assert.That(_board.WorldToCell(enemy.transform.position), Is.EqualTo(new Vector2Int(1, 0)));
            Assert.That(enemy.HasMoved, Is.True);
        }

        private void ConfigureBoard(string map)
        {
            _mapText = new TextAsset(map);
            SerializedObject serializedBoard = new SerializedObject(_board);
            serializedBoard.FindProperty("_mapText").objectReferenceValue = _mapText;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();
            _board.LoadBoard();
        }

        private Unit CreateEnemy(Vector2Int start, int moveBudget)
        {
            GameObject enemyObject = Track(new GameObject("Enemy"));
            Unit enemy = enemyObject.AddComponent<Unit>();
            SerializedObject serializedEnemy = new SerializedObject(enemy);
            serializedEnemy.FindProperty("_team").enumValueIndex = (int)Team.Enemy;
            serializedEnemy.FindProperty("_moveBudget").intValue = moveBudget;
            serializedEnemy.ApplyModifiedPropertiesWithoutUndo();
            enemy.transform.position = _board.CellToWorld(start);
            return enemy;
        }

        private GameObject Track(GameObject gameObject)
        {
            _objects.Add(gameObject);
            return gameObject;
        }
    }
}
