using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace Bloodmoon
{
    internal sealed class ClimbModule : Module
    {
        public override string Name => "Climb speed";

        private ConfigEntry<float> multiplier;
        private bool applied;
        private float original = 1f;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            multiplier = config.Bind("Movement", "ClimbSpeedMultiplier", 1f,
                new ConfigDescription("Climbing speed multiplier", new AcceptableValueRange<float>(0.5f, 5f)));

            items.Add(new SliderItem
            {
                Category = Category.Movement,
                Name = "Climb speed",
                Description = "How fast you climb walls. 1x is normal.",
                Entry = multiplier,
                Min = 0.5f, Max = 5f, Step = 0.25f,
                Suffix = "x",
            });
            items.Add(new ButtonItem
            {
                Category = Category.Movement,
                Name = "Reset climb speed",
                Description = "Put climbing speed back to normal (1x).",
                OnPress = () => multiplier.Value = 1f,
            });
        }

        public override void Tick()
        {
            var character = Character.localCharacter;
            if (character == null) return;
            var climbing = character.refs.climbing;

            // Leave the game alone at 1x; otherwise remember its value so we can put it back
            bool active = !Mathf.Approximately(multiplier.Value, 1f);
            if (active)
            {
                if (!applied) original = climbing.climbSpeedMod;
                climbing.climbSpeedMod = multiplier.Value;
                applied = true;
            }
            else if (applied)
            {
                climbing.climbSpeedMod = original;
                applied = false;
            }
        }
    }
}
