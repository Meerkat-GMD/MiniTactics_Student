using System;

namespace MiniTactics.Lesson00
{
    public sealed class Unit
    {
        public Unit(string name, int maxHp, int attackPower)
        {
            Name = name;
            MaxHp = maxHp;
            AttackPower = attackPower;
            CurrentHp = maxHp;
        }

        public string Name { get; }
        public int MaxHp { get; }
        public int AttackPower { get; }
        public int CurrentHp { get; private set; }
        public bool IsAlive => CurrentHp > 0;

        public void TakeDamage(int damage)
        {
            if (!IsAlive || damage <= 0) return;
            CurrentHp = Math.Max(0, CurrentHp - damage);
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0) return;
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
        }
    }
}
