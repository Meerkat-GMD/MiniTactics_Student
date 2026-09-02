using System;
using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private BoardView _board;
        [SerializeField] private Vector2Int[] _spawnCells = Array.Empty<Vector2Int>();

        private void Awake()
        {
            SpawnAll();
        }

        public Unit[] SpawnAll()
        {
            Unit[] existing = GetComponentsInChildren<Unit>(true);
            if (existing.Length > 0 || _unitPrefab == null)
            {
                return existing;
            }

            Unit[] spawned = new Unit[_spawnCells.Length];
            for (int index = 0; index < _spawnCells.Length; index++)
            {
                Unit unit = Instantiate(_unitPrefab, transform);
                unit.name = $"Squad Unit {index + 1}";
                Vector2Int cell = _spawnCells[index];
                unit.transform.position = _board != null
                    ? _board.CellToWorld(cell)
                    : new Vector3(cell.x, cell.y, 0f);
                spawned[index] = unit;
            }

            return spawned;
        }
    }
}
