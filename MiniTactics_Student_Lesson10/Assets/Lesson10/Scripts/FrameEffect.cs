using UnityEngine;

namespace MiniTactics.Lesson10
{
    // Plays a list of frames once at a position (the dust where a unit fell), then removes itself.
    public sealed class FrameEffect : MonoBehaviour
    {
        private const float FramesPerSecond = 10f;

        private SpriteRenderer _renderer;
        private Sprite[] _frames;
        private float _time;

        public static void Spawn(Sprite[] frames, Vector3 position, int sortingOrder)
        {
            SpawnOwned(frames, position, sortingOrder, null);
        }

        public static FrameEffect SpawnOwned(Sprite[] frames, Vector3 position, int sortingOrder, Transform parent)
        {
            GameObject effect = new GameObject("Frame Effect", typeof(SpriteRenderer), typeof(FrameEffect));
            effect.transform.SetParent(parent);
            effect.transform.position = position;
            FrameEffect player = effect.GetComponent<FrameEffect>();
            player._renderer = effect.GetComponent<SpriteRenderer>();
            player._renderer.sortingOrder = sortingOrder;
            player._frames = frames;
            return player;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            int index = (int)(_time * FramesPerSecond);
            if (index >= _frames.Length)
            {
                Destroy(gameObject);
                return;
            }

            _renderer.sprite = _frames[index];
        }
    }
}
