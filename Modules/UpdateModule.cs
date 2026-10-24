using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;

namespace Bloodmoon
{
    // Asks GitHub for the newest Bloodmoon release and says so when it's newer than what's installed
    internal sealed class UpdateModule : Module
    {
        public override string Name => "Updates";

        private const string ReleasesPage = "https://github.com/brageat/bloodmoon/releases/latest";

        // Read by the menu header and the startup notice
        public static string LatestVersion { get; private set; }
        public static bool UpdateAvailable { get; private set; }

        private ConfigEntry<bool> checkOnStart;
        private ConfigEntry<string> apiUrl;
        private ButtonItem statusRow;
        private bool started;
        private bool checking;

        public override void Init(ConfigFile config, List<MenuItem> items)
        {
            checkOnStart = config.Bind("Updates", "CheckOnStart", true, "Check for a new Bloodmoon version when the game starts");
            apiUrl = config.Bind("Updates", "ReleaseApiUrl",
                "https://api.github.com/repos/brageat/bloodmoon/releases/latest",
                "GitHub API address of the latest release");

            statusRow = new ButtonItem
            {
                Category = Category.Misc,
                Name = "Update: not checked yet",
                Description = "Shows whether a newer Bloodmoon is out. Press Enter to open the download page.",
                OnPress = () => Application.OpenURL(ReleasesPage),
            };
            items.Add(statusRow);
            items.Add(new ButtonItem
            {
                Category = Category.Misc,
                Name = "Check for updates",
                Description = "Asks GitHub again for the newest version.",
                OnPress = StartCheck,
            });
        }

        public override void Tick()
        {
            if (started) return;
            started = true;
            if (checkOnStart.Value) StartCheck();
        }

        private void StartCheck()
        {
            if (checking) return;
            Plugin.Runner.StartCoroutine(Check());
        }

        private IEnumerator Check()
        {
            checking = true;
            statusRow.Name = "Update: checking…";

            // 1) GitHub's API. It only allows 60 checks an hour per internet address, which a
            //    shared address (VPN, school) can use up, so...
            string found = null, apiError = null;
            yield return Fetch(apiUrl.Value, "application/vnd.github+json",
                text => found = Match(text, "\"tag_name\"\\s*:\\s*\"v?([0-9][0-9.]*)\""),
                err => apiError = err);

            // 2) ...fall back to the normal releases page, which redirects to the newest tag
            if (found == null)
            {
                Plugin.Log.LogInfo($"Update check: API failed ({apiError}), trying the releases page");
                string pageError = null;
                yield return Fetch(ReleasesPage, "text/html",
                    text => found = Match(text, "/releases/tag/v?([0-9][0-9.]*)"),
                    err => pageError = err);

                if (found == null)
                {
                    statusRow.Name = "Update: couldn't check";
                    statusRow.Description = $"Couldn't reach GitHub ({pageError ?? apiError ?? "no version found"}). Press Enter to open the download page.";
                    Plugin.Log.LogInfo($"Update check failed: {pageError ?? apiError}");
                    checking = false;
                    yield break;
                }
            }

            {
                LatestVersion = found;
                UpdateAvailable = IsNewer(LatestVersion, Plugin.Version);

                if (UpdateAvailable)
                {
                    statusRow.Name = $"UPDATE: v{LatestVersion} is out!";
                    statusRow.Description = $"You have v{Plugin.Version}. Press Enter to open the download page for v{LatestVersion}.";
                    Plugin.Log.LogWarning($"A new Bloodmoon is out: v{LatestVersion} (you have v{Plugin.Version})");
                }
                else
                {
                    statusRow.Name = $"Up to date (v{Plugin.Version})";
                    statusRow.Description = "You have the newest Bloodmoon. Press Enter to open the releases page.";
                    Plugin.Log.LogInfo($"Bloodmoon is up to date (v{Plugin.Version})");
                }
            }
            checking = false;
        }

        private static IEnumerator Fetch(string url, string accept, Action<string> onText, Action<string> onError)
        {
            using (var req = UnityWebRequest.Get(url))
            {
                req.SetRequestHeader("User-Agent", "Bloodmoon/" + Plugin.Version); // GitHub requires one
                req.SetRequestHeader("Accept", accept);
                req.timeout = 15;
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success) onText(req.downloadHandler.text);
                else onError($"{req.responseCode} {req.error}");
            }
        }

        private static string Match(string text, string pattern)
        {
            var m = Regex.Match(text, pattern);
            return m.Success ? m.Groups[1].Value.TrimEnd('.') : null;
        }

        private static bool IsNewer(string latest, string current)
        {
            return Version.TryParse(latest, out var a) && Version.TryParse(current, out var b) && a > b;
        }
    }
}
