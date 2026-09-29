using UnityEngine;

namespace MiniTactics.Lesson08
{
    // The Unit object stands on its cell at once, so the game rules never wait for an animation.
    // Only the Visual child (picture and health bar) slides, lunges, shakes and pulses.
    public sealed class UnitAnimator : MonoBehaviour
    {
        private const float MoveSeconds = 0.25f;
        private const float AttackSeconds = 0.3f;
        private const float SkillSeconds = 0.4f;
        private const float HitSeconds = 0.25f;

        [SerializeField] private Unit _unit;
        [SerializeField] private SpriteRenderer _source;
        [SerializeField] private Transform _visual;
        [SerializeField] private SpriteRenderer _body;

        private Vector3 _lastPosition;
        private Vector3 _moveFrom;
        private Vector3 _attackDirection;
        private int _lastHp;
        private float _moveTime = MoveSeconds;
        private float _attackTime = AttackSeconds;
        private float _skillTime = SkillSeconds;
        private float _hitTime = HitSeconds;

        private void OnEnable()
        {
            _unit.HpChanged += OnHpChanged;
        }

        private void OnDisable()
        {
            _unit.HpChanged -= OnHpChanged;
        }

        private void Start()
        {
            _body.sprite = _source.sprite;
            _body.color = _source.color;
            _body.flipX = _unit.Team == Team.Enemy;
            _lastPosition = transform.position;
            _lastHp = _unit.CurrentHp;
        }

        public void PlayAttack(Unit target)
        {
            _attackDirection = (target.transform.position - transform.position).normalized;
            Face(_attackDirection);
            _attackTime = 0f;
        }

        public void PlaySkill(Unit target)
        {
            Face(target.transform.position - transform.position);
            _skillTime = 0f;
        }

        private void OnHpChanged(Unit unit)
        {
            if (unit.CurrentHp < _lastHp)
            {
                _hitTime = 0f;
            }

            _lastHp = unit.CurrentHp;
        }

        private void LateUpdate()
        {
            if (transform.position != _lastPosition)
            {
                _moveFrom = _visual.position - transform.position;
                Face(transform.position - _lastPosition);
                _lastPosition = transform.position;
                _moveTime = 0f;
            }

            _moveTime += Time.deltaTime;
            _attackTime += Time.deltaTime;
            _skillTime += Time.deltaTime;
            _hitTime += Time.deltaTime;

            Vector3 slide = Vector3.Lerp(_moveFrom, Vector3.zero, _moveTime / MoveSeconds);
            Vector3 hop = Vector3.up * (0.15f * Wave(_moveTime, MoveSeconds));
            Vector3 lunge = _attackDirection * (0.35f * Wave(_attackTime, AttackSeconds));
            Vector3 shake = Vector3.right * (0.06f * Mathf.Sin(_hitTime * 60f) * Wave(_hitTime, HitSeconds));
            _visual.localPosition = slide + hop + lunge + shake;
            _visual.localScale = Vector3.one * (1f + 0.25f * Wave(_skillTime, SkillSeconds));
        }

        private void Face(Vector3 direction)
        {
            if (direction.x != 0f)
            {
                _body.flipX = direction.x < 0f;
            }
        }

        // Rises from 0 to 1 and falls back to 0 during the duration, then stays 0.
        private static float Wave(float time, float duration)
        {
            if (time >= duration)
            {
                return 0f;
            }

            return Mathf.Sin(time / duration * Mathf.PI);
        }
    }
}
