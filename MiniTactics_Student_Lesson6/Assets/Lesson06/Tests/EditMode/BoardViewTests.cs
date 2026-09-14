using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class BoardViewTests
    {
        private GameObject _boardObject;
        private GameObject _gridObject;
        private GameObject _tilemapObject;
        private Tilemap _tilemap;
        private Tile _testTile;
        private MapData _mapData;
        private BoardView _board;
        private readonly List<GameObject> _unitObjects = new List<GameObject>();

        [SetUp]
        public void SetUp()
        {
            _boardObject = new GameObject("Board");
            _gridObject = new GameObject("Grid", typeof(Grid));
            _gridObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
            _tilemapObject = new GameObject("Ground Tilemap", typeof(Tilemap), typeof(TilemapRenderer));
            _tilemapObject.transform.SetParent(_gridObject.transform, false);
            _tilemap = _tilemapObject.GetComponent<Tilemap>();
            _board = _boardObject.AddComponent<BoardView>();

            SerializedObject serializedBoard = new SerializedObject(_board);
            serializedBoard.FindProperty("_tilemap").objectReferenceValue = _tilemap;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();

            _testTile = ScriptableObject.CreateInstance<Tile>();
            for (int y = -3; y <= 6; y++)
            {
                for (int x = -1; x <= 10; x++)
                {
                    _tilemap.SetTile(new Vector3Int(x, y, 0), _testTile);
                }
            }

            _tilemap.SetTile(new Vector3Int(2, 1, 0), null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_boardObject);
            Object.DestroyImmediate(_gridObject);
            Object.DestroyImmediate(_testTile);
            Object.DestroyImmediate(_mapData);

            foreach (GameObject unitObject in _unitObjects)
            {
                Object.DestroyImmediate(unitObject);
            }
        }

        [Test]
        public void TileCount_CountsOnlyActuallyPaintedGroundCells()
        {
            Assert.That(_board.TileCount, Is.EqualTo(119));
            Assert.That(_board.Width, Is.EqualTo(12));
            Assert.That(_board.Height, Is.EqualTo(10));
        }

        [TestCase(-1, -3, -1f, -3f)]
        [TestCase(5, 4, 5f, 4f)]
        [TestCase(10, 6, 10f, 6f)]
        public void CellToWorld_MapsIntegerCellToSameWorldCoordinates(
            int cellX,
            int cellY,
            float worldX,
            float worldY)
        {
            Vector3 world = _board.CellToWorld(new Vector2Int(cellX, cellY));

            Assert.That(world, Is.EqualTo(new Vector3(worldX, worldY, 0f)));
            Assert.That(_board.WorldToCell(world), Is.EqualTo(new Vector2Int(cellX, cellY)));
        }

        [TestCase(-1, -3, true)]
        [TestCase(10, 6, true)]
        [TestCase(2, 1, false)]
        [TestCase(-2, -3, false)]
        [TestCase(-1, -4, false)]
        [TestCase(11, 6, false)]
        public void IsInside_UsesActualPaintedGroundCells(int x, int y, bool expected)
        {
            Assert.That(_board.IsInside(new Vector2Int(x, y)), Is.EqualTo(expected));
        }

        [Test]
        public void LoadBoard_LoadsAssetMapAndReusesSharedTypes()
        {
            _mapData = MapTestData.Create("PFP\nMRP");
            SerializedObject serializedBoard = new SerializedObject(_board);
            serializedBoard.FindProperty("_mapData").objectReferenceValue = _mapData;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();

            _board.LoadBoard();

            Assert.That(_board.Board.Cells.Count, Is.EqualTo(6));
            Assert.That(
                _board.Board.GetTerrain(new Vector2Int(0, 1)),
                Is.SameAs(MapTestData.TileFor('P').RuntimeTerrain));
            Assert.That(
                _board.Board.GetTerrain(new Vector2Int(2, 1)),
                Is.SameAs(MapTestData.TileFor('P').RuntimeTerrain));
            Assert.That(
                _board.Board.GetTerrain(new Vector2Int(1, 1)),
                Is.SameAs(MapTestData.TileFor('F').RuntimeTerrain));
        }

        [Test]
        public void GetOccupiedCells_ExcludesMoverAndInactiveUnitsButIncludesAllActiveUnits()
        {
            Unit movingUnit = CreateUnit("Moving Unit", new Vector2Int(1, 1), true, true);
            CreateUnit("Fixed Blocker", new Vector2Int(2, 1), false, true);
            CreateUnit("Active Movable Unit", new Vector2Int(3, 1), true, true);
            CreateUnit("Duplicate Active Unit", new Vector2Int(3, 1), true, true);
            CreateUnit("Inactive Unit", new Vector2Int(4, 1), true, false);

            IReadOnlyCollection<Vector2Int> occupiedCells = _board.GetOccupiedCells(movingUnit);

            Assert.That(occupiedCells, Has.No.Member(new Vector2Int(1, 1)));
            Assert.That(occupiedCells, Has.Member(new Vector2Int(2, 1)));
            Assert.That(occupiedCells, Has.Member(new Vector2Int(3, 1)));
            Assert.That(occupiedCells, Has.No.Member(new Vector2Int(4, 1)));
            Assert.That(occupiedCells.Count, Is.EqualTo(2));
        }

        private Unit CreateUnit(string name, Vector2Int cell, bool canMove, bool active)
        {
            GameObject unitObject = new GameObject(name);
            _unitObjects.Add(unitObject);
            unitObject.transform.position = _board.CellToWorld(cell);
            Unit unit = unitObject.AddComponent<Unit>();

            SerializedObject serializedUnit = new SerializedObject(unit);
            serializedUnit.FindProperty("_canMove").boolValue = canMove;
            serializedUnit.ApplyModifiedPropertiesWithoutUndo();

            unitObject.SetActive(active);
            return unit;
        }
    }
}
