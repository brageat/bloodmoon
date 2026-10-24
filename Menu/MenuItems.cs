using System;
using BepInEx.Configuration;
using UnityEngine;

namespace Bloodmoon
{
    // Sidebar tabs, shown in this order
    internal enum Category { Movement, Player, World, Visuals, Misc }

    // One row in the menu. Modules create these in Init().
    internal abstract class MenuItem
    {
        public Category Category;
        public string Name;
        public string Description;
    }

    // Left/Right changes the value in steps
    internal sealed class SliderItem : MenuItem
    {
        public ConfigEntry<float> Entry;
        public float Min, Max, Step;
        public string Format = "0.00";
        public string Suffix = "";

        public string Display => Entry.Value.ToString(Format) + Suffix;
        public float Fraction => Mathf.InverseLerp(Min, Max, Entry.Value);

        public void Nudge(int dir)
        {
            float v = Mathf.Round((Entry.Value + dir * Step) / Step) * Step;
            Entry.Value = Mathf.Clamp(v, Min, Max);
        }
    }

    // Enter flips it; Left = off, Right = on
    internal sealed class ToggleItem : MenuItem
    {
        public ConfigEntry<bool> Entry;
    }

    // Enter runs the action
    internal sealed class ButtonItem : MenuItem
    {
        public Action OnPress;
    }
}
