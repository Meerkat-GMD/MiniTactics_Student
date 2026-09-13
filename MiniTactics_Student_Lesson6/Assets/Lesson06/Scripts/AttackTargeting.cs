using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson06
{
    public static class AttackTargeting
    {
        public static IReadOnlyList<Unit> GetAdjacentEnemies(Unit attacker, IEnumerable<Unit> units, BoardView board)
        {
            var enemies = new List<Unit>();
            if (attacker == null || !attacker.isActiveAndEnabled || !attacker.IsAlive ||
                units == null || board == null) return enemies;

            Vector2Int origin = board.WorldToCell(attacker.transform.position);
            foreach (Unit unit in units)
            {
                if (unit == null || unit == attacker || !unit.gameObject.activeInHierarchy ||
                    !unit.IsAlive || unit.Team == attacker.Team || enemies.Contains(unit)) continue;

                Vector2Int cell = board.WorldToCell(unit.transform.position);
                if (Mathf.Abs(cell.x - origin.x) + Mathf.Abs(cell.y - origin.y) == 1)
                    enemies.Add(unit);
            }

            return enemies;
        }
    }
}
