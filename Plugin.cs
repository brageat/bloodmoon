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
        public const string Version = "1.3.0";

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
            new RespawnModule(),
            new TeleportModule(),
            new ItemsModule(),
            new StopFogModule(),
            new UpdateModule(),
            new ResetModule(),
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
                DrawToast(); // short messages from modules, shown even with the menu open
            }
            catch (System.Exception e) { LogOnce("menu", e); }
        }

        // A short red notice at the top of the screen when a new version is out
        private const float NoticeSeconds = 12f;
        private float noticeUntil = -1f;
        private UnityEngine.GUIStyle noticeStyle;

        // A short message at the top of the screen, e.g. "Teleporting to the summit..."
        private static string toastText;
        private static float toastUntil;

        internal static void Toast(string text, float seconds)
        {
            toastText = text;
            toastUntil = UnityEngine.Time.unscaledTime + seconds;
        }

        internal static void HideToast() => toastUntil = 0f;

        private UnityEngine.GUIStyle toastStyle;

        // Big and in the upper middle of the screen, so it's hard to miss
        private void DrawToast()
        {
            if (toastText == null || UnityEngine.Time.unscaledTime > toastUntil) return;

            if (toastStyle == null)
            {
                toastStyle = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.box)
                {
                    fontSize = 30,
                    fontStyle = UnityEngine.FontStyle.Bold,
                    alignment = UnityEngine.TextAnchor.MiddleCenter,
                    padding = new UnityEngine.RectOffset(36, 36, 20, 20),
                    border = new UnityEngine.RectOffset(3, 3, 3, 3),
                };
                toastStyle.normal.background = Theme.Bordered(Theme.Black, Theme.RedBright);
                toastStyle.normal.textColor = Theme.RedBright;
            }

            var size = toastStyle.CalcSize(new UnityEngine.GUIContent(toastText));
            float y = UnityEngine.Screen.height * 0.22f;
            UnityEngine.GUI.Box(new UnityEngine.Rect((UnityEngine.Screen.width - size.x) / 2f, y, size.x, size.y), toastText, toastStyle);
        }

        private void DrawUpdateNotice()
        {
            if (!UpdateModule.UpdateAvailable) return;
            if (noticeUntil < 0f) noticeUntil = UnityEngine.Time.unscaledTime + NoticeSeconds;
            if (UnityEngine.Time.unscaledTime > noticeUntil) return;
            DrawBanner($"BLOODMOON UPDATE: v{UpdateModule.LatestVersion} is out (you have v{Version}). Open the menu ({ToggleKey.Value}) > Misc to download.", 70f);
        }

        // Black box with a red border and red text, centred near the top of the screen
        private void DrawBanner(string text, float y)
        {
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

            var size = noticeStyle.CalcSize(new UnityEngine.GUIContent(text));
            UnityEngine.GUI.Box(new UnityEngine.Rect((UnityEngine.Screen.width - size.x) / 2f, y, size.x, size.y), text, noticeStyle);
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
