using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson04
{
    public readonly struct MovementCell
    {
        public MovementCell(Vector2Int cell, bool isOccupied)
        {
            Cell = cell;
            IsOccupied = isOccupied;
        }

        public Vector2Int Cell { get; }
        public bool IsOccupied { get; }
    }

    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TextAsset _mapText;
        [SerializeField] private TileBase _plainTile;
        [SerializeField] private TileBase _forestTile;
        [SerializeField] private TileBase _mountainTile;
        [SerializeField] private TileBase _riverTile;

        private Board _board;

        public Board Board => _board ?? LoadBoard();

        public BoundsInt CellBounds
        {
            get
            {
                if (_tilemap == null)
                {
                    return new BoundsInt();
                }

                bool foundTile = false;
                int minX = 0;
                int minY = 0;
                int maxX = 0;
                int maxY = 0;

                foreach (Vector3Int position in _tilemap.cellBounds.allPositionsWithin)
                {
                    if (!_tilemap.HasTile(position))
                    {
                        continue;
                    }

                    if (!foundTile)
                    {
                        minX = maxX = position.x;
                        minY = maxY = position.y;
                        foundTile = true;
                        continue;
                    }

                    minX = Mathf.Min(minX, position.x);
                    minY = Mathf.Min(minY, position.y);
                    maxX = Mathf.Max(maxX, position.x);
                    maxY = Mathf.Max(maxY, position.y);
                }

                return foundTile
                    ? new BoundsInt(minX, minY, 0, maxX - minX + 1, maxY - minY + 1, 1)
                    : new BoundsInt();
            }
        }

        public int Width => CellBounds.size.x;
        public int Height => CellBounds.size.y;

        public int TileCount
        {
            get
            {
                if (_tilemap == null)
                {
                    return 0;
                }

                int count = 0;
                foreach (Vector3Int position in _tilemap.cellBounds.allPositionsWithin)
                {
                    if (_tilemap.HasTile(position))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public Board LoadBoard()
        {
            if (_mapText != null)
            {
                _board = MapLoader.Parse(_mapText.text);
                PaintTerrain();
                return _board;
            }

            Dictionary<Vector2Int, TerrainType> cells =
                new Dictionary<Vector2Int, TerrainType>();

            if (_tilemap != null)
            {
                foreach (Vector3Int position in _tilemap.cellBounds.allPositionsWithin)
                {
                    if (_tilemap.HasTile(position))
                    {
                        cells.Add(new Vector2Int(position.x, position.y), TerrainTypes.Plain);
                    }
                }
            }

            _board = new Board(cells);
            return _board;
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            if (_tilemap == null)
            {
                Debug.LogError("BoardView requires a ground Tilemap.", this);
                return Vector3.zero;
            }

            return _tilemap.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            if (_tilemap == null)
            {
                Debug.LogError("BoardView requires a ground Tilemap.", this);
                return Vector2Int.zero;
            }

            Vector3Int cell = _tilemap.WorldToCell(worldPosition);
            return new Vector2Int(cell.x, cell.y);
        }

        public bool IsInside(Vector2Int cell)
        {
            return _mapText != null
                ? Board.Contains(cell)
                : _tilemap != null && _tilemap.HasTile(new Vector3Int(cell.x, cell.y, 0));
        }

        public MovementRangeResult GetMovementRange(Unit selectedUnit)
        {
            if (selectedUnit == null)
            {
                return MovementRangeResult.Empty(Vector2Int.zero);
            }

            Vector2Int start = WorldToCell(selectedUnit.transform.position);
            return MovementRange.Calculate(Board, start, selectedUnit.MoveBudget);
        }

        public bool CanMoveTo(Unit selectedUnit, Vector2Int targetCell)
        {
            if (selectedUnit == null || IsOccupiedByOther(selectedUnit, targetCell))
            {
                return false;
            }

            Vector2Int start = WorldToCell(selectedUnit.transform.position);
            return targetCell != start &&
                   GetMovementRange(selectedUnit).TryGetCost(targetCell, out _);
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

        private void PaintTerrain()
        {
            if (_tilemap == null ||
                _plainTile == null ||
                _forestTile == null ||
                _mountainTile == null ||
                _riverTile == null)
            {
                return;
            }

            _tilemap.ClearAllTiles();
            foreach (KeyValuePair<Vector2Int, TerrainType> cell in _board.Cells)
            {
                _tilemap.SetTile(
                    new Vector3Int(cell.Key.x, cell.Key.y, 0),
                    TileFor(cell.Value));
            }
        }

        private TileBase TileFor(TerrainType terrain)
        {
            if (ReferenceEquals(terrain, TerrainTypes.Forest))
            {
                return _forestTile;
            }

            if (ReferenceEquals(terrain, TerrainTypes.Mountain))
            {
                return _mountainTile;
            }

            if (ReferenceEquals(terrain, TerrainTypes.River))
            {
                return _riverTile;
            }

            return _plainTile;
        }
    }
}
