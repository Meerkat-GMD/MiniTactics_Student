using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MiniTactics.Lesson12
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private TurnManager _turns;
        [SerializeField] private UnitSpawner _spawner;
        [SerializeField] private PresentationQueue _presentation;
        public bool IsPresenting
        {
            get
            {
                if (!_presentation.isActiveAndEnabled)
                    throw new System.InvalidOperationException("GameManager requires an enabled PresentationQueue in this scene.");
                return _presentation.IsBusy;
            }
        }
        public bool CanShowResult => !IsPlaying && !IsPresenting;
        public TurnManager Turns => _turns;
        public BattleResult Result { get; private set; } = BattleResult.Playing;
        public bool IsPlaying => Result == BattleResult.Playing;
        public bool InputBlocked { get; private set; }
        public event System.Action<BattleResult> ResultChanged;

        public void SetInputBlocked(bool blocked) => InputBlocked = blocked;
        public void CancelPresentation() => _presentation.Cancel();

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
            foreach (Unit unit in _spawner.Units) unit.Died += OnUnitDied;
        }

        private void OnUnitDied(Unit deadUnit)
        {
            bool playerAlive = false;
            bool enemyAlive = false;
            foreach (Unit unit in _spawner.Units)
            {
                if (!unit.IsAlive) continue;
                if (unit.Team == Team.Player) playerAlive = true;
                else enemyAlive = true;
            }
            if (!playerAlive) Finish(BattleResult.PlayerLost);
            else if (!enemyAlive) Finish(BattleResult.PlayerWon);
        }

        public void Finish(BattleResult result)
        {
            if (!IsPlaying) return;
            Result = result;
            ResultChanged?.Invoke(result);
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            foreach (Unit unit in _spawner.Units) unit.Died -= OnUnitDied;
            ResultChanged = null;
            Instance = null;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) Restart();
        }

        public void Restart()
        {
            if (InputBlocked) return;
            _presentation.Cancel();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
