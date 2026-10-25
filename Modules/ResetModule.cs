using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace Bloodmoon
{
    // One button that puts every mod setting back to its default
    internal sealed class ResetModule : Module
    {
        public override string Name => "Reset";

        // Not mod settings: the menu key and how updates are checked stay as they are
        private static readonly HashSet<string> Keep = new HashSet<string> { "General", "Updates" };

        private ConfigFile config;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            this.config = config;
            items.Add(new ButtonItem
            {
                Category = Category.Misc,
                Name = "Reset all settings",
                Description = "Puts every mod back to default: speeds to 1x, every toggle off (fly, noclip, infinite health...), fog back to normal. Your menu key stays the same.",
                OnPress = ResetAll,
            });
        }

        private void ResetAll()
        {
            int n = 0;
            // Each module watches its settings every frame, so changing them here also undoes their effects
            foreach (var entry in config.Keys.ToList().Select(k => config[k]))
            {
                if (Keep.Contains(entry.Definition.Section)) continue;
                if (Equals(entry.BoxedValue, entry.DefaultValue)) continue;
                entry.BoxedValue = entry.DefaultValue;
                n++;
            }
            Plugin.Log.LogInfo($"Reset all settings: {n} changed back to default");
        }
    }
}
