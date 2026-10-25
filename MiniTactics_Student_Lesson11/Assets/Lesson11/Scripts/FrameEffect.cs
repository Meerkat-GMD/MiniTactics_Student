using UnityEngine;
namespace MiniTactics.Lesson11
{
    // Provided drawing only: independent scene-root frames outlive a hidden unit.
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

        public static void SpawnOwned(Sprite[] frames, Vector3 position, int sortingOrder, Transform parent)
        {
            GameObject effect = new GameObject("Frame Effect", typeof(SpriteRenderer), typeof(FrameEffect));
            FrameEffect player = effect.GetComponent<FrameEffect>();
            player._renderer = effect.GetComponent<SpriteRenderer>();
            player._frames = frames;
            effect.transform.SetParent(parent, false);
            effect.transform.position = position;
            player._renderer.sortingOrder = sortingOrder;
            player._renderer.sprite = frames[0];
        }

        private void Update()
        {
            _time += Time.deltaTime;
            int index = (int)(_time * FramesPerSecond);
            if (index >= _frames.Length) Destroy(gameObject);
            else _renderer.sprite = _frames[index];
        }
    }
}
