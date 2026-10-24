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

        // The afflictions that count as damage, each with its own Block toggle in the Effects tab.
        // Weight, webs, etc. are left to the game.
        private static readonly (STATUSTYPE type, string name, string what)[] Effects =
        {
            (STATUSTYPE.Injury, "Injury", "damage from falls, hits and other injuries"),
            (STATUSTYPE.Hunger, "Hunger", "getting hungry over time"),
            (STATUSTYPE.Cold, "Cold", "freezing in snow, wind and at night"),
            (STATUSTYPE.Poison, "Poison", "poison from food, plants and creatures"),
            (STATUSTYPE.Hot, "Heat", "overheating in the sun and near lava"),
            (STATUSTYPE.Drowsy, "Drowsy", "getting sleepy"),
            (STATUSTYPE.Curse, "Curse", "curses"),
            (STATUSTYPE.Spores, "Spores", "spores from mushrooms"),
        };

        // CharacterData.isInvincible is private, so set it through reflection
        private static readonly FieldInfo InvincibleField =
            typeof(CharacterData).GetField("isInvincible", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private ConfigEntry<bool> godMode;
        private ConfigEntry<float> health;
        private ConfigEntry<bool> blockAll;
        private ConfigEntry<bool> showEach;
        private readonly ConfigEntry<bool>[] blocks = new ConfigEntry<bool>[Effects.Length];
        private bool applied;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            godMode = config.Bind("Player", "InfiniteHealth", false, "Block all damage and clear every effect");
            health = config.Bind("Player", "SetHealth", 100f, "Last value used by the Set health slider");

            items.Add(new ToggleItem
            {
                Category = Category.Player,
                Name = "Infinite health",
                Description = "Blocks all damage and clears every effect. To block only some effects, use the Effects tab instead.",
                Entry = godMode,
            });
            items.Add(new SliderItem
            {
                Category = Category.Player,
                Name = "Set health",
                Description = "Sets how injured you are: 100% = no injury. Only changes injury; your other effects stay.",
                Entry = health,
                Min = 10f, Max = 100f, Step = 10f,
                Format = "0", Suffix = "%",
                OnChanged = SetHealth,
            });
            items.Add(new ButtonItem
            {
                Category = Category.Player,
                Name = "Heal fully",
                Description = "Heals all injury once. Your other effects stay. To clear those, use the Effects tab.",
                OnPress = () => { health.Value = 100f; SetHealth(100f); },
            });

            blockAll = config.Bind("Effects", "BlockAll", false, "Block every effect");
            showEach = config.Bind("Effects", "ShowEachEffect", false, "Show the separate Block toggles");

            items.Add(new ToggleItem
            {
                Category = Category.Effects,
                Name = "Block all effects",
                Description = "Keeps every effect away. Unlike Infinite health, hits still land for a moment before being cleared.",
                Entry = blockAll,
            });
            items.Add(new ButtonItem
            {
                Category = Category.Effects,
                Name = "Clear all effects",
                Description = "Clears injury, hunger, cold, poison, heat, drowsiness, curse and spores once.",
                OnPress = ClearAll,
            });
            items.Add(new ToggleItem
            {
                Category = Category.Effects,
                Name = "Show each effect",
                Description = "Shows or hides the separate Block toggles below, to keep this tab short.",
                Entry = showEach,
            });

            for (int i = 0; i < Effects.Length; i++)
            {
                var (_, name, what) = Effects[i];
                blocks[i] = config.Bind("Effects", "Block" + name, false, $"Block {name.ToLowerInvariant()}");
                items.Add(new ToggleItem
                {
                    Category = Category.Effects,
                    Name = "Block " + name.ToLowerInvariant(),
                    Description = $"Stops {what}, and clears it if you already have it. Other effects still work.",
                    Entry = blocks[i],
                    IsShown = () => showEach.Value,
                });
            }
        }

        public override void Tick()
        {
            var character = Character.localCharacter;
            if (character == null) return;
            var afflictions = character.refs.afflictions;

            // Invincibility blocks everything, so only Infinite health uses it
            if (godMode.Value)
            {
                SetInvincible(character, true);
                applied = true;
            }
            else if (applied)
            {
                SetInvincible(character, false);
                applied = false;
            }

            for (int i = 0; i < Effects.Length; i++)
                if (godMode.Value || blockAll.Value || blocks[i].Value)
                    Clear(afflictions, Effects[i].type);
        }

        // If a game update renames the field, the affliction clearing still keeps you alive
        private static void SetInvincible(Character character, bool value)
        {
            InvincibleField?.SetValue(character.data, value);
        }

        // Only touch statuses that are actually set, so we don't spam network updates every frame
        private static void Clear(CharacterAfflictions afflictions, STATUSTYPE type)
        {
            if (afflictions.GetCurrentStatus(type) > 0f)
                afflictions.SetStatus(type, 0f, true);
        }

        private static void SetHealth(float percent)
        {
            var character = Character.localCharacter;
            if (character == null) return;
            character.refs.afflictions.SetStatus(STATUSTYPE.Injury, 1f - percent / 100f, true);
        }

        private static void ClearAll()
        {
            var character = Character.localCharacter;
            if (character == null) return;
            foreach (var effect in Effects)
                Clear(character.refs.afflictions, effect.type);
        }
    }
}
