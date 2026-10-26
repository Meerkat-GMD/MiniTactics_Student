using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson02
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return _tilemap.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            Vector3Int cell = _tilemap.WorldToCell(worldPosition);
            return new Vector2Int(cell.x, cell.y);
        }

        public bool IsInside(Vector2Int cell)
        {
            return _tilemap.HasTile(new Vector3Int(cell.x, cell.y, 0));
        }

        public bool TryGetMovementCost(Vector2Int cell, out int cost)
        {
            cost = 1;
            return IsInside(cell);
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
            foreach (Unit unit in Object.FindObjectsByType<Unit>(FindObjectsInactive.Exclude))
            {
                if (unit != selectedUnit)
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
