using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private BoardView _board;
        [SerializeField] private Vector2Int[] _playerCells;
        [SerializeField] private Vector2Int[] _enemyCells;

        public List<Unit> Units { get; } = new List<Unit>();

        private void Awake()
        {
            Spawn(_playerCells, Team.Player, "Player");
            Spawn(_enemyCells, Team.Enemy, "Enemy");
        }

        private void Spawn(Vector2Int[] cells, Team team, string label)
        {
            for (int index = 0; index < cells.Length; index++)
            {
                Unit unit = Instantiate(_unitPrefab, transform);
                unit.name = $"{label} {index + 1}";
                unit.transform.position = _board.CellToWorld(cells[index]);
                unit.Initialize(team);
                Units.Add(unit);
            }
        }
    }
}
