using UnityEngine;

namespace MiniTactics.Lesson05
{
    // Provided picture data. Gameplay statistics are introduced in a later lesson.
    [CreateAssetMenu(menuName = "Mini Tactics/Unit Look")]
    public sealed class UnitLook : ScriptableObject
    {
        [SerializeField] private Sprite[] _idle;
        [SerializeField] private Sprite[] _run;

        public Sprite[] Idle => _idle;
        public Sprite[] Run => _run;
    }
}
