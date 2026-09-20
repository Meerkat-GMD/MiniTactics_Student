using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MiniTactics.Lesson07
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private UnitSpawner _spawner;

        public static GameManager Instance { get; private set; }

        public BattleResult Result { get; private set; } = BattleResult.Playing;
        public bool IsPlaying => Result == BattleResult.Playing;

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

            Instance = null;
        }

        private void Update()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
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
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
