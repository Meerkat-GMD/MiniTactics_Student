using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson06
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private MapData _mapData;

        public Board Board { get; private set; }

        private void Awake()
        {
            Board = MapLoader.Load(_mapData);
            PaintTerrain();
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

        public MovementRangeResult GetMovementRange(Unit unit)
        {
            Vector2Int start = WorldToCell(unit.transform.position);
            return MovementRange.Calculate(Board, start, unit.MoveBudget, GetOccupiedCells(unit));
        }

        public bool CanMoveTo(Unit unit, Vector2Int targetCell)
        {
            return targetCell != WorldToCell(unit.transform.position)
                && GetMovementRange(unit).Contains(targetCell);
        }

        private HashSet<Vector2Int> GetOccupiedCells(Unit unit)
        {
            HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
            Unit[] units = FindObjectsByType<Unit>(FindObjectsInactive.Exclude);
            foreach (Unit other in units)
            {
                if (other != unit)
                {
                    occupiedCells.Add(WorldToCell(other.transform.position));
                }
            }

            return occupiedCells;
        }

        private void PaintTerrain()
        {
            _tilemap.ClearAllTiles();
            for (int y = 0; y < _mapData.Height; y++)
            {
                for (int x = 0; x < _mapData.Width; x++)
                {
                    TileData tileData = _mapData.GetTile(new Vector2Int(x, y));
                    _tilemap.SetTile(new Vector3Int(x, y, 0), tileData.Tile);
                }
            }
        }
    }
}
