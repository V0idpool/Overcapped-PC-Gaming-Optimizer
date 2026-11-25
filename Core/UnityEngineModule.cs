using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public static class UnityEngineModule
    {
        public static string[] AvailablePresets => new[]
        {
            "Performance (Max FPS)",
            "Balanced (Quality & Speed)",
            "Recommended (Optimized)",
            "Ultra (Maximum Visuals)"
        };

        private static readonly string[] AntiCheatMarkers =
        {
            "EasyAntiCheat",
            "EasyAntiCheat_EOS",
            "BattlEye",
            "EAAntiCheat",
            "start_protected_game.exe",
            "EasyAntiCheat_EOS_Setup.exe",
            "BEService_x64.exe"
        };

        private static readonly HashSet<string> ManagedStandaloneFlags = new(StringComparer.OrdinalIgnoreCase)
        {
            "-popupwindow"
        };

        private static readonly HashSet<string> ManagedPairedFlags = new(StringComparer.OrdinalIgnoreCase)
        {
            "-window-mode",
            "-screen-fullscreen"
        };

        public static string? FindDataDirectory(string installDir, string? exeName = null)
        {
            if (string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                return null;

            try
            {
                if (!string.IsNullOrWhiteSpace(exeName))
                {
                    string stem = Path.GetFileNameWithoutExtension(exeName);
                    string exact = Path.Combine(installDir, $"{stem}_Data");
                    if (Directory.Exists(exact)) return exact;
                }

                return Directory.EnumerateDirectories(installDir, "*_Data", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public static string ResolveLocalLowConfigDirectory(string installDir, string? exeName = null)
        {
            string? dataDir = FindDataDirectory(installDir, exeName);
            if (string.IsNullOrWhiteSpace(dataDir)) return string.Empty;

            string appInfoPath = Path.Combine(dataDir, "app.info");
            if (!File.Exists(appInfoPath)) return string.Empty;

            try
            {
                string[] lines = File.ReadAllLines(appInfoPath)
                    .Select(l => l.Trim())
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToArray();

                if (lines.Length >= 2)
                {
                    string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string localLow = Path.Combine(userProfile, "AppData", "LocalLow", lines[0], lines[1]);
                    if (Directory.Exists(localLow)) return localLow;
                }
            }
            catch { }

            return string.Empty;
        }

        public static bool HasAntiCheat(string installDir)
        {
            if (string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                return false;

            try
            {
                foreach (string marker in AntiCheatMarkers)
                {
                    string full = Path.Combine(installDir, marker);
                    if (File.Exists(full) || Directory.Exists(full))
                        return true;
                }
            }
            catch { }

            return false;
        }

        public static string ApplyProfile(GameProfile game)
        {
            if (string.IsNullOrWhiteSpace(game.InstallDirectory) || !Directory.Exists(game.InstallDirectory))
                throw new DirectoryNotFoundException("Game install directory not found.");

            if (string.IsNullOrWhiteSpace(game.ConfigDirectory))
            {
                string detectedLocalLow = ResolveLocalLowConfigDirectory(game.InstallDirectory, game.ExecutableName);
                if (!string.IsNullOrWhiteSpace(detectedLocalLow))
                    game.ConfigDirectory = detectedLocalLow;
            }

            List<string> unityArgs = game.ScreenMode == 1
                ? new List<string> { "-window-mode", "exclusive", "-screen-fullscreen", "1" }
                : new List<string> { "-popupwindow", "-screen-fullscreen", "1" };

            game.LaunchArguments = MergeLaunchArguments(game.LaunchArguments, unityArgs);

            if (HasAntiCheat(game.InstallDirectory))
            {
                return $"Updated Unity launch arguments ({game.LaunchArguments}).\n\nSkipped *_Data\\boot.config edits automatically because anti-cheat was detected in this install folder.";
            }

            string? dataDir = FindDataDirectory(game.InstallDirectory, game.ExecutableName);
            if (string.IsNullOrWhiteSpace(dataDir))
            {
                return $"Updated Unity launch arguments ({game.LaunchArguments}).\n\nNo *_Data folder found for boot.config tuning.";
            }

            string bootConfigPath = Path.Combine(dataDir, "boot.config");
            if (!File.Exists(bootConfigPath))
            {
                return $"Updated Unity launch arguments ({game.LaunchArguments}).\n\nNo boot.config file found in {Path.GetFileName(dataDir)}.";
            }

            string backupPath = bootConfigPath + ".universal.bak";
            if (!File.Exists(backupPath))
                File.Copy(bootConfigPath, backupPath, overwrite: false);

            var updates = BuildBootConfigSettings(game.SelectedPreset);
            MergeBootConfig(bootConfigPath, updates);

            return $"Applied Unity Engine preset '{game.SelectedPreset}' to {Path.GetFileName(dataDir)}\\boot.config (multi-threaded gfx jobs & GC time-slice) and updated launch arguments.";
        }

        private static Dictionary<string, string> BuildBootConfigSettings(string preset)
        {
            string gcSliceMs = preset switch
            {
                "Performance (Max FPS)" => "2",
                "Balanced (Quality & Speed)" => "3",
                "Ultra (Maximum Visuals)" => "4",
                _ => "3"
            };

            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["gfx-enable-gfx-jobs"] = "1",
                ["gfx-enable-native-gfx-jobs"] = "1",
                ["wait-for-native-debugger"] = "0",
                ["gc-max-time-slice"] = gcSliceMs
            };
        }

        private static void MergeBootConfig(string filePath, Dictionary<string, string> updates)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();
            var remaining = new Dictionary<string, string>(updates, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].Trim();
                int eq = trimmed.IndexOf('=');
                if (eq <= 0 || trimmed.StartsWith("#") || trimmed.StartsWith(";"))
                    continue;

                string key = trimmed[..eq].Trim();
                if (remaining.TryGetValue(key, out string? val))
                {
                    lines[i] = $"{key}={val}";
                    remaining.Remove(key);
                }
            }

            foreach (var kvp in remaining)
                lines.Add($"{kvp.Key}={kvp.Value}");

            File.WriteAllLines(filePath, lines);
        }

        private static string MergeLaunchArguments(string existingArgs, List<string> newTokens)
        {
            var kept = new List<string>();
            var tokens = TokenizeArgs(existingArgs);

            for (int i = 0; i < tokens.Count; i++)
            {
                string tok = tokens[i];
                if (ManagedStandaloneFlags.Contains(tok))
                    continue;

                if (ManagedPairedFlags.Contains(tok))
                {
                    if (i + 1 < tokens.Count && !tokens[i + 1].StartsWith("-"))
                        i++;
                    continue;
                }

                kept.Add(tok);
            }

            kept.AddRange(newTokens);
            return string.Join(" ", kept);
        }

        private static List<string> TokenizeArgs(string? raw)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(raw)) return result;

            bool inQuotes = false;
            var current = new System.Text.StringBuilder();

            foreach (char c in raw)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    current.Append(c);
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        result.Add(current.ToString());
                        current.Clear();
                    }
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
                result.Add(current.ToString());

            return result;
        }
    }
}