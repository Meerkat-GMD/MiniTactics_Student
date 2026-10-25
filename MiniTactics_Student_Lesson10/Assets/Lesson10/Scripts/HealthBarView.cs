using UnityEngine;
namespace MiniTactics.Lesson10
{
    // Provided geometry only. The observer supplies the numbers.
    public sealed class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Transform _fill;
        public void Show(int hp, int maxHp)
        {
            Vector3 scale = _fill.localScale;
            scale.x = (float)hp / maxHp;
            _fill.localScale = scale;
        }
    }
}
