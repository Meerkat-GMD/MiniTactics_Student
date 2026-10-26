using System;

namespace MiniTactics.Lesson00
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Unit swordsman = new Unit("검사", 30, 6);
            string[] effects = { "Poison", "Burn", "Regen" };

            for (int turn = 1; turn <= 3; turn++)
            {
                Console.WriteLine($"--- {turn}턴 ---");
                foreach (string type in effects)
                {
                    ApplyEffect(type, swordsman);
                    Console.WriteLine($"{GetEffectName(type)} → {swordsman.Name} HP {swordsman.CurrentHp}/{swordsman.MaxHp}");
                }
            }
        }

        private static string GetEffectName(string type)
        {
            switch (type)
            {
                case "Poison":
                    return "독";
                case "Burn":
                    return "화상";
                case "Regen":
                    return "재생";
                default:
                    return "알 수 없음";
            }
        }

        private static void ApplyEffect(string type, Unit target)
        {
            switch (type)
            {
                case "Poison":
                    target.TakeDamage(2);
                    break;
                case "Burn":
                    target.TakeDamage(3);
                    break;
                case "Regen":
                    target.Heal(4);
                    break;
            }
        }
    }
}
