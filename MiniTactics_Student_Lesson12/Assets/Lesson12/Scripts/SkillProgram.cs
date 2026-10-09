using System;
using System.Collections.Generic;

namespace MiniTactics.Lesson12
{
    public sealed class SkillProgram
    {
        public enum OpCode { AREA, DAMAGE, HEAL, PUSH }

        public readonly struct Instruction
        {
            public Instruction(OpCode code, int value)
            {
                Code = code;
                Value = value;
            }

            public OpCode Code { get; }
            public int Value { get; }
        }

        public IReadOnlyList<Instruction> Instructions { get; }

        private SkillProgram(List<Instruction> instructions)
        {
            Instructions = instructions.AsReadOnly();
        }

        public static bool TryParse(string source, out SkillProgram program, out string error)
        {
            program = null;
            error = "Program is empty.";
            if (string.IsNullOrWhiteSpace(source)) return false;
            var instructions = new List<Instruction>();
            string[] lines = source.Split('\n');
            for (int index = 0; index < lines.Length; index++)
            {
                if (string.IsNullOrWhiteSpace(lines[index])) continue;
                if (!TryInstruction(lines[index], out Instruction instruction, out error))
                {
                    error = $"Line {index + 1}: {error}";
                    return false;
                }

                instructions.Add(instruction);
            }

            program = new SkillProgram(instructions);
            error = string.Empty;
            return true;
        }

        private static bool TryInstruction(string line, out Instruction instruction, out string error)
        {
            instruction = default;
            error = "Expected AREA 0..3, DAMAGE/HEAL 0..99, or PUSH 1.";
            string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return false;
            if (!Enum.TryParse(parts[0], out OpCode code)) return false;
            if (!Enum.IsDefined(typeof(OpCode), code) || code.ToString() != parts[0]) return false;
            if (!int.TryParse(parts[1], out int value)) return false;
            int maximum = code == OpCode.AREA ? 3 : 99;
            if (value < 0 || value > maximum) return false;
            if (code == OpCode.PUSH && value != 1) return false;
            instruction = new Instruction(code, value);
            error = string.Empty;
            return true;
        }
    }
}
