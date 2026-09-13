using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson03
{
    public sealed class Board
    {
        private readonly Dictionary<Vector2Int, TerrainType> _cells =
            new Dictionary<Vector2Int, TerrainType>();

        public Board(IReadOnlyDictionary<Vector2Int, TerrainType> cells)
        {
        }

        public IReadOnlyDictionary<Vector2Int, TerrainType> Cells => _cells;

        public bool Contains(Vector2Int cell)
        {
            return false;
        }

        public TerrainType GetTerrain(Vector2Int cell)
        {
            return null;
        }
    }
}
