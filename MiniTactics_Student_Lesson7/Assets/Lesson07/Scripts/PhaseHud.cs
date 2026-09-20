using UnityEngine;
using UnityEngine.UI;

namespace MiniTactics.Lesson07
{
    public sealed class PhaseHud : MonoBehaviour
    {
        [SerializeField] private Text _phaseText;
        [SerializeField] private TurnManager _turnManager;

        private void Update()
        {
            BattleResult result = GameManager.Instance.Result;
            if (result == BattleResult.PlayerWon)
            {
                _phaseText.text = "VICTORY";
            }
            else if (result == BattleResult.PlayerLost)
            {
                _phaseText.text = "DEFEAT";
            }
            else
            {
                _phaseText.text = _turnManager.Phase == BattlePhase.Player ? "PLAYER PHASE" : "ENEMY PHASE";
            }
        }
    }
}
