using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson10
{
    [CreateAssetMenu(menuName = "Mini Tactics/Skills/Power Strike")]
    public sealed class PowerStrikeSkill : Skill
    {
        [SerializeField, Min(0)] private int _bonusDamage = 4;

        protected override List<Unit> FindTargets() => EnemiesInRange();
        protected override void Activate(Unit target) => Strike(target, _bonusDamage);
    }
}
