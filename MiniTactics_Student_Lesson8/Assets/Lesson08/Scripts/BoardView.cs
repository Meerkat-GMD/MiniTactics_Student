using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson08
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private MapData _mapData;
        [SerializeField] private UnitSpawner _spawner;

        public Board Board { get; private set; }

        private void Awake()
        {
            Board = MapLoader.Load(_mapData);
            _tilemap.ClearAllTiles();
            foreach (var pair in Board.Cells)
            {
                Vector2Int cell = pair.Key;
                _tilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), pair.Value.Tile);
            }
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return _tilemap.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            Vector3Int cell = _tilemap.WorldToCell(worldPosition);
            return new Vector2Int(cell.x, cell.y);
        }

        public int GetDefenseBonus(Vector2Int cell) => Board.GetTile(cell).DefenseBonus;

        public bool IsInside(Vector2Int cell)
        {
            return Board.Contains(cell);
        }

        public bool TryGetMovementCost(Vector2Int cell, out int cost)
        {
            cost = 0;
            if (!Board.Contains(cell))
            {
                return false;
            }

            TileData terrain = Board.GetTile(cell);
            cost = terrain.MovementCost;
            return terrain.IsWalkable;
        }

        public MovementRangeResult CalculateMovementRange(Unit selectedUnit)
        {
            if (selectedUnit == null)
            {
                return MovementRangeResult.Empty();
            }

            Vector2Int startCell = WorldToCell(selectedUnit.transform.position);
            return MovementRange.Calculate(this, startCell, selectedUnit.MoveBudget, GetOccupiedCells(selectedUnit));
        }

        public bool CanMoveTo(Unit selectedUnit, Vector2Int targetCell)
        {
            return selectedUnit != null &&
                targetCell != WorldToCell(selectedUnit.transform.position) &&
                CalculateMovementRange(selectedUnit).TryGetCost(targetCell, out _) &&
                !IsOccupiedByOther(selectedUnit, targetCell);
        }

        public HashSet<Vector2Int> GetOccupiedCells(Unit selectedUnit)
        {
            HashSet<Vector2Int> cells = new HashSet<Vector2Int>();
            foreach (Unit unit in _spawner.Units)
            {
                if (unit.IsAlive && unit != selectedUnit)
                {
                    cells.Add(WorldToCell(unit.transform.position));
                }
            }

            return cells;
        }

        public bool IsOccupiedByOther(Unit selectedUnit, Vector2Int cell)
        {
            return GetOccupiedCells(selectedUnit).Contains(cell);
        }
    }
}
