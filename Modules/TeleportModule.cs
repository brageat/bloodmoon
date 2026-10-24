using System.Collections.Generic;
using BepInEx.Configuration;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bloodmoon
{
    internal sealed class TeleportModule : Module
    {
        public override string Name => "Teleport";

        private const float MaxLookDistance = 1000f;

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

        private ConfigEntry<bool> lookTeleport;
        private ConfigEntry<Key> teleportKey;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            lookTeleport = config.Bind("Teleport", "TeleportOnKey", false, "Teleport to where you look when pressing the teleport key");
            teleportKey = config.Bind("Teleport", "TeleportKey", Key.E, "Key for teleporting to where you look");

            items.Add(new ToggleItem
            {
                Category = Category.Teleport,
                Name = $"TP on {teleportKey.Value} (where you look)",
                Description = $"While on, pressing {teleportKey.Value} teleports you to the spot you're looking at (up to {MaxLookDistance:0} m). {teleportKey.Value} still interacts too. Change the key in the config file.",
                Entry = lookTeleport,
            });

            items.Add(new ButtonItem
            {
                Category = Category.Teleport,
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
                    Description = $"Teleports to the {name} campfire area and lights the campfires before it, so the run and fog move on properly. Moves EVERYONE in the lobby (host only; otherwise it asks the host).",
                    OnPress = () => JumpToBiome(target),
                });
            }
        }

        public override void Tick()
        {
            if (!lookTeleport.Value || Plugin.MenuOpen) return;
            var kb = Keyboard.current;
            if (kb == null || teleportKey.Value == Key.None || !kb[teleportKey.Value].wasPressedThisFrame) return;

            var character = Character.localCharacter;
            if (character == null) return;
            if (TryGetLookPoint(character, out Vector3 point))
                Warp(character, point);
        }

        // ---- teleport to where you look ----

        private static bool TryGetLookPoint(Character character, out Vector3 point)
        {
            point = default;
            var cam = Camera.main;
            if (cam == null) return false;

            var ray = new Ray(cam.transform.position, cam.transform.forward);
            var hits = Physics.RaycastAll(ray, MaxLookDistance, ~0, QueryTriggerInteraction.Ignore);
            float best = float.MaxValue;
            bool found = false;

            foreach (var hit in hits)
            {
                // Skip our own body
                if (hit.collider.GetComponentInParent<Character>() == character) continue;
                if (hit.distance >= best) continue;
                best = hit.distance;
                // A little off the surface and up, so we don't land inside it
                point = hit.point + hit.normal * 0.5f + Vector3.up * 1f;
                found = true;
            }
            return found;
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

            // Light every campfire before the target that the jump would skip, without the "reveal" cutscene
            for (int i = 0; i < (int)target; i++)
            {
                var root = MapHandler.GetCampfireRoot(i);
                var fire = root != null ? root.GetComponentInChildren<Campfire>(true) : null;
                if (fire != null && !fire.Lit) fire.LightWithoutReveal();
            }

            MapHandler.JumpToSegment(target);
        }
    }
}
