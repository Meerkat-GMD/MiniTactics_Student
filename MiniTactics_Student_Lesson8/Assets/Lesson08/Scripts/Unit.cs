using System;
using UnityEngine;

namespace MiniTactics.Lesson08
{
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField] private bool _canMove = true;
        [SerializeField] private UnitAnimator _animator;

        public UnitClass UnitClass { get; private set; }
        public Team Team { get; private set; }
        public bool HasMoved { get; set; }
        public int CurrentHp { get; private set; }
        public int MaxHp => UnitClass.MaxHp;
        public int AttackPower => UnitClass.AttackPower;
        public int Defense => UnitClass.Defense;
        public bool IsAlive => CurrentHp > 0;
        public int MoveBudget => UnitClass.MoveBudget;
        public int AttackRange => UnitClass.AttackRange;
        public bool CanMove => _canMove;
        public event Action<Unit> HpChanged;
        public event Action<Unit> Died;

        public void Initialize(Team team, UnitClass unitClass)
        {
            UnitClass = unitClass;
            Team = team;
            HasMoved = false;
            _animator.SetLook(team == Team.Player ? unitClass.PlayerLook : unitClass.EnemyLook);
            CurrentHp = MaxHp;
            HpChanged?.Invoke(this);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive || damage <= 0) return;
            CurrentHp = Mathf.Max(0, CurrentHp - damage);
            HpChanged?.Invoke(this);
            if (IsAlive) return;
            Died?.Invoke(this);
            gameObject.SetActive(false);
        }

        public void MoveTo(BoardView board, Vector2Int targetCell)
        {
            transform.position = board.CellToWorld(targetCell);
        }
    }
}
