using UnityEngine;

namespace MiniTactics.Lesson05
{
    public sealed class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private BoardView _board;
        [SerializeField] private Vector2Int[] _spawnCells;

        private void Awake()
        {
            for (int i = 0; i < _spawnCells.Length; i++)
            {
                Unit unit = Instantiate(_unitPrefab, _board.CellToWorld(_spawnCells[i]), Quaternion.identity, transform);
                unit.name = "Unit " + (i + 1);
            }
        }
    }
}
