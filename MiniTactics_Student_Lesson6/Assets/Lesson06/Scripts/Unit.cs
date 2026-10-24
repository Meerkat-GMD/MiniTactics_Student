using UnityEngine;

namespace MiniTactics.Lesson06
{
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;

        [SerializeField] private UnitAnimator _animator;
        [SerializeField] private UnitLook _playerLook;
        [SerializeField] private UnitLook _enemyLook;

        public Team Team { get; private set; }
        public bool HasMoved { get; set; }

        public void Initialize(Team team)
        {
            Team = team;
            HasMoved = false;
            _animator.SetLook(team == Team.Player ? _playerLook : _enemyLook);
        }

        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;

        public void MoveTo(BoardView board, Vector2Int targetCell)
        {
            transform.position = board.CellToWorld(targetCell);
        }
    }
}
