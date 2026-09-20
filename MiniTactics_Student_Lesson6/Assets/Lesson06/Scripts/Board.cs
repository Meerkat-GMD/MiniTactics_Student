using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class Board
    {
        private readonly Dictionary<Vector2Int, TerrainType> _cells;

        public Board(IReadOnlyDictionary<Vector2Int, TerrainType> cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            _cells = new Dictionary<Vector2Int, TerrainType>(cells);
        }

        public IReadOnlyDictionary<Vector2Int, TerrainType> Cells => _cells;

        public bool Contains(Vector2Int cell)
        {
            return _cells.ContainsKey(cell);
        }

        public TerrainType GetTerrain(Vector2Int cell)
        {
            return _cells[cell];
        }
    }
}
