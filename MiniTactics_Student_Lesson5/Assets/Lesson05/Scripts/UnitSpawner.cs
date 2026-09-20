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
            for (int index = 0; index < _spawnCells.Length; index++)
            {
                Unit unit = Instantiate(_unitPrefab, transform);
                unit.name = $"Squad Unit {index + 1}";
                unit.transform.position = _board.CellToWorld(_spawnCells[index]);
            }
        }
    }
}
