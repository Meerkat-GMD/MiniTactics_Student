using System;

namespace MiniTactics.Lesson06
{
    public sealed class TerrainType
    {
        public TerrainType(string name, int movementCost, bool isWalkable, int defenseBonus = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Terrain needs a name.", nameof(name));
            }

            if (movementCost <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(movementCost),
                    "Movement cost must be positive.");
            }

            if (defenseBonus < 0) throw new ArgumentOutOfRangeException(nameof(defenseBonus));
            DefenseBonus = defenseBonus;
            Name = name;
            MovementCost = movementCost;
            IsWalkable = isWalkable;
        }

        public int DefenseBonus { get; }
        public string Name { get; }
        public int MovementCost { get; }
        public bool IsWalkable { get; }
    }

    public static class TerrainTypes
    {
        public static readonly TerrainType Plain = new TerrainType("평지", 1, true);
        public static readonly TerrainType Forest = new TerrainType("숲", 2, true, 1);
        public static readonly TerrainType Mountain = new TerrainType("산", 3, true, 2);
        public static readonly TerrainType River = new TerrainType("강", 1, false);
    }
}
