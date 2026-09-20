using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson02
{
    public sealed class BoardView : MonoBehaviour
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

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

        public List<Vector2Int> GetMovementRange(Unit unit)
        {
            Vector2Int startCell = WorldToCell(unit.transform.position);
            List<Vector2Int> range = new List<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int> { startCell };
            Queue<(Vector2Int Cell, int Distance)> frontier = new Queue<(Vector2Int, int)>();
            frontier.Enqueue((startCell, 0));

            while (frontier.Count > 0)
            {
                (Vector2Int cell, int distance) = frontier.Dequeue();
                range.Add(cell);

                if (distance >= unit.MoveDistance)
                {
                    continue;
                }

                foreach (Vector2Int direction in Directions)
                {
                    Vector2Int nextCell = cell + direction;
                    if (IsInside(nextCell) && visited.Add(nextCell))
                    {
                        frontier.Enqueue((nextCell, distance + 1));
                    }
                }
            }

            return range;
        }

        public bool IsOccupiedByOther(Unit unit, Vector2Int cell)
        {
            Unit[] units = FindObjectsByType<Unit>(FindObjectsInactive.Exclude);
            foreach (Unit other in units)
            {
                if (other != unit && WorldToCell(other.transform.position) == cell)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanMoveTo(Unit unit, Vector2Int targetCell)
        {
            return GetMovementRange(unit).Contains(targetCell)
                && !IsOccupiedByOther(unit, targetCell);
        }
    }
}
