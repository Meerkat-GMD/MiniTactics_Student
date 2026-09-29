using UnityEngine;

namespace MiniTactics.Lesson08
{
    // The Unit object stands on its cell at once, so the game rules never wait for an animation.
    // Only the Visual child (picture and health bar) slides, lunges, shakes and plays the frames of a UnitLook.
    public sealed class UnitAnimator : MonoBehaviour
    {
        private const float FramesPerSecond = 10f;
        private const float MoveSeconds = 0.25f;
        private const float LungeSeconds = 0.3f;
        private const float HitSeconds = 0.25f;

        [SerializeField] private Unit _unit;
        [SerializeField] private Transform _visual;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private UnitLook _playerLook;
        [SerializeField] private UnitLook _enemyLook;
        [SerializeField] private Sprite[] _deathFrames;

        private UnitLook _look;
        private Sprite[] _action;
        private float _actionTime;
        private float _loopTime;
        private Vector3 _lastPosition;
        private Vector3 _moveFrom;
        private Vector3 _attackDirection;
        private int _lastHp;
        private float _moveTime = MoveSeconds;
        private float _lungeTime = LungeSeconds;
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
            // Unit.Initialize passes the unit's own look before Start; until then the team's look is shown.
            if (_look == null)
            {
                _look = _unit.Team == Team.Enemy ? _enemyLook : _playerLook;
            }

            _body.flipX = _unit.Team == Team.Enemy;
            _lastPosition = transform.position;
            _lastHp = _unit.CurrentHp;
        }

        public void SetLook(UnitLook look)
        {
            _look = look;
        }

        public void PlayAttack(Unit target)
        {
            _attackDirection = (target.transform.position - transform.position).normalized;
            Face(_attackDirection);
            _lungeTime = 0f;
            StartAction(_look.Attack);
        }

        public void PlaySkill(Unit target)
        {
            Face(target.transform.position - transform.position);
            StartAction(_look.Skill);
        }

        private void StartAction(Sprite[] frames)
        {
            _action = frames;
            _actionTime = 0f;
        }

        private void OnHpChanged(Unit unit)
        {
            if (unit.CurrentHp < _lastHp)
            {
                _hitTime = 0f;
            }

            if (!unit.IsAlive)
            {
                FrameEffect.Spawn(_deathFrames, _visual.position, _body.sortingOrder);
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
            _lungeTime += Time.deltaTime;
            _hitTime += Time.deltaTime;
            _actionTime += Time.deltaTime;
            _loopTime += Time.deltaTime;

            Vector3 slide = Vector3.Lerp(_moveFrom, Vector3.zero, _moveTime / MoveSeconds);
            Vector3 hop = Vector3.up * (0.15f * Wave(_moveTime, MoveSeconds));
            Vector3 lunge = _attackDirection * (0.25f * Wave(_lungeTime, LungeSeconds));
            Vector3 shake = Vector3.right * (0.06f * Mathf.Sin(_hitTime * 60f) * Wave(_hitTime, HitSeconds));
            _visual.localPosition = slide + hop + lunge + shake;
            _body.sprite = CurrentFrame();
        }

        // An attack or skill plays once; otherwise the run frames loop while sliding and the idle frames loop at rest.
        private Sprite CurrentFrame()
        {
            if (_action != null)
            {
                int index = (int)(_actionTime * FramesPerSecond);
                if (index < _action.Length)
                {
                    return _action[index];
                }

                _action = null;
            }

            Sprite[] loop = _moveTime < MoveSeconds ? _look.Run : _look.Idle;
            return loop[(int)(_loopTime * FramesPerSecond) % loop.Length];
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
