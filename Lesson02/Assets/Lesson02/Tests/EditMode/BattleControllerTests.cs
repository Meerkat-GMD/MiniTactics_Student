using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson02.Tests
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
            SerializedObject serializedController = new SerializedObject(_controller);
            serializedController.FindProperty("_board").objectReferenceValue = _board;
            serializedController.FindProperty("_overlayRoot").objectReferenceValue = _overlayRootObject.transform;
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
        public void MovementRange_IncludesStartAndPassesThroughOccupiedCell()
        {
            MovementCell[] range = _board.GetMovementRange(_selectedUnit).ToArray();

            Assert.That(range.Single(cell => cell.Cell == new Vector2Int(5, 4)).IsOccupied, Is.False);
            Assert.That(range.Single(cell => cell.Cell == new Vector2Int(6, 4)).IsOccupied, Is.True);
            Assert.That(range.Single(cell => cell.Cell == new Vector2Int(7, 4)).IsOccupied, Is.False);
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
    }
}
