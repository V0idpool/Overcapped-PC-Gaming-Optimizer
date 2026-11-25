using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public static class SourceEngineModule
    {
        private const string BlockStart = "// >>> OVERCAPPED OPTIMIZER START";
        private const string BlockEnd = "// <<< OVERCAPPED OPTIMIZER END";

        public static string[] AvailablePresets => new[]
        {
            "Performance (Max FPS)",
            "Balanced (Quality & Speed)",
            "Recommended (Optimized)",
            "Ultra (Maximum Visuals)"
        };

        private static readonly string[] KnownSourceMods =
        {
            "csgo", "dota", "citadel", "tf", "left4dead2", "left4dead",
            "garrysmod", "cstrike", "hl2", "portal2", "portal", "dod", "insurgency", "nmrih"
        };

        private static readonly HashSet<string> ManagedStandaloneFlags = new(StringComparer.OrdinalIgnoreCase)
        {
            "-novid", "-fullscreen", "-noborder", "-windowed"
        };

        private static readonly HashSet<string> ManagedPairedFlags = new(StringComparer.OrdinalIgnoreCase)
        {
            "+fps_max", "+exec"
        };

        public static bool IsSource2(string installDir)
        {
            if (string.IsNullOrWhiteSpace(installDir)) return false;
            return File.Exists(Path.Combine(installDir, "game", "bin", "win64", "engine2.dll"));
        }

        public static string ResolveCfgDirectory(string installDir)
        {
            if (string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                return string.Empty;

            bool isSource2 = IsSource2(installDir);
            string searchRoot = isSource2 ? Path.Combine(installDir, "game") : installDir;
            if (!Directory.Exists(searchRoot)) searchRoot = installDir;

            foreach (string mod in KnownSourceMods)
            {
                string candidate = Path.Combine(searchRoot, mod, "cfg");
                if (Directory.Exists(candidate))
                    return candidate;
            }

            try
            {
                foreach (string subDir in Directory.EnumerateDirectories(searchRoot))
                {
                    string dirName = Path.GetFileName(subDir);
                    if (dirName.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                        dirName.Equals("core", StringComparison.OrdinalIgnoreCase) ||
                        dirName.Equals("platform", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string cfgDir = Path.Combine(subDir, "cfg");
                    if (Directory.Exists(cfgDir))
                        return cfgDir;
                }
            }
            catch { }

            string rootCfg = Path.Combine(searchRoot, "cfg");
            return Directory.Exists(rootCfg) ? rootCfg : string.Empty;
        }

        public static string ApplyProfile(GameProfile game)
        {
            bool source2 = IsSource2(game.InstallDirectory);

            string cfgDir = !string.IsNullOrWhiteSpace(game.ConfigDirectory) && Directory.Exists(game.ConfigDirectory)
                ? game.ConfigDirectory
                : ResolveCfgDirectory(game.InstallDirectory);

            if (!string.IsNullOrWhiteSpace(cfgDir))
                game.ConfigDirectory = cfgDir;

            var launchFlags = new List<string> { "-novid" };
            if (game.ScreenMode == 1)
            {
                launchFlags.Add("-fullscreen");
            }
            else
            {
                launchFlags.Add("-windowed");
                launchFlags.Add("-noborder");
            }

            launchFlags.Add("+fps_max");
            launchFlags.Add(Math.Max(0, game.FpsLimit).ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(cfgDir))
            {
                launchFlags.Add("+exec");
                launchFlags.Add("autoexec.cfg");
            }

            game.LaunchArguments = MergeLaunchArguments(game.LaunchArguments, launchFlags);

            if (string.IsNullOrWhiteSpace(cfgDir))
            {
                return $"Updated {(source2 ? "Source 2" : "Source")} launch arguments ({game.LaunchArguments}).\n\nSelect the game's cfg folder to also generate autoexec.cfg.";
            }

            string autoexecPath = Path.Combine(cfgDir, "autoexec.cfg");
            if (File.Exists(autoexecPath))
            {
                string backup = autoexecPath + ".universal.bak";
                if (!File.Exists(backup))
                    File.Copy(autoexecPath, backup, overwrite: false);
            }

            List<string> blockLines = BuildAutoexecLines(game, source2);
            WriteManagedAutoexecBlock(autoexecPath, blockLines);

            return $"Applied {(source2 ? "Source 2" : "Source")} preset '{game.SelectedPreset}' to autoexec.cfg and updated launch arguments ({game.LaunchArguments}).";
        }

        private static List<string> BuildAutoexecLines(GameProfile game, bool source2)
        {
            int fpsLimit = Math.Max(0, game.FpsLimit);
            var lines = new List<string>
            {
                BlockStart,
                $"// Preset: {game.SelectedPreset}",
                $"fps_max \"{fpsLimit}\""
            };

            if (source2)
            {
                lines.Add($"fps_max_ui \"{Math.Max(30, game.MenuFpsLimit)}\"");
                lines.Add("engine_low_latency_sleep_after_client_tick \"1\"");

                if (game.SelectedPreset == "Performance (Max FPS)")
                {
                    lines.Add("r_drawtracers_firstperson \"0\"");
                    lines.Add("cl_ragdoll_physics_enable \"0\"");
                }
            }
            else
            {
                lines.Add("mat_vsync \"0\"");
                lines.Add("mat_queue_mode \"2\"");
                lines.Add("m_rawinput \"1\"");
                lines.Add("m_customaccel \"0\"");

                switch (game.SelectedPreset)
                {
                    case "Performance (Max FPS)":
                        lines.Add("r_dynamic \"0\"");
                        lines.Add("r_shadows \"0\"");
                        lines.Add("mat_antialias \"0\"");
                        lines.Add("mat_forceaniso \"1\"");
                        lines.Add("mat_motion_blur_enabled \"0\"");
                        lines.Add("mat_hdr_level \"0\"");
                        lines.Add("r_decals \"32\"");
                        break;

                    case "Balanced (Quality & Speed)":
                        lines.Add("r_dynamic \"0\"");
                        lines.Add("r_shadows \"1\"");
                        lines.Add("mat_antialias \"2\"");
                        lines.Add("mat_forceaniso \"4\"");
                        lines.Add("mat_motion_blur_enabled \"0\"");
                        lines.Add("r_decals \"128\"");
                        break;

                    case "Ultra (Maximum Visuals)":
                        lines.Add("r_dynamic \"1\"");
                        lines.Add("r_shadows \"1\"");
                        lines.Add("mat_antialias \"4\"");
                        lines.Add("mat_forceaniso \"16\"");
                        lines.Add("mat_motion_blur_enabled \"0\"");
                        lines.Add("r_decals \"512\"");
                        break;

                    default:
                        lines.Add("r_dynamic \"0\"");
                        lines.Add("r_shadows \"1\"");
                        lines.Add("mat_antialias \"2\"");
                        lines.Add("mat_forceaniso \"8\"");
                        lines.Add("mat_motion_blur_enabled \"0\"");
                        lines.Add("r_decals \"200\"");
                        break;
                }
            }

            lines.Add(BlockEnd);
            return lines;
        }

        private static void WriteManagedAutoexecBlock(string filePath, List<string> blockLines)
        {
            List<string> existing = File.Exists(filePath)
                ? File.ReadAllLines(filePath).ToList()
                : new List<string>();

            int startIdx = existing.FindIndex(l => l.Trim().Equals(BlockStart, StringComparison.Ordinal));
            int endIdx = startIdx >= 0
                ? existing.FindIndex(startIdx, l => l.Trim().Equals(BlockEnd, StringComparison.Ordinal))
                : -1;

            if (startIdx >= 0 && endIdx >= startIdx)
            {
                existing.RemoveRange(startIdx, (endIdx - startIdx) + 1);
                existing.InsertRange(startIdx, blockLines);
            }
            else
            {
                if (existing.Count > 0 && !string.IsNullOrWhiteSpace(existing[^1]))
                    existing.Add(string.Empty);
                existing.AddRange(blockLines);
            }

            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllLines(filePath, existing);
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
                    if (i + 1 < tokens.Count && !tokens[i + 1].StartsWith("-") && !tokens[i + 1].StartsWith("+"))
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
            var current = new StringBuilder();

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