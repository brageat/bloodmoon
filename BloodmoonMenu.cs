using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Bloodmoon
{
    internal class BloodmoonMenu
    {
        public bool Open;

        private const int WindowId = 0xB100D;
        private Rect windowRect = new Rect(40, 40, 320, 0);

        // Menu rows, navigated with the arrow keys
        private const int RowSpeed = 0, RowReset = 1, RowCount = 2;
        private int selected;

        private const float SpeedStep = 0.25f, SpeedMin = 0.5f, SpeedMax = 5f;

        // Palette
        private static readonly Color Night = new Color(0.06f, 0.01f, 0.02f, 0.96f);
        private static readonly Color Panel = new Color(0.12f, 0.02f, 0.03f, 1f);
        private static readonly Color PanelLit = new Color(0.22f, 0.03f, 0.05f, 1f);
        private static readonly Color Blood = new Color(0.62f, 0.03f, 0.07f, 1f);
        private static readonly Color BloodBright = new Color(0.94f, 0.13f, 0.17f, 1f);
        private static readonly Color Bone = new Color(0.94f, 0.87f, 0.85f, 1f);
        private static readonly Color Ash = new Color(0.62f, 0.44f, 0.45f, 1f);

        // ---- input ----

        public void HandleInput(Keyboard kb)
        {
            if (Repeat(kb.upArrowKey)) selected = (selected + RowCount - 1) % RowCount;
            if (Repeat(kb.downArrowKey)) selected = (selected + 1) % RowCount;

            switch (selected)
            {
                case RowSpeed:
                    int dir = (Repeat(kb.rightArrowKey) ? 1 : 0) - (Repeat(kb.leftArrowKey) ? 1 : 0);
                    if (dir != 0)
                    {
                        float v = Mathf.Round((Plugin.SpeedMultiplier.Value + dir * SpeedStep) / SpeedStep) * SpeedStep;
                        Plugin.SpeedMultiplier.Value = Mathf.Clamp(v, SpeedMin, SpeedMax);
                    }
                    break;

                case RowReset:
                    if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
                        Plugin.SpeedMultiplier.Value = 1f;
                    break;
            }
        }

        // True on the first press, then repeatedly while the key is held
        private readonly Dictionary<KeyControl, float> nextRepeat = new Dictionary<KeyControl, float>();
        private bool Repeat(KeyControl key)
        {
            float now = Time.unscaledTime;
            if (key.wasPressedThisFrame)
            {
                nextRepeat[key] = now + 0.35f;
                return true;
            }
            if (key.isPressed && nextRepeat.TryGetValue(key, out float t) && now >= t)
            {
                nextRepeat[key] = now + 0.08f;
                return true;
            }
            return false;
        }

        // ---- drawing ----

        private bool stylesReady;
        private GUIStyle window, title, subtitle, section, label, labelLit, value, hint, footer, divider, row, rowLit;
        private Texture2D moon, trackTex, fillTex, thumbTex, thumbLitTex;

        public void Draw()
        {
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

            GUILayout.Label("MOVEMENT", section);

            // Speed row
            bool lit = selected == RowSpeed;
            GUILayout.BeginVertical(lit ? rowLit : row);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed multiplier", lit ? labelLit : label);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{Plugin.SpeedMultiplier.Value:0.00}x", value);
            GUILayout.EndHorizontal();
            DrawBar(Plugin.SpeedMultiplier.Value, lit);
            GUILayout.EndVertical();

            // Reset row
            lit = selected == RowReset;
            GUILayout.BeginHorizontal(lit ? rowLit : row);
            GUILayout.Label("Reset to 1x", lit ? labelLit : label);
            GUILayout.FlexibleSpace();
            if (lit) GUILayout.Label("ENTER", hint);
            GUILayout.EndHorizontal();

            Divider();
            GUILayout.Label("Up/Down select  ·  Left/Right adjust", footer);
            GUILayout.Label($"Enter press  ·  {Plugin.ToggleKey.Value} hide", footer);
        }

        // Read-only slider: dark track, red fill, round thumb
        private void DrawBar(float speed, bool lit)
        {
            Rect r = GUILayoutUtility.GetRect(10, 14, GUILayout.ExpandWidth(true));
            float t = Mathf.InverseLerp(SpeedMin, SpeedMax, speed);

            var track = new Rect(r.x, r.y + 5, r.width, 4);
            GUI.DrawTexture(track, trackTex);
            GUI.DrawTexture(new Rect(track.x, track.y, track.width * t, track.height), fillTex);

            float x = Mathf.Lerp(r.x, r.xMax - 14, t);
            GUI.DrawTexture(new Rect(x, r.y, 14, 14), lit ? thumbLitTex : thumbTex);
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
            label = Text(13, Ash);
            labelLit = Text(13, Bone, FontStyle.Bold);
            value = Text(13, BloodBright, FontStyle.Bold);
            hint = Text(10, BloodBright, FontStyle.Bold);
            footer = Text(10, Ash);
            footer.alignment = TextAnchor.MiddleCenter;

            row = new GUIStyle { padding = new RectOffset(10, 8, 6, 6), margin = new RectOffset(0, 0, 2, 2) };
            rowLit = new GUIStyle(row) { border = new RectOffset(3, 0, 0, 0) };
            rowLit.normal.background = RowTex(PanelLit, BloodBright);

            divider = new GUIStyle
            {
                fixedHeight = 1,
                margin = new RectOffset(0, 0, 10, 8),
                stretchWidth = true,
            };
            divider.normal.background = SolidTex(new Color(Blood.r, Blood.g, Blood.b, 0.6f));

            trackTex = SolidTex(Panel);
            fillTex = SolidTex(Blood);
            thumbTex = CircleTex(14, BloodBright, Blood);
            thumbLitTex = CircleTex(14, Bone, BloodBright);
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

        // Highlighted row: fill with a bright accent bar down the left edge
        private static Texture2D RowTex(Color fill, Color accent)
        {
            const int n = 8;
            var t = NewTex(n, n);
            t.filterMode = FilterMode.Point;
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    t.SetPixel(x, y, x < 3 ? accent : fill);
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
