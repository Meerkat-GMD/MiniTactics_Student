using System;
using UnityEngine;

namespace MiniTactics.Lesson08
{
    public sealed class Unit : MonoBehaviour
    {
        private static readonly Color PlayerColor = new Color(0.35f, 0.75f, 1f);
        private static readonly Color EnemyColor = new Color(1f, 0.4f, 0.4f);

        [SerializeField] private bool _canMove = true;

        public event Action<Unit> HpChanged;
        public event Action<Unit> Died;

        public UnitClass UnitClass { get; private set; }
        public int MoveBudget => UnitClass.MoveBudget;
        public bool CanMove => _canMove;
        public int MaxHp => UnitClass.MaxHp;
        public int AttackPower => UnitClass.AttackPower;
        public int Defense => UnitClass.Defense;
        public int AttackRange => UnitClass.AttackRange;
        public int CurrentHp { get; private set; }
        public bool IsAlive => CurrentHp > 0;
        public Team Team { get; private set; }
        public bool HasMoved { get; set; }

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

        public void Initialize(Team team, UnitClass unitClass)
        {
            Team = team;
            UnitClass = unitClass;
            CurrentHp = unitClass.MaxHp;
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = unitClass.Sprite;
            renderer.color = team == Team.Player ? PlayerColor : EnemyColor;
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
