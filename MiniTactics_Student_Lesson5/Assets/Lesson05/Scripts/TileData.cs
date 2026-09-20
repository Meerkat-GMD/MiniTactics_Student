using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniTactics.Lesson05
{
    [CreateAssetMenu(fileName = "TileData", menuName = "Mini Tactics/Tile Data")]
    public sealed class TileData : ScriptableObject
    {
        [SerializeField] private string _displayName = "평지";
        [SerializeField, Min(1)] private int _movementCost = 1;
        [SerializeField] private bool _isWalkable = true;
        [SerializeField] private TileBase _tile;

        public string DisplayName => _displayName;
        public int MovementCost => _movementCost;
        public bool IsWalkable => _isWalkable;
        public TileBase Tile => _tile;
    }
}
