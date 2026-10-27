using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private PresentationQueue _presentation;
        [SerializeField] private BoardView _board;
        [SerializeField] private UnitPlacement[] _playerUnits;
        [SerializeField] private UnitPlacement[] _enemyUnits;
        private readonly List<Unit> _units = new List<Unit>();
        public IReadOnlyList<Unit> Units => _units;

        private void Awake()
        {
            Spawn(_playerUnits, Team.Player);
            Spawn(_enemyUnits, Team.Enemy);
        }

        private void Spawn(UnitPlacement[] placements, Team team)
        {
            for (int i = 0; i < placements.Length; i++)
            {
                UnitPlacement placement = placements[i];
                Unit unit = Instantiate(_unitPrefab, _board.CellToWorld(placement.Cell), Quaternion.identity, transform);
                unit.name = team + " " + (i + 1);
                unit.name += " " + placement.UnitClass.DisplayName;
                unit.Initialize(team, placement.UnitClass, _presentation);
                _units.Add(unit);
            }
        }
    }
}
