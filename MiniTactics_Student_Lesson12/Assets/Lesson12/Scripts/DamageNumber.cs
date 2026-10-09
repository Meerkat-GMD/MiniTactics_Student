using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class DamageNumber : MonoBehaviour
    {
        [SerializeField] private TextMesh _text;
        private float _age;
        private Color _color;
        private bool _automaticLifetime = true;

        public void SetAutomaticLifetime(bool automatic) => _automaticLifetime = automatic;

        public void ResetVisual()
        {
            _age = 0f;
            _color = Color.white;
            _text.text = string.Empty;
            _text.color = Color.white;
        }

        public void Show(int amount, bool healing, Vector3 position)
        {
            transform.position = position + Vector3.up * 0.65f;
            _age = 0f;
            _color = healing ? new Color(0.2f, 1f, 0.4f) : new Color(1f, 0.3f, 0.2f);
            _text.text = (healing ? "+" : "-") + amount;
            _text.color = _color;
        }

        public bool Advance(float seconds)
        {
            _age += seconds;
            transform.position += Vector3.up * (seconds * 0.7f);
            _color.a = Mathf.Max(0f, 1f - _age / 0.7f);
            _text.color = _color;
            return _age >= 0.7f;
        }

        private void Update()
        {
            if (_automaticLifetime && Advance(Time.deltaTime)) Destroy(gameObject);
        }
    }
}
