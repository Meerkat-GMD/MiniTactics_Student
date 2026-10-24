using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private BoardView _board;
        [SerializeField] private Vector2Int[] _spawnCells;
        [SerializeField] private Vector2Int[] _enemyCells;
        private readonly List<Unit> _units = new List<Unit>();
        public IReadOnlyList<Unit> Units => _units;

        private void Awake()
        {
            Spawn(_spawnCells, Team.Player);
            Spawn(_enemyCells, Team.Enemy);
        }

        private void Spawn(Vector2Int[] cells, Team team)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Unit unit = Instantiate(_unitPrefab, _board.CellToWorld(cells[i]), Quaternion.identity, transform);
                unit.name = team + " " + (i + 1);
                unit.Initialize(team);
                _units.Add(unit);
            }
        }
    }
}
