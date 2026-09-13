using System;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public static class CombatCalculator
    {
        public static int CalculateDamage(Unit attacker, Unit defender, TerrainType terrain)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException(nameof(attacker));
            }

            if (defender == null)
            {
                throw new ArgumentNullException(nameof(defender));
            }

            if (terrain == null)
            {
                throw new ArgumentNullException(nameof(terrain));
            }

            if (!attacker.IsAlive || !defender.IsAlive)
            {
                throw new InvalidOperationException("Dead units cannot calculate combat damage.");
            }

            // 학습 과제: 방어자의 지형과 두 유닛의 능력치로 피해를 계산한다.
            return 0;
        }
    }
}
