using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class BattleControllerTests
    {
        private GameObject _boardObject;
        private GameObject _gridObject;
        private GameObject _overlayRootObject;
        private GameObject _controllerObject;
        private GameObject _selectedObject;
        private GameObject _blockerObject;
        private Tile _testTile;
        private BoardView _board;
        private BattleController _controller;
        private Unit _selectedUnit;
        private GameManager _game;
        private TurnManager _turns;

        [SetUp]
        public void SetUp()
        {
            _boardObject = new GameObject("Board");
            _gridObject = new GameObject("Grid", typeof(Grid));
            _gridObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
            GameObject tilemapObject = new GameObject("Ground Tilemap", typeof(Tilemap));
            tilemapObject.transform.SetParent(_gridObject.transform, false);
            Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();

            _testTile = ScriptableObject.CreateInstance<Tile>();
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), _testTile);
                }
            }

            _board = _boardObject.AddComponent<BoardView>();
            SerializedObject serializedBoard = new SerializedObject(_board);
            serializedBoard.FindProperty("_tilemap").objectReferenceValue = tilemap;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();

            _selectedObject = new GameObject("Selected Unit");
            _selectedObject.transform.position = new Vector3(5f, 4f, 0f);
            _selectedUnit = _selectedObject.AddComponent<Unit>();

            _blockerObject = new GameObject("Fixed Unit");
            _blockerObject.transform.position = new Vector3(6f, 4f, 0f);
            _blockerObject.AddComponent<Unit>();

            _overlayRootObject = new GameObject("MovementOverlayRoot");
            _controllerObject = new GameObject("BattleController");
            _controller = _controllerObject.AddComponent<BattleController>();
            _game = _controllerObject.AddComponent<GameManager>();
            _turns = _controllerObject.AddComponent<TurnManager>();
            _turns.Bind(new[] { _selectedUnit, _blockerObject.GetComponent<Unit>() },
                System.Array.Empty<EnemyAI>(), Vector2Int.zero, _game);
            SerializedObject serializedController = new SerializedObject(_controller);
            serializedController.FindProperty("_board").objectReferenceValue = _board;
            serializedController.FindProperty("_overlayRoot").objectReferenceValue = _overlayRootObject.transform;
            if (serializedController.FindProperty("_gameManager") != null)
                serializedController.FindProperty("_gameManager").objectReferenceValue = _game;
            if (serializedController.FindProperty("_turnManager") != null)
                serializedController.FindProperty("_turnManager").objectReferenceValue = _turns;
            serializedController.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_controllerObject);
            Object.DestroyImmediate(_overlayRootObject);
            Object.DestroyImmediate(_selectedObject);
            Object.DestroyImmediate(_blockerObject);
            Object.DestroyImmediate(_boardObject);
            Object.DestroyImmediate(_gridObject);
            Object.DestroyImmediate(_testTile);
        }

        [Test]
        public void MovementRange_DoesNotIncludeOccupiedCellButCanRouteAroundIt()
        {
            MovementRangeResult range = _board.GetMovementRange(_selectedUnit);

            Assert.That(range.TryGetCost(new Vector2Int(5, 4), out int startCost), Is.True);
            Assert.That(startCost, Is.Zero);
            Assert.That(range.TryGetCost(new Vector2Int(6, 4), out _), Is.False);
            Assert.That(range.TryGetCost(new Vector2Int(7, 4), out int detourCost), Is.True);
            Assert.That(detourCost, Is.EqualTo(4));
            Assert.That(_board.IsOccupiedByOther(_selectedUnit, new Vector2Int(6, 4)), Is.True);
        }

        [Test]
        public void TryMoveSelectedUnit_RejectsOccupiedCellAndSnapsToValidCellCenter()
        {
            _controller.Select(_selectedUnit);

            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(6, 4)), Is.False);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));

            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(5, 6)), Is.True);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 6f, 0f)));
            Assert.That(_controller.SelectedUnit, Is.Null);
        }

        [Test]
        public void OnUndoClicked_RestoresMultipleMovesInReverseOrder()
        {
            Unit second = _blockerObject.GetComponent<Unit>();
            // An unspent third player keeps both moves in the same undoable phase.
            Unit third = _overlayRootObject.AddComponent<Unit>();
            _turns.Bind(new[] { _selectedUnit, second, third }, System.Array.Empty<EnemyAI>(), Vector2Int.zero, _game);
            _controller.Select(_selectedUnit);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(5, 6)), Is.True);

            _controller.Select(second);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(7, 6)), Is.True);

            _controller.OnUndoClicked();
            Assert.That(second.transform.position, Is.EqualTo(new Vector3(6f, 4f, 0f)));
            Assert.That(second.HasMoved, Is.False);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 6f, 0f)));

            _controller.OnUndoClicked();
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));
            Assert.That(_selectedUnit.HasMoved, Is.False);

            _controller.OnUndoClicked();
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));
        }

        [TestCase(BattlePhase.Enemy)]
        [TestCase(BattlePhase.Finished)]
        public void SelectionAndDirectIdleClick_RejectNonPlayerPhase(BattlePhase phase)
        {
            if (phase == BattlePhase.Enemy) _turns.RequestEndPlayerPhase();
            else _turns.MarkFinished(BattleResult.PlayerWon);
            _controller.Select(_selectedUnit);
            Assert.That(_controller.SelectedUnit, Is.Null);
            new IdleState(_controller.StateMachine, _controller).HandleBoardClick(_selectedUnit, new Vector2Int(5, 4));
            Assert.That(_controller.SelectedUnit, Is.Null);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Selection_RejectsEnemyOrMovedPlayer(bool enemy)
        {
            if (enemy)
            {
                var unit = new SerializedObject(_selectedUnit);
                unit.FindProperty("_team").enumValueIndex = (int)Team.Enemy;
                unit.ApplyModifiedPropertiesWithoutUndo();
            }
            else _selectedUnit.SetMoved(true);
            _controller.Select(_selectedUnit);
            Assert.That(_controller.SelectedUnit, Is.Null);
            _controller.StateMachine.HandleBoardClick(_selectedUnit, new Vector2Int(5, 4));
            Assert.That(_controller.SelectedUnit, Is.Null);
        }

        [Test]
        public void SuccessfulMove_MarksMovedAndUndoRestoresAction()
        {
            _controller.Select(_selectedUnit);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(5, 6)), Is.True);
            Assert.That(_selectedUnit.HasMoved, Is.True);
            _controller.OnUndoClicked();
            Assert.That(_selectedUnit.HasMoved, Is.False);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5, 4, 0)));
        }

        [Test]
        public void LastPlayerMove_StartsEnemyPhaseAndClearsSharedHistory()
        {
            _blockerObject.GetComponent<Unit>().SetMoved(true);
            _controller.Select(_selectedUnit);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(5, 6)), Is.True);
            Assert.That(_turns.Phase, Is.EqualTo(BattlePhase.Enemy));
            _turns.BeginPlayerPhase();
            _controller.OnUndoClicked();
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5, 6, 0)));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void DirectSelectedStateClickAndUndo_RejectEndedMatchOrEnemyPhase(bool ended)
        {
            _controller.Select(_selectedUnit);
            _controller.TryMoveSelectedUnit(new Vector2Int(5, 6));
            Unit second = _blockerObject.GetComponent<Unit>();
            _controller.Select(second);
            if (ended) _game.TryFinish(BattleResult.PlayerWon);
            else _turns.RequestEndPlayerPhase();
            _controller.StateMachine.HandleBoardClick(null, new Vector2Int(7, 4));
            _controller.OnUndoClicked();
            _controller.StateMachine.HandleUndo();
            Assert.That(second.transform.position, Is.EqualTo(new Vector3(6, 4, 0)));
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5, 6, 0)));
            _controller.Select(second);
            Assert.That(_controller.SelectedUnit, Is.Null);
        }

        [Test]
        public void EnterEnemyGoal_FinishesBothManagersBeforeEnemyPhase()
        {
            ObjectiveZone goal = _overlayRootObject.AddComponent<ObjectiveZone>();
            var zone = new SerializedObject(goal);
            zone.FindProperty("_owner").enumValueIndex = (int)Team.Enemy;
            zone.FindProperty("_cell").vector2IntValue = new Vector2Int(5, 6);
            zone.ApplyModifiedPropertiesWithoutUndo();
            var turns = new SerializedObject(_turns);
            if (turns.FindProperty("_board") != null)
            {
                turns.FindProperty("_board").objectReferenceValue = _board;
                var goals = turns.FindProperty("_objectives");
                goals.arraySize = 1;
                goals.GetArrayElementAtIndex(0).objectReferenceValue = goal;
                turns.ApplyModifiedPropertiesWithoutUndo();
            }
            _blockerObject.GetComponent<Unit>().SetMoved(true);
            _controller.Select(_selectedUnit);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(5, 6)), Is.True);
            Assert.That(_game.Result, Is.EqualTo(BattleResult.PlayerWon));
            Assert.That(_turns.Phase, Is.EqualTo(BattlePhase.Finished));
            _controller.OnUndoClicked();
            Assert.That(_selectedUnit.HasMoved, Is.True);
        }

        [Test]
        public void RejectedMove_DoesNotCreateUndoHistory()
        {
            _controller.Select(_selectedUnit);

            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(6, 4)), Is.False);
            _controller.OnUndoClicked();

            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));
        }

        [Test]
        public void TerrainCostLimitsDestinationAndPathIsRemembered()
        {
            TextAsset mapText = new TextAsset("PFP");
            SerializedObject serializedBoard = new SerializedObject(_board);
            serializedBoard.FindProperty("_mapText").objectReferenceValue = mapText;
            serializedBoard.FindProperty("_plainTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_forestTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_mountainTile").objectReferenceValue = _testTile;
            serializedBoard.FindProperty("_riverTile").objectReferenceValue = _testTile;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();
            _board.LoadBoard();

            _selectedObject.transform.position = _board.CellToWorld(Vector2Int.zero);
            SerializedObject serializedUnit = new SerializedObject(_selectedUnit);
            serializedUnit.FindProperty("_moveBudget").intValue = 3;
            serializedUnit.ApplyModifiedPropertiesWithoutUndo();

            _controller.Select(_selectedUnit);

            Assert.That(_controller.LastMovementRange.TryGetCost(new Vector2Int(2, 0), out int cost), Is.True);
            Assert.That(cost, Is.EqualTo(3));
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(2, 0)), Is.True);
            Assert.That(
                _controller.LastMovementRange.GetPathTo(new Vector2Int(2, 0)),
                Is.EqualTo(new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0)
                }));

            Object.DestroyImmediate(mapText);
        }
    }
}
