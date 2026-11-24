using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public sealed class GameProfilePack
    {
        public string PackId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public string MatchExecutable { get; set; } = string.Empty;
        public string ConfigFormat { get; set; } = "SpaceDelimited"; // "SpaceDelimited" or "Ini"
        public string RelativeConfigPath { get; set; } = string.Empty;
        public string IniSection { get; set; } = "SystemSettings";
        public Dictionary<string, string> UniversalSettings { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> Presets { get; set; } = new();
    }

    public static class JsonProfilePackManager
    {
        private static readonly List<GameProfilePack> LoadedPacks = new();
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static IReadOnlyList<GameProfilePack> Packs => LoadedPacks;

        public static void LoadAllPacks()
        {
            LoadedPacks.Clear();
            Directory.CreateDirectory(AppSettings.PacksDirectory);
            EnsureStarterRustPack();

            var searchDirs = new[]
            {
                AppSettings.PacksDirectory,
                Path.Combine(AppContext.BaseDirectory, "Profiles")
            };

            foreach (string dir in searchDirs.Where(Directory.Exists))
            {
                foreach (string file in Directory.GetFiles(dir, "*.json"))
                {
                    try
                    {
                        var pack = JsonSerializer.Deserialize<GameProfilePack>(File.ReadAllText(file), JsonOptions);
                        if (pack != null && !string.IsNullOrWhiteSpace(pack.PackId) &&
                            !LoadedPacks.Any(p => p.PackId.Equals(pack.PackId, StringComparison.OrdinalIgnoreCase)))
                        {
                            LoadedPacks.Add(pack);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogError(ex);
                    }
                }
            }
        }

        public static GameProfilePack? FindMatchingPack(GameProfile game)
        {
            if (!string.IsNullOrWhiteSpace(game.JsonPackId))
                return LoadedPacks.FirstOrDefault(p => p.PackId.Equals(game.JsonPackId, StringComparison.OrdinalIgnoreCase));

            return LoadedPacks.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.MatchExecutable) &&
                p.MatchExecutable.Equals(game.ExecutableName, StringComparison.OrdinalIgnoreCase));
        }

        public static void ApplyPack(GameProfile game, GameProfilePack pack)
        {
            string configPath = !string.IsNullOrWhiteSpace(pack.RelativeConfigPath)
                ? Path.Combine(game.InstallDirectory, pack.RelativeConfigPath)
                : Path.Combine(game.ConfigDirectory, "config.cfg");

            var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in pack.UniversalSettings)
            {
                string? val = ResolveDynamicToken(kvp.Value, game);
                if (val != null) merged[kvp.Key] = val;
            }

            if (pack.Presets.TryGetValue(game.SelectedPreset, out var presetDict))
            {
                foreach (var kvp in presetDict)
                {
                    string? val = ResolveDynamicToken(kvp.Value, game);
                    if (val != null) merged[kvp.Key] = val;
                }
            }

            if (pack.ConfigFormat.Equals("Ini", StringComparison.OrdinalIgnoreCase))
            {
                UnrealEngineModule.MergeIniSections(configPath, new Dictionary<string, Dictionary<string, string>>
                {
                    [pack.IniSection] = merged
                });
            }
            else
            {
                ApplySpaceDelimitedConfig(configPath, merged);
            }
        }

        private static string? ResolveDynamicToken(string rawValue, GameProfile game)
        {
            return rawValue switch
            {
                "{AUTO_GC_BUFFER}" => HardwareDetector.GetTotalMemoryInGB() >= 24.0
                    ? "4096"
                    : (HardwareDetector.GetTotalMemoryInGB() >= 12.0 ? "2048" : "1024"),
                "{AUTO_VRAM_POOL}" => UnrealEngineModule.CalculateTexturePoolSizeMb(HardwareDetector.GetGpuVramInGB()).ToString(CultureInfo.InvariantCulture),
                "{FPS_LIMIT}" => game.FpsLimit.ToString(CultureInfo.InvariantCulture),
                "{MENU_FPS_LIMIT}" => game.MenuFpsLimit.ToString(CultureInfo.InvariantCulture),
                "{SCREEN_MODE}" => game.ScreenMode.ToString(CultureInfo.InvariantCulture),
                "{NVIDIA_REFLEX}" => Optimizer.IsNvidiaGpu(HardwareDetector.GetGpuName()) ? "2" : null,
                _ => rawValue
            };
        }

        private static void ApplySpaceDelimitedConfig(string filePath, Dictionary<string, string> updates)
        {
            var existing = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (File.Exists(filePath))
            {
                File.Copy(filePath, filePath + ".universal.bak", overwrite: true);
                foreach (string line in File.ReadAllLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;
                    string[] parts = line.Split(new[] { ' ' }, 2);
                    if (parts.Length == 2)
                        existing[parts[0].Trim()] = parts[1].Trim().Trim('"');
                }
            }

            foreach (var kvp in updates)
                existing[kvp.Key] = kvp.Value;

            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            using var writer = new StreamWriter(filePath);
            foreach (var kvp in existing)
                writer.WriteLine($"{kvp.Key} \"{kvp.Value}\"");
        }
        // Because Rust is life
        private static void EnsureStarterRustPack()
        {
            string rustPackPath = Path.Combine(AppSettings.PacksDirectory, "rust.json");
            if (File.Exists(rustPackPath)) return;

            var rustPack = new GameProfilePack
            {
                PackId = "rust",
                GameName = "Rust",
                MatchExecutable = "RustClient.exe",
                ConfigFormat = "SpaceDelimited",
                RelativeConfigPath = Path.Combine("cfg", "client.cfg"),
                UniversalSettings = new Dictionary<string, string>
                {
                    ["graphics.vsync"] = "False",
                    ["client.headbob"] = "False",
                    ["effects.sharpen"] = "False",
                    ["effects.motionblur"] = "False",
                    ["effects.antialiasing"] = "0",
                    ["effects.maxgibs"] = "0",
                    ["global.showblood"] = "True",
                    ["gc.buffer"] = "{AUTO_GC_BUFFER}",
                    ["gc.incremental_milliseconds"] = "1",
                    ["graphics.screenmode"] = "{SCREEN_MODE}",
                    ["fps.limit"] = "{FPS_LIMIT}",
                    ["fps.limitinmenu"] = "{MENU_FPS_LIMIT}",
                    ["graphics.reflex"] = "{NVIDIA_REFLEX}"
                },
                Presets = new Dictionary<string, Dictionary<string, string>>
                {
                    ["Competitive (Max FPS)"] = new()
                    {
                        ["graphics.drawdistance"] = "500",
                        ["graphics.lodbias"] = "1",
                        ["graphics.af"] = "1",
                        ["graphics.shadowquality"] = "0",
                        ["graphicssettings.shadowcascades"] = "0",
                        ["tree.meshes"] = "0",
                        ["effects.ao"] = "False",
                        ["grass.quality"] = "0",
                        ["particle.quality"] = "0",
                        ["water.quality"] = "0",
                        ["water.reflections"] = "0"
                    },
                    ["Balanced (Good Performance & Looks)"] = new()
                    {
                        ["graphics.drawdistance"] = "1000",
                        ["graphics.lodbias"] = "1.5",
                        ["graphics.af"] = "2",
                        ["graphics.shadowquality"] = "1",
                        ["graphicssettings.shadowcascades"] = "1",
                        ["tree.meshes"] = "30",
                        ["effects.ao"] = "True",
                        ["grass.quality"] = "25",
                        ["particle.quality"] = "25",
                        ["water.quality"] = "1",
                        ["water.reflections"] = "1"
                    },
                    ["Recommended (Optimized)"] = new()
                    {
                        ["graphics.drawdistance"] = "1500",
                        ["graphics.lodbias"] = "2.0",
                        ["graphics.af"] = "4",
                        ["graphics.shadowquality"] = "1",
                        ["graphicssettings.shadowcascades"] = "1",
                        ["tree.meshes"] = "50",
                        ["effects.ao"] = "True",
                        ["grass.quality"] = "50",
                        ["particle.quality"] = "50",
                        ["water.quality"] = "1",
                        ["water.reflections"] = "1"
                    },
                    ["Ultra (Maximum Visuals)"] = new()
                    {
                        ["graphics.drawdistance"] = "2500",
                        ["graphics.lodbias"] = "5",
                        ["graphics.af"] = "16",
                        ["graphics.shadowquality"] = "3",
                        ["graphicssettings.shadowcascades"] = "2",
                        ["graphics.impostorshadows"] = "True",
                        ["effects.antialiasing"] = "2",
                        ["tree.meshes"] = "100",
                        ["effects.ao"] = "True",
                        ["grass.quality"] = "100",
                        ["particle.quality"] = "100",
                        ["water.quality"] = "2",
                        ["water.reflections"] = "2"
                    }
                }
            };

            File.WriteAllText(rustPackPath, JsonSerializer.Serialize(rustPack, JsonOptions));
        }
    }
}