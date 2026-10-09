using UnityEngine;

namespace MiniTactics.Lesson11
{
    [CreateAssetMenu(fileName = "Stage", menuName = "Mini Tactics/Stage Definition")]
    public sealed class StageDefinition : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private MapData _map;
        [SerializeField] private UnitPlacement[] _playerUnits;
        [SerializeField] private UnitPlacement[] _enemyUnits;

        public string DisplayName => _displayName;
        public MapData Map => _map;
        public UnitPlacement[] PlayerUnits => _playerUnits;
        public UnitPlacement[] EnemyUnits => _enemyUnits;
    }
}
