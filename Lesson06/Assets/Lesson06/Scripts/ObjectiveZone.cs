using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class ObjectiveZone : MonoBehaviour
    {
        [SerializeField] private Team _owner;
        [SerializeField] private Vector2Int _cell;

        public Team Owner => _owner;
        public Vector2Int Cell => _cell;

        public BattleResult GetResultFor(Unit unit)
        {
            if (unit == null || !unit.isActiveAndEnabled || !unit.IsAlive || !unit.IsTurnUnit)
            {
                return BattleResult.Playing;
            }

            if (unit.Team == Team.Player && _owner == Team.Enemy)
            {
                return BattleResult.PlayerWon;
            }

            if (unit.Team == Team.Enemy && _owner == Team.Player)
            {
                return BattleResult.PlayerLost;
            }

            return BattleResult.Playing;
        }
    }
}
