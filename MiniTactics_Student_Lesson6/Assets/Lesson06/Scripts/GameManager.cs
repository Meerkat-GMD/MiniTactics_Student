using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MiniTactics.Lesson06
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private TurnManager _turns;
        public TurnManager Turns => _turns;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) Restart();
        }

        public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
