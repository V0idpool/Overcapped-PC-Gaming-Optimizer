using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public sealed class SettingsData
    {
        public string BackupsPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "backups");
        public string? ActiveGameId { get; set; }
        public bool AutoFlushEnabled { get; set; } = false;
        public bool AutoFlushSfx { get; set; } = false;
        public int AutoFlushInterval { get; set; } = 15;
        public string AutoFlushUnit { get; set; } = "Minutes";
        public bool SmartFlushEnabled { get; set; } = true;
        public int SmartFlushThresholdGB { get; set; } = 2;
        public bool PCoresOnly { get; set; } = false;
        public bool AutoPowerPlan { get; set; } = false;
        public bool GameModeAndDisableDvr { get; set; } = false;
        public bool DisableFSO { get; set; } = false;
        public bool ThrottleBackgroundApps { get; set; } = false;
        public bool PauseWindowsSearchWhileGaming { get; set; } = false;
        public bool ReduceWindowsTelemetry { get; set; } = false;
        public List<string> BackgroundAppsToThrottle { get; set; } = new(AppSettings.DefaultThrottledApps);
        public bool DisableDeliveryOptimization { get; set; } = false;
        public bool FpsOverlayEnabled { get; set; } = false;
        public bool FpsOverlayShow1PercentLow { get; set; } = true;
        public bool AutoSaveSessionBenchmarks { get; set; } = true;
        public string FpsOverlayPosition { get; set; } = "Top Left";
        public int FpsOverlayCustomX { get; set; } = 24;
        public int FpsOverlayCustomY { get; set; } = 24;
        public int FpsOverlayOpacity { get; set; } = 75;
        public bool DisableNicPowerSaving { get; set; } = false;
        public bool DisableInterruptModeration { get; set; } = false;
        public bool EnableNicRss { get; set; } = true;
        public string LastPingServer { get; set; } = "1.1.1.1";
        public bool CheckForUpdates { get; set; } = true;
        public List<GameProfile> Games { get; set; } = new();
        public bool StartAsAdmin { get; set; } = false;
        public bool HotkeysEnabled { get; set; } = true;
        public string HotkeyToggleFpsOverlay { get; set; } = "Ctrl+Shift+O";
        public string HotkeyToggle1PercentLow { get; set; } = "Ctrl+Shift+L";
        public string HotkeyFlushRam { get; set; } = "Ctrl+Shift+R";
        public string HotkeyToggleWindow { get; set; } = "Ctrl+Shift+M";
        public int DashboardOverlayOpacity { get; set; } = 85;
    }

    public static class AppSettings
    {
        public static readonly string[] DefaultThrottledApps =
        {
            "chrome",
            "msedge",
            "firefox",
            "brave",
            "opera",
            "opera_gx",
            "steamwebhelper",
            "epicgameslauncher"
        };

        public static string DataDirectory { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Overcapped Optimizer");

        public static string SettingsFilePath { get; } = Path.Combine(DataDirectory, "settings.json");
        public static string PacksDirectory { get; } = Path.Combine(DataDirectory, "Profiles");
        public static string BenchmarksDirectory { get; } = Path.Combine(DataDirectory, "Benchmarks");
        public static string RegistrySnapshotFilePath { get; } = Path.Combine(DataDirectory, "registry_snapshot.json");

        public static SettingsData Current { get; private set; } = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static string NormalizeProcessName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string trimmed = raw.Trim().Trim('"');
            if (trimmed.Contains(Path.DirectorySeparatorChar) || trimmed.Contains(Path.AltDirectorySeparatorChar))
            {
                trimmed = Path.GetFileName(trimmed);
            }

            if (trimmed.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed[..^4];
            }

            return trimmed.Trim();
        }

        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(DataDirectory);
                Directory.CreateDirectory(PacksDirectory);
                Directory.CreateDirectory(BenchmarksDirectory);

                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    Current = JsonSerializer.Deserialize<SettingsData>(json, JsonOptions) ?? new SettingsData();

                    if (Current.BackgroundAppsToThrottle == null)
                    {
                        Current.BackgroundAppsToThrottle = new List<string>(DefaultThrottledApps);
                    }
                    else
                    {
                        Current.BackgroundAppsToThrottle = Current.BackgroundAppsToThrottle
                            .Select(NormalizeProcessName)
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();
                    }

                    Current.Games ??= new List<GameProfile>();
                }
                else
                {
                    Current = new SettingsData();
                    Save();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                Current = new SettingsData();
            }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(DataDirectory);
                Directory.CreateDirectory(BenchmarksDirectory);
                string json = JsonSerializer.Serialize(Current, JsonOptions);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
            }
        }
    }
}