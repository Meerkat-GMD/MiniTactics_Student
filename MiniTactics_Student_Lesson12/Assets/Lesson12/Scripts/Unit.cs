using System;
using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField] private bool _canMove = true;
        [SerializeField] private UnitAnimator _animator;

        private PresentationQueue _presentation;

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

        public void Initialize(Team team, UnitClass unitClass, PresentationQueue presentation)
        {
            _presentation = presentation;
            UnitClass = unitClass;
            Team = team;
            HasMoved = false;
            _animator.SetLook(team == Team.Player ? unitClass.PlayerLook : unitClass.EnemyLook);
            CurrentHp = MaxHp;
            HpChanged?.Invoke(this);
        }

        internal void Present(PresentationEvent item)
        {
            if (!_presentation.TryEnqueue(item))
                throw new InvalidOperationException("Unit requires an enabled PresentationQueue in this scene.");
            _animator.SetAutomaticPlayback(false);
            GetComponentInChildren<UnitHealthBar>(true).SetAutomaticRefresh(false);
        }

        internal void ResetPresentation()
        {
            _animator.SetAutomaticPlayback(true);
            GetComponentInChildren<UnitHealthBar>(true).SetAutomaticRefresh(true);
            if (!IsAlive) gameObject.SetActive(false);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive || damage <= 0) return;
            int before = CurrentHp;
            CurrentHp = Mathf.Max(0, CurrentHp - damage);
            Present(PresentationEvent.HealthChanged(this, before, CurrentHp));
            HpChanged?.Invoke(this);
            if (IsAlive) return;
            Died?.Invoke(this);
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0) return;
            int before = CurrentHp;
            CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
            if (CurrentHp == before) return;
            Present(PresentationEvent.HealthChanged(this, before, CurrentHp));
            HpChanged?.Invoke(this);
        }

        public void MoveTo(BoardView board, Vector2Int targetCell)
        {
            Vector3 from = transform.position;
            Vector3 to = board.CellToWorld(targetCell);
            transform.position = to;
            _animator.PreservePosition(to - from);
            Present(PresentationEvent.Moved(this, from, to));
        }
    }
}
