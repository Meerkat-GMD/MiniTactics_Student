using System;
using UnityEditor;
using UnityEngine;

namespace MiniTactics.Lesson04.Tests
{
    internal static class MapTestData
    {
        internal static TileData TileFor(char symbol)
        {
            string name = symbol switch { 'P' => "Plain", 'F' => "Forest", 'M' => "Mountain", 'R' => "River", _ => throw new ArgumentException() };
            return AssetDatabase.LoadAssetAtPath<TileData>($"Assets/Lesson04/Maps/{name}.asset");
        }

        internal static MapData Create(string text)
        {
            string[] rows = text.Split('\n');
            MapData map = ScriptableObject.CreateInstance<MapData>();
            var serialized = new SerializedObject(map);
            serialized.FindProperty("_width").intValue = rows[0].Length;
            serialized.FindProperty("_height").intValue = rows.Length;
            var cells = serialized.FindProperty("_cells");
            cells.arraySize = rows[0].Length * rows.Length;
            for (int row = 0; row < rows.Length; row++)
            for (int x = 0; x < rows[0].Length; x++)
                cells.GetArrayElementAtIndex(row * rows[0].Length + x).objectReferenceValue = TileFor(rows[row][x]);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return map;
        }

        internal static void SetCells(MapData map, params TileData[] tiles)
        {
            var serialized = new SerializedObject(map);
            var cells = serialized.FindProperty("_cells");
            cells.arraySize = tiles.Length;
            for (int i = 0; i < tiles.Length; i++) cells.GetArrayElementAtIndex(i).objectReferenceValue = tiles[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
