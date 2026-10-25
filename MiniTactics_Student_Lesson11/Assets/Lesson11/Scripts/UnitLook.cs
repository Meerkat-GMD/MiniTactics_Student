using UnityEngine;

namespace MiniTactics.Lesson11
{
    // Provided picture data. Gameplay statistics are introduced in a later lesson.
    [CreateAssetMenu(menuName = "Mini Tactics/Unit Look")]
    public sealed class UnitLook : ScriptableObject
    {
        [SerializeField] private Sprite[] _idle;
        [SerializeField] private Sprite[] _run;

        [SerializeField] private Sprite[] _attack;

        [SerializeField] private Sprite[] _skill;

        public Sprite[] Skill => _skill;
        public Sprite[] Attack => _attack;
        public Sprite[] Idle => _idle;
        public Sprite[] Run => _run;
    }
}
