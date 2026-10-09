using UnityEngine;
using UnityEngine.UI;

namespace MiniTactics.Lesson12
{
    public sealed class PhaseHud : MonoBehaviour
    {
        [SerializeField] private Text _phaseText;
        [SerializeField] private TurnManager _turnManager;

        private void Update()
        {
            BattleResult result = GameManager.Instance.Result;
            if (GameManager.Instance.CanShowResult && result == BattleResult.PlayerWon)
            {
                _phaseText.text = "승리";
            }
            else if (GameManager.Instance.CanShowResult && result == BattleResult.PlayerLost)
            {
                _phaseText.text = "패배";
            }
            else
            {
                _phaseText.text = _turnManager.Phase == BattlePhase.Player ? "아군 차례" : "적 차례";
            }
        }
    }
}
