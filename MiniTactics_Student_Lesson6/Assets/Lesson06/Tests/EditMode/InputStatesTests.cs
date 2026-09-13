using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class InputStatesTests
    {
        private GameObject _root;
        private GameObject _gridObject;
        private Tile _tile;
        private BoardView _board;
        private BattleController _controller;
        private Unit _first;
        private Unit _second;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("Test Root");
            _gridObject = new GameObject("Grid", typeof(Grid));
            _gridObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
            GameObject tilemapObject = new GameObject("Ground", typeof(Tilemap));
            tilemapObject.transform.SetParent(_gridObject.transform, false);
            Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();
            _tile = ScriptableObject.CreateInstance<Tile>();
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++) tilemap.SetTile(new Vector3Int(x, y), _tile);
            }

            _board = _root.AddComponent<BoardView>();
            SerializedObject board = new SerializedObject(_board);
            board.FindProperty("_tilemap").objectReferenceValue = tilemap;
            board.ApplyModifiedPropertiesWithoutUndo();

            _first = CreateUnit("First", new Vector3(1f, 1f));
            _second = CreateUnit("Second", new Vector3(3f, 1f));

            GameObject overlay = new GameObject("MovementOverlayRoot");
            overlay.transform.SetParent(_root.transform);
            _controller = new GameObject("Controller").AddComponent<BattleController>();
            _controller.transform.SetParent(_root.transform);
            GameManager game = _root.AddComponent<GameManager>();
            TurnManager turns = _root.AddComponent<TurnManager>();
            turns.Bind(new[] { _first, _second }, System.Array.Empty<EnemyAI>(), Vector2Int.zero, game);
            SerializedObject controller = new SerializedObject(_controller);
            controller.FindProperty("_board").objectReferenceValue = _board;
            controller.FindProperty("_overlayRoot").objectReferenceValue = overlay.transform;
            controller.FindProperty("_gameManager").objectReferenceValue = game;
            controller.FindProperty("_turnManager").objectReferenceValue = turns;
            controller.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
            Object.DestroyImmediate(_gridObject);
            Object.DestroyImmediate(_first.gameObject);
            Object.DestroyImmediate(_second.gameObject);
            Object.DestroyImmediate(_tile);
        }

        [Test]
        public void SelectingAndMoving_TransitionsFromIdleToSelectedAndBackToIdle()
        {
            Assert.That(_controller.StateMachine.Current, Is.TypeOf<IdleState>());

            _controller.StateMachine.HandleBoardClick(_first, new Vector2Int(1, 1));

            Assert.That(_controller.StateMachine.Current, Is.TypeOf<UnitSelectedState>());
            Assert.That(_controller.SelectedUnit, Is.SameAs(_first));

            _controller.StateMachine.HandleBoardClick(null, new Vector2Int(1, 3));

            Assert.That(_first.transform.position, Is.EqualTo(new Vector3(1f, 3f, 0f)));
            Assert.That(_controller.StateMachine.Current, Is.TypeOf<IdleState>());
            Assert.That(_controller.SelectedUnit, Is.Null);
        }

        [Test]
        public void ClickingAnotherMovableUnit_SwitchesSelection()
        {
            _controller.StateMachine.HandleBoardClick(_first, new Vector2Int(1, 1));
            _controller.StateMachine.HandleBoardClick(_second, new Vector2Int(3, 1));

            Assert.That(_controller.StateMachine.Current, Is.TypeOf<UnitSelectedState>());
            Assert.That(_controller.SelectedUnit, Is.SameAs(_second));
        }

        [Test]
        public void Undo_IsHandledByTheCurrentState()
        {
            _controller.StateMachine.HandleBoardClick(_first, new Vector2Int(1, 1));
            _controller.StateMachine.HandleBoardClick(null, new Vector2Int(1, 3));
            _controller.StateMachine.HandleUndo();

            Assert.That(_first.transform.position, Is.EqualTo(new Vector3(1f, 1f, 0f)));
        }

        private Unit CreateUnit(string name, Vector3 position)
        {
            GameObject unitObject = new GameObject(name);
            unitObject.transform.position = position;
            return unitObject.AddComponent<Unit>();
        }
    }
}
