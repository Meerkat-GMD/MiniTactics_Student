using NUnit.Framework;
using System.Reflection;
using UnityEngine;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class GameManagerTests
    {
        private GameObject _firstObject;
        private GameObject _secondObject;

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_secondObject);
            Object.DestroyImmediate(_firstObject);
        }

        [Test]
        public void AddComponent_DoesNotRunSingletonLifecycleInEditMode()
        {
            _firstObject = new GameObject("Game Manager");
            _firstObject.AddComponent<GameManager>();

            Assert.That(GameManager.Instance == null, Is.True);
        }

        [Test]
        public void Awake_FirstInstanceBecomesTheSingleton()
        {
            GameManager first = CreateManager("First Game Manager", out _firstObject);

            Assert.That(GameManager.Instance, Is.SameAs(first));
        }

        [Test]
        public void Awake_DuplicateNeverReplacesTheFirstInstance()
        {
            GameManager first = CreateManager("First Game Manager", out _firstObject);
            _secondObject = new GameObject("Duplicate Game Manager");
            GameManager duplicate = _secondObject.AddComponent<GameManager>();
            InvokeLifecycle(duplicate, "Awake");

            Assert.That(GameManager.Instance, Is.SameAs(first));
        }

        [Test]
        public void Awake_DuplicateDestructionKeepsTheFirstInstanceAssigned()
        {
            GameManager first = CreateManager("First Game Manager", out _firstObject);
            _secondObject = new GameObject("Duplicate Game Manager");
            GameManager duplicate = _secondObject.AddComponent<GameManager>();
            InvokeLifecycle(duplicate, "Awake");

            Assert.That(duplicate == null, Is.True);
            Assert.That(GameManager.Instance, Is.SameAs(first));
        }

        [Test]
        public void OnDestroy_ValidInstanceClearsTheSingleton()
        {
            GameManager manager = CreateManager("Game Manager", out _firstObject);

            Object.DestroyImmediate(_firstObject);
            _firstObject = null;

            Assert.That(GameManager.Instance == null, Is.True);
        }

        [Test]
        public void Result_StartsPlayingAndIsPlayingIsTrue()
        {
            GameManager manager = CreateManager("Game Manager", out _firstObject);

            Assert.That(manager.Result, Is.EqualTo(BattleResult.Playing));
            Assert.That(manager.IsPlaying, Is.True);
        }

        [Test]
        public void TryFinish_IgnoresPlaying()
        {
            GameManager manager = CreateManager("Game Manager", out _firstObject);

            manager.TryFinish(BattleResult.Playing);

            Assert.That(manager.Result, Is.EqualTo(BattleResult.Playing));
            Assert.That(manager.IsPlaying, Is.True);
        }

        [Test]
        public void TryFinish_SetsTheFirstTerminalResultOnly()
        {
            GameManager manager = CreateManager("Game Manager", out _firstObject);

            manager.TryFinish(BattleResult.PlayerWon);
            manager.TryFinish(BattleResult.PlayerLost);

            Assert.That(manager.Result, Is.EqualTo(BattleResult.PlayerWon));
            Assert.That(manager.IsPlaying, Is.False);
        }

        private static GameManager CreateManager(string name, out GameObject gameObject)
        {
            gameObject = new GameObject(name);
            GameManager manager = gameObject.AddComponent<GameManager>();
            InvokeLifecycle(manager, "Awake");
            return manager;
        }

        private static void InvokeLifecycle(GameManager manager, string methodName)
        {
            MethodInfo method = typeof(GameManager).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(manager, null);
        }
    }
}
