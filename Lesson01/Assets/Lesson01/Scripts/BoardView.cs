using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson01
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
            // TODO(학생) 2: 사각형 크기가 아니라 해당 셀에 실제 Ground Tile이 있는지 확인하세요.
            return false;
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

            // TODO(학생) 3: 준비된 Queue와 HashSet을 이용해 상하좌우 BFS를 완성하세요.
            // 시작 셀을 거리 0으로 넣고, 거리가 MoveDistance보다 작을 때만 이웃을 확장합니다.
            // 다른 Unit이 있는 셀은 IsOccupied=true로 표시하되 이웃 탐색은 계속해야 합니다.
            HashSet<Vector2Int> occupiedCells = GetOccupiedCells(selectedUnit);
            Queue<(Vector2Int Cell, int Distance)> frontier = new Queue<(Vector2Int, int)>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int> { startCell };
            
            _ = CardinalDirections;
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
