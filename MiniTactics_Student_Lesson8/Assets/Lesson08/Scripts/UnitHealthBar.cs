using UnityEngine;

namespace MiniTactics.Lesson08
{
    public sealed class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private Transform _fill;

        private void OnEnable()
        {
            _unit.HpChanged += Refresh;
        }

        private void OnDisable()
        {
            _unit.HpChanged -= Refresh;
        }

        private void Refresh(Unit unit)
        {
            Vector3 scale = _fill.localScale;
            scale.x = (float)unit.CurrentHp / unit.MaxHp;
            _fill.localScale = scale;
        }
    }
}
