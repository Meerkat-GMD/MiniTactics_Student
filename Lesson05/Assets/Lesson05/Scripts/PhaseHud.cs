using UnityEngine;
using UnityEngine.UI;

namespace MiniTactics.Lesson05
{
    public sealed class PhaseHud : MonoBehaviour
    {
        [SerializeField] private Text _phaseText;
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private GameManager _gameManager;

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Bind(TurnManager turnManager, GameManager gameManager = null)
        {
            Unsubscribe();
            _turnManager = turnManager;
            _gameManager = gameManager;
            Subscribe();
            Refresh();
        }

        public void Refresh()
        {
            if (_phaseText == null)
            {
                return;
            }

            BattleResult result = _gameManager != null
                ? _gameManager.Result
                : _turnManager != null ? _turnManager.Result : BattleResult.Playing;
            if (result == BattleResult.PlayerWon)
            {
                _phaseText.text = "VICTORY";
                return;
            }

            if (result == BattleResult.PlayerLost)
            {
                _phaseText.text = "DEFEAT";
                return;
            }

            _phaseText.text = _turnManager == null
                ? string.Empty
                : _turnManager.Phase == BattlePhase.Player ? "PLAYER PHASE"
                : _turnManager.Phase == BattlePhase.Enemy ? "ENEMY PHASE"
                : "BATTLE FINISHED";
        }

        private void Subscribe()
        {
            if (_turnManager != null)
            {
                _turnManager.PhaseChanged += OnPhaseChanged;
            }
        }

        private void Unsubscribe()
        {
            if (_turnManager != null)
            {
                _turnManager.PhaseChanged -= OnPhaseChanged;
            }
        }

        private void OnPhaseChanged(BattlePhase phase)
        {
            Refresh();
        }
    }
}
