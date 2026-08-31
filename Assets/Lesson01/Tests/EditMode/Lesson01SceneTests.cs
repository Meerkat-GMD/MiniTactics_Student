using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson01.Tests
{
    public sealed class Lesson01SceneTests
    {
        private const string ScenePath = "Assets/Lesson01/Scenes/Lesson01.unity";

        [Test]
        public void Lesson01Scene_ContainsWiredClickMovementBoardUnitsAndCamera()
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
            Assert.That(controller, Is.Not.Null);
            Assert.That(GameObject.Find("MovementOverlayRoot"), Is.Not.Null);
            Assert.That(units, Has.Length.EqualTo(2));
            Assert.That(units.Any(unit => unit.transform.position == new Vector3(5f, 4f, 0f) && unit.CanMove), Is.True);
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

            string[] prefabPaths =
            {
                "Assets/Lesson01/Prefabs/Unit.prefab"
            };

            Assert.That(
                prefabPaths.All(path => AssetDatabase.LoadAssetAtPath<GameObject>(path) != null),
                Is.True);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson01/Prefabs/Cursor.prefab"),
                Is.Null);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson01/Prefabs/Tile.prefab"),
                Is.Null);

            for (int index = 0; index <= 6; index++)
            {
                Assert.That(
                    AssetDatabase.LoadAssetAtPath<Tile>($"Assets/Lesson01/Tiles/Strategy_{index}.asset"),
                    Is.Not.Null);
            }
        }
    }
}
