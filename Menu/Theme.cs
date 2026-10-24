using UnityEngine;

namespace Bloodmoon
{
    // Black and red palette + procedurally drawn textures (no image files needed)
    internal static class Theme
    {
        public static readonly Color Black = new Color(0.02f, 0.02f, 0.02f, 0.97f);
        public static readonly Color Panel = new Color(0.07f, 0.07f, 0.07f, 1f);
        public static readonly Color PanelLit = new Color(0.14f, 0.02f, 0.03f, 1f);
        public static readonly Color Red = new Color(0.80f, 0.04f, 0.07f, 1f);
        public static readonly Color RedBright = new Color(1.00f, 0.16f, 0.20f, 1f);
        public static readonly Color RedDeep = new Color(0.40f, 0.01f, 0.03f, 1f);
        public static readonly Color Text = new Color(0.94f, 0.94f, 0.94f, 1f);
        public static readonly Color Muted = new Color(0.55f, 0.55f, 0.55f, 1f);
        public static readonly Color Off = new Color(0.20f, 0.20f, 0.20f, 1f);

        public static Texture2D NewTex(int w, int h, FilterMode filter = FilterMode.Bilinear)
        {
            return new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = filter,
                wrapMode = TextureWrapMode.Clamp,
            };
        }

        public static Texture2D Solid(Color c)
        {
            var t = NewTex(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        // Fill with a 1px edge, for 9-sliced boxes (style.border = 1..2)
        public static Texture2D Bordered(Color fill, Color edge)
        {
            const int n = 8;
            var t = NewTex(n, n, FilterMode.Point);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    t.SetPixel(x, y, x < 1 || y < 1 || x >= n - 1 || y >= n - 1 ? edge : fill);
            t.Apply();
            return t;
        }

        // Fill with an accent bar down the left edge (style.border.left = 3)
        public static Texture2D AccentRow(Color fill, Color accent)
        {
            const int n = 8;
            var t = NewTex(n, n, FilterMode.Point);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    t.SetPixel(x, y, x < 3 ? accent : fill);
            t.Apply();
            return t;
        }

        public static Texture2D Circle(int size, Color fill, Color edge)
        {
            var t = NewTex(size, size);
            float r = size / 2f, rInner = r - 2f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                    Color c = d <= rInner ? fill : edge;
                    c.a = Mathf.Clamp01(r - d); // anti-aliased rim
                    t.SetPixel(x, y, c);
                }
            t.Apply();
            return t;
        }

        // Shaded red moon with a soft glow and a few craters
        public static Texture2D Moon(int size)
        {
            var t = NewTex(size, size);
            var center = new Vector2(size / 2f, size / 2f);
            float body = size * 0.36f;
            float glow = size * 0.5f;
            var light = center + new Vector2(-body * 0.35f, body * 0.35f);
            Vector3[] craters =
            {
                new Vector3(-0.30f, 0.15f, 0.22f),
                new Vector3(0.25f, -0.25f, 0.16f),
                new Vector3(0.20f, 0.35f, 0.10f),
                new Vector3(-0.15f, -0.40f, 0.09f),
            };

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    var p = new Vector2(x + 0.5f, y + 0.5f);
                    float d = Vector2.Distance(p, center);
                    Color c;

                    if (d <= body)
                    {
                        float shade = 1f - Mathf.Clamp01(Vector2.Distance(p, light) / (body * 1.8f));
                        c = Color.Lerp(new Color(0.30f, 0.01f, 0.03f), RedBright, shade);

                        foreach (var cr in craters)
                        {
                            var cp = center + new Vector2(cr.x, cr.y) * body;
                            if (Vector2.Distance(p, cp) < cr.z * body)
                                c = Color.Lerp(c, new Color(0.25f, 0.0f, 0.02f), 0.45f);
                        }
                        c.a = Mathf.Clamp01(body - d + 0.5f);
                    }
                    else if (d <= glow)
                    {
                        float g = 1f - (d - body) / (glow - body);
                        c = new Color(RedBright.r, 0.05f, 0.08f, g * g * 0.55f);
                    }
                    else c = Color.clear;

                    t.SetPixel(x, y, c);
                }
            t.Apply();
            return t;
        }
    }
}
