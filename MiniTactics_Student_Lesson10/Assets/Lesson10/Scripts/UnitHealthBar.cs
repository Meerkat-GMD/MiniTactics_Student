using UnityEngine;
namespace MiniTactics.Lesson10
{
    public sealed class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private HealthBarView _view;
        private void OnEnable() => _unit.HpChanged += Refresh;
        private void OnDisable() => _unit.HpChanged -= Refresh;
        private void Start() => Refresh(_unit);
        private void Refresh(Unit unit) => _view.Show(unit.CurrentHp, unit.MaxHp);
    }
}
