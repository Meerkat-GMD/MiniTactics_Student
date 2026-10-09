using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson12
{
    [CreateAssetMenu(menuName = "Mini Tactics/Skills/Heal")]
    public sealed class HealSkill : Skill
    {
        [SerializeField, Min(1)] private int _healAmount = 5;

        protected override List<Unit> FindTargets() => WoundedAlliesInRange();
        protected override void Activate(Unit target) => Restore(target, _healAmount);
    }
}
