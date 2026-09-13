using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class Lesson06SceneTests
    {
        private const string ScenePath = "Assets/Lesson06/Scenes/Lesson06.unity";

        [Test]
        public void PrepareStudentScene_PreservesTheUsableUndoListener()
        {
            byte[] originalScene = System.IO.File.ReadAllBytes(ScenePath);
            try
            {
                System.Type builder = System.AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType("MiniTactics.Lesson06.Editor.Lesson06SceneBuilder"))
                    .First(type => type != null);
                builder.GetMethod("PrepareStudentScene").Invoke(null, null);
                var controller = Object.FindAnyObjectByType<BattleController>();
                var button = GameObject.Find("Undo Button").GetComponent<Button>();
                Assert.That(button.onClick.GetPersistentEventCount(), Is.EqualTo(1));
                Assert.That(button.onClick.GetPersistentTarget(0), Is.SameAs(controller));
                Assert.That(button.onClick.GetPersistentMethodName(0), Is.EqualTo(nameof(BattleController.OnUndoClicked)));
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                System.IO.File.WriteAllBytes(ScenePath, originalScene);
                AssetDatabase.ImportAsset(ScenePath);
            }
        }

        [TearDown]
        public void CloseScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        [Test]
        public void StudentScene_MoveBesideEnemyCompletesAndCanUndoBeforeOtherPlayersAct()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindAnyObjectByType<BattleController>();
            var board = Object.FindAnyObjectByType<BoardView>();
            Unit player = GameObject.Find("Player 3").GetComponent<Unit>();
            controller.Select(player);

            Assert.That(controller.TryMoveSelectedUnit(new Vector2Int(9, 7)), Is.True);
            Assert.That(controller.StateMachine.Current, Is.TypeOf<IdleState>());
            Assert.That(player.HasMoved, Is.True);
            controller.OnUndoClicked();

            Assert.That(player.HasMoved, Is.False);
            Assert.That(board.WorldToCell(player.transform.position), Is.EqualTo(new Vector2Int(8, 6)));
            Assert.That(controller.CanPlayerAct(player), Is.True);
        }

        [Test]
        public void StudentScene_FiveVisibleBarsAndCombatReferencesAreReadyForExercises()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Unit[] units = Object.FindObjectsByType<Unit>();
            Unit[] combatUnits = units.Where(unit => unit.IsTurnUnit).ToArray();
            Assert.That(Object.FindObjectsByType<UnitHealthBar>(FindObjectsInactive.Include), Has.Length.EqualTo(5));
            foreach (Unit unit in units)
            {
                UnitHealthBar[] bars = unit.GetComponentsInChildren<UnitHealthBar>(true);
                Assert.That(bars, Has.Length.EqualTo(unit.IsTurnUnit ? 1 : 0), unit.name);
                if (!unit.IsTurnUnit) continue;
                Assert.That(unit.CurrentHp, Is.EqualTo(10));
                Assert.That(unit.MaxHp, Is.EqualTo(10));
                Assert.That(unit.AttackPower, Is.EqualTo(6));
                Assert.That(unit.Defense, Is.EqualTo(1));
                AssertReference(bars[0], "_unit", unit);
                Transform fill = (Transform)new SerializedObject(bars[0]).FindProperty("_fill").objectReferenceValue;
                Assert.That(fill, Is.Not.Null);
                Assert.That(fill.IsChildOf(bars[0].transform), Is.True);
                Assert.That(fill.localScale.x, Is.EqualTo(1f));
                SpriteRenderer[] renderers = bars[0].GetComponentsInChildren<SpriteRenderer>(true);
                Assert.That(renderers, Has.Length.EqualTo(2));
                Assert.That(renderers.Select(renderer => renderer.sortingOrder), Is.EquivalentTo(new[] { 11, 12 }));
                foreach (SpriteRenderer renderer in renderers)
                {
                    Assert.That(renderer.sprite, Is.Not.Null);
                    Assert.That(renderer.enabled && renderer.gameObject.activeInHierarchy && renderer.color.a > 0, Is.True);
                    Assert.That(renderer.bounds.size.x > 0 && renderer.bounds.size.y > 0, Is.True);
                }
            }
            var game = Object.FindAnyObjectByType<GameManager>();
            var controller = Object.FindAnyObjectByType<BattleController>();
            Unit[] gameUnits = ReadCombatReferences(game);
            Assert.That(gameUnits, Is.EquivalentTo(combatUnits));
            Assert.That(ReadCombatReferences(controller), Is.EqualTo(gameUnits));
        }

        [Test]
        public async Task StudentScene_EnemyMovementUsesGoalAndSerializedOrderThenReturnsToPlayer()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var turns = Object.FindAnyObjectByType<TurnManager>();
            var board = Object.FindAnyObjectByType<BoardView>();
            Unit first = GameObject.Find("Enemy 1").GetComponent<Unit>();
            Unit second = GameObject.Find("Enemy 2").GetComponent<Unit>();
            foreach (EnemyAI ai in Object.FindObjectsByType<EnemyAI>()) ai.TurnDelayMilliseconds = 0;
            var order = new List<Unit>();
            turns.EnemyTurnStarted += order.Add;
            await turns.RunEnemyPhaseAsync(CancellationToken.None);

            Assert.That(order, Is.EqualTo(new[] { first, second }));
            Assert.That(board.WorldToCell(first.transform.position), Is.EqualTo(new Vector2Int(9, 4)));
            Assert.That(board.WorldToCell(second.transform.position), Is.EqualTo(new Vector2Int(5, 3)));
            Assert.That(turns.Phase, Is.EqualTo(BattlePhase.Player));
            Assert.That(Object.FindObjectsByType<Unit>().Where(unit => unit.Team == Team.Player && unit.IsTurnUnit)
                .All(turns.CanPlayerAct), Is.True);
        }

        [Test]
        public void StudentScene_EnemyGoalCaptureStillEndsBattleAndBlocksPlayerInput()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var turns = Object.FindAnyObjectByType<TurnManager>();
            var board = Object.FindAnyObjectByType<BoardView>();
            var game = Object.FindAnyObjectByType<GameManager>();
            var controller = Object.FindAnyObjectByType<BattleController>();
            Unit player = GameObject.Find("Player 3").GetComponent<Unit>();
            player.MoveTo(board, new Vector2Int(11, 6));

            Assert.That(turns.CheckGoal(player), Is.True);
            Assert.That(game.Result, Is.EqualTo(BattleResult.PlayerWon));
            Assert.That(turns.Phase, Is.EqualTo(BattlePhase.Finished));
            Assert.That(controller.CanAcceptPlayerInput, Is.False);
        }

        private static Unit[] ReadCombatReferences(Object component)
        {
            SerializedProperty units = new SerializedObject(component).FindProperty("_combatUnits");
            return Enumerable.Range(0, units.arraySize)
                .Select(index => (Unit)units.GetArrayElementAtIndex(index).objectReferenceValue).ToArray();
        }

        [Test]
        public async Task StudentScene_CombatEntryUsesGoalMovementAndHonorsFinishedResult()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var board = Object.FindAnyObjectByType<BoardView>();
            var game = Object.FindAnyObjectByType<GameManager>();
            typeof(GameManager).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(game, null);
            Unit enemy = GameObject.Find("Enemy 1").GetComponent<Unit>();
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            ai.TurnDelayMilliseconds = 0;
            await ai.TakeCombatTurnAsync(enemy, Object.FindObjectsByType<Unit>(), CancellationToken.None);
            Assert.That(board.WorldToCell(enemy.transform.position), Is.EqualTo(new Vector2Int(9, 4)));
            Assert.That(enemy.HasMoved, Is.True);

            game.TryFinish(BattleResult.PlayerWon);
            await ai.TakeCombatTurnAsync(enemy, Object.FindObjectsByType<Unit>(), CancellationToken.None);
            Assert.That(board.WorldToCell(enemy.transform.position), Is.EqualTo(new Vector2Int(9, 4)));
        }

        [Test]
        public void Lesson06Scene_PlayerMoveAndUndoUseSerializedManagers()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var controller = Object.FindAnyObjectByType<BattleController>();
            var board = Object.FindAnyObjectByType<BoardView>();
            Unit player = GameObject.Find("Player 1").GetComponent<Unit>();
            controller.Select(player);
            Assert.That(controller.TryMoveSelectedUnit(new Vector2Int(2, 1)), Is.True);
            Assert.That(player.HasMoved, Is.True);
            controller.OnUndoClicked();
            Assert.That(player.HasMoved, Is.False);
            Assert.That(player.transform.position, Is.EqualTo(board.CellToWorld(new Vector2Int(2, 2))));
        }

        [Test]
        public void Lesson06Scene_HasCompleteTurnObjectiveAndHudWiring()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Assert.That(Object.FindObjectsByType<GameManager>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<TurnManager>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<PhaseHud>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<BoardView>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<BattleController>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<Camera>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<UnitSpawner>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            var game = Object.FindAnyObjectByType<GameManager>();
            var turns = Object.FindAnyObjectByType<TurnManager>();
            var board = Object.FindAnyObjectByType<BoardView>();
            var controller = Object.FindAnyObjectByType<BattleController>();
            Assert.That(turns.Phase, Is.EqualTo(BattlePhase.Player));
            Assert.That(game.IsPlaying, Is.True);
            var singletonTypes = typeof(GameManager).Assembly.GetTypes().Where(type =>
                type.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static) != null);
            Assert.That(singletonTypes, Is.EquivalentTo(new[] { typeof(GameManager) }));
            Assert.That(typeof(TurnManager).GetProperty("Instance"), Is.Null);
            Assert.That(typeof(GameManager).GetMethod("RestartCurrentScene", System.Type.EmptyTypes), Is.Not.Null);
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path == ScenePath), Is.True);

            Unit[] units = Object.FindObjectsByType<Unit>(FindObjectsSortMode.None);
            Assert.That(units, Has.Length.EqualTo(6));
            Assert.That(units.Count(unit => unit.CanMove && unit.Team == Team.Player), Is.EqualTo(3));
            Assert.That(units.Count(unit => unit.CanMove && unit.Team == Team.Enemy), Is.EqualTo(2));
            Assert.That(units.Count(unit => !unit.CanMove && !unit.IsTurnUnit), Is.EqualTo(1));
            Assert.That(units.Select(unit => board.WorldToCell(unit.transform.position)).Distinct().Count(), Is.EqualTo(6));
            foreach (Unit unit in units)
                Assert.That(board.Board.Cells[board.WorldToCell(unit.transform.position)].IsWalkable, Is.True, unit.name);

            ObjectiveZone[] goals = Object.FindObjectsByType<ObjectiveZone>(FindObjectsSortMode.None);
            Assert.That(goals, Has.Length.EqualTo(2));
            Assert.That(goals.Select(goal => goal.Owner), Is.EquivalentTo(new[] { Team.Player, Team.Enemy }));
            Assert.That(goals.Select(goal => goal.Cell).Distinct().Count(), Is.EqualTo(2));
            foreach (ObjectiveZone goal in goals)
            {
                Assert.That(board.Board.Cells[goal.Cell].IsWalkable, Is.True);
                Assert.That(goal.transform.position, Is.EqualTo(board.CellToWorld(goal.Cell)));
                Assert.That(goal.GetComponent<SpriteRenderer>().sprite, Is.Not.Null);
                Assert.That(units.Any(unit => board.WorldToCell(unit.transform.position) == goal.Cell), Is.False);
            }
            AssertReference(controller, "_turnManager", turns);
            AssertReference(controller, "_gameManager", game);
            AssertReference(controller, "_camera", Camera.main);
            AssertReference(controller, "_overlayRoot", GameObject.Find("MovementOverlayRoot").transform);
            AssertReference(turns, "_gameManager", game);
            AssertReference(turns, "_board", board);
            var serializedTurns = new SerializedObject(turns);
            Assert.That(serializedTurns.FindProperty("_units").arraySize, Is.EqualTo(6));
            Assert.That(serializedTurns.FindProperty("_objectives").arraySize, Is.EqualTo(2));
            Assert.That(serializedTurns.FindProperty("_enemyGoalCell").vector2IntValue,
                Is.EqualTo(goals.Single(goal => goal.Owner == Team.Player).Cell));
            for (int index = 0; index < 2; index++)
                Assert.That(goals, Does.Contain(serializedTurns.FindProperty("_objectives").GetArrayElementAtIndex(index).objectReferenceValue));
            var boundUnits = Enumerable.Range(0, 6).Select(index =>
                (Unit)serializedTurns.FindProperty("_units").GetArrayElementAtIndex(index).objectReferenceValue).ToArray();
            Assert.That(boundUnits, Is.EquivalentTo(units));
            Unit[] enemies = boundUnits.Where(unit => unit.Team == Team.Enemy).ToArray();
            Assert.That(Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None), Has.Length.EqualTo(2));
            Assert.That(serializedTurns.FindProperty("_enemyAis").arraySize, Is.EqualTo(2));
            for (int index = 0; index < 2; index++)
            {
                var ai = (EnemyAI)serializedTurns.FindProperty("_enemyAis").GetArrayElementAtIndex(index).objectReferenceValue;
                Assert.That(ai.gameObject, Is.SameAs(enemies[index].gameObject));
                AssertReference(ai, "_board", board);
            }
            PhaseHud hud = Object.FindAnyObjectByType<PhaseHud>();
            AssertReference(hud, "_turnManager", turns);
            AssertReference(hud, "_gameManager", game);
            Text text = (Text)new SerializedObject(hud).FindProperty("_phaseText").objectReferenceValue;
            Assert.That(text, Is.Not.Null);
            Assert.That(text.font, Is.Not.Null);
            Assert.That(text.font, Is.SameAs(Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")));
            Assert.That(text.isActiveAndEnabled && text.color.a > 0 && text.fontSize > 0, Is.True);
            Assert.That(text.rectTransform.rect.width, Is.GreaterThan(0));
            hud.Bind(turns, game);
            Assert.That(text.text, Is.EqualTo("PLAYER PHASE"));
            turns.MarkFinished(BattleResult.PlayerWon);
            Assert.That(text.text, Is.EqualTo("VICTORY"));
            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindAnyObjectByType<EventSystem>().GetComponent("InputSystemUIInputModule"), Is.Not.Null);
            AssertReference(Object.FindAnyObjectByType<UnitSpawner>(), "_board", board);
            Assert.That(new SerializedObject(Object.FindAnyObjectByType<UnitSpawner>()).FindProperty("_unitPrefab").objectReferenceValue, Is.Not.Null);
        }

        private static void AssertReference(Object target, string name, Object expected)
        {
            Assert.That(new SerializedObject(target).FindProperty(name).objectReferenceValue, Is.SameAs(expected), name);
        }

        [Test]
        public void Lesson06Scene_ContainsWiredClickMovementBoardUnitsAndCamera()
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
            Assert.That(units.Count(unit => unit.CanMove), Is.EqualTo(5));
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
            foreach (string field in new[] { "_tilemap", "_plainTile", "_forestTile", "_mountainTile", "_riverTile" })
                Assert.That(serializedBoard.FindProperty(field).objectReferenceValue, Is.Not.Null, field);

            string[] prefabPaths =
            {
                "Assets/Lesson06/Prefabs/Unit.prefab"
            };

            Assert.That(
                prefabPaths.All(path => AssetDatabase.LoadAssetAtPath<GameObject>(path) != null),
                Is.True);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson06/Prefabs/Cursor.prefab"),
                Is.Null);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lesson06/Prefabs/Tile.prefab"),
                Is.Null);

            for (int index = 0; index <= 6; index++)
            {
                Assert.That(
                    AssetDatabase.LoadAssetAtPath<Tile>($"Assets/Lesson06/Tiles/Strategy_{index}.asset"),
                    Is.Not.Null);
            }

            Button undoButton = GameObject.Find("Undo Button")?.GetComponent<Button>();
            Assert.That(undoButton, Is.Not.Null);
            Assert.That(undoButton.onClick.GetPersistentEventCount(), Is.EqualTo(1));
            Assert.That(undoButton.onClick.GetPersistentTarget(0), Is.SameAs(controller));
            Assert.That(undoButton.onClick.GetPersistentListenerState(0), Is.EqualTo(UnityEngine.Events.UnityEventCallState.RuntimeOnly));
            Assert.That(
                undoButton.onClick.GetPersistentMethodName(0),
                Is.EqualTo(nameof(BattleController.OnUndoClicked)));
            Assert.That(Object.FindAnyObjectByType<EventSystem>(), Is.Not.Null);
        }
    }
}
