using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bloodmoon
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "com.brageat.bloodmoon";
        public const string Name = "Bloodmoon";
        public const string Version = "0.1.0";

        internal static ManualLogSource Log;
        internal static ConfigEntry<Key> ToggleKey;
        internal static ConfigEntry<float> SpeedMultiplier;

        private BloodmoonMenu menu;
        private CursorLockMode prevLock;
        private bool prevVisible;

        private void Awake()
        {
            Log = Logger;

            // Save manually when the menu closes instead of on every slider tick
            Config.SaveOnConfigSet = false;
            ToggleKey = Config.Bind("General", "ToggleKey", Key.Insert, "Key that opens/closes the menu");
            SpeedMultiplier = Config.Bind("Movement", "SpeedMultiplier", 1f,
                new ConfigDescription("Movement speed multiplier", new AcceptableValueRange<float>(0.5f, 5f)));

            menu = new BloodmoonMenu();
            Log.LogInfo($"{Name} {Version} has risen. Press {ToggleKey.Value} to open.");
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb != null && ToggleKey.Value != Key.None && kb[ToggleKey.Value].wasPressedThisFrame)
                SetOpen(!menu.Open);

            SpeedHack.Tick(SpeedMultiplier.Value);
        }

        private void LateUpdate()
        {
            // The game re-locks the cursor every frame, so keep freeing it while the menu is up
            if (menu.Open)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnGUI() => menu.Draw();

        private void SetOpen(bool open)
        {
            if (open)
            {
                prevLock = Cursor.lockState;
                prevVisible = Cursor.visible;
            }
            else
            {
                Cursor.lockState = prevLock;
                Cursor.visible = prevVisible;
                Config.Save();
            }
            menu.Open = open;
        }
    }
}
