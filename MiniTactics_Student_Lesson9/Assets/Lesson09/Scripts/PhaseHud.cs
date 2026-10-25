using UnityEngine;

namespace MiniTactics.Lesson09
{
    public sealed class PhaseHud : MonoBehaviour
    {
        [SerializeField] private PhaseHudView _view;

        private void Update()
        {
            bool player = GameManager.Instance.Turns.Phase == BattlePhase.Player;
            BattleResult result = GameManager.Instance.Result;
            string text = result == BattleResult.Playing ? (player ? "아군 차례" : "적군 차례") :
                (result == BattleResult.PlayerWon ? "승리" : "패배");
            _view.ShowPhase(text, player && GameManager.Instance.IsPlaying);
        }

        public void OnRestartClicked() => GameManager.Instance.Restart();
    }
}
