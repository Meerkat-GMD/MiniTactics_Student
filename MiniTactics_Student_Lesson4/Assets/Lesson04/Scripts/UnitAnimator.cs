using UnityEngine;

namespace MiniTactics.Lesson04
{
    // Provided graphics only: the root is already at its logical destination.
    public sealed class UnitAnimator : MonoBehaviour
    {
        private const float FramesPerSecond = 10f;
        private const float MoveSeconds = 0.25f;
        [SerializeField] private Transform _visual;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private UnitLook _look;
        private Vector3 _lastPosition;
        private Vector3 _moveFrom;
        private float _moveTime = MoveSeconds;
        private float _loopTime;

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

        private void LateUpdate()
        {
            if (transform.position != _lastPosition)
            {
                _moveFrom = _visual.localPosition + _lastPosition - transform.position;
                Vector3 direction = transform.position - _lastPosition;
                if (direction.x != 0f)
                {
                    _body.flipX = direction.x < 0f;
                }

                _lastPosition = transform.position;
                _moveTime = 0f;
            }

            _moveTime += Time.deltaTime;
            _loopTime += Time.deltaTime;
            _visual.localPosition = Vector3.Lerp(_moveFrom, Vector3.zero, _moveTime / MoveSeconds);
            Sprite[] frames = _moveTime < MoveSeconds ? _look.Run : _look.Idle;
            _body.sprite = frames[(int)(_loopTime * FramesPerSecond) % frames.Length];
        }
    }
}
