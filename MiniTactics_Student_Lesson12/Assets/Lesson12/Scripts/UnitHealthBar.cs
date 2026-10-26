using UnityEngine;
namespace MiniTactics.Lesson12
{
    public sealed class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private HealthBarView _view;
        private bool _automaticRefresh = true;
        public void Show(int hp, int maxHp) => _view.Show(hp, maxHp);
        private void OnEnable() => _unit.HpChanged += Refresh;
        private void OnDisable() => _unit.HpChanged -= Refresh;
        private void Start() => Refresh(_unit);
        public void SetAutomaticRefresh(bool automatic)
        {
            _automaticRefresh = automatic;
            if (automatic) Refresh(_unit);
        }
        private void Refresh(Unit unit)
        {
            if (_automaticRefresh) Show(unit.CurrentHp, unit.MaxHp);
        }
    }
}
