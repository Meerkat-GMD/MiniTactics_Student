using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson03
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
        private static readonly Vector2Int[] CardinalDirections =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        [SerializeField] private Tilemap _tilemap;

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
            return _tilemap != null && _tilemap.HasTile(new Vector3Int(cell.x, cell.y, 0));
        }

        public IReadOnlyList<MovementCell> GetMovementRange(Unit selectedUnit)
        {
            List<MovementCell> result = new List<MovementCell>();
            if (selectedUnit == null || _tilemap == null)
            {
                return result;
            }

            Vector2Int startCell = WorldToCell(selectedUnit.transform.position);
            if (!HasGround(startCell))
            {
                return result;
            }

            HashSet<Vector2Int> occupiedCells = GetOccupiedCells(selectedUnit);
            Queue<(Vector2Int Cell, int Distance)> frontier = new Queue<(Vector2Int, int)>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int> { startCell };
            frontier.Enqueue((startCell, 0));

            while (frontier.Count > 0)
            {
                (Vector2Int cell, int distance) = frontier.Dequeue();
                bool isOccupied = cell != startCell && occupiedCells.Contains(cell);
                result.Add(new MovementCell(cell, isOccupied));

                if (distance >= selectedUnit.MoveDistance)
                {
                    continue;
                }

                foreach (Vector2Int direction in CardinalDirections)
                {
                    Vector2Int nextCell = cell + direction;
                    if (visited.Add(nextCell) && HasGround(nextCell))
                    {
                        frontier.Enqueue((nextCell, distance + 1));
                    }
                }
            }

            return result;
        }

        public bool CanMoveTo(Unit selectedUnit, Vector2Int targetCell)
        {
            foreach (MovementCell movementCell in GetMovementRange(selectedUnit))
            {
                if (movementCell.Cell == targetCell)
                {
                    return !movementCell.IsOccupied;
                }
            }

            return false;
        }

        private bool HasGround(Vector2Int cell)
        {
            return IsInside(cell);
        }

        private HashSet<Vector2Int> GetOccupiedCells(Unit selectedUnit)
        {
            HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
            Unit[] units = Object.FindObjectsByType<Unit>(FindObjectsInactive.Exclude);

            foreach (Unit unit in units)
            {
                if (unit != selectedUnit)
                {
                    occupiedCells.Add(WorldToCell(unit.transform.position));
                }
            }

            return occupiedCells;
        }
    }
}
