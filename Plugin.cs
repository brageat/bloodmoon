using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine.InputSystem;

namespace Bloodmoon
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "com.brageat.bloodmoon";
        public const string Name = "Bloodmoon";
        public const string Version = "0.2.0";

        internal static ManualLogSource Log;
        internal static ConfigEntry<Key> ToggleKey;
        internal static ConfigEntry<float> SpeedMultiplier;

        private BloodmoonMenu menu;

        private void Awake()
        {
            Log = Logger;

            // Save manually when the menu closes instead of on every change
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
            if (kb != null)
            {
                if (ToggleKey.Value != Key.None && kb[ToggleKey.Value].wasPressedThisFrame)
                {
                    menu.Open = !menu.Open;
                    if (!menu.Open) Config.Save();
                }

                if (menu.Open)
                {
                    try { menu.HandleInput(kb); }
                    catch (System.Exception e) { LogOnce("input", e); }
                }
            }

            try { SpeedHack.Tick(SpeedMultiplier.Value); }
            catch (System.Exception e) { LogOnce("speed", e); }
        }

        private void OnGUI()
        {
            if (!menu.Open) return;
            try { menu.Draw(); }
            catch (System.Exception e) { LogOnce("menu", e); }
        }

        // Unity's own log writer fails in PEAK, so errors must go through our logger or they vanish
        private readonly HashSet<string> loggedErrors = new HashSet<string>();
        private void LogOnce(string where, System.Exception e)
        {
            if (loggedErrors.Add(where)) Log.LogError($"[{where}] {e}");
        }
    }
}
