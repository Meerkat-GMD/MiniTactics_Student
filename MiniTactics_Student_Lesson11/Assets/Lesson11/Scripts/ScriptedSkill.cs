using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson11
{
    [CreateAssetMenu(fileName = "ScriptedSkill", menuName = "Mini Tactics/Skills/Scripted Skill")]
    public sealed class ScriptedSkill : Skill
    {
        [SerializeField] private bool _targetsAllies;
        [SerializeField, TextArea(3, 8)] private string _source;

        protected override List<Unit> FindTargets()
        {
            if (!ReadProgram(out _)) return new List<Unit>();
            return _targetsAllies ? WoundedAlliesInRange() : EnemiesInRange();
        }

        protected override void Activate(Unit target)
        {
            if (!ReadProgram(out SkillProgram program)) return;
            int radius = 0;
            foreach (SkillProgram.Instruction instruction in program.Instructions)
            {
                if (instruction.Code == SkillProgram.OpCode.AREA)
                {
                    radius = instruction.Value;
                }
                else
                {
                    Execute(instruction, target, radius);
                }
            }
        }

        private void Execute(SkillProgram.Instruction instruction, Unit center, int radius)
        {
            bool heal = instruction.Code == SkillProgram.OpCode.HEAL;
            Team team = heal ? User.Team : (User.Team == Team.Player ? Team.Enemy : Team.Player);
            foreach (Unit target in Turns.GetUnitsInRange(center, radius, team))
            {
                switch (instruction.Code)
                {
                    case SkillProgram.OpCode.DAMAGE:
                        Strike(target, instruction.Value);
                        break;
                    case SkillProgram.OpCode.HEAL:
                        Restore(target, instruction.Value);
                        break;
                    case SkillProgram.OpCode.PUSH:
                        Turns.PushAway(User, target);
                        break;
                }
            }
        }

        private bool ReadProgram(out SkillProgram program)
        {
            if (SkillProgram.TryParse(_source, out program, out string error)) return true;
            Debug.LogError($"{name}: {error}", this);
            return false;
        }
    }
}
