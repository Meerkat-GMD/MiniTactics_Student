using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniTactics.Lesson07
{
    public sealed class Unit : MonoBehaviour
    {
        private static readonly Color PlayerColor = new Color(0.35f, 0.75f, 1f);
        private static readonly Color EnemyColor = new Color(1f, 0.4f, 0.4f);

        [FormerlySerializedAs("_moveDistance")]
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;
        [SerializeField, Min(1)] private int _maxHp = 10;
        [SerializeField, Min(0)] private int _attackPower = 6;
        [SerializeField, Min(0)] private int _defense = 1;

        public event Action<Unit> HpChanged;
        public event Action<Unit> Died;

        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;
        public int MaxHp => _maxHp;
        public int AttackPower => _attackPower;
        public int Defense => _defense;
        public int CurrentHp { get; private set; }
        public bool IsAlive => CurrentHp > 0;
        public Team Team { get; private set; }
        public bool HasMoved { get; set; }

        private void Awake()
        {
            CurrentHp = _maxHp;
        }

        public void TakeDamage(int damage)
        {
            CurrentHp = Mathf.Max(0, CurrentHp - damage);
            HpChanged?.Invoke(this);

            if (!IsAlive)
            {
                Died?.Invoke(this);
                gameObject.SetActive(false);
            }
        }

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
