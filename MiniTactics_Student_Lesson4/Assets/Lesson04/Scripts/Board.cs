using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class Board
    {
        private readonly Dictionary<Vector2Int, TileData> _cells;

        public Board(Dictionary<Vector2Int, TileData> cells)
        {
            _cells = cells;
        }

        public bool Contains(Vector2Int cell)
        {
            return _cells.ContainsKey(cell);
        }

        public TileData GetTile(Vector2Int cell)
        {
            return _cells[cell];
        }
    }
}
