using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace MiniTactics.Lesson06
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public BattleResult Result { get; private set; } = BattleResult.Playing;
        public bool IsPlaying => Result == BattleResult.Playing;

        [SerializeField] private List<Unit> _combatUnits = new List<Unit>();
        private readonly List<Unit> _observedUnits = new List<Unit>();

        private void Start()
        {
            ObserveUnits(_combatUnits);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }

                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            StopObservingUnits();

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (Application.isPlaying && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                RestartCurrentScene();
        }

        public void TryFinish(BattleResult result)
        {
            if (!IsPlaying || result == BattleResult.Playing)
            {
                return;
            }

            Result = result;
        }

        public void ObserveUnits(IEnumerable<Unit> units)
        {
            StopObservingUnits();
            // 학습 과제: 유효한 전투 유닛을 중복 없이 관찰한다.
        }

        public void StopObservingUnits()
        {
            // 학습 과제: 기록을 비우기 전에 기존 관찰 연결을 해제한다.
            _observedUnits.Clear();
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

        private void HandleObservedUnitDied(Unit unit)
        {
            // 학습 과제: 사망한 팀의 생존 전투 유닛 수로 전멸 결과를 판단한다.
        }

        private int CountAliveCombatUnits(Team team)
        {
            // 학습 과제: 활성 상태, 생존, 턴 참여 여부와 팀을 구분한다.
            return 0;
        }
    }
}
