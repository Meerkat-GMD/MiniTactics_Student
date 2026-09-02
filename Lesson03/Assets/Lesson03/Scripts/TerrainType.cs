namespace MiniTactics.Lesson03
{
    public sealed class TerrainType
    {
        public TerrainType(string name, int movementCost, bool isWalkable)
        {
        }

        public string Name => string.Empty;
        public int MovementCost => 1;
        public bool IsWalkable => true;
    }

    public static class TerrainTypes
    {
        public static readonly TerrainType Plain = null;
        public static readonly TerrainType Forest = null;
        public static readonly TerrainType Mountain = null;
        public static readonly TerrainType River = null;
    }
}
