using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public static class MapLoader
    {
        public static Board Parse(string mapText)
        {
            if (string.IsNullOrWhiteSpace(mapText))
            {
                throw new ArgumentException("Map text cannot be empty.", nameof(mapText));
            }

            string normalized = mapText.Replace("\r\n", "\n").TrimEnd('\n', '\r');
            string[] rows = normalized.Split('\n');
            int width = rows[0].Length;

            if (width == 0)
            {
                throw new ArgumentException("Map rows cannot be empty.", nameof(mapText));
            }

            Dictionary<Vector2Int, TerrainType> cells =
                new Dictionary<Vector2Int, TerrainType>();

            for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                string row = rows[rowIndex];
                if (row.Length != width)
                {
                    throw new ArgumentException(
                        $"Map row {rowIndex + 1} must contain {width} cells.",
                        nameof(mapText));
                }

                int y = rows.Length - rowIndex - 1;
                for (int x = 0; x < width; x++)
                {
                    cells.Add(new Vector2Int(x, y), FromSymbol(row[x], x, y));
                }
            }

            return new Board(cells);
        }

        private static TerrainType FromSymbol(char symbol, int x, int y)
        {
            return symbol switch
            {
                'P' => TerrainTypes.Plain,
                'F' => TerrainTypes.Forest,
                'M' => TerrainTypes.Mountain,
                'R' => TerrainTypes.River,
                _ => throw new ArgumentException(
                    $"Unknown terrain symbol '{symbol}' at ({x},{y}).")
            };
        }
    }
}
