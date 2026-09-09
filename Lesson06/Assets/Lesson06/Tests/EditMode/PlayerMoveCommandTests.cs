using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class PlayerMoveCommandTests
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
            _unit = _unitObject.AddComponent<Unit>();
            _unit.transform.position = _board.CellToWorld(new Vector2Int(5, 4));
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
        public void ExecuteAndUndo_MoveAndToggleMovedState()
        {
            PlayerMoveCommand command = new PlayerMoveCommand(
                _unit,
                _board,
                new Vector2Int(5, 6));

            command.Execute();

            Assert.That(_board.WorldToCell(_unit.transform.position), Is.EqualTo(new Vector2Int(5, 6)));
            Assert.That(_unit.transform.position, Is.EqualTo(_board.CellToWorld(new Vector2Int(5, 6))));
            Assert.That(_unit.HasMoved, Is.True);

            command.Undo();

            Assert.That(_board.WorldToCell(_unit.transform.position), Is.EqualTo(new Vector2Int(5, 4)));
            Assert.That(_unit.transform.position, Is.EqualTo(_board.CellToWorld(new Vector2Int(5, 4))));
            Assert.That(_unit.HasMoved, Is.False);
        }

        [Test]
        public void Clear_RemovesCommandsAndLeavesNothingToUndo()
        {
            CommandHistory history = new CommandHistory();
            List<string> events = new List<string>();

            history.Execute(new RecordingCommand(events, "first"));
            history.Execute(new RecordingCommand(events, "second"));
            history.Clear();

            Assert.That(history.Count, Is.Zero);
            Assert.That(history.UndoLast(), Is.False);
            Assert.That(events, Is.EqualTo(new[] { "first.Execute", "second.Execute" }));
        }

        private sealed class RecordingCommand : IUndoableCommand
        {
            private readonly List<string> _events;
            private readonly string _name;

            public RecordingCommand(List<string> events, string name)
            {
                _events = events;
                _name = name;
            }

            public void Execute() => _events.Add(_name + ".Execute");
            public void Undo() => _events.Add(_name + ".Undo");
        }
    }
}
