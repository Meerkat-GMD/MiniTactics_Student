using System;
using UnityEngine;

namespace MiniTactics.Lesson07
{
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;
        [SerializeField] private UnitAnimator _animator;
        [SerializeField] private UnitLook _playerLook;
        [SerializeField] private UnitLook _enemyLook;
        [SerializeField, Min(1)] private int _maxHp = 10;
        [SerializeField, Min(1)] private int _attackPower = 6;
        [SerializeField, Min(0)] private int _defense = 1;

        public Team Team { get; private set; }
        public bool HasMoved { get; set; }
        public int CurrentHp { get; private set; }
        public int MaxHp => _maxHp;
        public int AttackPower => _attackPower;
        public int Defense => _defense;
        public bool IsAlive => CurrentHp > 0;
        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;
        public event Action<Unit> HpChanged;
        public event Action<Unit> Died;

        public void Initialize(Team team)
        {
            Team = team;
            HasMoved = false;
            _animator.SetLook(team == Team.Player ? _playerLook : _enemyLook);
            CurrentHp = _maxHp;
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
