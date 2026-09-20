using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiniTactics.Lesson08
{
    public sealed class ActionMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _skillButton;
        [SerializeField] private Button _waitButton;
        [SerializeField] private Text _skillLabel;

        public event Action AttackClicked;
        public event Action SkillClicked;
        public event Action WaitClicked;

        private void Awake()
        {
            _attackButton.onClick.AddListener(OnAttackClicked);
            _skillButton.onClick.AddListener(OnSkillClicked);
            _waitButton.onClick.AddListener(OnWaitClicked);
            Hide();
        }

        private void OnDestroy()
        {
            _attackButton.onClick.RemoveListener(OnAttackClicked);
            _skillButton.onClick.RemoveListener(OnSkillClicked);
            _waitButton.onClick.RemoveListener(OnWaitClicked);
        }

        public void Show(bool canAttack, bool canSkill, string skillName)
        {
            _panel.SetActive(true);
            _attackButton.interactable = canAttack;
            _skillButton.interactable = canSkill;
            _skillLabel.text = skillName;
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        private void OnAttackClicked() => AttackClicked?.Invoke();
        private void OnSkillClicked() => SkillClicked?.Invoke();
        private void OnWaitClicked() => WaitClicked?.Invoke();
    }
}
