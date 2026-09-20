using UnityEngine;

namespace MiniTactics.Lesson05
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Mini Tactics/Map Data")]
    public sealed class MapData : ScriptableObject
    {
        [SerializeField, Min(1)] private int _width = 12;
        [SerializeField, Min(1)] private int _height = 10;
        [Tooltip("맨 위 행부터 왼쪽에서 오른쪽 순서. Index = (Height - 1 - y) * Width + x")]
        [SerializeField] private TileData[] _cells = new TileData[120];

        public int Width => _width;
        public int Height => _height;

        public TileData GetTile(Vector2Int cell)
        {
            return _cells[(_height - 1 - cell.y) * _width + cell.x];
        }
    }
}
