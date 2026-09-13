using System;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace MiniTactics.Lesson02.Tests
{
    public sealed class Lesson02StudentProgressTests
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
            serializedController.FindProperty("_overlayRoot").objectReferenceValue =
                _overlayRootObject.transform;
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
        public void Contracts_DeclareExecuteInheritanceAndUndo()
        {
            MethodInfo execute = typeof(ICommand).GetMethod("Execute");
            Assert.That(execute, Is.Not.Null, "ICommand에 public void Execute()를 선언하세요.");
            Assert.That(execute.ReturnType, Is.EqualTo(typeof(void)));
            Assert.That(execute.GetParameters(), Is.Empty);

            Assert.That(
                typeof(IUndoableCommand).GetInterfaces(),
                Does.Contain(typeof(ICommand)),
                "IUndoableCommand가 ICommand를 상속하게 하세요.");

            MethodInfo undo = typeof(IUndoableCommand).GetMethod("Undo");
            Assert.That(undo, Is.Not.Null, "IUndoableCommand에 public void Undo()를 선언하세요.");
            Assert.That(undo.ReturnType, Is.EqualTo(typeof(void)));
            Assert.That(undo.GetParameters(), Is.Empty);
        }

        [Test]
        public void MoveUnitCommand_ExecutesAndUndoesCapturedMove()
        {
            object move = CreateMove(new Vector2Int(5, 6));
            MethodInfo execute = RequireMethod(typeof(MoveUnitCommand), "Execute");
            MethodInfo undo = RequireMethod(typeof(MoveUnitCommand), "Undo");

            execute.Invoke(move, null);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 6f, 0f)));

            undo.Invoke(move, null);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));
        }

        [Test]
        public void CommandHistory_UndoesMovesInReverseOrder()
        {
            object history = Activator.CreateInstance(typeof(CommandHistory));
            MethodInfo execute = typeof(CommandHistory).GetMethod(
                "Execute",
                new[] { typeof(IUndoableCommand) });
            Assert.That(execute, Is.Not.Null, "CommandHistory.Execute(IUndoableCommand)를 작성하세요.");
            MethodInfo undoLast = RequireMethod(typeof(CommandHistory), "UndoLast");

            object firstMove = CreateMove(new Vector2Int(5, 6));
            execute.Invoke(history, new[] { firstMove });
            object secondMove = CreateMove(new Vector2Int(7, 6));
            execute.Invoke(history, new[] { secondMove });

            Assert.That(undoLast.Invoke(history, null), Is.EqualTo(true));
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 6f, 0f)));
            Assert.That(undoLast.Invoke(history, null), Is.EqualTo(true));
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));
        }

        [Test]
        public void CommandHistory_EmptyUndoIsSafe()
        {
            object history = Activator.CreateInstance(typeof(CommandHistory));
            MethodInfo undoLast = RequireMethod(typeof(CommandHistory), "UndoLast");

            Assert.That(undoLast.Invoke(history, null), Is.EqualTo(false));
        }

        [Test]
        public void BattleController_ValidMoveCanBeUndone()
        {
            MethodInfo onUndoClicked = RequireMethod(typeof(BattleController), "OnUndoClicked");

            _controller.Select(_selectedUnit);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(5, 6)), Is.True);
            _controller.Select(_selectedUnit);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(7, 6)), Is.True);

            onUndoClicked.Invoke(_controller, null);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 6f, 0f)));
            onUndoClicked.Invoke(_controller, null);
            Assert.That(_selectedUnit.transform.position, Is.EqualTo(new Vector3(5f, 4f, 0f)));
        }

        private object CreateMove(Vector2Int destination)
        {
            ConstructorInfo constructor = typeof(MoveUnitCommand).GetConstructor(
                new[] { typeof(Unit), typeof(BoardView), typeof(Vector2Int) });
            Assert.That(constructor, Is.Not.Null, "MoveUnitCommand 생성자를 작성하세요.");
            Assert.That(
                typeof(IUndoableCommand).IsAssignableFrom(typeof(MoveUnitCommand)),
                Is.True,
                "MoveUnitCommand가 IUndoableCommand를 구현하게 하세요.");

            return constructor.Invoke(new object[] { _selectedUnit, _board, destination });
        }

        private static MethodInfo RequireMethod(Type type, string methodName)
        {
            MethodInfo method = type.GetMethod(methodName);
            Assert.That(method, Is.Not.Null, $"{type.Name}.{methodName}()를 작성하세요.");
            return method;
        }
    }
}
