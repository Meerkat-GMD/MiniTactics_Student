using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace MiniTactics.Lesson04.Editor
{
    public static class Lesson04SceneBuilder
    {
        private const string ScenePath = "Assets/Lesson04/Scenes/Lesson04.unity";

        [MenuItem("Mini Tactics/Lesson 04/Build Teacher Scene")]
        public static void BuildTeacherScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            BoardView board = Object.FindAnyObjectByType<BoardView>();
            BattleController controller = Object.FindAnyObjectByType<BattleController>();
            Button undoButton = GetOrCreateUndoButton();

            ConfigureBoard(board);
            ConfigureUnits(board);

            ClearPersistentListeners(undoButton);
            UnityEventTools.AddPersistentListener(undoButton.onClick, controller.OnUndoClicked);
            EditorUtility.SetDirty(undoButton);

            EnsureEventSystem();
            EditorSceneManager.MarkSceneDirty(undoButton.gameObject.scene);
            EditorSceneManager.SaveScene(undoButton.gameObject.scene, ScenePath);
        }

        private static void ConfigureBoard(BoardView board)
        {
            SerializedObject serializedBoard = new SerializedObject(board);
            serializedBoard.FindProperty("_mapData").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<MapData>("Assets/Lesson04/Maps/map01.asset");
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();
            board.LoadBoard();
            EditorUtility.SetDirty(board);
        }

        private static void ConfigureUnits(BoardView board)
        {
            foreach (Unit unit in Object.FindObjectsByType<Unit>(FindObjectsSortMode.None))
            {
                if (unit.CanMove)
                {
                    Object.DestroyImmediate(unit.gameObject);
                }
            }

            GameObject spawnerObject = GameObject.Find("Unit Spawner") ?? new GameObject("Unit Spawner");
            UnitSpawner spawner = spawnerObject.GetComponent<UnitSpawner>();
            if (spawner == null)
            {
                spawner = spawnerObject.AddComponent<UnitSpawner>();
            }

            SerializedObject serializedSpawner = new SerializedObject(spawner);
            serializedSpawner.FindProperty("_unitPrefab").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Unit>("Assets/Lesson04/Prefabs/Unit.prefab");
            serializedSpawner.FindProperty("_board").objectReferenceValue = board;
            SerializedProperty cells = serializedSpawner.FindProperty("_spawnCells");
            cells.arraySize = 3;
            cells.GetArrayElementAtIndex(0).vector2IntValue = new Vector2Int(2, 2);
            cells.GetArrayElementAtIndex(1).vector2IntValue = new Vector2Int(5, 4);
            cells.GetArrayElementAtIndex(2).vector2IntValue = new Vector2Int(8, 6);
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();

            foreach (Unit unit in spawner.SpawnAll())
            {
                SerializedObject serializedUnit = new SerializedObject(unit);
                serializedUnit.FindProperty("_moveBudget").intValue = 4;
                serializedUnit.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(unit);
            }

            EditorUtility.SetDirty(spawner);
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
