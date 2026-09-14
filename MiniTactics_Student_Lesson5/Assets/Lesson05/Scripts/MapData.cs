using System;
using UnityEngine;

namespace MiniTactics.Lesson05
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Mini Tactics/Map Data")]
    public sealed class MapData : ScriptableObject
    {
        [SerializeField, Min(1)] private int _width = 12;
        [SerializeField, Min(1)] private int _height = 10;
        [Tooltip("Top row first, left to right. Index = (Height - 1 - y) * Width + x.")]
        [SerializeField] private TileData[] _cells = new TileData[120];

        public int Width => _width;
        public int Height => _height;

        public void Validate()
        {
            if (_width < 1 || _height < 1 || _cells == null ||
                _cells.LongLength != (long)_width * _height)
                throw new ArgumentException($"Map '{name}' must contain Width * Height cells.");
            foreach (TileData tile in _cells)
            {
                if (tile == null) throw new ArgumentException($"Map '{name}' contains an unassigned tile.");
                tile.Validate();
            }
        }

        public TileData GetTile(Vector2Int cell)
        {
            if (cell.x < 0 || cell.x >= _width || cell.y < 0 || cell.y >= _height)
                throw new ArgumentOutOfRangeException(nameof(cell));
            return _cells[(_height - 1 - cell.y) * _width + cell.x];
        }
    }
}
