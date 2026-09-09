using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace MiniTactics.Lesson06.Editor
{
    public static class Lesson06SceneBuilder
    {
        private const string ScenePath = "Assets/Lesson06/Scenes/Lesson06.unity";

        [MenuItem("Mini Tactics/Lesson 06/Build Student Scene")]
        public static void BuildStudentScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            BoardView board = Object.FindAnyObjectByType<BoardView>();
            BattleController controller = Object.FindAnyObjectByType<BattleController>();
            Button undoButton = GetOrCreateUndoButton();

            ConfigureBoard(board);
            Unit[] units = ConfigureUnits(board);
            Unit[] combatUnits = units.Where(unit => unit.IsTurnUnit).ToArray();
            GameManager game = GetOrCreate<GameManager>("Game Manager");
            SetReferences(game, "_combatUnits", combatUnits);
            foreach (Unit unit in combatUnits) ConfigureHealthBar(unit);
            TurnManager turns = GetOrCreate<TurnManager>("Turn Manager");
            ObjectiveZone playerGoal = ConfigureGoal("Player Goal", Team.Player, new Vector2Int(0, 4), board);
            ObjectiveZone enemyGoal = ConfigureGoal("Enemy Goal", Team.Enemy, new Vector2Int(11, 6), board);
            EnemyAI[] ais = new EnemyAI[2];
            for (int index = 0; index < 2; index++)
            {
                ais[index] = units[index + 3].gameObject.AddComponent<EnemyAI>();
                SetReference(ais[index], "_board", board);
            }
            turns.Bind(units, ais, playerGoal.Cell, game);
            turns.BindObjectives(board, new[] { playerGoal, enemyGoal });
            EditorUtility.SetDirty(turns);
            SetReference(controller, "_gameManager", game);
            SetReference(controller, "_turnManager", turns);
            SetReference(controller, "_board", board);
            SetReference(controller, "_camera", Camera.main);
            SetReference(controller, "_overlayRoot", GameObject.Find("MovementOverlayRoot").transform);
            controller.BindCombatUnits(combatUnits);
            EditorUtility.SetDirty(controller);
            ConfigureHud(turns, game);

            ClearPersistentListeners(undoButton);
            UnityEventTools.AddPersistentListener(undoButton.onClick, controller.OnUndoClicked);
            EditorUtility.SetDirty(undoButton);

            EnsureEventSystem();
            EditorSceneManager.MarkSceneDirty(undoButton.gameObject.scene);
            EditorSceneManager.SaveScene(undoButton.gameObject.scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        private static void ConfigureBoard(BoardView board)
        {
            SerializedObject serializedBoard = new SerializedObject(board);
            serializedBoard.FindProperty("_mapText").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Lesson06/Maps/map01.txt");
            serializedBoard.FindProperty("_plainTile").objectReferenceValue = LoadTile(0);
            serializedBoard.FindProperty("_forestTile").objectReferenceValue = LoadTile(2);
            serializedBoard.FindProperty("_mountainTile").objectReferenceValue = LoadTile(4);
            serializedBoard.FindProperty("_riverTile").objectReferenceValue = LoadTile(5);
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();
            board.LoadBoard();
            EditorUtility.SetDirty(board);
        }

        private static Unit[] ConfigureUnits(BoardView board)
        {
            foreach (Unit unit in Object.FindObjectsByType<Unit>(FindObjectsInactive.Include))
            {
                Object.DestroyImmediate(unit.gameObject);
            }

            GameObject spawnerObject = GameObject.Find("Unit Spawner") ?? new GameObject("Unit Spawner");
            UnitSpawner spawner = spawnerObject.GetComponent<UnitSpawner>();
            if (spawner == null)
            {
                spawner = spawnerObject.AddComponent<UnitSpawner>();
            }

            spawner.Configure(AssetDatabase.LoadAssetAtPath<Unit>("Assets/Lesson06/Prefabs/Unit.prefab"), board,
                new[]
                {
                    new UnitSpawner.SpawnDefinition("Player 1", new Vector2Int(2, 2), Team.Player),
                    new UnitSpawner.SpawnDefinition("Player 2", new Vector2Int(5, 4), Team.Player),
                    new UnitSpawner.SpawnDefinition("Player 3", new Vector2Int(8, 6), Team.Player),
                    new UnitSpawner.SpawnDefinition("Enemy 1", new Vector2Int(9, 8), Team.Enemy),
                    new UnitSpawner.SpawnDefinition("Enemy 2", new Vector2Int(9, 3), Team.Enemy),
                    new UnitSpawner.SpawnDefinition("Fixed Blocker", new Vector2Int(6, 4), Team.Player, false)
                });
            Unit[] units = spawner.SpawnAll();
            foreach (Unit unit in units)
            {
                unit.ConfigureCombat(maxHp: 10, currentHp: 10, attackPower: 6, defense: 1);
                EditorUtility.SetDirty(unit);
            }
            units[5].GetComponent<SpriteRenderer>().color = new Color(0.55f, 0.55f, 0.55f);
            EditorUtility.SetDirty(spawner);
            return units;
        }

        private static T GetOrCreate<T>(string name) where T : Component
        {
            T component = Object.FindAnyObjectByType<T>();
            return component != null ? component : new GameObject(name).AddComponent<T>();
        }

        private static void SetReference(Object target, string field, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetReferences(Object target, string field, Object[] values)
        {
            var serialized = new SerializedObject(target);
            SerializedProperty list = serialized.FindProperty(field);
            list.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++)
                list.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureHealthBar(Unit unit)
        {
            // Units are recreated on every build, so each combat unit gets one fresh bound bar.
            GameObject barObject = new GameObject("Health Bar");
            barObject.transform.SetParent(unit.transform, false);
            barObject.transform.localPosition = new Vector3(-0.4f, 0.6f, 0f);
            barObject.transform.localScale = new Vector3(0.8f, 0.12f, 1f);
            Sprite sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            SpriteRenderer unitRenderer = unit.GetComponent<SpriteRenderer>();
            CreateBarSprite("Background", barObject.transform, sprite, unitRenderer,
                new Color(0.08f, 0.1f, 0.12f), 1, new Vector2(1.05f, 1.25f));
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(barObject.transform, false);
            CreateBarSprite("Fill Sprite", fill.transform, sprite, unitRenderer,
                new Color(0.25f, 0.95f, 0.3f), 2, Vector2.one);
            UnitHealthBar healthBar = barObject.AddComponent<UnitHealthBar>();
            SetReference(healthBar, "_fill", fill.transform);
            healthBar.Bind(unit);
            EditorUtility.SetDirty(healthBar);
        }

        private static void CreateBarSprite(string name, Transform parent, Sprite sprite,
            SpriteRenderer unitRenderer, Color color, int sortingOffset, Vector2 size)
        {
            GameObject target = new GameObject(name, typeof(SpriteRenderer));
            target.transform.SetParent(parent, false);
            target.transform.localPosition = new Vector3(0.5f, 0f, 0f);
            target.transform.localScale = new Vector3(size.x / sprite.bounds.size.x, size.y / sprite.bounds.size.y, 1f);
            SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingLayerID = unitRenderer.sortingLayerID;
            renderer.sortingOrder = unitRenderer.sortingOrder + sortingOffset;
        }

        private static ObjectiveZone ConfigureGoal(string name, Team owner, Vector2Int cell, BoardView board)
        {
            GameObject target = GameObject.Find(name) ?? new GameObject(name);
            ObjectiveZone goal = target.GetComponent<ObjectiveZone>();
            if (goal == null) goal = target.AddComponent<ObjectiveZone>();
            var serialized = new SerializedObject(goal);
            serialized.FindProperty("_owner").enumValueIndex = (int)owner;
            serialized.FindProperty("_cell").vector2IntValue = cell;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            target.transform.position = board.CellToWorld(cell);
            SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
            if (renderer == null) renderer = target.AddComponent<SpriteRenderer>();
            renderer.sprite = ((Tile)LoadTile(0)).sprite;
            renderer.sortingOrder = 1;
            renderer.color = owner == Team.Player ? new Color(0.2f, 0.6f, 1f) : new Color(1f, 0.25f, 0.25f);
            return goal;
        }

        private static void ConfigureHud(TurnManager turns, GameManager game)
        {
            GameObject target = GameObject.Find("Phase HUD") ?? new GameObject("Phase HUD", typeof(RectTransform), typeof(Text));
            target.transform.SetParent(GameObject.Find("Lesson UI").transform, false);
            Text text = target.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 32;
            text.color = Color.white;
            text.raycastTarget = false;
            text.alignment = TextAnchor.UpperLeft;
            RectTransform rect = text.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(32, -32);
            rect.sizeDelta = new Vector2(500, 70);
            PhaseHud hud = target.GetComponent<PhaseHud>();
            if (hud == null) hud = target.AddComponent<PhaseHud>();
            SetReference(hud, "_phaseText", text);
            hud.Bind(turns, game);
            EditorUtility.SetDirty(hud);
            EditorUtility.SetDirty(text);
            GameObject hintObject = GameObject.Find("Battle Instructions") ?? new GameObject("Battle Instructions", typeof(RectTransform), typeof(Text));
            hintObject.transform.SetParent(target.transform.parent, false);
            Text hint = hintObject.GetComponent<Text>();
            hint.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            hint.fontSize = 22;
            hint.color = Color.white;
            hint.raycastTarget = false;
            hint.text = "Select a blue unit and move. Then click an adjacent enemy to attack or empty space to skip.\nDefeat both enemies or reach the red goal. Protect the blue goal. Undo before an attack; R: Restart";
            RectTransform hintRect = hint.rectTransform;
            hintRect.anchorMin = hintRect.anchorMax = hintRect.pivot = Vector2.zero;
            hintRect.anchoredPosition = new Vector2(32, 24);
            hintRect.sizeDelta = new Vector2(1100, 65);
        }

        private static TileBase LoadTile(int index)
        {
            return AssetDatabase.LoadAssetAtPath<TileBase>(
                $"Assets/Lesson06/Tiles/Strategy_{index}.asset");
        }

        public static void PrepareStudentScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Button undoButton = GameObject.Find("Undo Button").GetComponent<Button>();

            ClearPersistentListeners(undoButton);
            EditorUtility.SetDirty(undoButton);
            EditorSceneManager.MarkSceneDirty(undoButton.gameObject.scene);
            EditorSceneManager.SaveScene(undoButton.gameObject.scene, ScenePath);
        }

        private static Button GetOrCreateUndoButton()
        {
            GameObject canvasObject = GameObject.Find("Lesson UI");
            if (canvasObject == null)
            {
                canvasObject = new GameObject(
                    "Lesson UI",
                    typeof(RectTransform),
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster));

                Canvas canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
            }

            GameObject buttonObject = GameObject.Find("Undo Button");
            if (buttonObject == null)
            {
                buttonObject = new GameObject(
                    "Undo Button",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(Button));
                buttonObject.transform.SetParent(canvasObject.transform, false);

                RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
                buttonRect.anchorMin = Vector2.one;
                buttonRect.anchorMax = Vector2.one;
                buttonRect.pivot = Vector2.one;
                buttonRect.anchoredPosition = new Vector2(-32f, -32f);
                buttonRect.sizeDelta = new Vector2(180f, 56f);

                Image image = buttonObject.GetComponent<Image>();
                image.color = new Color(0.12f, 0.18f, 0.24f, 0.95f);

                GameObject labelObject = new GameObject(
                    "Label",
                    typeof(RectTransform),
                    typeof(Text));
                labelObject.transform.SetParent(buttonObject.transform, false);

                RectTransform labelRect = labelObject.GetComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;

                Text label = labelObject.GetComponent<Text>();
                label.text = "Undo";
                label.alignment = TextAnchor.MiddleCenter;
                label.color = Color.white;
                label.fontSize = 28;
                label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            return buttonObject.GetComponent<Button>();
        }

        private static void EnsureEventSystem()
        {
            EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                new GameObject(
                    "EventSystem",
                    typeof(EventSystem),
                    typeof(InputSystemUIInputModule));
            }
        }

        private static void ClearPersistentListeners(Button button)
        {
            for (int index = button.onClick.GetPersistentEventCount() - 1; index >= 0; index--)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, index);
            }
        }
    }
}
