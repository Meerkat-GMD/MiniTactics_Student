using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MiniTactics.Lesson12
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _spawner;
        [SerializeField] private PresentationQueue _presentation;

        public static GameManager Instance { get; private set; }

        public event Action<BattleResult> ResultChanged;
        public bool InputBlocked { get; private set; }
        public void SetInputBlocked(bool blocked) => InputBlocked = blocked;

        public BattleResult Result { get; private set; } = BattleResult.Playing;
        public bool IsPlaying => Result == BattleResult.Playing;
        public bool IsPresenting => _presentation != null && _presentation.IsBusy;
        public bool CanShowResult => !IsPlaying && !IsPresenting;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            foreach (Unit unit in _spawner.Units)
            {
                unit.Died += OnUnitDied;
            }
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            foreach (Unit unit in _spawner.Units)
            {
                unit.Died -= OnUnitDied;
            }

            ResultChanged = null;
            Instance = null;
        }

        private void Update()
        {
            if (!InputBlocked && Keyboard.current.rKey.wasPressedThisFrame)
            {
                Restart();
            }
        }

        public void Finish(BattleResult result)
        {
            if (!IsPlaying)
            {
                return;
            }

            Result = result;
            ResultChanged?.Invoke(result);
        }

        public void Restart()
        {
            if (InputBlocked) return;
            CancelPresentation();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void CancelPresentation()
        {
            if (_presentation != null) _presentation.Cancel();
        }

        private void OnUnitDied(Unit deadUnit)
        {
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.Team == deadUnit.Team && unit.IsAlive)
                {
                    return;
                }
            }

            Finish(deadUnit.Team == Team.Enemy ? BattleResult.PlayerWon : BattleResult.PlayerLost);
        }
    }
}
