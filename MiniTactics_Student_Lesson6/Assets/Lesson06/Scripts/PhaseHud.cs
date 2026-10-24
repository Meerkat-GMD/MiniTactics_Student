using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class PhaseHud : MonoBehaviour
    {
        [SerializeField] private PhaseHudView _view;

        private void Update()
        {
            bool player = GameManager.Instance.Turns.Phase == BattlePhase.Player;
            _view.ShowPhase(player ? "아군 차례" : "적군 차례", player);
        }

        public void OnRestartClicked() => GameManager.Instance.Restart();
    }
}
