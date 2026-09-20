using UnityEngine;
using UnityEngine.Serialization;

namespace MiniTactics.Lesson06
{
    public sealed class Unit : MonoBehaviour
    {
        private static readonly Color PlayerColor = new Color(0.35f, 0.75f, 1f);
        private static readonly Color EnemyColor = new Color(1f, 0.4f, 0.4f);

        [FormerlySerializedAs("_moveDistance")]
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;

        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;
        public Team Team { get; private set; }
        public bool HasMoved { get; set; }

        public void Initialize(Team team)
        {
            Team = team;
            GetComponent<SpriteRenderer>().color = team == Team.Player ? PlayerColor : EnemyColor;
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
