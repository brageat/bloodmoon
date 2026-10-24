using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using UnityEngine;

namespace Bloodmoon
{
    // Freezes the rising fog. Only the host's game decides when fog moves, so this works solo or as host.
    internal sealed class StopFogModule : Module
    {
        public override string Name => "Stop rising fog";

        private const float RescanInterval = 1f; // fog objects change with each biome

        // Fog.sinceStop is private: it's the height-fog's wait timer
        private static readonly FieldInfo FogSinceStop =
            typeof(Fog).GetField("sinceStop", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private ConfigEntry<bool> enabled;
        private float nextScan;

        private OrbFogHandler orb;
        private readonly List<Fog> heightFogs = new List<Fog>();

        // Original speeds, so turning it off puts everything back
        private readonly Dictionary<Object, float> originalSpeeds = new Dictionary<Object, float>();

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            enabled = config.Bind("World", "StopRisingFog", false, "Freeze the rising fog and its timer");

            items.Add(new ToggleItem
            {
                Category = Category.World,
                Name = "Stop rising fog",
                Description = "Freezes the rising fog and holds its countdown at zero. Works solo or when you're the host. Turning it off restarts the countdown.",
                Entry = enabled,
            });
        }

        public override void Tick()
        {
            if (Time.unscaledTime >= nextScan)
            {
                nextScan = Time.unscaledTime + RescanInterval;
                orb = Object.FindAnyObjectByType<OrbFogHandler>();
                heightFogs.Clear();
                heightFogs.AddRange(Object.FindObjectsByType<Fog>(FindObjectsSortMode.None));
            }

            if (enabled.Value) Freeze();
            else if (originalSpeeds.Count > 0) Restore();
        }

        private void Freeze()
        {
            if (orb != null)
            {
                if (!originalSpeeds.ContainsKey(orb)) originalSpeeds[orb] = orb.speed;
                orb.speed = 0f;            // can't close in
                orb.currentWaitTime = 0f;  // countdown never runs out
            }

            foreach (var fog in heightFogs)
            {
                if (fog == null) continue;
                if (!originalSpeeds.ContainsKey(fog)) originalSpeeds[fog] = fog.fogSpeed;
                fog.fogSpeed = 0f;
                FogSinceStop?.SetValue(fog, 0f);
            }
        }

        private void Restore()
        {
            foreach (var pair in originalSpeeds)
            {
                if (pair.Key is OrbFogHandler o && o != null) o.speed = pair.Value;
                else if (pair.Key is Fog f && f != null) f.fogSpeed = pair.Value;
            }
            originalSpeeds.Clear();
        }
    }
}
