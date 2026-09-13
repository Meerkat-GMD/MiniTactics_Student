using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson05
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public BattleResult Result { get; private set; } = BattleResult.Playing;
        public bool IsPlaying => Result == BattleResult.Playing;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            if (Application.isPlaying && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                RestartCurrentScene();
        }

        public void TryFinish(BattleResult result)
        {
        }

        public void RestartCurrentScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex < 0)
            {
                Debug.LogWarning("Cannot restart the active scene because it is not in Build Settings.");
                return;
            }

            SceneManager.LoadScene(activeScene.buildIndex);
        }
    }
}
