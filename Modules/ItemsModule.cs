using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace Bloodmoon
{
    // Spawns items with the game's own ItemDatabase.Add: network-spawned (everyone sees it), then put in your hand
    internal sealed class ItemsModule : Module
    {
        public override string Name => "Items";

        // (name in the game files, name shown in the menu, what it's for)
        private static readonly (string file, string label, string what)[] Items =
        {
            ("Strange gem", "Strange gem", "the gem for the end of the run"),
            ("Dynamite", "Dynamite", "blows things up. Throw it fast!"),
            ("ScoutEffigy", "Scout effigy", "brings a dead friend back"),
            ("FirstAidKit", "First aid kit", "heals injury"),
            ("Cure-All", "Cure-all", "clears every bad effect"),
            ("Energy Drink", "Energy drink", "a burst of extra stamina"),
            ("RopeSpool", "Rope spool", "a rope you can lay down a wall"),
            ("RopeShooter", "Rope cannon", "shoots a rope up a wall"),
            ("ChainShooter", "Chain launcher", "shoots a chain between two points"),
            ("ClimbingSpike", "Climbing spike", "a piton to rest on mid-climb"),
            ("RescueHook", "Rescue claw", "pulls a friend to you"),
            ("MagicBean", "Magic bean", "grows a vine you can climb"),
            ("CloudFungus", "Cloud fungus", "makes a cloud platform to stand on"),
            ("Flare", "Flare", "lights up the area"),
            ("Lantern", "Lantern", "light and warmth"),
            ("PortableStovetopItem", "Portable stove", "a campfire you can carry, for warmth and cooking"),
            ("Backpack", "Backpack", "extra inventory slots"),
        };

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            foreach (var (file, label, what) in Items)
            {
                string name = file;
                items.Add(new ButtonItem
                {
                    Category = Category.Items,
                    Name = label,
                    Description = $"Spawns a {label.ToLowerInvariant()} in your hand ({what}). If your hands are full, it lands in front of you.",
                    OnPress = () => Spawn(name),
                });
            }
        }

        private static void Spawn(string fileName)
        {
            var character = Character.localCharacter;
            if (character == null) return;

            var item = Find(fileName);
            if (item == null)
            {
                Plugin.Log.LogWarning($"Items: couldn't find '{fileName}' in the game's item list");
                return;
            }

            // A little in front of you at chest height
            Vector3 spot = character.Center + character.data.lookDirection_Flat * 1.2f + Vector3.up * 0.5f;
            ItemDatabase.Add(item, spot);
            Plugin.Log.LogInfo($"Items: spawned {item.gameObject.name}");
        }

        // Item prefab names have their own capitalisation, so match ignoring case
        private static Item Find(string fileName)
        {
            foreach (var pair in ItemDatabase.Instance.itemLookup)
            {
                var item = pair.Value;
                if (item != null && string.Equals(item.gameObject.name, fileName, StringComparison.OrdinalIgnoreCase))
                    return item;
            }
            return null;
        }
    }
}
