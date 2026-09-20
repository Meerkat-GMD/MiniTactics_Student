using UnityEngine;

namespace MiniTactics.Lesson08
{
    public static class CombatCalculator
    {
        public static int CalculateDamage(int attackPower, int defense, int terrainDefense)
        {
            return Mathf.Max(1, attackPower - defense - terrainDefense);
        }
    }
}
