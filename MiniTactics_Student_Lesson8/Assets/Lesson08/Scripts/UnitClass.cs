using UnityEngine;

namespace MiniTactics.Lesson08
{
    [CreateAssetMenu(menuName = "Mini Tactics/Unit Class")]
    public sealed class UnitClass : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField, Min(1)] private int _maxHp;
        [SerializeField, Min(0)] private int _attackPower;
        [SerializeField, Min(0)] private int _defense;
        [SerializeField, Min(0)] private int _moveBudget;
        [SerializeField, Min(1)] private int _attackRange = 1;
        [SerializeField] private Sprite _sprite;

        public string DisplayName => _displayName;
        public int MaxHp => _maxHp;
        public int AttackPower => _attackPower;
        public int Defense => _defense;
        public int MoveBudget => _moveBudget;
        public int AttackRange => _attackRange;
        public Sprite Sprite => _sprite;
    }
}
