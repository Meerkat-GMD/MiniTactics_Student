using UnityEngine;

namespace MiniTactics.Lesson12
{
    // Plays a list of frames once at a position (the dust where a unit fell), then removes itself.
    public sealed class FrameEffect : MonoBehaviour
    {
        private const float FramesPerSecond = 10f;

        private SpriteRenderer _renderer;
        private Sprite[] _frames;
        private float _time;
        private bool _automaticLifetime = true;

        private void Awake() => _renderer = GetComponent<SpriteRenderer>();
        public void SetAutomaticLifetime(bool automatic) => _automaticLifetime = automatic;

        public void ResetVisual()
        {
            _frames = null;
            _time = 0f;
            _renderer.sprite = null;
            _renderer.color = Color.white;
            _renderer.flipX = false;
            _renderer.flipY = false;
            _renderer.sortingOrder = 0;
        }

        public void Show(Sprite[] frames, Vector3 position, int sortingOrder)
        {
            transform.position = position;
            _time = 0f;
            _frames = frames;
            _renderer.sortingOrder = sortingOrder;
            _renderer.sprite = frames[0];
        }

        public static void Spawn(Sprite[] frames, Vector3 position, int sortingOrder)
        {
            SpawnOwned(frames, position, sortingOrder, null);
        }

        public static FrameEffect SpawnOwned(Sprite[] frames, Vector3 position, int sortingOrder, Transform parent)
        {
            GameObject effect = new GameObject("Frame Effect", typeof(SpriteRenderer), typeof(FrameEffect));
            effect.transform.SetParent(parent);
            FrameEffect player = effect.GetComponent<FrameEffect>();
            player.Show(frames, position, sortingOrder);
            return player;
        }

        public bool Advance(float seconds)
        {
            _time += seconds;
            int index = (int)(_time * FramesPerSecond);
            if (index >= _frames.Length) return true;
            _renderer.sprite = _frames[index];
            return false;
        }

        private void Update()
        {
            if (_automaticLifetime && Advance(Time.deltaTime)) Destroy(gameObject);
        }
    }
}
