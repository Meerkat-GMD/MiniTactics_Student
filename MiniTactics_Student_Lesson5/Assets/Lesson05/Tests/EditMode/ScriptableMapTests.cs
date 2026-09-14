using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace MiniTactics.Lesson05.Tests
{
    public sealed class ScriptableMapTests
    {
        private const string Root = "Assets/Lesson05/";
        // Literal snapshot from the released map; catches transposition and lost cells.
        private const string Layout = "PPPPPPPPPPPP\nPPFFFFPPPPPP\nPPFMMFPPPPRP\nPPFPPFPPPPRP\nPPFPPFPPPPRP\nPPPPPFPPPPRP\nPPMMMPPPPPRP\nPPPPPPFFFFRP\nPPPPRRPPPPPP\nPPPPPPPPPPPP";

        [Test]
        public void ShippedMap_PreservesEveryCellAndSharedTerrain()
        {
            MapData map = AssetDatabase.LoadAssetAtPath<MapData>(Root + "Maps/map01.asset");
            Assert.That(map, Is.Not.Null);
            Board board = MapLoader.Load(map);
            Assert.That(board.Cells.Count, Is.EqualTo(120));
            Assert.That(board.Cells.Values.Distinct().Count(), Is.EqualTo(4));
            string[] rows = Layout.Split('\n');
            for (int row = 0; row < 10; row++)
            for (int x = 0; x < 12; x++)
            {
                var cell = new Vector2Int(x, 9 - row);
                TileData expected = MapTestData.TileFor(rows[row][x]);
                Assert.That(map.GetTile(cell), Is.SameAs(expected), cell.ToString());
                Assert.That(board.GetTerrain(cell), Is.SameAs(expected.RuntimeTerrain), cell.ToString());
            }
        }

        [Test]
        public void AuthoredRules_ChangeReachabilityAndRemainSharedWithoutChangingOtherAssets()
        {
            TileData tile = Object.Instantiate(MapTestData.TileFor('P'));
            MapData map = MapTestData.Create("PP");
            try
            {
                var data = new SerializedObject(tile);
                data.FindProperty("_displayName").stringValue = "Mud";
                data.FindProperty("_movementCost").intValue = 4;
                data.ApplyModifiedPropertiesWithoutUndo();
                MapTestData.SetCells(map, tile, tile);
                Board board = MapLoader.Load(map);
                Assert.That(board.GetTerrain(Vector2Int.zero), Is.SameAs(board.GetTerrain(Vector2Int.right)));
                Assert.That(board.GetTerrain(Vector2Int.zero).Name, Is.EqualTo("Mud"));
                Assert.That(MovementRange.Calculate(board, Vector2Int.zero, 3)
                    .TryGetCost(Vector2Int.right, out _), Is.False);
                Assert.That(MovementRange.Calculate(board, Vector2Int.zero, 4)
                    .TryGetCost(Vector2Int.right, out int cost), Is.True);
                Assert.That(cost, Is.EqualTo(4));
                data.FindProperty("_isWalkable").boolValue = false;
                data.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(MapLoader.Load(map).GetTerrain(Vector2Int.right).IsWalkable, Is.False);
                Assert.That(MapTestData.TileFor('P').RuntimeTerrain.MovementCost, Is.EqualTo(1));
            }
            finally { Object.DestroyImmediate(map); Object.DestroyImmediate(tile); }
        }

        [Test]
        public void InvalidLayout_IsRejectedBeforePainting()
        {
            MapData map = MapTestData.Create("PP");
            try
            {
                Assert.That(() => MapLoader.Load(null), Throws.ArgumentNullException);
                var serialized = new SerializedObject(map);
                serialized.FindProperty("_cells").arraySize = 1;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(() => MapLoader.Load(map), Throws.ArgumentException);
                serialized.FindProperty("_cells").arraySize = 2;
                serialized.FindProperty("_cells").GetArrayElementAtIndex(1).objectReferenceValue = null;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(() => MapLoader.Load(map), Throws.ArgumentException);
                Assert.That(() => map.GetTile(new Vector2Int(-1, 0)), Throws.TypeOf<ArgumentOutOfRangeException>());
            }
            finally { Object.DestroyImmediate(map); }
        }

        [Test]
        public void Awake_PaintsAuthoredVisualsWithoutWaitingForFirstClick()
        {
            EditorSceneManager.OpenScene(Root + "Scenes/Lesson05.unity", OpenSceneMode.Single);
            BoardView view = Object.FindAnyObjectByType<BoardView>();
            var serialized = new SerializedObject(view);
            MapData map = AssetDatabase.LoadAssetAtPath<MapData>(Root + "Maps/map01.asset");
            Assert.That(serialized.FindProperty("_mapData").objectReferenceValue, Is.SameAs(map));
            Tilemap tilemap = (Tilemap)serialized.FindProperty("_tilemap").objectReferenceValue;
            tilemap.ClearAllTiles();
            typeof(BoardView).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(view, null);
            Assert.That(view.TileCount, Is.EqualTo(120));
            Assert.That(tilemap.GetTile(new Vector3Int(2, 8, 0)), Is.SameAs(MapTestData.TileFor('F').Tile));
            Assert.That(tilemap.GetTile(new Vector3Int(4, 1, 0)), Is.SameAs(MapTestData.TileFor('R').Tile));
        }
    }
}
