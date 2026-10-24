using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson05
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private MapData _mapData;

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

        public bool IsInside(Vector2Int cell)
        {
            return Board.Contains(cell);
        }

        public MovementRangeResult CalculateMovementRange(Unit selectedUnit)
        {
            if (selectedUnit == null)
            {
                return MovementRangeResult.Empty(Vector2Int.zero);
            }

            Vector2Int startCell = WorldToCell(selectedUnit.transform.position);
            return MovementRange.Calculate(Board, startCell, selectedUnit.MoveBudget);
        }

        public bool CanMoveTo(Unit selectedUnit, Vector2Int targetCell)
        {
            return CalculateMovementRange(selectedUnit).TryGetCost(targetCell, out _) &&
                !IsOccupiedByOther(selectedUnit, targetCell);
        }

        public bool IsOccupiedByOther(Unit selectedUnit, Vector2Int cell)
        {
            Unit[] units = Object.FindObjectsByType<Unit>(FindObjectsInactive.Exclude);
            foreach (Unit unit in units)
            {
                if (unit != selectedUnit && WorldToCell(unit.transform.position) == cell)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
