using UnityEngine;
using UnityEngine.Serialization;

namespace MiniTactics.Lesson05
{
    [ExecuteAlways]
    public sealed class Unit : MonoBehaviour
    {
        [FormerlySerializedAs("_moveDistance")]
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;
        [SerializeField] private Team _team = Team.Player;
        [SerializeField] private bool _hasMoved;

        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;
        public Team Team => _team;
        public bool HasMoved => _hasMoved;
        public bool IsTurnUnit => _canMove;

        private void Awake()
        {
            ApplyTeamTint();
        }

        private void OnEnable()
        {
            ApplyTeamTint();
        }

        private void ApplyTeamTint()
        {
            if (!_canMove)
            {
                return;
            }

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = _team == Team.Player
                    ? new Color(0.35f, 0.75f, 1f)
                    : new Color(1f, 0.4f, 0.4f);
            }
        }

        public void SetMoved(bool moved) => _hasMoved = moved;

        public void Configure(Team team, bool canMove, int moveBudget = 4)
        {
            _team = team;
            _canMove = canMove;
            _moveBudget = Mathf.Max(0, moveBudget);
            _hasMoved = false;
            ApplyTeamTint();
        }

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
