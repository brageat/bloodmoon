using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace Bloodmoon
{
    internal sealed class SpeedModule : Module
    {
        public override string Name => "Speed";

        private ConfigEntry<float> multiplier;
        private bool applied;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            multiplier = config.Bind("Movement", "SpeedMultiplier", 1f,
                new ConfigDescription("Movement speed multiplier", new AcceptableValueRange<float>(0.5f, 5f)));

            items.Add(new SliderItem
            {
                Category = Category.Movement,
                Name = "Speed multiplier",
                Description = "How fast your character moves. 1x is normal.",
                Entry = multiplier,
                Min = 0.5f, Max = 5f, Step = 0.25f,
                Suffix = "x",
            });
            items.Add(new ButtonItem
            {
                Category = Category.Movement,
                Name = "Reset speed",
                Description = "Put your speed back to normal (1x).",
                OnPress = () => multiplier.Value = 1f,
            });
        }

        public override void Tick()
        {
            var character = Character.localCharacter;
            if (character == null) return;

            // Leave the game alone at 1x so its own modifiers still work normally
            bool active = !Mathf.Approximately(multiplier.Value, 1f);
            if (!active && !applied) return;

            character.refs.movement.movementModifier = active ? multiplier.Value : 1f;
            applied = active;
        }
    }
}
