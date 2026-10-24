using System.Collections.Generic;
using BepInEx.Configuration;

namespace Bloodmoon
{
    internal sealed class StaminaModule : Module
    {
        public override string Name => "Infinite stamina";

        private ConfigEntry<bool> enabled;
        private bool applied;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            enabled = config.Bind("Player", "InfiniteStamina", false, "Never run out of stamina");

            items.Add(new ToggleItem
            {
                Category = Category.Player,
                Name = "Infinite stamina",
                Description = "Sprint, jump and climb forever without running out of stamina.",
                Entry = enabled,
            });
        }

        public override void Tick()
        {
            var character = Character.localCharacter;
            if (character == null) return;

            if (enabled.Value)
            {
                // The game's own infinite-stamina switch, plus a top-up in case something bypasses it
                character.infiniteStam = true;
                float max = character.GetMaxStamina();
                if (character.data.currentStamina < max) character.data.currentStamina = max;
                applied = true;
            }
            else if (applied)
            {
                character.infiniteStam = false;
                applied = false;
            }
        }
    }
}
