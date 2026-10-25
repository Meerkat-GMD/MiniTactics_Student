using System.Collections.Generic;
using UnityEngine;

namespace MiniTactics.Lesson11
{
    public abstract class Skill : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField, Min(1)] private int _range = 1;

        private Unit _user;
        private TurnManager _turns;

        protected Unit User => _user;
        protected TurnManager Turns => _turns;

        public string DisplayName => _displayName;

        public List<Unit> GetTargets(Unit user, TurnManager turns)
        {
            _user = user;
            _turns = turns;
            return FindTargets();
        }

        public void Use(Unit user, Unit target, TurnManager turns)
        {
            _user = user;
            _turns = turns;
            Activate(target);
        }

        protected abstract List<Unit> FindTargets();
        protected abstract void Activate(Unit target);

        protected List<Unit> EnemiesInRange()
        {
            Team enemyTeam = _user.Team == Team.Player ? Team.Enemy : Team.Player;
            return _turns.GetUnitsInRange(_user, _range, enemyTeam);
        }

        protected List<Unit> WoundedAlliesInRange()
        {
            List<Unit> allies = _turns.GetUnitsInRange(_user, _range, _user.Team);
            List<Unit> targets = new List<Unit>();
            foreach (Unit unit in allies)
            {
                if (unit.CurrentHp < unit.MaxHp)
                {
                    targets.Add(unit);
                }
            }

            return targets;
        }

        protected void Strike(Unit target, int bonusDamage)
        {
            _turns.Attack(_user, target, bonusDamage);
        }

        protected void Restore(Unit target, int amount)
        {
            target.Heal(amount);
        }
    }
}
