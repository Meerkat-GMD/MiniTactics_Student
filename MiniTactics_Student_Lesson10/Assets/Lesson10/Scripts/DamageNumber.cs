using UnityEngine;

namespace MiniTactics.Lesson10
{
    public sealed class DamageNumber : MonoBehaviour
    {
        [SerializeField] private TextMesh _text;
        private float _age;
        private Color _color;

        public void Show(int amount, bool healing, Vector3 position)
        {
            transform.position = position + Vector3.up * 0.65f;
            _age = 0f;
            _color = healing ? new Color(0.2f, 1f, 0.4f) : new Color(1f, 0.3f, 0.2f);
            _text.text = (healing ? "+" : "-") + amount;
            _text.color = _color;
        }

        private void Update()
        {
            _age += Time.deltaTime;
            transform.position += Vector3.up * (Time.deltaTime * 0.7f);
            _color.a = 1f - _age / 0.7f;
            _text.color = _color;
            if (_age >= 0.7f) Destroy(gameObject);
        }
    }
}
