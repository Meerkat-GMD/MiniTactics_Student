using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MiniTactics.Lesson05.Tests
{
    public sealed class ManualEndTurnTests
    {
        private BattleController _controller;
        private TurnManager _turns;
        private Unit _player;
        private Transform _overlays;

        [SetUp]
        public void OpenScene()
        {
            EditorSceneManager.OpenScene("Assets/Lesson05/Scenes/Lesson05.unity", OpenSceneMode.Single);
            _controller = Object.FindAnyObjectByType<BattleController>();
            _turns = Object.FindAnyObjectByType<TurnManager>();
            _player = GameObject.Find("Player 1").GetComponent<Unit>();
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

        private void AssertCleared()
        {
            Assert.That(_controller.SelectedUnit, Is.Null);
            Assert.That(_controller.StateMachine.Current, Is.TypeOf<IdleState>());
            Assert.That(_overlays.GetComponentsInChildren<SpriteRenderer>(), Is.Empty);
        }
    }
}
