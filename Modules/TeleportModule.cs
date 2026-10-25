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

        // Rows for up to this many other players (PEAK lobbies are small); unused rows are hidden
        private const int PlayerSlots = 8;

        // Other real players in the lobby, refreshed every frame; row i in the menu = others[i]
        private readonly List<Character> others = new List<Character>();
        private readonly ButtonItem[] tpToRows = new ButtonItem[PlayerSlots];
        private readonly ButtonItem[] bringRows = new ButtonItem[PlayerSlots];
        private ConfigEntry<bool> hideMountain;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            items.Add(new ButtonItem
            {
                Category = Category.World,
                Name = "Instant campfire",
                Description = "Instantly lights the nearest unlit campfire, with no holding and no waiting for everyone. Lighting it moves the run on to the next biome, as normal.",
                OnPress = LightNearestCampfire,
            });

            // ---- players ----
            items.Add(new SectionItem { Category = Category.Teleport, Name = "Players" });
            items.Add(new SectionItem
            {
                Category = Category.Teleport,
                Name = "No other players in your lobby",
                IsShown = () => others.Count == 0,
            });
            items.Add(new ButtonItem
            {
                Category = Category.Teleport,
                Name = "Bring everyone to me",
                Description = "Teleports every other player to you.",
                OnPress = BringEveryone,
                IsShown = () => others.Count > 0,
            });
            for (int i = 0; i < PlayerSlots; i++)
            {
                int slot = i;
                tpToRows[i] = new ButtonItem
                {
                    Category = Category.Teleport,
                    Name = "TP to player",
                    Description = "Teleports you to this player.",
                    OnPress = () => TpTo(slot),
                    IsShown = () => slot < others.Count,
                };
                bringRows[i] = new ButtonItem
                {
                    Category = Category.Teleport,
                    Name = "Bring player to me",
                    Description = "Teleports this player to you.",
                    OnPress = () => Bring(slot),
                    IsShown = () => slot < others.Count,
                };
                items.Add(tpToRows[i]);
                items.Add(bringRows[i]);
            }

            // ---- mountain (biomes) ----
            items.Add(new SectionItem { Category = Category.Teleport, Name = "Mountain" });
            hideMountain = config.Bind("Teleport", "HideMountainTPs", true, "Hide the TP-to-biome buttons");
            items.Add(new ToggleItem
            {
                Category = Category.Teleport,
                Name = "Hide mountain TPs",
                Description = "Hides the TP-to-biome buttons below, so you don't jump the whole lobby by accident. Turn it off to show them.",
                Entry = hideMountain,
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
                    IsShown = () => !hideMountain.Value,
                });
            }
        }

        public override void Tick()
        {
            // Keep the player rows in sync with who's in the lobby
            others.Clear();
            foreach (var c in Character.AllCharacters)
                if (c != null && !c.IsLocal && !c.isBot) others.Add(c);

            for (int i = 0; i < PlayerSlots && i < others.Count; i++)
            {
                string who = others[i].characterName;
                if (string.IsNullOrEmpty(who)) who = "Player " + (i + 1);
                if (others[i].data.dead) who += " (dead)";
                tpToRows[i].Name = "TP to " + who;
                bringRows[i].Name = "Bring " + who + " to me";
            }
        }

        // ---- players ----

        private Character Other(int slot) => slot < others.Count ? others[slot] : null;

        private void TpTo(int slot)
        {
            var me = Character.localCharacter;
            var target = Other(slot);
            if (me == null || target == null) return;
            Warp(me, target.Center + Vector3.up * 1.5f);
            Plugin.Log.LogInfo($"TP: you -> {target.characterName}");
        }

        private void Bring(int slot)
        {
            var me = Character.localCharacter;
            var target = Other(slot);
            if (me == null || target == null) return;
            if (target.data.dead) { Plugin.Log.LogInfo($"TP: {target.characterName} is dead, not bringing"); return; }
            Warp(target, SpotNearMe(me, slot));
            Plugin.Log.LogInfo($"TP: {target.characterName} -> you");
        }

        private void BringEveryone()
        {
            var me = Character.localCharacter;
            if (me == null) return;
            for (int i = 0; i < others.Count; i++)
                if (!others[i].data.dead) Warp(others[i], SpotNearMe(me, i));
            Plugin.Log.LogInfo($"TP: brought {others.Count} player(s) to you");
        }

        // In front of you, spread sideways so players don't land inside each other
        private static Vector3 SpotNearMe(Character me, int index)
        {
            float side = (index % 2 == 0 ? 1f : -1f) * (1 + index / 2) * 1.2f;
            return me.Center + me.data.lookDirection_Flat * 2f + me.data.lookDirection_Right * side + Vector3.up * 1.5f;
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
