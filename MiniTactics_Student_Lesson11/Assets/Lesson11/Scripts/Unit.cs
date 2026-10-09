using System;
using UnityEngine;

namespace MiniTactics.Lesson11
{
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField] private bool _canMove = true;
        private PresentationQueue _presentation;

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
            int before = CurrentHp;
            CurrentHp = Mathf.Max(0, CurrentHp - damage);
            bool queued = before != CurrentHp && TryPresent(PresentationEvent.HealthChanged(this, before, CurrentHp));
            HpChanged?.Invoke(this);

            if (!IsAlive)
            {
                Died?.Invoke(this);
                if (!queued) gameObject.SetActive(false);
            }
        }

        public void Heal(int amount)
        {
            int before = CurrentHp;
            CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
            if (before != CurrentHp) TryPresent(PresentationEvent.HealthChanged(this, before, CurrentHp));
            HpChanged?.Invoke(this);
        }

        public void Initialize(Team team, UnitClass unitClass, PresentationQueue presentation = null)
        {
            _presentation = presentation;
            Team = team;
            UnitClass = unitClass;
            CurrentHp = unitClass.MaxHp;
            UnitLook look = team == Team.Player ? unitClass.PlayerLook : unitClass.EnemyLook;
            GetComponent<UnitAnimator>().SetLook(look);
        }

        internal bool TryPresent(PresentationEvent item)
        {
            if (_presentation == null || !_presentation.TryEnqueue(item)) return false;
            GetComponent<UnitAnimator>().SetAutomaticPlayback(false);
            GetComponentInChildren<UnitHealthBar>(true).SetAutomaticRefresh(false);
            return true;
        }

        internal void ResetPresentation()
        {
            GetComponent<UnitAnimator>().SetAutomaticPlayback(true);
            GetComponentInChildren<UnitHealthBar>(true).SetAutomaticRefresh(true);
            if (!IsAlive) gameObject.SetActive(false);
        }

        public void MoveTo(BoardView board, Vector2Int targetCell)
        {
            if (board == null)
            {
                return;
            }

            Vector3 from = transform.position;
            Vector3 to = board.CellToWorld(targetCell);
            bool queued = from != to && TryPresent(PresentationEvent.Moved(this, from, to));
            transform.position = to;
            if (queued) GetComponent<UnitAnimator>().PreservePosition(to - from);
        }
    }
}
