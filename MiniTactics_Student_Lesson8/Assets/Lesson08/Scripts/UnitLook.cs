using UnityEngine;

namespace MiniTactics.Lesson08
{
    // The animation frames of one class in one team colour. Art data provided with the starter.
    [CreateAssetMenu(menuName = "Mini Tactics/Unit Look")]
    public sealed class UnitLook : ScriptableObject
    {
        [SerializeField] private Sprite[] _idle;
        [SerializeField] private Sprite[] _run;
        [SerializeField] private Sprite[] _attack;
        [SerializeField] private Sprite[] _skill;

        public Sprite[] Idle => _idle;
        public Sprite[] Run => _run;
        public Sprite[] Attack => _attack;
        public Sprite[] Skill => _skill;
    }
}
