using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace MiniTactics.Lesson05.Tests
{
    public sealed class Lesson05SceneTests
    {
        private const string ScenePath = "Assets/Lesson05/Scenes/Lesson05.unity";

        [TearDown]
        public void CloseScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        [Test]
        public void Lesson05Scene_ContainsWiredClickMovementBoardUnitsAndCamera()
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
            Assert.That(units, Has.Length.EqualTo(6));
            Assert.That(units.Count(unit => unit.CanMove && unit.Team == Team.Player), Is.EqualTo(3));
            Assert.That(units.Count(unit => unit.CanMove && unit.Team == Team.Enemy), Is.EqualTo(2));
            Assert.That(units.Count(unit => !unit.CanMove), Is.EqualTo(1));
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
            Assert.That(serializedBoard.FindProperty("_mapText").objectReferenceValue, Is.Not.Null);
            AssertReference(board, "_tilemap", ground);
            foreach (string field in new[] { "_plainTile", "_forestTile", "_mountainTile", "_riverTile" })
                Assert.That(serializedBoard.FindProperty(field).objectReferenceValue, Is.Not.Null, field);

            string[] prefabPaths =
            {
                "Assets/Lesson05/Prefabs/Unit.prefab"
            };

            Assert.That(
                prefabPaths.All(path => AssetDatabase.LoadAssetAtPath<GameObject>(path) != null),
                Is.True);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson05/Prefabs/Cursor.prefab"),
                Is.Null);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson05/Prefabs/Tile.prefab"),
                Is.Null);

            for (int index = 0; index <= 6; index++)
            {
                Assert.That(
                    AssetDatabase.LoadAssetAtPath<Tile>($"Assets/Lesson05/Tiles/Strategy_{index}.asset"),
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
            Assert.That(Object.FindAnyObjectByType<EventSystem>().GetComponent("InputSystemUIInputModule"), Is.Not.Null);
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path == ScenePath), Is.True);

            GameManager game = RequireOne<GameManager>();
            TurnManager turns = RequireOne<TurnManager>();
            PhaseHud hud = RequireOne<PhaseHud>();
            UnitSpawner spawner = RequireOne<UnitSpawner>();
            AssertReference(controller, "_gameManager", game);
            AssertReference(controller, "_turnManager", turns);
            AssertReference(controller, "_camera", camera);
            AssertReference(controller, "_overlayRoot", GameObject.Find("MovementOverlayRoot").transform);
            AssertReference(turns, "_gameManager", game);
            AssertReference(turns, "_board", board);
            AssertReference(spawner, "_board", board);
            AssertReference(spawner, "_unitPrefab", AssetDatabase.LoadAssetAtPath<Unit>(prefabPaths[0]));

            var serializedTurns = new SerializedObject(turns);
            Unit[] boundUnits = ReadReferences<Unit>(serializedTurns.FindProperty("_units"));
            Assert.That(boundUnits, Is.EquivalentTo(units));
            Assert.That(units.Select(unit => board.WorldToCell(unit.transform.position)).Distinct().Count(), Is.EqualTo(6));
            foreach (Unit unit in units)
            {
                Assert.That(board.Board.Cells[board.WorldToCell(unit.transform.position)].IsWalkable, Is.True, unit.name);
                Assert.That(unit.GetComponent<SpriteRenderer>().sprite, Is.Not.Null, unit.name);
                if (unit.CanMove)
                    Assert.That(unit.GetComponent<SpriteRenderer>().color,
                        Is.EqualTo(unit.Team == Team.Player ? new Color(0.35f, 0.75f, 1f) : new Color(1f, 0.4f, 0.4f)));
            }

            ObjectiveZone[] goals = Object.FindObjectsByType<ObjectiveZone>(FindObjectsSortMode.None);
            Assert.That(goals, Has.Length.EqualTo(2));
            Assert.That(goals.Select(goal => goal.Owner), Is.EquivalentTo(new[] { Team.Player, Team.Enemy }));
            Assert.That(goals.Single(goal => goal.Owner == Team.Player).Cell, Is.EqualTo(new Vector2Int(0, 4)));
            Assert.That(goals.Single(goal => goal.Owner == Team.Enemy).Cell, Is.EqualTo(new Vector2Int(11, 6)));
            Assert.That(ReadReferences<ObjectiveZone>(serializedTurns.FindProperty("_objectives")), Is.EquivalentTo(goals));
            Assert.That(serializedTurns.FindProperty("_enemyGoalCell").vector2IntValue, Is.EqualTo(new Vector2Int(0, 4)));
            foreach (ObjectiveZone goal in goals)
            {
                Assert.That(board.Board.Cells[goal.Cell].IsWalkable, Is.True);
                Assert.That(goal.transform.position, Is.EqualTo(board.CellToWorld(goal.Cell)));
                Assert.That(goal.GetComponent<SpriteRenderer>().sprite, Is.Not.Null);
                Assert.That(units.Any(unit => board.WorldToCell(unit.transform.position) == goal.Cell), Is.False);
            }

            EnemyAI[] ais = ReadReferences<EnemyAI>(serializedTurns.FindProperty("_enemyAis"));
            Unit[] enemies = boundUnits.Where(unit => unit.Team == Team.Enemy).ToArray();
            Assert.That(ais, Has.Length.EqualTo(2));
            Assert.That(ais, Is.EquivalentTo(Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None)));
            for (int index = 0; index < ais.Length; index++)
            {
                Assert.That(ais[index].gameObject, Is.SameAs(enemies[index].gameObject));
                AssertReference(ais[index], "_board", board);
            }

            AssertReference(hud, "_turnManager", turns);
            AssertReference(hud, "_gameManager", game);
            Text phaseText = (Text)new SerializedObject(hud).FindProperty("_phaseText").objectReferenceValue;
            Assert.That(phaseText, Is.Not.Null);
            Assert.That(phaseText.font, Is.Not.Null);
            Assert.That(phaseText.isActiveAndEnabled && phaseText.color.a > 0 && phaseText.fontSize > 0, Is.True);
            Assert.That(phaseText.rectTransform.rect.width, Is.GreaterThan(0));
            Assert.That(GameObject.Find("Battle Instructions").GetComponent<Text>().font, Is.Not.Null);
            Assert.That(GameObject.Find("Lesson UI").GetComponent<Canvas>(), Is.Not.Null);
        }

        [Test]
        public void SuppliedScene_PreservesSelectionMovementAndUndo()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            BattleController controller = Object.FindAnyObjectByType<BattleController>();
            Unit player = GameObject.Find("Player 1").GetComponent<Unit>();
            controller.StateMachine.HandleBoardClick(player, new Vector2Int(2, 2));
            Assert.That(controller.SelectedUnit, Is.SameAs(player));
            Assert.That(controller.TryMoveSelectedUnit(new Vector2Int(2, 1)), Is.True);
            Assert.That(player.transform.position, Is.EqualTo(new Vector3(2, 1, 0)));
            controller.OnUndoClicked();
            Assert.That(player.transform.position, Is.EqualTo(new Vector3(2, 2, 0)));
        }

        private static T RequireOne<T>() where T : Object
        {
            T[] objects = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
            Assert.That(objects, Has.Length.EqualTo(1));
            return objects[0];
        }

        private static T[] ReadReferences<T>(SerializedProperty property) where T : Object
        {
            return Enumerable.Range(0, property.arraySize)
                .Select(index => (T)property.GetArrayElementAtIndex(index).objectReferenceValue).ToArray();
        }

        private static void AssertReference(Object target, string field, Object expected)
        {
            Assert.That(expected, Is.Not.Null, field);
            Assert.That(new SerializedObject(target).FindProperty(field).objectReferenceValue, Is.SameAs(expected), field);
        }
    }
}
