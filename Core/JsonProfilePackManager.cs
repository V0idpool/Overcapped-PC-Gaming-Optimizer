using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        public string ConfigFormat { get; set; } = "SpaceDelimited";
        public string RelativeConfigPath { get; set; } = string.Empty;
        public string IniSection { get; set; } = "SystemSettings";
        public Dictionary<string, string> UniversalSettings { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> Presets { get; set; } = new();
    }

    public static class JsonProfilePackManager
    {
        public static readonly string[] SupportedConfigFormats =
        {
            "SpaceDelimited",
            "Ini",
            "KeyValueEquals",
            "Json"
        };

        public static readonly string[] DynamicTokens =
        {
            "{AUTO_GC_BUFFER}",
            "{AUTO_VRAM_POOL}",
            "{FPS_LIMIT}",
            "{MENU_FPS_LIMIT}",
            "{SCREEN_MODE}",
            "{NVIDIA_REFLEX}"
        };

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
                            pack.UniversalSettings ??= new Dictionary<string, string>();
                            pack.Presets ??= new Dictionary<string, Dictionary<string, string>>();
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
            {
                var byId = LoadedPacks.FirstOrDefault(p => p.PackId.Equals(game.JsonPackId, StringComparison.OrdinalIgnoreCase));
                if (byId != null) return byId;
            }

            return LoadedPacks.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.MatchExecutable) &&
                p.MatchExecutable.Equals(game.ExecutableName, StringComparison.OrdinalIgnoreCase));
        }

        public static string ResolvePackConfigPath(GameProfile game, GameProfilePack pack)
        {
            if (!string.IsNullOrWhiteSpace(pack.RelativeConfigPath))
            {
                string raw = pack.RelativeConfigPath.Trim();
                string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string localLow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow");

                string expanded = raw
                    .Replace("{DOCUMENTS}", myDocs, StringComparison.OrdinalIgnoreCase)
                    .Replace("{LOCALLOW}", localLow, StringComparison.OrdinalIgnoreCase);

                expanded = Environment.ExpandEnvironmentVariables(expanded);

                if (Path.IsPathRooted(expanded))
                    return expanded;

                if (!string.IsNullOrWhiteSpace(game.InstallDirectory))
                    return Path.Combine(game.InstallDirectory, expanded);

                return expanded;
            }

            string fallbackFile = pack.ConfigFormat.ToLowerInvariant() switch
            {
                "ini" => "Engine.ini",
                "json" => "settings.json",
                "keyvalueequals" => "settings.cfg",
                _ => "config.cfg"
            };

            return !string.IsNullOrWhiteSpace(game.ConfigDirectory)
                ? Path.Combine(game.ConfigDirectory, fallbackFile)
                : Path.Combine(game.InstallDirectory, fallbackFile);
        }

        public static string MakePortableConfigPath(string fullFilePath, string? gameInstallDir)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath)) return string.Empty;

            string normalized = Path.GetFullPath(fullFilePath);

            if (!string.IsNullOrWhiteSpace(gameInstallDir) && Directory.Exists(gameInstallDir))
            {
                string normInstall = Path.GetFullPath(gameInstallDir).TrimEnd('\\') + "\\";
                if (normalized.StartsWith(normInstall, StringComparison.OrdinalIgnoreCase))
                    return normalized[normInstall.Length..];
            }

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd('\\');
            if (!string.IsNullOrWhiteSpace(localAppData) &&
                normalized.StartsWith(localAppData + "\\", StringComparison.OrdinalIgnoreCase))
            {
                return "%LOCALAPPDATA%" + normalized[localAppData.Length..];
            }

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\');
            if (!string.IsNullOrWhiteSpace(appData) &&
                normalized.StartsWith(appData + "\\", StringComparison.OrdinalIgnoreCase))
            {
                return "%APPDATA%" + normalized[appData.Length..];
            }

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).TrimEnd('\\');
            string localLow = Path.Combine(userProfile, "AppData", "LocalLow").TrimEnd('\\');
            if (!string.IsNullOrWhiteSpace(localLow) &&
                normalized.StartsWith(localLow + "\\", StringComparison.OrdinalIgnoreCase))
            {
                return "{LOCALLOW}" + normalized[localLow.Length..];
            }

            string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\');
            if (!string.IsNullOrWhiteSpace(myDocs) &&
                normalized.StartsWith(myDocs + "\\", StringComparison.OrdinalIgnoreCase))
            {
                return "{DOCUMENTS}" + normalized[myDocs.Length..];
            }

            if (!string.IsNullOrWhiteSpace(userProfile) &&
                normalized.StartsWith(userProfile + "\\", StringComparison.OrdinalIgnoreCase))
            {
                return "%USERPROFILE%" + normalized[userProfile.Length..];
            }

            return normalized;
        }

        public static string SanitizePackId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "custom_pack";

            var chars = raw.Trim().ToLowerInvariant()
                .Select(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' ? c : '_')
                .ToArray();

            string cleaned = new string(chars).Trim('_');
            while (cleaned.Contains("__")) cleaned = cleaned.Replace("__", "_");
            return string.IsNullOrWhiteSpace(cleaned) ? "custom_pack" : cleaned;
        }

        public static string SerializePack(GameProfilePack pack) =>
            JsonSerializer.Serialize(pack, JsonOptions);

        public static GameProfilePack ClonePack(GameProfilePack source)
        {
            string json = SerializePack(source);
            var clone = JsonSerializer.Deserialize<GameProfilePack>(json, JsonOptions) ?? new GameProfilePack();
            clone.UniversalSettings ??= new Dictionary<string, string>();
            clone.Presets ??= new Dictionary<string, Dictionary<string, string>>();
            return clone;
        }

        public static GameProfilePack LoadPackFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("JSON profile pack file not found.", filePath);

            string json = File.ReadAllText(filePath);
            var pack = JsonSerializer.Deserialize<GameProfilePack>(json, JsonOptions)
                ?? throw new InvalidDataException("The selected file is not a valid Overcapped JSON profile pack.");

            pack.UniversalSettings ??= new Dictionary<string, string>();
            pack.Presets ??= new Dictionary<string, Dictionary<string, string>>();
            if (string.IsNullOrWhiteSpace(pack.PackId))
                pack.PackId = SanitizePackId(Path.GetFileNameWithoutExtension(filePath));
            if (string.IsNullOrWhiteSpace(pack.ConfigFormat))
                pack.ConfigFormat = "SpaceDelimited";
            if (string.IsNullOrWhiteSpace(pack.IniSection))
                pack.IniSection = "SystemSettings";

            return pack;
        }

        public static string SavePackToLibrary(GameProfilePack pack)
        {
            Directory.CreateDirectory(AppSettings.PacksDirectory);
            pack.PackId = SanitizePackId(string.IsNullOrWhiteSpace(pack.PackId) ? pack.GameName : pack.PackId);

            string filePath = Path.Combine(AppSettings.PacksDirectory, $"{pack.PackId}.json");
            File.WriteAllText(filePath, SerializePack(pack));
            LoadAllPacks();
            return filePath;
        }

        public static void ExportPackToFile(GameProfilePack pack, string filePath)
        {
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, SerializePack(pack));
        }

        public static List<string> ReadIniSections(string filePath)
        {
            var sections = new List<string>();
            if (!File.Exists(filePath)) return sections;

            foreach (string rawLine in File.ReadAllLines(filePath))
            {
                string line = rawLine.Trim();
                if (line.Length > 2 && line.StartsWith("[") && line.EndsWith("]"))
                {
                    string name = line[1..^1].Trim();
                    if (!string.IsNullOrWhiteSpace(name) &&
                        !sections.Any(s => s.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        sections.Add(name);
                    }
                }
            }

            return sections;
        }

        public static Dictionary<string, string> ParseConfigFile(string filePath, string configFormat, string? defaultIniSection)
        {
            var entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(filePath)) return entries;

            string ext = Path.GetExtension(filePath);

            if (configFormat.Equals("Json", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(filePath));
                    FlattenJsonElement(doc.RootElement, string.Empty, entries);
                }
                catch (Exception ex) { ExceptionHandler.LogError(ex); }
                return entries;
            }

            string[] lines = File.ReadAllLines(filePath);

            if (configFormat.Equals("Ini", StringComparison.OrdinalIgnoreCase))
            {
                string currentSection = string.Empty;
                string normalizedDefault = (defaultIniSection ?? string.Empty).Trim().Trim('[', ']');

                foreach (string rawLine in lines)
                {
                    string line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#") || line.StartsWith("//"))
                        continue;

                    if (line.Length > 2 && line.StartsWith("[") && line.EndsWith("]"))
                    {
                        currentSection = line[1..^1].Trim();
                        continue;
                    }

                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;

                    string key = line[..eq].Trim();
                    string val = line[(eq + 1)..].Trim().Trim('"');
                    if (string.IsNullOrWhiteSpace(key)) continue;

                    if (currentSection.Equals("Core.System", StringComparison.OrdinalIgnoreCase) &&
                        key.Equals("Paths", StringComparison.OrdinalIgnoreCase))
                        continue;

                    bool useSectionPrefix = !string.IsNullOrWhiteSpace(currentSection) &&
                                            !currentSection.Equals(normalizedDefault, StringComparison.OrdinalIgnoreCase);

                    string entryKey = useSectionPrefix ? $"[{currentSection}]{key}" : key;
                    entries[entryKey] = val;
                }

                return entries;
            }

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line.StartsWith("#") || line.StartsWith(";"))
                    continue;

                if (line.Contains('=') && !line.StartsWith("bind ", StringComparison.OrdinalIgnoreCase))
                {
                    int eq = line.IndexOf('=');
                    if (eq > 0)
                    {
                        string key = line[..eq].Trim();
                        string val = line[(eq + 1)..].Trim().Trim('"');
                        if (!string.IsNullOrWhiteSpace(key))
                            entries[key] = val;
                        continue;
                    }
                }

                string[] parts = line.Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string val = parts[1].Trim().Trim('"');
                    if (!string.IsNullOrWhiteSpace(key))
                        entries[key] = val;
                }
            }

            return entries;
        }

        private static void FlattenJsonElement(JsonElement element, string prefix, Dictionary<string, string> output)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty prop in element.EnumerateObject())
                    {
                        string nextPrefix = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                        FlattenJsonElement(prop.Value, nextPrefix, output);
                    }
                    break;

                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    if (!string.IsNullOrEmpty(prefix))
                        output[prefix] = element.ToString();
                    break;
            }
        }

        public static void ApplyPack(GameProfile game, GameProfilePack pack)
        {
            string configPath = ResolvePackConfigPath(game, pack);

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
                string defaultSection = string.IsNullOrWhiteSpace(pack.IniSection) ? "SystemSettings" : pack.IniSection;
                var sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

                foreach (var kvp in merged)
                {
                    string targetSection = defaultSection;
                    string targetKey = kvp.Key;

                    if (targetKey.StartsWith("["))
                    {
                        int closeBracket = targetKey.IndexOf(']');
                        if (closeBracket > 1 && closeBracket < targetKey.Length - 1)
                        {
                            targetSection = targetKey[1..closeBracket].Trim();
                            targetKey = targetKey[(closeBracket + 1)..].Trim();
                        }
                    }

                    if (!sections.TryGetValue(targetSection, out var sectionDict))
                    {
                        sectionDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        sections[targetSection] = sectionDict;
                    }

                    sectionDict[targetKey] = kvp.Value;
                }

                UnrealEngineModule.MergeIniSections(configPath, sections);
            }
            else if (pack.ConfigFormat.Equals("KeyValueEquals", StringComparison.OrdinalIgnoreCase))
            {
                ApplyKeyValueEqualsConfig(configPath, merged);
            }
            else if (pack.ConfigFormat.Equals("Json", StringComparison.OrdinalIgnoreCase))
            {
                ApplyJsonConfig(configPath, merged);
            }
            else
            {
                ApplySpaceDelimitedConfig(configPath, merged);
            }
        }

        private static void ApplyJsonConfig(string filePath, Dictionary<string, string> updates)
        {
            JsonObject root;

            if (File.Exists(filePath))
            {
                string backup = filePath + ".universal.bak";
                if (!File.Exists(backup))
                    File.Copy(filePath, backup, overwrite: false);

                try
                {
                    root = JsonNode.Parse(File.ReadAllText(filePath)) as JsonObject ?? new JsonObject();
                }
                catch
                {
                    root = new JsonObject();
                }
            }
            else
            {
                root = new JsonObject();
            }

            foreach (var kvp in updates)
            {
                string[] segments = kvp.Key.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length == 0) continue;

                JsonObject cursor = root;
                for (int i = 0; i < segments.Length - 1; i++)
                {
                    string seg = FindExistingJsonKey(cursor, segments[i]) ?? segments[i];
                    if (cursor[seg] is not JsonObject nextObj)
                    {
                        nextObj = new JsonObject();
                        cursor[seg] = nextObj;
                    }
                    cursor = nextObj;
                }

                string leafKey = FindExistingJsonKey(cursor, segments[^1]) ?? segments[^1];
                cursor[leafKey] = CreateTypedJsonValue(kvp.Value, cursor[leafKey]);
            }

            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, root.ToJsonString(JsonOptions));
        }

        private static string? FindExistingJsonKey(JsonObject obj, string targetKey)
        {
            foreach (var kvp in obj)
            {
                if (kvp.Key.Equals(targetKey, StringComparison.OrdinalIgnoreCase))
                    return kvp.Key;
            }
            return null;
        }

        private static JsonNode CreateTypedJsonValue(string rawValue, JsonNode? existingNode)
        {
            if (existingNode is JsonValue existingVal)
            {
                JsonElement el = existingVal.GetValue<JsonElement>();
                if (el.ValueKind == JsonValueKind.True || el.ValueKind == JsonValueKind.False)
                {
                    if (bool.TryParse(rawValue, out bool b)) return JsonValue.Create(b)!;
                    if (rawValue == "1") return JsonValue.Create(true)!;
                    if (rawValue == "0") return JsonValue.Create(false)!;
                }
                else if (el.ValueKind == JsonValueKind.Number)
                {
                    if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l))
                        return JsonValue.Create(l)!;
                    if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                        return JsonValue.Create(d)!;
                }
                else if (el.ValueKind == JsonValueKind.String)
                {
                    return JsonValue.Create(rawValue)!;
                }
            }

            if (bool.TryParse(rawValue, out bool parsedBool))
                return JsonValue.Create(parsedBool)!;
            if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsedLong))
                return JsonValue.Create(parsedLong)!;
            if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedDouble))
                return JsonValue.Create(parsedDouble)!;

            return JsonValue.Create(rawValue)!;
        }

        private static void ApplyKeyValueEqualsConfig(string filePath, Dictionary<string, string> updates)
        {
            List<string> lines = File.Exists(filePath)
                ? File.ReadAllLines(filePath).ToList()
                : new List<string>();

            if (File.Exists(filePath))
                File.Copy(filePath, filePath + ".universal.bak", overwrite: true);

            var remaining = new Dictionary<string, string>(updates, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].Trim();
                int eq = trimmed.IndexOf('=');
                if (eq <= 0 || trimmed.StartsWith(";") || trimmed.StartsWith("#") || trimmed.StartsWith("//"))
                    continue;

                string key = trimmed[..eq].Trim();
                if (remaining.TryGetValue(key, out string? newVal))
                {
                    lines[i] = $"{key}={newVal}";
                    remaining.Remove(key);
                }
            }

            foreach (var kvp in remaining)
                lines.Add($"{kvp.Key}={kvp.Value}");

            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllLines(filePath, lines);
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