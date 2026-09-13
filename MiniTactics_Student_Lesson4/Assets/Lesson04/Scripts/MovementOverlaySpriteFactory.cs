using UnityEngine;

namespace MiniTactics.Lesson04
{
    internal static class MovementOverlaySpriteFactory
    {
        private const int TextureSize = 16;

        public static Sprite Create()
        {
            Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false)
            {
                name = "Movement Overlay Texture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            Color32[] pixels = new Color32[TextureSize * TextureSize];
            Color32 border = new Color32(255, 255, 255, 255);
            Color32 fill = new Color32(255, 255, 255, 115);

            for (int y = 0; y < TextureSize; y++)
            {
                for (int x = 0; x < TextureSize; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == TextureSize - 1 || y == TextureSize - 1;
                    pixels[y * TextureSize + x] = isBorder ? border : fill;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, TextureSize, TextureSize),
                new Vector2(0.5f, 0.5f),
                TextureSize);
            sprite.name = "Movement Overlay Sprite";
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }
    }
}
