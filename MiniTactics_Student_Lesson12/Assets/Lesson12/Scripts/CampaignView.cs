using System;
using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class CampaignView : MonoBehaviour
    {
        public enum Command
        {
            NewGame, Continue, Mute, Quit, Next, Retry, Title, Help,
            CloseHelp, ConfirmReset, CancelReset, StartAgain, EndTurn
        }

        [Serializable]
        public struct ButtonBinding
        {
            public Command Command;
            public UnityEngine.UI.Button Button;
        }

        [SerializeField] private GameObject _title;
        [SerializeField] private GameObject _battle;
        [SerializeField] private GameObject _result;
        [SerializeField] private GameObject _ending;
        [SerializeField] private GameObject _reset;
        [SerializeField] private GameObject _help;
        [SerializeField] private UnityEngine.UI.Text _stageName;
        [SerializeField] private UnityEngine.UI.Text _resultLabel;
        [SerializeField] private UnityEngine.UI.Text[] _muteLabels;
        [SerializeField] private UnityEngine.UI.Button _continue;
        [SerializeField] private UnityEngine.UI.Button _next;
        [SerializeField] private UnityEngine.UI.Button _retryResult;
        [SerializeField] private ButtonBinding[] _buttons;

        public event Action<Command> Clicked;
        public bool IsModalOpen => _reset.activeSelf || _help.activeSelf;
        public bool IsResetOpen => _reset.activeSelf;

        private void Awake()
        {
            foreach (ButtonBinding binding in _buttons)
            {
                Command command = binding.Command;
                binding.Button.onClick.AddListener(() => Clicked?.Invoke(command));
            }
            HideScreens();
        }

        public void ShowTitle(bool hasProgress, bool muted)
        {
            HideScreens();
            _title.SetActive(true);
            _continue.interactable = hasProgress;
            SetMuted(muted);
        }

        public void ShowBattle(string stageName, bool muted)
        {
            HideScreens();
            _battle.SetActive(true);
            _stageName.text = stageName;
            SetMuted(muted);
        }

        public void ShowEnding(bool muted)
        {
            HideScreens();
            _ending.SetActive(true);
            SetMuted(muted);
        }

        public void ShowResult(bool won)
        {
            _result.SetActive(true);
            _resultLabel.text = won ? "승리" : "패배";
            _next.gameObject.SetActive(won);
            _retryResult.gameObject.SetActive(!won);
        }

        public void ShowResetConfirmation(bool show) => _reset.SetActive(show);
        public void ShowHelp(bool show) => _help.SetActive(show);

        public void SetMuted(bool muted)
        {
            foreach (UnityEngine.UI.Text label in _muteLabels)
                label.text = muted ? "소리 켜기" : "음소거";
        }

        private void HideScreens()
        {
            foreach (GameObject screen in new[] { _title, _battle, _result, _ending, _reset, _help })
                screen.SetActive(false);
        }

        private void OnDestroy() => Clicked = null;
    }
}
