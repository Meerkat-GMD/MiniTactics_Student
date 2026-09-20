using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace MiniTactics.Lesson04.Tests
{
    public sealed class Lesson04SceneTests
    {
        private const string ScenePath = "Assets/Lesson04/Scenes/Lesson04.unity";

        [Test]
        public void Lesson04Scene_ContainsWiredClickMovementBoardUnitsAndCamera()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath), Is.Not.Null);
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            BoardView board = Object.FindAnyObjectByType<BoardView>();
            BattleController controller = Object.FindAnyObjectByType<BattleController>();
            Unit[] units = Object.FindObjectsByType<Unit>(FindObjectsSortMode.None);
            Camera camera = Camera.main;
            GameObject grid = GameObject.Find("Grid");
            GameObject groundObject = GameObject.Find("Ground Tilemap");
            GameObject decorationObject = GameObject.Find("Decoration Tilemap");
            Tilemap ground = groundObject != null ? groundObject.GetComponent<Tilemap>() : null;
            Tilemap decoration = decorationObject != null ? decorationObject.GetComponent<Tilemap>() : null;

            Assert.That(board, Is.Not.Null);
            Assert.That(board.Width, Is.EqualTo(12));
            Assert.That(board.Height, Is.EqualTo(10));
            Assert.That(grid, Is.Not.Null);
            Assert.That(grid.transform.position, Is.EqualTo(new Vector3(-0.5f, -0.5f, 0f)));
            Assert.That(ground, Is.Not.Null);
            Assert.That(decoration, Is.Not.Null);
            Assert.That(ground.GetUsedTilesCount(), Is.EqualTo(4));
            Assert.That(decoration.GetUsedTilesCount(), Is.EqualTo(3));
            Assert.That(board.TileCount, Is.EqualTo(120));
            Assert.That(board.Board.Cells.Count, Is.EqualTo(120));
            Assert.That(board.Board.Cells.Values.Distinct().Count(), Is.EqualTo(4));
            Assert.That(controller, Is.Not.Null);
            Assert.That(GameObject.Find("MovementOverlayRoot"), Is.Not.Null);
            Assert.That(Object.FindAnyObjectByType<UnitSpawner>(), Is.Not.Null);
            Assert.That(units, Has.Length.EqualTo(4));
            Assert.That(units.Count(unit => unit.CanMove), Is.EqualTo(3));
            Assert.That(units.Any(unit => unit.transform.position == new Vector3(2f, 2f, 0f) && unit.CanMove), Is.True);
            Assert.That(units.Any(unit => unit.transform.position == new Vector3(5f, 4f, 0f) && unit.CanMove), Is.True);
            Assert.That(units.Any(unit => unit.transform.position == new Vector3(8f, 6f, 0f) && unit.CanMove), Is.True);
            Assert.That(units.Any(unit => unit.transform.position == new Vector3(6f, 4f, 0f) && !unit.CanMove), Is.True);
            Assert.That(units.All(unit => unit.GetComponent<SpriteRenderer>() != null), Is.True);
            Assert.That(units.All(unit => unit.GetComponent<Collider2D>() != null), Is.True);
            Assert.That(camera, Is.Not.Null);
            Assert.That(camera.orthographic, Is.True);
            Assert.That(camera.transform.position, Is.EqualTo(new Vector3(5.5f, 4.5f, -10f)));

            SerializedObject serializedController = new SerializedObject(controller);
            Assert.That(
                serializedController.FindProperty("_board").objectReferenceValue,
                Is.SameAs(board));

            SerializedObject serializedBoard = new SerializedObject(board);
            Assert.That(serializedBoard.FindProperty("_mapData").objectReferenceValue, Is.Not.Null);

            string[] prefabPaths =
            {
                "Assets/Lesson04/Prefabs/Unit.prefab"
            };

            Assert.That(
                prefabPaths.All(path => AssetDatabase.LoadAssetAtPath<GameObject>(path) != null),
                Is.True);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson04/Prefabs/Cursor.prefab"),
                Is.Null);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson04/Prefabs/Tile.prefab"),
                Is.Null);

            for (int index = 0; index <= 6; index++)
            {
                Assert.That(
                    AssetDatabase.LoadAssetAtPath<Tile>($"Assets/Lesson04/Tiles/Strategy_{index}.asset"),
                    Is.Not.Null);
            }

            Button undoButton = GameObject.Find("Undo Button")?.GetComponent<Button>();
            Assert.That(undoButton, Is.Not.Null);
            Assert.That(undoButton.onClick.GetPersistentEventCount(), Is.EqualTo(1));
            Assert.That(undoButton.onClick.GetPersistentTarget(0), Is.SameAs(controller));
            Assert.That(
                undoButton.onClick.GetPersistentMethodName(0),
                Is.EqualTo(nameof(BattleController.OnUndoClicked)));
            Assert.That(Object.FindAnyObjectByType<EventSystem>(), Is.Not.Null);
        }
    }
}
