using UnityEngine;

namespace MiniTactics.Lesson09
{
    public sealed class PhaseHud : MonoBehaviour
    {
        [SerializeField] private PhaseHudView _view;

        private void Update()
        {
            BattleResult result = GameManager.Instance.Result;
            if (result != BattleResult.Playing)
            {
                _view.ShowPhase(result == BattleResult.PlayerWon ? "승리" : "패배", false);
                return;
            }
            bool player = GameManager.Instance.Turns.Phase == BattlePhase.Player;
            _view.ShowPhase(player ? "아군 차례" : "적군 차례", player);
        }

        public void OnRestartClicked() => GameManager.Instance.Restart();
    }
}
