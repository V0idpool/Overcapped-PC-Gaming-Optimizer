using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public static class GameScanner
    {
        private static readonly string[] IgnoredExeKeywords =
        {
            "unins", "crash", "report", "redist", "vcredist", "dxsetup",
            "easyanticheat", "battleye", "beclient", "beservice", "epicwebhelper",
            "ueprereq", "prereq", "unitycrashhandler", "cleanup", "updater", "installer"
        };

        public static List<GameProfile> ScanAllInstalledGames()
        {
            var discovered = new List<GameProfile>();
            discovered.AddRange(ScanSteamGames());
            discovered.AddRange(ScanEpicGames());

            return discovered
                .GroupBy(g => g.InstallDirectory.TrimEnd('\\'), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static GameProfile CreateManualProfile(string exePath, string? displayName = null)
        {
            string fullExe = Path.GetFullPath(exePath);
            string installDir = Path.GetDirectoryName(fullExe) ?? string.Empty;

            if (installDir.Contains(@"\Binaries\Win64", StringComparison.OrdinalIgnoreCase))
            {
                DirectoryInfo? dir = new DirectoryInfo(installDir);
                if (dir.Parent?.Parent?.Parent != null)
                    installDir = dir.Parent.Parent.Parent.FullName;
            }

            string name = string.IsNullOrWhiteSpace(displayName)
                ? Path.GetFileNameWithoutExtension(fullExe).Replace("-Win64-Shipping", "", StringComparison.OrdinalIgnoreCase)
                : displayName.Trim();

            return BuildGameProfile(name, installDir, GameLauncherSource.Manual, fullExe);
        }

        private static List<GameProfile> ScanSteamGames()
        {
            var results = new List<GameProfile>();
            string? steamPath = GetSteamInstallPath();
            if (string.IsNullOrWhiteSpace(steamPath) || !Directory.Exists(steamPath))
                return results;

            var libraryRoots = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { steamPath };
            string vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

            if (File.Exists(vdfPath))
            {
                string content = File.ReadAllText(vdfPath);
                foreach (Match match in Regex.Matches(content, "\"path\"\\s+\"([^\"]+)\"", RegexOptions.IgnoreCase))
                {
                    string libPath = match.Groups[1].Value.Replace(@"\\", @"\");
                    if (Directory.Exists(libPath)) libraryRoots.Add(libPath);
                }
            }

            foreach (string libRoot in libraryRoots)
            {
                string steamApps = Path.Combine(libRoot, "steamapps");
                if (!Directory.Exists(steamApps)) continue;

                foreach (string acfFile in Directory.GetFiles(steamApps, "appmanifest_*.acf"))
                {
                    try
                    {
                        string acf = File.ReadAllText(acfFile);
                        string name = MatchVdfValue(acf, "name");
                        string installDirName = MatchVdfValue(acf, "installdir");

                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(installDirName))
                            continue;

                        if (name.Contains("Steamworks Common", StringComparison.OrdinalIgnoreCase) ||
                            name.Contains("Proton ", StringComparison.OrdinalIgnoreCase) ||
                            name.Contains("Steam Linux Runtime", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string fullInstallDir = Path.Combine(steamApps, "common", installDirName);
                        if (!Directory.Exists(fullInstallDir)) continue;

                        results.Add(BuildGameProfile(name, fullInstallDir, GameLauncherSource.Steam, null));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogError(ex);
                    }
                }
            }

            return results;
        }

        private static List<GameProfile> ScanEpicGames()
        {
            var results = new List<GameProfile>();
            string manifestsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Epic", "EpicGamesLauncher", "Data", "Manifests");

            if (!Directory.Exists(manifestsDir)) return results;

            foreach (string itemFile in Directory.GetFiles(manifestsDir, "*.item"))
            {
                try
                {
                    using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(itemFile));
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("bIsIncompleteInstall", out JsonElement incomplete) && incomplete.GetBoolean())
                        continue;

                    string? name = root.TryGetProperty("DisplayName", out JsonElement n) ? n.GetString() : null;
                    string? installDir = root.TryGetProperty("InstallLocation", out JsonElement loc) ? loc.GetString() : null;
                    string? launchRel = root.TryGetProperty("LaunchExecutable", out JsonElement exe) ? exe.GetString() : null;

                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                        continue;

                    string? launchExe = !string.IsNullOrWhiteSpace(launchRel)
                        ? Path.Combine(installDir, launchRel)
                        : null;

                    results.Add(BuildGameProfile(name, installDir, GameLauncherSource.Epic, launchExe));
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                }
            }

            return results;
        }

        private static GameProfile BuildGameProfile(string name, string installDir, GameLauncherSource launcher, string? explicitExePath)
        {
            GameEngineType engine = DetectEngine(installDir);
            (string targetExe, string launchExe) = ResolveExecutables(installDir, engine, explicitExePath);
            string configDir = ResolveConfigDirectory(name, installDir, engine, targetExe);

            var profile = new GameProfile
            {
                Name = name,
                InstallDirectory = installDir,
                Launcher = launcher,
                Engine = engine,
                ExecutablePath = targetExe,
                ExecutableName = string.IsNullOrEmpty(targetExe) ? string.Empty : Path.GetFileName(targetExe),
                LaunchExecutablePath = launchExe,
                ConfigDirectory = configDir
            };

            var matchedPack = JsonProfilePackManager.FindMatchingPack(profile);
            if (matchedPack != null)
            {
                profile.JsonPackId = matchedPack.PackId;
                if (string.IsNullOrWhiteSpace(profile.ConfigDirectory) && !string.IsNullOrWhiteSpace(matchedPack.RelativeConfigPath))
                {
                    string resolved = Path.Combine(installDir, matchedPack.RelativeConfigPath);
                    profile.ConfigDirectory = Path.GetDirectoryName(resolved) ?? installDir;
                }
            }

            return profile;
        }

        private static GameEngineType DetectEngine(string installDir)
        {
            try
            {
                if (Directory.Exists(Path.Combine(installDir, "Engine", "Binaries")))
                    return GameEngineType.UnrealEngine;

                if (Directory.EnumerateFiles(installDir, "*-Win64-Shipping.exe", SearchOption.AllDirectories).Any())
                    return GameEngineType.UnrealEngine;

                if (File.Exists(Path.Combine(installDir, "UnityPlayer.dll")) ||
                    Directory.EnumerateDirectories(installDir, "*_Data", SearchOption.TopDirectoryOnly).Any())
                    return GameEngineType.Unity;

                if (File.Exists(Path.Combine(installDir, "bin", "tier0.dll")) ||
                    File.Exists(Path.Combine(installDir, "game", "bin", "win64", "engine2.dll")))
                    return GameEngineType.Source;
            }
            catch { }

            return GameEngineType.Generic;
        }

        private static (string TargetExe, string LaunchExe) ResolveExecutables(string installDir, GameEngineType engine, string? explicitExePath)
        {
            string targetExe = string.Empty;
            string launchExe = (!string.IsNullOrWhiteSpace(explicitExePath) && File.Exists(explicitExePath)) ? explicitExePath : string.Empty;

            try
            {
                var rootExes = Directory.EnumerateFiles(installDir, "*.exe", SearchOption.TopDirectoryOnly)
                    .Where(p => !IsIgnoredExe(p))
                    .ToList();

                string? eacLauncher = rootExes.FirstOrDefault(p =>
                    Path.GetFileName(p).Equals("start_protected_game.exe", StringComparison.OrdinalIgnoreCase));

                string? clientExe = rootExes.FirstOrDefault(p =>
                    Path.GetFileNameWithoutExtension(p).EndsWith("Client", StringComparison.OrdinalIgnoreCase));

                if (clientExe != null)
                {
                    string stem = Path.GetFileNameWithoutExtension(clientExe)[..^6];
                    string? pairedLauncher = rootExes.FirstOrDefault(p =>
                        Path.GetFileNameWithoutExtension(p).Equals(stem, StringComparison.OrdinalIgnoreCase));

                    if (pairedLauncher != null)
                        return (clientExe, string.IsNullOrWhiteSpace(launchExe) ? pairedLauncher : launchExe);
                }

                if (engine == GameEngineType.UnrealEngine)
                {
                    var shippingExes = Directory.EnumerateFiles(installDir, "*-Win64-Shipping.exe", SearchOption.AllDirectories)
                        .Where(p => !IsIgnoredExe(p))
                        .OrderByDescending(p => new FileInfo(p).Length)
                        .ToList();

                    if (shippingExes.Count > 0)
                    {
                        targetExe = shippingExes[0];
                        if (string.IsNullOrWhiteSpace(launchExe))
                            launchExe = eacLauncher ?? rootExes.FirstOrDefault() ?? targetExe;

                        return (targetExe, launchExe);
                    }
                }

                if (!string.IsNullOrWhiteSpace(launchExe) &&
                    !Path.GetFileName(launchExe).Equals("start_protected_game.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return (launchExe, launchExe);
                }

                var candidates = rootExes
                    .Where(p => !Path.GetFileName(p).Equals("start_protected_game.exe", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p => new FileInfo(p).Length)
                    .ToList();

                if (candidates.Count > 0)
                {
                    targetExe = candidates[0];
                    string finalLaunch = !string.IsNullOrWhiteSpace(launchExe)
                        ? launchExe
                        : (eacLauncher ?? targetExe);

                    return (targetExe, finalLaunch);
                }

                if (eacLauncher != null)
                    return (eacLauncher, eacLauncher);
            }
            catch { }

            return (targetExe, launchExe);
        }

        private static string ResolveConfigDirectory(string gameName, string installDir, GameEngineType engine, string targetExe)
        {
            if (engine == GameEngineType.UnrealEngine)
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string exeStem = Path.GetFileNameWithoutExtension(targetExe).Replace("-Win64-Shipping", "", StringComparison.OrdinalIgnoreCase);

                var candidateFolderNames = new[]
                {
                    exeStem,
                    gameName.Replace(" ", ""),
                    gameName
                }.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase);

                string[] subPaths =
                {
                    Path.Combine("Saved", "Config", "Windows"),
                    Path.Combine("Saved", "Config", "WindowsNoEditor"),
                    Path.Combine("Saved", "Config", "WinGDK")
                };

                foreach (string folderName in candidateFolderNames)
                {
                    foreach (string sub in subPaths)
                    {
                        string path = Path.Combine(localAppData, folderName, sub);
                        if (Directory.Exists(path)) return path;
                    }
                }
            }
            return string.Empty;
        }

        private static bool IsIgnoredExe(string path)
        {
            string file = Path.GetFileName(path);
            return IgnoredExeKeywords.Any(k => file.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        private static string? GetSteamInstallPath()
        {
            using RegistryKey? key64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
            if (key64?.GetValue("InstallPath") is string path64 && !string.IsNullOrWhiteSpace(path64))
                return path64;

            using RegistryKey? key32 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");
            if (key32?.GetValue("InstallPath") is string path32 && !string.IsNullOrWhiteSpace(path32))
                return path32;

            return null;
        }

        private static string MatchVdfValue(string content, string key)
        {
            Match m = Regex.Match(content, $"\"{Regex.Escape(key)}\"\\s+\"([^\"]+)\"", RegexOptions.IgnoreCase);
            return m.Success ? m.Groups[1].Value : string.Empty;
        }
    }
}