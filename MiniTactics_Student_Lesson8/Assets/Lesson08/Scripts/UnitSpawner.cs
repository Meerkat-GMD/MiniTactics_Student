using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson08
{
    public sealed class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private BoardView _board;
        [SerializeField] private UnitPlacement[] _playerUnits;
        [SerializeField] private UnitPlacement[] _enemyUnits;

        public List<Unit> Units { get; } = new List<Unit>();

        private void Awake()
        {
            Spawn(_playerUnits, Team.Player, "Player");
            Spawn(_enemyUnits, Team.Enemy, "Enemy");
        }

        private void Spawn(UnitPlacement[] placements, Team team, string label)
        {
            for (int index = 0; index < placements.Length; index++)
            {
                UnitPlacement placement = placements[index];
                Unit unit = Instantiate(_unitPrefab, transform);
                unit.name = $"{label} {placement.UnitClass.DisplayName} {index + 1}";
                unit.transform.position = _board.CellToWorld(placement.Cell);
                unit.Initialize(team, placement.UnitClass);
                Units.Add(unit);
            }
        }
    }
}
