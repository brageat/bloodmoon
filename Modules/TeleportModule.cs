using System.Collections.Generic;
using BepInEx.Configuration;
using Photon.Pun;
using UnityEngine;

namespace Bloodmoon
{
    internal sealed class TeleportModule : Module
    {
        public override string Name => "Teleport";

        // Biomes in climbing order. Segment 2 is Alpine or Mesa depending on the run.
        private static readonly (Segment segment, string name)[] Biomes =
        {
            (Segment.Beach, "Beach"),
            (Segment.Tropics, "Tropics"),
            (Segment.Alpine, "Alpine / Mesa"),
            (Segment.Caldera, "Caldera"),
            (Segment.TheKiln, "The Kiln"),
            (Segment.Peak, "Peak"),
        };

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            items.Add(new ButtonItem
            {
                Category = Category.World,
                Name = "Instant campfire",
                Description = "Instantly lights the nearest unlit campfire, with no holding and no waiting for everyone. Lighting it moves the run on to the next biome, as normal.",
                OnPress = LightNearestCampfire,
            });

            foreach (var (segment, name) in Biomes)
            {
                var target = segment;
                items.Add(new ButtonItem
                {
                    Category = Category.Teleport,
                    Name = "TP to " + name,
                    Description = $"Current or earlier biome: teleports only you to the {name} campfire and lights nothing. Later biome: lights the campfires on the way and moves EVERYONE there (host only).",
                    OnPress = () => JumpToBiome(target),
                });
            }
        }

        private static void Warp(Character character, Vector3 position)
        {
            // The game's own teleport, sent to everyone so other players see us move
            character.refs.view.RPC("WarpPlayerRPC", RpcTarget.All, position, true);
        }

        // ---- campfires and biomes ----

        private static void LightNearestCampfire()
        {
            var character = Character.localCharacter;
            if (character == null) return;

            Campfire nearest = null;
            float best = float.MaxValue;
            foreach (var fire in Object.FindObjectsByType<Campfire>(FindObjectsSortMode.None))
            {
                if (fire.Lit) continue;
                float d = Vector3.Distance(character.Center, fire.transform.position);
                if (d < best) { best = d; nearest = fire; }
            }

            if (nearest == null) Plugin.Log.LogInfo("Instant campfire: no unlit campfire found");
            else nearest.DebugLight();
        }

        private static void JumpToBiome(Segment target)
        {
            if (!MapHandler.Exists) return;

            // Same or earlier biome: just move ourselves. Lighting campfires / jumping would
            // drag the whole lobby along and leave anyone who isn't there to the fog.
            if (target <= MapHandler.CurrentSegmentNumber)
            {
                WarpToCampfire(target);
                return;
            }

            // Light every campfire before the target that the jump would skip, without the "reveal" cutscene
            // (one broken campfire must not stop the jump, so each is guarded)
            for (int i = 0; i < (int)target; i++)
            {
                try
                {
                    var root = MapHandler.GetCampfireRoot(i);
                    var fire = root != null ? root.GetComponentInChildren<Campfire>(true) : null;
                    if (fire != null && !fire.Lit) fire.LightWithoutReveal();
                }
                catch (System.Exception e)
                {
                    Plugin.Log.LogWarning($"TP to biome: couldn't light campfire {i}: {e.Message}");
                }
            }

            MapHandler.JumpToSegment(target);
            Plugin.Log.LogInfo($"TP to biome: jumped to {target}");
        }

        // Teleport only the local player next to a biome's campfire, without touching it
        private static void WarpToCampfire(Segment segment)
        {
            var character = Character.localCharacter;
            if (character == null) return;

            var root = MapHandler.GetCampfireRoot((int)segment);
            var fire = root != null ? root.GetComponentInChildren<Campfire>(true) : null;
            if (fire == null)
            {
                Plugin.Log.LogInfo($"TP to biome: no campfire found for {segment}");
                return;
            }

            // Beside the fire, not in it
            Vector3 spot = fire.Center() + fire.transform.forward * 2.5f + Vector3.up * 1.5f;
            Warp(character, spot);
            Plugin.Log.LogInfo($"TP to biome: moved only you to the {segment} campfire");
        }
    }
}
