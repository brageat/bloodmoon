using UnityEngine;

namespace Bloodmoon
{
    internal class BloodmoonMenu
    {
        public bool Open;

        private const int WindowId = 0xB100D;
        private Rect windowRect = new Rect(40, 40, 320, 0);

        // Palette
        private static readonly Color Night = new Color(0.06f, 0.01f, 0.02f, 0.96f);
        private static readonly Color Panel = new Color(0.12f, 0.02f, 0.03f, 1f);
        private static readonly Color Blood = new Color(0.62f, 0.03f, 0.07f, 1f);
        private static readonly Color BloodBright = new Color(0.94f, 0.13f, 0.17f, 1f);
        private static readonly Color Bone = new Color(0.94f, 0.87f, 0.85f, 1f);
        private static readonly Color Ash = new Color(0.62f, 0.44f, 0.45f, 1f);

        private bool stylesReady;
        private GUIStyle window, title, subtitle, section, label, value, slider, thumb, button, footer, divider;
        private Texture2D moon;

        public void Draw()
        {
            if (!Open) return;
            EnsureStyles();
            windowRect = GUILayout.Window(WindowId, windowRect, DrawWindow, GUIContent.none, window);
        }

        private void DrawWindow(int id)
        {
            // Header: blood moon + title
            GUILayout.BeginHorizontal();
            GUILayout.Label(moon, GUILayout.Width(40), GUILayout.Height(40));
            GUILayout.Space(6);
            GUILayout.BeginVertical();
            GUILayout.Label("BLOODMOON", title);
            GUILayout.Label($"v{Plugin.Version}  ·  PEAK", subtitle);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            Divider();

            // Movement
            GUILayout.Label("MOVEMENT", section);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed multiplier", label);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{Plugin.SpeedMultiplier.Value:0.00}x", value);
            GUILayout.EndHorizontal();

            float speed = GUILayout.HorizontalSlider(Plugin.SpeedMultiplier.Value, 0.5f, 5f, slider, thumb);
            speed = Mathf.Round(speed * 20f) / 20f; // snap to 0.05
            if (!Mathf.Approximately(speed, Plugin.SpeedMultiplier.Value))
                Plugin.SpeedMultiplier.Value = speed;

            GUILayout.Space(6);
            if (GUILayout.Button("Reset to 1x", button))
                Plugin.SpeedMultiplier.Value = 1f;

            Divider();
            GUILayout.Label($"{Plugin.ToggleKey.Value} to hide  ·  drag to move", footer);

            GUI.DragWindow();
        }

        private void Divider() => GUILayout.Box(GUIContent.none, divider);

        private void EnsureStyles()
        {
            if (stylesReady) return;
            stylesReady = true;

            window = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(14, 14, 12, 12),
                border = new RectOffset(2, 2, 2, 2),
            };
            window.normal.background = window.onNormal.background = BorderedTex(Night, Blood);

            title = Text(20, BloodBright, FontStyle.Bold);
            subtitle = Text(11, Ash);
            section = Text(11, Blood, FontStyle.Bold);
            section.margin = new RectOffset(0, 0, 4, 6);
            label = Text(13, Bone);
            value = Text(13, BloodBright, FontStyle.Bold);
            footer = Text(10, Ash);
            footer.alignment = TextAnchor.MiddleCenter;

            slider = new GUIStyle
            {
                fixedHeight = 14,
                border = new RectOffset(2, 2, 0, 0),
                margin = new RectOffset(0, 0, 6, 4),
            };
            slider.normal.background = TrackTex(Panel, Blood);

            thumb = new GUIStyle { fixedWidth = 14, fixedHeight = 14 };
            thumb.normal.background = CircleTex(14, BloodBright, Blood);
            thumb.hover.background = thumb.active.background = CircleTex(14, Bone, BloodBright);

            button = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 26,
                border = new RectOffset(2, 2, 2, 2),
            };
            button.normal.background = BorderedTex(Panel, Blood);
            button.hover.background = BorderedTex(new Color(0.22f, 0.03f, 0.05f), BloodBright);
            button.active.background = BorderedTex(Blood, BloodBright);
            button.normal.textColor = Bone;
            button.hover.textColor = button.active.textColor = Color.white;

            divider = new GUIStyle
            {
                fixedHeight = 1,
                margin = new RectOffset(0, 0, 10, 8),
                stretchWidth = true,
            };
            divider.normal.background = SolidTex(new Color(Blood.r, Blood.g, Blood.b, 0.6f));

            moon = MoonTex(80);
        }

        private static GUIStyle Text(int size, Color color, FontStyle style = FontStyle.Normal)
        {
            var s = new GUIStyle(GUI.skin.label) { fontSize = size, fontStyle = style, margin = new RectOffset(0, 0, 0, 0) };
            s.normal.textColor = color;
            return s;
        }

        // ---- procedural textures ----

        private static Texture2D NewTex(int w, int h)
        {
            return new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
            };
        }

        private static Texture2D SolidTex(Color c)
        {
            var t = NewTex(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        private static Texture2D BorderedTex(Color fill, Color edge)
        {
            const int n = 8;
            var t = NewTex(n, n);
            t.filterMode = FilterMode.Point;
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    t.SetPixel(x, y, x < 1 || y < 1 || x >= n - 1 || y >= n - 1 ? edge : fill);
            t.Apply();
            return t;
        }

        // Thin bar centred vertically in a 14px-tall texture
        private static Texture2D TrackTex(Color fill, Color edge)
        {
            const int w = 8, h = 14;
            var t = NewTex(w, h);
            t.filterMode = FilterMode.Point;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bool inBar = y >= 5 && y <= 8;
                    bool isEdge = y == 5 || y == 8 || x == 0 || x == w - 1;
                    t.SetPixel(x, y, !inBar ? Color.clear : isEdge ? edge : fill);
                }
            t.Apply();
            return t;
        }

        private static Texture2D CircleTex(int size, Color fill, Color edge)
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
        private static Texture2D MoonTex(int size)
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
                        c = Color.Lerp(new Color(0.30f, 0.01f, 0.03f), BloodBright, shade);

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
                        c = new Color(BloodBright.r, 0.05f, 0.08f, g * g * 0.55f);
                    }
                    else c = Color.clear;

                    t.SetPixel(x, y, c);
                }
            t.Apply();
            return t;
        }
    }
}
