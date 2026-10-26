using UnityEngine;

namespace MiniTactics.Lesson12
{
    // Provided drawing only. Gameplay moves the root and requests visual effects.
    public sealed class UnitAnimator : MonoBehaviour
    {
        public const float FramesPerSecond = 10f;
        public const float MoveSeconds = 0.25f;
        public const float AttackSeconds = 0.3f;
        public const float HitSeconds = 0.25f;
        [SerializeField] private Transform _visual;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private UnitLook _look;
        [SerializeField] private Sprite[] _deathFrames;
        public UnitLook Look => _look;
        public Sprite[] DeathFrames => _deathFrames;
        public int SortingOrder => _body.sortingOrder;
        private bool _automaticPlayback = true;
        private Vector3 _moveTo;
        private Vector3 _lastPosition;
        private Vector3 _moveFrom;
        private Vector3 _attackDirection;
        private float _moveTime = MoveSeconds;
        private float _attackTime = AttackSeconds;
        private float _hitTime = HitSeconds;
        private float _loopTime;
        private float _skillTime = float.PositiveInfinity;

        private void Start()
        {
            _lastPosition = transform.position;
            _body.sprite = _look.Idle[0];
        }

        public void SetLook(UnitLook look)
        {
            _look = look;
            _loopTime = 0f;
            _body.sprite = _look.Idle[0];
        }

        public void SetAutomaticPlayback(bool automatic)
        {
            if (_automaticPlayback == automatic) return;
            _automaticPlayback = automatic;
            _lastPosition = transform.position;
            _moveFrom = _moveTo = automatic ? Vector3.zero : _visual.position;
            _moveTime = MoveSeconds;
            _attackTime = AttackSeconds;
            _hitTime = HitSeconds;
            _skillTime = float.PositiveInfinity;
            if (automatic) _visual.localPosition = Vector3.zero;
        }

        public void PreservePosition(Vector3 rootDelta)
        {
            _visual.position -= rootDelta;
            _lastPosition = transform.position;
        }

        private void SetPosition(Vector3 position)
        {
            _moveFrom = _moveTo = position;
            _moveTime = MoveSeconds;
            _attackTime = AttackSeconds;
            _hitTime = HitSeconds;
            _skillTime = float.PositiveInfinity;
            _visual.position = position;
        }

        public float PlayMove(Vector3 from, Vector3 to)
        {
            SetPosition(from);
            _moveTo = to;
            _moveTime = 0f;
            if (to.x != from.x) _body.flipX = to.x < from.x;
            return MoveSeconds;
        }

        public float PlayAttack(Vector3 from, Vector3 targetPosition)
        {
            SetPosition(from);
            _attackDirection = (targetPosition - from).normalized;
            if (_attackDirection.x != 0f) _body.flipX = _attackDirection.x < 0f;
            _attackTime = 0f;
            _body.sprite = _look.Attack[0];
            return AttackSeconds;
        }

        public float PlaySkill(Vector3 from, Vector3 targetPosition)
        {
            SetPosition(from);
            if (targetPosition.x != from.x) _body.flipX = targetPosition.x < from.x;
            _skillTime = 0f;
            _body.sprite = _look.Skill[0];
            return _look.Skill.Length / FramesPerSecond;
        }

        public float PlayHit(Vector3 position)
        {
            SetPosition(position);
            _hitTime = 0f;
            return HitSeconds;
        }

        public void PlayAttack(Vector3 targetPosition)
        {
            _attackDirection = (targetPosition - transform.position).normalized;
            if (_attackDirection.x != 0f) _body.flipX = _attackDirection.x < 0f;
            _skillTime = float.PositiveInfinity;
            _attackTime = 0f;
            _body.sprite = _look.Attack[0];
        }

        public void PlaySkill(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            if (direction.x != 0f) _body.flipX = direction.x < 0f;
            _attackTime = AttackSeconds;
            _skillTime = 0f;
            _body.sprite = _look.Skill[0];
        }

        public void PlayHit() => _hitTime = 0f;

        public void PlayDeath()
        {
            FrameEffect.Spawn(_deathFrames, _visual.position, _body.sortingOrder);
        }

        private void LateUpdate()
        {
            if (_automaticPlayback && transform.position != _lastPosition)
            {
                _moveFrom = _visual.localPosition + _lastPosition - transform.position;
                Vector3 direction = transform.position - _lastPosition;
                if (direction.x != 0f && _attackTime >= AttackSeconds &&
                    _skillTime >= _look.Skill.Length / FramesPerSecond) _body.flipX = direction.x < 0f;
                _lastPosition = transform.position;
                _moveTime = 0f;
            }
            _moveTime += Time.deltaTime;
            _loopTime += Time.deltaTime;
            Vector3 position = _automaticPlayback
                ? transform.position + Vector3.Lerp(_moveFrom, Vector3.zero, _moveTime / MoveSeconds)
                : Vector3.Lerp(_moveFrom, _moveTo, _moveTime / MoveSeconds);
            Vector3 offset = Vector3.zero;
            Sprite[] frames = _moveTime < MoveSeconds ? _look.Run : _look.Idle;
            int frame = (int)(_loopTime * FramesPerSecond) % frames.Length;
            if (_attackTime < AttackSeconds)
            {
                _attackTime += Time.deltaTime;
                float progress = Mathf.Clamp01(_attackTime / AttackSeconds);
                offset += _attackDirection * (Mathf.Sin(progress * Mathf.PI) * 0.3f);
                frames = _look.Attack;
                frame = Mathf.Min((int)(progress * frames.Length), frames.Length - 1);
            }
            if (_skillTime < _look.Skill.Length / FramesPerSecond)
            {
                _skillTime += Time.deltaTime;
                frames = _look.Skill;
                frame = Mathf.Min((int)(_skillTime * FramesPerSecond), frames.Length - 1);
            }
            if (_hitTime < HitSeconds)
            {
                _hitTime += Time.deltaTime;
                offset.x += Mathf.Sin(_hitTime * 80f) * 0.06f;
            }
            _visual.position = position + offset;
            _body.sprite = frames[frame];
        }
    }
}
