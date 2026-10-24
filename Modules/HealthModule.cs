using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using STATUSTYPE = CharacterAfflictions.STATUSTYPE;

namespace Bloodmoon
{
    // PEAK has no HP bar: damage comes as afflictions that fill the stamina bar (1.0 = full = pass out)
    internal sealed class HealthModule : Module
    {
        public override string Name => "Health";

        // The afflictions that count as damage. Weight, webs, etc. are left to the game.
        private static readonly STATUSTYPE[] Harmful =
        {
            STATUSTYPE.Injury, STATUSTYPE.Hunger, STATUSTYPE.Cold, STATUSTYPE.Poison,
            STATUSTYPE.Curse, STATUSTYPE.Drowsy, STATUSTYPE.Hot, STATUSTYPE.Spores,
        };

        // CharacterData.isInvincible is private, so set it through reflection
        private static readonly FieldInfo InvincibleField =
            typeof(CharacterData).GetField("isInvincible", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private ConfigEntry<bool> godMode;
        private ConfigEntry<float> health;
        private bool applied;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            godMode = config.Bind("Player", "InfiniteHealth", false, "Block damage and clear afflictions");
            health = config.Bind("Player", "SetHealth", 100f, "Last value used by the Set health slider");

            items.Add(new ToggleItem
            {
                Category = Category.Player,
                Name = "Infinite health",
                Description = "Blocks damage, and instantly clears injury, hunger, cold, poison, heat, drowsiness, curse and spores.",
                Entry = godMode,
            });
            items.Add(new SliderItem
            {
                Category = Category.Player,
                Name = "Set health",
                Description = "Sets your health right away: clears afflictions, then adds injury to match. 100% is full health.",
                Entry = health,
                Min = 10f, Max = 100f, Step = 10f,
                Format = "0", Suffix = "%",
                OnChanged = SetHealth,
            });
            items.Add(new ButtonItem
            {
                Category = Category.Player,
                Name = "Heal fully",
                Description = "Clears all damage once, back to full health.",
                OnPress = () => { health.Value = 100f; SetHealth(100f); },
            });
        }

        public override void Tick()
        {
            var character = Character.localCharacter;
            if (character == null) return;

            if (godMode.Value)
            {
                SetInvincible(character, true);
                ClearHarmful(character.refs.afflictions);
                applied = true;
            }
            else if (applied)
            {
                SetInvincible(character, false);
                applied = false;
            }
        }

        // If a game update renames the field, the affliction clearing still keeps you alive
        private static void SetInvincible(Character character, bool value)
        {
            InvincibleField?.SetValue(character.data, value);
        }

        private static void ClearHarmful(CharacterAfflictions afflictions)
        {
            // Only touch statuses that are actually set, so we don't spam network updates every frame
            foreach (var type in Harmful)
                if (afflictions.GetCurrentStatus(type) > 0f)
                    afflictions.SetStatus(type, 0f, true);
        }

        private static void SetHealth(float percent)
        {
            var character = Character.localCharacter;
            if (character == null) return;

            var afflictions = character.refs.afflictions;
            ClearHarmful(afflictions);
            float damage = 1f - percent / 100f;
            if (damage > 0f) afflictions.SetStatus(STATUSTYPE.Injury, damage, true);
        }
    }
}
