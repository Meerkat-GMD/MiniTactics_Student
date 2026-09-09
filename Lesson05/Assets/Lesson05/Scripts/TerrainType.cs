using System;

namespace MiniTactics.Lesson05
{
    public sealed class TerrainType
    {
        public TerrainType(string name, int movementCost, bool isWalkable)
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

            Name = name;
            MovementCost = movementCost;
            IsWalkable = isWalkable;
        }

        public string Name { get; }
        public int MovementCost { get; }
        public bool IsWalkable { get; }
    }

    public static class TerrainTypes
    {
        public static readonly TerrainType Plain = new TerrainType("평지", 1, true);
        public static readonly TerrainType Forest = new TerrainType("숲", 2, true);
        public static readonly TerrainType Mountain = new TerrainType("산", 3, true);
        public static readonly TerrainType River = new TerrainType("강", 1, false);
    }
}
