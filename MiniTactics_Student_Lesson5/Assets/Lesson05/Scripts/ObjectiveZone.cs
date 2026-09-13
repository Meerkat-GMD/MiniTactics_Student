using UnityEngine;

namespace MiniTactics.Lesson05
{
    public sealed class ObjectiveZone : MonoBehaviour
    {
        [SerializeField] private Team _owner;
        [SerializeField] private Vector2Int _cell;

        public Team Owner => _owner;
        public Vector2Int Cell => _cell;

        public BattleResult GetResultFor(Unit unit)
        {
            return BattleResult.Playing;
        }
    }
}
