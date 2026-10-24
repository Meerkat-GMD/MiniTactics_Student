using UnityEngine;

namespace MiniTactics.Lesson04
{
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;

        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;

        public void MoveTo(BoardView board, Vector2Int targetCell)
        {
            transform.position = board.CellToWorld(targetCell);
        }
    }
}
