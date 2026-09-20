using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson04
{
    public static class MapLoader
    {
        public static Board Load(MapData mapData)
        {
            Dictionary<Vector2Int, TileData> cells = new Dictionary<Vector2Int, TileData>();
            for (int y = 0; y < mapData.Height; y++)
            {
                for (int x = 0; x < mapData.Width; x++)
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    cells.Add(cell, mapData.GetTile(cell));
                }
            }

            return new Board(cells);
        }
    }
}
