using System;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class UnitSpawner : MonoBehaviour
    {
        [Serializable]
        public struct SpawnDefinition
        {
            public string Name;
            public Vector2Int Cell;
            public Team Team;
            public bool CanMove;
            public int MoveBudget;

            public SpawnDefinition(string name, Vector2Int cell, Team team, bool canMove = true, int moveBudget = 4)
            {
                Name = name;
                Cell = cell;
                Team = team;
                CanMove = canMove;
                MoveBudget = moveBudget;
            }
        }

        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private BoardView _board;
        [SerializeField] private Vector2Int[] _spawnCells = Array.Empty<Vector2Int>();
        [SerializeField] private SpawnDefinition[] _spawns = Array.Empty<SpawnDefinition>();

        public void Configure(Unit prefab, BoardView board, SpawnDefinition[] spawns)
        {
            _unitPrefab = prefab;
            _board = board;
            _spawns = spawns ?? Array.Empty<SpawnDefinition>();
        }

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

            int count = _spawns.Length > 0 ? _spawns.Length : _spawnCells.Length;
            Unit[] spawned = new Unit[count];
            for (int index = 0; index < count; index++)
            {
                Unit unit = Instantiate(_unitPrefab, transform);
                SpawnDefinition spawn = _spawns.Length > 0 ? _spawns[index] :
                    new SpawnDefinition($"Squad Unit {index + 1}", _spawnCells[index], Team.Player);
                unit.name = spawn.Name;
                unit.Configure(spawn.Team, spawn.CanMove, spawn.MoveBudget);
                Vector2Int cell = spawn.Cell;
                unit.transform.position = _board != null
                    ? _board.CellToWorld(cell)
                    : new Vector3(cell.x, cell.y, 0f);
                spawned[index] = unit;
            }

            return spawned;
        }
    }
}
