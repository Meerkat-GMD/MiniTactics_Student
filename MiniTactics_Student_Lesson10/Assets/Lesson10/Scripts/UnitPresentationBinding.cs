using UnityEngine;
namespace MiniTactics.Lesson10
{
    public sealed class UnitPresentationBinding : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private UnitAnimator _animator;
        private int _lastHp;
        private void OnEnable()
        {
            _unit.HpChanged += OnHpChanged;
            _unit.Died += OnDied;
        }
        private void OnDisable()
        {
            _unit.HpChanged -= OnHpChanged;
            _unit.Died -= OnDied;
        }
        private void Start() => _lastHp = _unit.CurrentHp;
        private void OnHpChanged(Unit unit)
        {
            if (unit.CurrentHp < _lastHp && unit.IsAlive) _animator.PlayHit();
            _lastHp = unit.CurrentHp;
        }
        private void OnDied(Unit unit) => _animator.PlayDeath();
    }
}
