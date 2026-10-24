using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class Board
    {
        private readonly Dictionary<Vector2Int, TileData> _cells;

        public Board(IReadOnlyDictionary<Vector2Int, TileData> cells)
        {
            _cells = new Dictionary<Vector2Int, TileData>(cells);
        }

        public IReadOnlyDictionary<Vector2Int, TileData> Cells => _cells;
        public bool Contains(Vector2Int cell) => _cells.ContainsKey(cell);
        public TileData GetTile(Vector2Int cell) => _cells[cell];
    }
}
