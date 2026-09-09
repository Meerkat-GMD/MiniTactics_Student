using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace MiniTactics.Lesson05.Tests
{
    public sealed class MoveUnitCommandTests
    {
        private GameObject _boardObject;
        private GameObject _gridObject;
        private GameObject _unitObject;
        private Tile _testTile;
        private BoardView _board;
        private Unit _unit;

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
            tilemap.SetTile(new Vector3Int(5, 4, 0), _testTile);
            tilemap.SetTile(new Vector3Int(5, 6, 0), _testTile);

            _board = _boardObject.AddComponent<BoardView>();
            SerializedObject serializedBoard = new SerializedObject(_board);
            serializedBoard.FindProperty("_tilemap").objectReferenceValue = tilemap;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();

            _unitObject = new GameObject("Unit");
            _unitObject.transform.position = new Vector3(5f, 4f, 0f);
            _unit = _unitObject.AddComponent<Unit>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_unitObject);
            Object.DestroyImmediate(_boardObject);
            Object.DestroyImmediate(_gridObject);
            Object.DestroyImmediate(_testTile);
        }

        [Test]
        public void ExecuteAndUndo_MoveBetweenDestinationAndCapturedOrigin()
        {
            MoveUnitCommand command = new MoveUnitCommand(
                _unit,
                _board,
                new Vector2Int(5, 6));

            command.Execute();
            Assert.That(_unit.transform.position, Is.EqualTo(new Vector3(5f, 6f, 0f)));

            command.Undo();
            Assert.That(_unit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));
        }

        [Test]
        public void Constructor_RejectsMissingUnitOrBoard()
        {
            Assert.Throws<ArgumentNullException>(
                () => new MoveUnitCommand(null, _board, Vector2Int.zero));
            Assert.Throws<ArgumentNullException>(
                () => new MoveUnitCommand(_unit, null, Vector2Int.zero));
        }
    }
}
