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
        public const string Version = "1.0.0";

        internal static ManualLogSource Log;
        internal static ConfigEntry<Key> ToggleKey;
        internal static bool MenuOpen => instance != null && instance.menu != null && instance.menu.Open;
        private static Plugin instance;
        internal static UnityEngine.MonoBehaviour Runner => instance; // for modules that need coroutines

        // Every mod in the menu. Add new modules here.
        private readonly List<Module> Modules = new List<Module>
        {
            new SpeedModule(),
            new ClimbModule(),
            new FlyModule(),
            new StaminaModule(),
            new HealthModule(),
            new TeleportModule(),
            new ItemsModule(),
            new StopFogModule(),
            new UpdateModule(),
        };

        private BloodmoonMenu menu;

        private void Awake()
        {
            Log = Logger;
            instance = this;

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
                    if (!menu.Open) SaveSettings();
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
            try
            {
                if (menu.Open) menu.Draw();
                else DrawUpdateNotice();
            }
            catch (System.Exception e) { LogOnce("menu", e); }
        }

        // A short red notice at the top of the screen when a new version is out
        private const float NoticeSeconds = 12f;
        private float noticeUntil = -1f;
        private UnityEngine.GUIStyle noticeStyle;

        private void DrawUpdateNotice()
        {
            if (!UpdateModule.UpdateAvailable) return;
            if (noticeUntil < 0f) noticeUntil = UnityEngine.Time.unscaledTime + NoticeSeconds;
            if (UnityEngine.Time.unscaledTime > noticeUntil) return;

            if (noticeStyle == null)
            {
                noticeStyle = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.box)
                {
                    fontSize = 15,
                    fontStyle = UnityEngine.FontStyle.Bold,
                    alignment = UnityEngine.TextAnchor.MiddleCenter,
                    padding = new UnityEngine.RectOffset(16, 16, 10, 10),
                    border = new UnityEngine.RectOffset(2, 2, 2, 2),
                };
                noticeStyle.normal.background = Theme.Bordered(Theme.Black, Theme.Red);
                noticeStyle.normal.textColor = Theme.RedBright;
            }

            var text = $"BLOODMOON UPDATE: v{UpdateModule.LatestVersion} is out (you have v{Version}). Open the menu ({ToggleKey.Value}) > Misc to download.";
            var size = noticeStyle.CalcSize(new UnityEngine.GUIContent(text));
            UnityEngine.GUI.Box(new UnityEngine.Rect((UnityEngine.Screen.width - size.x) / 2f, 20f, size.x, size.y), text, noticeStyle);
        }

        private void SaveSettings()
        {
            try
            {
                Config.Save();
                Log.LogInfo("Settings saved");
            }
            catch (System.Exception e) { LogOnce("save", e); }
        }

        // Unity's own log writer fails in PEAK, so errors must go through our logger or they vanish
        private readonly HashSet<string> loggedErrors = new HashSet<string>();
        private void LogOnce(string where, System.Exception e)
        {
            if (loggedErrors.Add(where)) Log.LogError($"[{where}] {e}");
        }
    }
}
