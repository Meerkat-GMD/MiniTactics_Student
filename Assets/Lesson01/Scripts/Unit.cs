using UnityEngine;

namespace MiniTactics.Lesson01
{
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _moveDistance = 2;
        [SerializeField] private bool _canMove = true;

        public int MoveDistance => _moveDistance;
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
