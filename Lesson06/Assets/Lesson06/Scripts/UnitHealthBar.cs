using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private Transform _fill;

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Bind(Unit unit)
        {
            Unsubscribe();
            _unit = unit;
            Subscribe();
            Refresh();
        }

        private void Subscribe()
        {
            // 학습 과제: 활성 수명과 중복 등록을 고려하여 Unit을 관찰한다.
        }

        private void Unsubscribe()
        {
            // 학습 과제: 이전에 등록한 콜백을 정확히 해제한다.
        }

        private void HandleHpChanged(Unit unit)
        {
            // 학습 과제: 바인딩한 Unit의 HP 변화에 반응한다.
        }

        private void HandleDied(Unit unit)
        {
            // 학습 과제: 바인딩한 Unit의 사망에 반응한다.
        }

        private void Refresh()
        {
            if (_unit == null || _fill == null)
            {
                return;
            }

            Vector3 scale = _fill.localScale;
            scale.x = _unit.MaxHp > 0
                ? Mathf.Clamp01(_unit.CurrentHp / (float)_unit.MaxHp)
                : 0f;
            _fill.localScale = scale;
        }
    }
}
