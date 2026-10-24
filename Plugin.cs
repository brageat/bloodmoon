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
        public const string Version = "0.9.0";

        internal static ManualLogSource Log;
        internal static ConfigEntry<Key> ToggleKey;

        // Every mod in the menu. Add new modules here.
        private readonly List<Module> Modules = new List<Module>
        {
            new SpeedModule(),
            new ClimbModule(),
            new FlyModule(),
            new StaminaModule(),
            new HealthModule(),
            new StopFogModule(),
        };

        private BloodmoonMenu menu;

        private void Awake()
        {
            Log = Logger;

            // Save manually when the menu closes instead of on every change
            Config.SaveOnConfigSet = false;
            ToggleKey = Config.Bind("General", "ToggleKey", Key.Insert, "Key that opens/closes the menu");

            var items = new List<MenuItem>();
            foreach (var module in Modules)
            {
                try { module.Init(Config, items); }
                catch (System.Exception e) { LogOnce(module.Name, e); }
            }

            menu = new BloodmoonMenu(items);
            Log.LogInfo($"{Name} {Version} has risen with {Modules.Count} module(s). Press {ToggleKey.Value} to open.");
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

            foreach (var module in Modules)
            {
                try { module.Tick(); }
                catch (System.Exception e) { LogOnce(module.Name, e); }
            }
        }

        private void FixedUpdate()
        {
            foreach (var module in Modules)
            {
                try { module.FixedTick(); }
                catch (System.Exception e) { LogOnce(module.Name + " (physics)", e); }
            }
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
