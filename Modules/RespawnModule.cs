using System.Collections.Generic;
using BepInEx.Configuration;
using Photon.Pun;
using UnityEngine;

namespace Bloodmoon
{
    // Revives you the same way the Scout effigy / statues do (RPCA_ReviveAtPosition), so everyone sees it
    internal sealed class RespawnModule : Module
    {
        public override string Name => "Respawn";

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            items.Add(new ButtonItem
            {
                Category = Category.Player,
                Name = "Respawn at campfire",
                Description = "Only when you're dead or passed out: brings you back next to the lit campfire closest to where you died. Like any revive, you drop your items.",
                OnPress = () => Respawn(atCampfire: true),
            });
            items.Add(new ButtonItem
            {
                Category = Category.Player,
                Name = "Respawn where you died",
                Description = "Only when you're dead or passed out: brings you back at the spot you died. Careful if that's inside the fog! You drop your items.",
                OnPress = () => Respawn(atCampfire: false),
            });
        }

        private static void Respawn(bool atCampfire)
        {
            var character = Character.localCharacter;
            if (character == null) return;

            // Reviving drops all your items, so never do it to a living player
            if (!character.data.dead && !character.data.fullyPassedOut)
            {
                Plugin.Log.LogInfo("Respawn: you're alive, nothing to do");
                return;
            }

            Vector3 died = character.LastLivingPosition;
            Vector3 spot = died + Vector3.up * 1f;

            if (atCampfire)
            {
                var fire = NearestLitCampfire(died);
                if (fire != null) spot = fire.Center() + fire.transform.forward * 2.5f + Vector3.up * 1.5f;
                else Plugin.Log.LogInfo("Respawn: no lit campfire yet, using where you died");
            }

            // (position, apply the normal post-revive status, no statue segment)
            character.refs.view.RPC("RPCA_ReviveAtPosition", RpcTarget.All, spot, true, -1);
            Plugin.Log.LogInfo($"Respawn: revived at {spot}");
        }

        private static Campfire NearestLitCampfire(Vector3 from)
        {
            Campfire best = null;
            float bestDist = float.MaxValue;
            foreach (var fire in Object.FindObjectsByType<Campfire>(FindObjectsSortMode.None))
            {
                if (!fire.Lit) continue;
                float d = Vector3.Distance(from, fire.transform.position);
                if (d < bestDist) { bestDist = d; best = fire; }
            }
            return best;
        }
    }
}
