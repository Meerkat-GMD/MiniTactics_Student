using UnityEngine;
using UnityEngine.Serialization;

namespace MiniTactics.Lesson05
{
    public sealed class Unit : MonoBehaviour
    {
        [FormerlySerializedAs("_moveDistance")]
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;

        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;

        public void MoveTo(BoardView board, Vector2Int targetCell)
        {
            if (board == null)
            {
                return;
            }

            transform.position = board.CellToWorld(targetCell);
        }
    }
}
