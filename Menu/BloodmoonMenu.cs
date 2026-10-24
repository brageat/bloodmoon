using System;
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
        private const float Width = 580, Height = 500, SidebarWidth = 150;
        private int scrollTop; // first row shown; long tabs scroll to follow the selection
        private Rect windowRect = new Rect(40, 40, Width, Height);

        private readonly List<MenuItem> allItems;
        private readonly Category[] categories; // only tabs that have at least one row
        private readonly int[] categoryCounts;
        private readonly List<MenuItem> visible = new List<MenuItem>();
        private int categoryIndex;
        private int selected;

        private Category Current => categories[categoryIndex];

        public BloodmoonMenu(List<MenuItem> items)
        {
            allItems = items;

            // Empty tabs are hidden; they show up again as soon as a module adds a row to them
            var used = new List<Category>();
            foreach (Category c in Enum.GetValues(typeof(Category)))
                if (items.Exists(i => i.Category == c)) used.Add(c);
            categories = used.ToArray();

            categoryCounts = new int[categories.Length];
            RefreshVisible();
        }

        // Rows can hide themselves (IsShown), so this runs every frame
        private void RefreshVisible()
        {
            visible.Clear();
            Array.Clear(categoryCounts, 0, categoryCounts.Length);
            foreach (var item in allItems)
            {
                if (item.IsShown != null && !item.IsShown()) continue;
                int tab = Array.IndexOf(categories, item.Category);
                if (tab >= 0) categoryCounts[tab]++;
                if (item.Category == Current) visible.Add(item);
            }
            selected = Mathf.Clamp(selected, 0, Mathf.Max(0, visible.Count - 1));
        }

        // ---- input ----

        public void HandleInput(Keyboard kb)
        {
            RefreshVisible();

            if (kb.tabKey.wasPressedThisFrame)
            {
                int step = kb.shiftKey.isPressed ? -1 : 1;
                categoryIndex = (categoryIndex + step + categories.Length) % categories.Length;
                selected = 0;
                scrollTop = 0;
                RefreshVisible();
            }

            if (visible.Count == 0) return;

            if (Repeat(kb.upArrowKey)) selected = (selected - 1 + visible.Count) % visible.Count;
            if (Repeat(kb.downArrowKey)) selected = (selected + 1) % visible.Count;

            int dir = (Repeat(kb.rightArrowKey) ? 1 : 0) - (Repeat(kb.leftArrowKey) ? 1 : 0);
            bool enter = kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame;

            switch (visible[selected])
            {
                case SliderItem slider:
                    if (dir != 0) slider.Nudge(dir);
                    break;
                case ToggleItem toggle:
                    if (dir != 0) toggle.Entry.Value = dir > 0;
                    if (enter) toggle.Entry.Value = !toggle.Entry.Value;
                    break;
                case ButtonItem button:
                    if (enter) button.OnPress?.Invoke();
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

        public void Draw()
        {
            EnsureStyles();
            if (Event.current.type == EventType.Layout) RefreshVisible(); // same rows for Layout and Repaint
            windowRect = GUILayout.Window(WindowId, windowRect, DrawWindow, GUIContent.none, window,
                GUILayout.Width(Width), GUILayout.Height(Height));
        }

        private void DrawWindow(int id)
        {
            DrawHeader();
            Divider();

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            DrawSidebar();
            GUILayout.Space(12);
            DrawItems();
            GUILayout.EndHorizontal();

            Divider();
            DrawFooter();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(moon, GUILayout.Width(48), GUILayout.Height(48));
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.Label("BLOODMOON", title);
            GUILayout.Label("mod menu for PEAK", subtitle);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label($"v{Plugin.Version}", badge);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawSidebar()
        {
            GUILayout.BeginVertical(sidebar, GUILayout.Width(SidebarWidth), GUILayout.ExpandHeight(true));
            GUILayout.Label("CATEGORIES", section);
            for (int i = 0; i < categories.Length; i++)
            {
                bool lit = i == categoryIndex;
                GUILayout.BeginHorizontal(lit ? tabLit : tab);
                GUILayout.Label(categories[i].ToString(), lit ? tabTextLit : tabText);
                GUILayout.FlexibleSpace();
                if (categoryCounts[i] > 0) GUILayout.Label(categoryCounts[i].ToString(), lit ? tabTextLit : count);
                GUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void DrawItems()
        {
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            GUILayout.Label(Current.ToString().ToUpperInvariant(), section);

            if (visible.Count == 0)
            {
                GUILayout.Space(8);
                GUILayout.Label("Nothing here yet. New mods will show up in this tab.", muted);
            }

            // Show as many rows as fit; scroll only when the tab really is too long.
            // Only change the window during Layout so Layout and Repaint draw the same rows.
            if (Event.current.type == EventType.Layout) UpdateScroll();

            if (scrollTop > 0) GUILayout.Label($"▲  {scrollTop} more above", more);

            for (int i = scrollTop; i < scrollEnd && i < visible.Count; i++)
            {
                bool lit = i == selected;
                switch (visible[i])
                {
                    case SliderItem s: DrawSlider(s, lit); break;
                    case ToggleItem t: DrawToggle(t, lit); break;
                    case ButtonItem b: DrawButton(b, lit); break;
                }
                // Measure the real row height so the fitting is exact next frame
                if (Event.current.type == EventType.Repaint)
                    rowHeights[visible[i]] = GUILayoutUtility.GetLastRect().height + RowMargin;
            }

            if (scrollEnd < visible.Count) GUILayout.Label($"▼  {visible.Count - scrollEnd} more below", more);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
                rowsAreaHeight = GUILayoutUtility.GetLastRect().height - SectionHeight;
        }

        // ---- fitting rows into the available height ----

        private const float RowMargin = 4f;       // row style margin top + bottom
        private const float SectionHeight = 26f;  // the tab title above the rows
        private const float MoreHeight = 20f;     // a "more above/below" line
        private float rowsAreaHeight = 300f;      // measured each repaint
        private int scrollEnd;
        private readonly Dictionary<MenuItem, float> rowHeights = new Dictionary<MenuItem, float>();

        private float RowHeight(MenuItem item) =>
            rowHeights.TryGetValue(item, out float h) ? h : (item is SliderItem ? 58f : 40f);

        // Index just past the last row that fits when starting at `start`
        private int FitFrom(int start)
        {
            float used = start > 0 ? MoreHeight : 0f;
            int end = start;
            while (end < visible.Count)
            {
                bool moreAfter = end < visible.Count - 1;
                float need = RowHeight(visible[end]) + (moreAfter ? MoreHeight : 0f);
                if (end > start && used + need > rowsAreaHeight) break;
                used += RowHeight(visible[end]);
                end++;
            }
            return end;
        }

        private void UpdateScroll()
        {
            scrollTop = Mathf.Clamp(scrollTop, 0, Mathf.Max(0, visible.Count - 1));
            if (selected < scrollTop) scrollTop = selected;
            while (scrollTop < selected && selected >= FitFrom(scrollTop)) scrollTop++;
            // Don't leave empty space at the bottom when rows above could fit
            while (scrollTop > 0 && FitFrom(scrollTop - 1) == visible.Count) scrollTop--;
            scrollEnd = FitFrom(scrollTop);
        }

        private void DrawSlider(SliderItem item, bool lit)
        {
            GUILayout.BeginVertical(lit ? rowLit : row);
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.Name, lit ? labelLit : label);
            GUILayout.FlexibleSpace();
            GUILayout.Label(item.Display, value);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            DrawBar(item.Fraction, lit);
            GUILayout.EndVertical();
        }

        private void DrawToggle(ToggleItem item, bool lit)
        {
            GUILayout.BeginHorizontal(lit ? rowLit : row);
            GUILayout.Label(item.Name, lit ? labelLit : label);
            GUILayout.FlexibleSpace();
            bool on = item.Entry.Value;
            GUILayout.Label(on ? "ON" : "OFF", on ? pillOn : pillOff);
            GUILayout.EndHorizontal();
        }

        private void DrawButton(ButtonItem item, bool lit)
        {
            GUILayout.BeginHorizontal(lit ? rowLit : row);
            GUILayout.Label(item.Name, lit ? labelLit : label);
            GUILayout.FlexibleSpace();
            if (lit) GUILayout.Label("ENTER", hint);
            GUILayout.EndHorizontal();
        }

        // Read-only slider: dark track, red fill, round thumb
        private void DrawBar(float t, bool lit)
        {
            Rect r = GUILayoutUtility.GetRect(10, 14, GUILayout.ExpandWidth(true));
            var track = new Rect(r.x, r.y + 5, r.width, 4);
            GUI.DrawTexture(track, trackTex);
            GUI.DrawTexture(new Rect(track.x, track.y, track.width * t, track.height), fillTex);
            float x = Mathf.Lerp(r.x, r.xMax - 14, t);
            GUI.DrawTexture(new Rect(x, r.y, 14, 14), lit ? thumbLitTex : thumbTex);
        }

        private void DrawFooter()
        {
            string desc = visible.Count > 0 ? visible[selected].Description : "";
            GUILayout.Label(desc, description);
            GUILayout.Label($"TAB category   ·   UP/DOWN select   ·   LEFT/RIGHT adjust   ·   ENTER press   ·   {Plugin.ToggleKey.Value.ToString().ToUpperInvariant()} hide", footer);
        }

        private void Divider() => GUILayout.Box(GUIContent.none, divider);

        // ---- styles ----

        private bool stylesReady;
        private GUIStyle window, title, subtitle, badge, section, label, labelLit, value, hint, muted, more, description, footer, divider;
        private GUIStyle sidebar, tab, tabLit, tabText, tabTextLit, count, row, rowLit, pillOn, pillOff;
        private Texture2D moon, trackTex, fillTex, thumbTex, thumbLitTex;

        private void EnsureStyles()
        {
            if (stylesReady) return;
            stylesReady = true;

            window = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(16, 16, 14, 12),
                border = new RectOffset(2, 2, 2, 2),
            };
            window.normal.background = window.onNormal.background = Theme.Bordered(Theme.Black, Theme.Red);

            title = Text(24, Theme.RedBright, FontStyle.Bold);
            subtitle = Text(11, Theme.Muted);
            badge = Text(11, Theme.Text, FontStyle.Bold);
            badge.padding = new RectOffset(8, 8, 3, 3);
            badge.border = new RectOffset(1, 1, 1, 1);
            badge.normal.background = Theme.Bordered(Theme.RedDeep, Theme.Red);
            section = Text(11, Theme.Red, FontStyle.Bold);
            section.margin = new RectOffset(0, 0, 2, 8);
            label = Text(14, Theme.Muted);
            labelLit = Text(14, Theme.Text, FontStyle.Bold);
            value = Text(14, Theme.RedBright, FontStyle.Bold);
            hint = Text(10, Theme.RedBright, FontStyle.Bold);
            muted = Text(12, Theme.Muted);
            muted.wordWrap = true;
            more = Text(13, Theme.RedBright, FontStyle.Bold);
            more.alignment = TextAnchor.MiddleCenter;
            more.fixedHeight = 18;
            more.margin = new RectOffset(0, 0, 1, 1);
            description = Text(12, Theme.Text);
            description.wordWrap = true;
            description.fixedHeight = 34;
            footer = Text(10, Theme.Muted);
            footer.alignment = TextAnchor.MiddleCenter;

            sidebar = new GUIStyle { padding = new RectOffset(10, 10, 10, 10), border = new RectOffset(1, 1, 1, 1) };
            sidebar.normal.background = Theme.Bordered(Theme.Panel, new Color(0.13f, 0.13f, 0.13f));
            tab = new GUIStyle { padding = new RectOffset(10, 8, 7, 7), margin = new RectOffset(0, 0, 1, 1) };
            tabLit = new GUIStyle(tab);
            tabLit.normal.background = Theme.Solid(Theme.Red);
            tabText = Text(13, Theme.Muted);
            tabTextLit = Text(13, Color.white, FontStyle.Bold);
            count = Text(11, new Color(0.35f, 0.35f, 0.35f));

            row = new GUIStyle { padding = new RectOffset(12, 10, 8, 8), margin = new RectOffset(0, 0, 2, 2) };
            row.normal.background = Theme.Solid(new Color(0.05f, 0.05f, 0.05f, 1f));
            rowLit = new GUIStyle(row) { border = new RectOffset(3, 0, 0, 0) };
            rowLit.normal.background = Theme.AccentRow(Theme.PanelLit, Theme.RedBright);

            pillOn = Text(11, Color.white, FontStyle.Bold);
            pillOn.padding = new RectOffset(10, 10, 2, 2);
            pillOn.normal.background = Theme.Solid(Theme.Red);
            pillOff = Text(11, Theme.Muted, FontStyle.Bold);
            pillOff.padding = new RectOffset(8, 8, 2, 2);
            pillOff.normal.background = Theme.Solid(Theme.Off);

            divider = new GUIStyle { fixedHeight = 1, margin = new RectOffset(0, 0, 10, 10), stretchWidth = true };
            divider.normal.background = Theme.Solid(new Color(Theme.Red.r, Theme.Red.g, Theme.Red.b, 0.7f));

            trackTex = Theme.Solid(Theme.Off);
            fillTex = Theme.Solid(Theme.Red);
            thumbTex = Theme.Circle(14, Theme.RedBright, Theme.Red);
            thumbLitTex = Theme.Circle(14, Color.white, Theme.RedBright);
            moon = Theme.Moon(96);
        }

        private static GUIStyle Text(int size, Color color, FontStyle style = FontStyle.Normal)
        {
            var s = new GUIStyle(GUI.skin.label) { fontSize = size, fontStyle = style, margin = new RectOffset(0, 0, 0, 0) };
            s.normal.textColor = color;
            return s;
        }
    }
}
