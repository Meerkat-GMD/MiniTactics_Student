using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniTactics.Lesson05.Tests
{
    public sealed class ManualEndTurnTests
    {
        private BattleController _controller;
        private TurnManager _turns;
        private Unit _player;
        private Unit _secondPlayer;
        private Transform _overlays;

        [SetUp]
        public void OpenScene()
        {
            EditorSceneManager.OpenScene("Assets/Lesson05/Scenes/Lesson05.unity", OpenSceneMode.Single);
            _controller = Object.FindAnyObjectByType<BattleController>();
            _turns = Object.FindAnyObjectByType<TurnManager>();
            _player = GameObject.Find("Player 1").GetComponent<Unit>();
            _secondPlayer = GameObject.Find("Player 2").GetComponent<Unit>();
            _overlays = GameObject.Find("MovementOverlayRoot").transform;
        }

        [TearDown]
        public void CloseScene() =>
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        [Test]
        public void PublicManualEnd_ClearsSelectionWhileTheTurnExerciseIsIncomplete()
        {
            _controller.Select(_player);
            RequestManualEnd();
            AssertCleared();
            Assert.That(_turns.Phase, Is.EqualTo(BattlePhase.Player));
        }

        [Test]
        public void EnterAfterMouseUndo_DoesNotSubmitSelectedUndoAgain()
        {
            _controller.Select(_player);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(2, 1)), Is.True);
            _controller.Select(_secondPlayer);
            Assert.That(_controller.TryMoveSelectedUnit(new Vector2Int(4, 4)), Is.True);

            Button undo = GameObject.Find("Undo Button").GetComponent<Button>();
            undo.onClick.AddListener(_controller.OnUndoClicked);
            EventSystem eventSystem = FindSceneEventSystem();
            eventSystem.SetSelectedGameObject(undo.gameObject);
            undo.onClick.Invoke();
            Assert.That(_secondPlayer.transform.position, Is.EqualTo(new Vector3(5, 4, 0)));

            SubmitCurrentSelection(eventSystem);
            RequestManualEnd();

            Assert.That(_player.transform.position, Is.EqualTo(new Vector3(2, 1, 0)),
                "Enter must not submit the still-selected Undo button before ending the phase.");
        }

        [Test]
        public void InputSystemWiring_RoutesBothEnterKeysToThePublicActionBeforeMouseHandling()
        {
            // EditMode has editor input buffers; the public action above exercises
            // the real serialized scene, while this check preserves both key bindings.
            string source = System.IO.File.ReadAllText("Assets/Lesson05/Scripts/BattleController.cs");
            int keyboardStart = source.IndexOf("Keyboard keyboard = Keyboard.current;");
            int mouseStart = source.IndexOf("if (Mouse.current == null");
            Assert.That(keyboardStart, Is.GreaterThanOrEqualTo(0));
            Assert.That(mouseStart, Is.GreaterThan(keyboardStart));
            string keyboardRoute = source.Substring(keyboardStart, mouseStart - keyboardStart);
            Assert.That(keyboardRoute, Does.Contain("keyboard.enterKey.wasPressedThisFrame"));
            Assert.That(keyboardRoute, Does.Contain("keyboard.numpadEnterKey.wasPressedThisFrame"));
            Assert.That(keyboardRoute, Does.Contain("OnEndTurnRequested();"));
        }

        private void RequestManualEnd()
        {
            MethodInfo action = typeof(BattleController).GetMethod("OnEndTurnRequested", BindingFlags.Instance | BindingFlags.Public);
            Assert.That(action, Is.Not.Null, "The controller must expose a public end-turn action.");
            action.Invoke(_controller, null);
        }

        private static void SubmitCurrentSelection(EventSystem eventSystem)
        {
            GameObject selected = eventSystem.currentSelectedGameObject;
            if (selected != null)
                ExecuteEvents.Execute(selected, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }

        private static EventSystem FindSceneEventSystem()
        {
            return Object.FindAnyObjectByType<EventSystem>();
        }

        private void AssertCleared()
        {
            Assert.That(_controller.SelectedUnit, Is.Null);
            Assert.That(_controller.StateMachine.Current, Is.TypeOf<IdleState>());
            Assert.That(_overlays.GetComponentsInChildren<SpriteRenderer>(), Is.Empty);
        }
    }
}
