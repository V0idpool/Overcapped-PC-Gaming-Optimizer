using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public sealed class UniversalSettingsData
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
        public bool FpsOverlayEnabled { get; set; } = false;
        public string FpsOverlayPosition { get; set; } = "Top Left";
        public int FpsOverlayCustomX { get; set; } = 24;
        public int FpsOverlayCustomY { get; set; } = 24;
        public int FpsOverlayOpacity { get; set; } = 75;
        public bool DisableNicPowerSaving { get; set; } = false;
        public string LastPingServer { get; set; } = "1.1.1.1";
        public bool CheckForUpdates { get; set; } = true;
        public List<GameProfile> Games { get; set; } = new();
    }

    public static class AppSettings
    {
        public static string DataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Overcapped Optimizer");

        public static string SettingsFilePath { get; } = Path.Combine(DataDirectory, "settings.json");
        public static string PacksDirectory { get; } = Path.Combine(DataDirectory, "Profiles");

        public static UniversalSettingsData Current { get; private set; } = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(DataDirectory);
                Directory.CreateDirectory(PacksDirectory);

                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    Current = JsonSerializer.Deserialize<UniversalSettingsData>(json, JsonOptions) ?? new UniversalSettingsData();
                }
                else
                {
                    Current = new UniversalSettingsData();
                    Save();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                Current = new UniversalSettingsData();
            }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(DataDirectory);
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