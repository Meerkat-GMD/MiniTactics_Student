using System;
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

        [NonSerialized] private TerrainType _runtimeTerrain;

        public TileBase Tile => _tile;

        // One immutable runtime flyweight per asset, shared by all its map cells.
        public TerrainType RuntimeTerrain
        {
            get
            {
                Validate();
                return _runtimeTerrain ??= new TerrainType(_displayName, _movementCost, _isWalkable);
            }
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(_displayName) || _movementCost < 1)
                throw new ArgumentException($"Tile data '{name}' has invalid terrain rules.");
            if (_tile == null)
                throw new ArgumentException($"Tile data '{name}' needs a visual tile.");
        }

        private void OnValidate() => _runtimeTerrain = null;
        private void OnEnable() => _runtimeTerrain = null;
    }
}
