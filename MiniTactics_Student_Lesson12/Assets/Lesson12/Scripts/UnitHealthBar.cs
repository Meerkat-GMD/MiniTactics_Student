using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private Transform _fill;
        private bool _automaticRefresh = true;

        private void OnEnable()
        {
            _unit.HpChanged += Refresh;
        }

        private void OnDisable()
        {
            _unit.HpChanged -= Refresh;
        }

        public void SetAutomaticRefresh(bool enabled)
        {
            _automaticRefresh = enabled;
            if (enabled) Show(_unit.CurrentHp, _unit.MaxHp);
        }

        private void Refresh(Unit unit)
        {
            if (_automaticRefresh) Show(unit.CurrentHp, unit.MaxHp);
        }

        public void Show(int hp, int maxHp)
        {
            Vector3 scale = _fill.localScale;
            scale.x = (float)hp / maxHp;
            _fill.localScale = scale;
        }
    }
}
