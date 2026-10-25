using TMPro;
using UnityEngine;

namespace MiniTactics.Lesson11
{
    public sealed class PhaseHudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _phaseText;
        [SerializeField] private UnityEngine.UI.Button _endTurnButton;

        public void ShowPhase(string text, bool canEndTurn)
        {
            _phaseText.text = text;
            _endTurnButton.interactable = canEndTurn;
        }
    }
}
