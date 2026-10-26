using UnityEngine;

namespace MiniTactics.Lesson12
{
    public readonly struct PresentationEvent
    {
        public enum EventKind { Move, Attack, Skill, Health }
        public EventKind Kind { get; }
        public Unit Actor { get; }
        public Unit Target { get; }
        public Vector3 From { get; }
        public Vector3 To { get; }
        public Vector3 TargetPosition { get; }
        public int BeforeHp { get; }
        public int AfterHp { get; }
        public int MaxHp { get; }
        public UnitLook Look { get; }
        public Sprite[] DeathFrames { get; }
        public int SortingOrder { get; }
        public float Seconds { get; }
        public float DeathSeconds { get; }

        private PresentationEvent(EventKind kind, Unit actor, Vector3 from, Vector3 to,
            Unit target = null, Vector3 targetPosition = default,
            int beforeHp = 0, int afterHp = 0, int maxHp = 0)
        {
            Kind = kind; Actor = actor; Target = target;
            From = from; To = to; TargetPosition = targetPosition;
            BeforeHp = beforeHp; AfterHp = afterHp; MaxHp = maxHp;
            UnitAnimator animator = actor.GetComponent<UnitAnimator>();
            // Initialized Flyweight frame assets stay unchanged throughout a battle.
            Look = animator.Look; DeathFrames = animator.DeathFrames;
            SortingOrder = animator.SortingOrder;
            Seconds = kind == EventKind.Move ? UnitAnimator.MoveSeconds :
                kind == EventKind.Skill ? Look.Skill.Length / UnitAnimator.FramesPerSecond :
                kind == EventKind.Attack ? UnitAnimator.AttackSeconds : 0.35f;
            DeathSeconds = DeathFrames.Length / UnitAnimator.FramesPerSecond;
        }

        public static PresentationEvent Moved(Unit unit, Vector3 from, Vector3 to) =>
            new PresentationEvent(EventKind.Move, unit, from, to);

        public static PresentationEvent Acted(Unit actor, Unit target, bool skill) =>
            new PresentationEvent(skill ? EventKind.Skill : EventKind.Attack, actor,
                actor.transform.position, actor.transform.position, target, target.transform.position);

        public static PresentationEvent HealthChanged(Unit unit, int before, int after) =>
            new PresentationEvent(EventKind.Health, unit, unit.transform.position,
                unit.transform.position, beforeHp: before, afterHp: after, maxHp: unit.MaxHp);
    }
}
