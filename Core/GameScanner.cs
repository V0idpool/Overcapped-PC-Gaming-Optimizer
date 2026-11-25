using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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
            "ueprereq", "prereq", "unitycrashhandler", "cleanup", "updater", "installer",
            "eaanticheat", "eadesktop", "eabackground", "ealauncher", "origin",
            "battle.net", "blizzarderror", "agent.exe", "galaxyclient", "gog",
            "gamelaunchhelper", "activationui", "touchup", "support", "benchmark"
        };

        private static readonly string[] IgnoredLauncherAppNames =
        {
            "battle.net", "blizzard battle.net", "ea app", "origin", "ea anticheat",
            "ea javelin", "punkbuster", "gog galaxy", "epic games launcher", "steam"
        };

        private static readonly string[] ConfigExtensions = { ".ini", ".cfg", ".json", ".xml" };

        public static List<GameProfile> ScanAllInstalledGames()
        {
            var discovered = new List<GameProfile>();
            discovered.AddRange(ScanSteamGames());
            discovered.AddRange(ScanEpicGames());
            discovered.AddRange(ScanGogGames());
            discovered.AddRange(ScanXboxGames());
            discovered.AddRange(ScanBattleNetGames());
            discovered.AddRange(ScanEaAppGames());

            return discovered
                .Where(g => !string.IsNullOrWhiteSpace(g.InstallDirectory) && !string.IsNullOrWhiteSpace(g.ExecutableName))
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
            string manifestsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Epic", "EpicGamesLauncher", "Data", "Manifests");

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

        private static List<GameProfile> ScanGogGames()
        {
            var results = new List<GameProfile>();
            string[] gogKeys =
            {
                @"SOFTWARE\WOW6432Node\GOG.com\Games",
                @"SOFTWARE\GOG.com\Games"
            };

            foreach (string keyPath in gogKeys)
            {
                try
                {
                    using RegistryKey? gamesRoot = Registry.LocalMachine.OpenSubKey(keyPath);
                    if (gamesRoot == null) continue;

                    foreach (string subName in gamesRoot.GetSubKeyNames())
                    {
                        try
                        {
                            using RegistryKey? gameKey = gamesRoot.OpenSubKey(subName);
                            if (gameKey == null) continue;

                            string? dependsOn = gameKey.GetValue("dependsOn") as string;
                            if (!string.IsNullOrWhiteSpace(dependsOn)) continue;

                            string? name = (gameKey.GetValue("gameName") ?? gameKey.GetValue("GAMENAME")) as string;
                            string? installDir = (gameKey.GetValue("path") ?? gameKey.GetValue("PATH")) as string;
                            string? exePath = (gameKey.GetValue("exe") ?? gameKey.GetValue("EXE")) as string;

                            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                                continue;

                            if (!string.IsNullOrWhiteSpace(exePath) && !Path.IsPathRooted(exePath))
                                exePath = Path.Combine(installDir, exePath);

                            results.Add(BuildGameProfile(name.Trim(), installDir.TrimEnd('\\'), GameLauncherSource.Gog, exePath));
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.LogError(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                }
            }

            return results;
        }

        private static List<GameProfile> ScanXboxGames()
        {
            var results = new List<GameProfile>();
            var xboxRoots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady || drive.DriveType != DriveType.Fixed) continue;

                    string rootPath = drive.RootDirectory.FullName;
                    string defaultXboxDir = Path.Combine(rootPath, "XboxGames");
                    if (Directory.Exists(defaultXboxDir))
                        xboxRoots.Add(defaultXboxDir);

                    string gamingRootFile = Path.Combine(rootPath, ".GamingRoot");
                    if (File.Exists(gamingRootFile))
                    {
                        byte[] bytes = File.ReadAllBytes(gamingRootFile);
                        if (bytes.Length > 10)
                        {
                            string decoded = Encoding.Unicode.GetString(bytes, 8, bytes.Length - 8);
                            foreach (string segment in decoded.Split('\0', StringSplitOptions.RemoveEmptyEntries))
                            {
                                string cleaned = segment.Trim().TrimStart('\\');
                                if (string.IsNullOrWhiteSpace(cleaned)) continue;
                                string candidate = Path.Combine(rootPath, cleaned);
                                if (Directory.Exists(candidate))
                                    xboxRoots.Add(candidate);
                            }
                        }
                    }
                }
                catch { }
            }

            foreach (string rootDir in xboxRoots)
            {
                try
                {
                    foreach (string gameDir in Directory.EnumerateDirectories(rootDir))
                    {
                        try
                        {
                            string contentDir = Directory.Exists(Path.Combine(gameDir, "Content"))
                                ? Path.Combine(gameDir, "Content")
                                : gameDir;

                            string configPath = Path.Combine(contentDir, "MicrosoftGame.config");
                            if (!File.Exists(configPath))
                                configPath = Path.Combine(gameDir, "MicrosoftGame.config");

                            string displayName = Path.GetFileName(gameDir);
                            string? explicitExe = null;

                            if (File.Exists(configPath))
                            {
                                try
                                {
                                    XDocument xml = XDocument.Load(configPath);
                                    string? xmlDisplayName = xml.Descendants("ShellVisuals")
                                        .Select(x => (string?)x.Attribute("DefaultDisplayName"))
                                        .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

                                    if (!string.IsNullOrWhiteSpace(xmlDisplayName) &&
                                        !xmlDisplayName.StartsWith("ms-resource:", StringComparison.OrdinalIgnoreCase))
                                    {
                                        displayName = xmlDisplayName.Trim();
                                    }

                                    string? exeRel = xml.Descendants("Executable")
                                        .Select(x => (string?)x.Attribute("Name"))
                                        .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s) && s.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

                                    if (!string.IsNullOrWhiteSpace(exeRel))
                                    {
                                        string candidate = Path.Combine(contentDir, exeRel);
                                        if (File.Exists(candidate))
                                            explicitExe = candidate;
                                    }
                                }
                                catch { }
                            }

                            var profile = BuildGameProfile(displayName, contentDir, GameLauncherSource.Xbox, explicitExe);
                            if (!string.IsNullOrWhiteSpace(profile.ExecutablePath))
                                results.Add(profile);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.LogError(ex);
                        }
                    }
                }
                catch { }
            }

            return results;
        }

        private static List<GameProfile> ScanBattleNetGames()
        {
            var results = new List<GameProfile>();
            string[] uninstallRoots =
            {
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (string rootPath in uninstallRoots)
            {
                try
                {
                    using RegistryKey? uninstall = Registry.LocalMachine.OpenSubKey(rootPath);
                    if (uninstall == null) continue;

                    foreach (string subName in uninstall.GetSubKeyNames())
                    {
                        try
                        {
                            using RegistryKey? appKey = uninstall.OpenSubKey(subName);
                            if (appKey == null) continue;

                            string? publisher = appKey.GetValue("Publisher") as string;
                            string? uninstallStr = appKey.GetValue("UninstallString") as string;
                            bool isBlizzard =
                                (!string.IsNullOrWhiteSpace(publisher) && publisher.Contains("Blizzard", StringComparison.OrdinalIgnoreCase)) ||
                                (!string.IsNullOrWhiteSpace(uninstallStr) && uninstallStr.Contains("Battle.net", StringComparison.OrdinalIgnoreCase));

                            if (!isBlizzard) continue;

                            string? name = appKey.GetValue("DisplayName") as string;
                            string? installDir = appKey.GetValue("InstallLocation") as string;

                            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                                continue;

                            if (IsIgnoredLauncherApp(name)) continue;

                            var profile = BuildGameProfile(name.Trim(), installDir.TrimEnd('\\'), GameLauncherSource.BattleNet, null);
                            if (!string.IsNullOrWhiteSpace(profile.ExecutablePath))
                                results.Add(profile);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.LogError(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                }
            }

            return results;
        }

        private static List<GameProfile> ScanEaAppGames()
        {
            var results = new List<GameProfile>();

            string[] eaRoots =
            {
                @"SOFTWARE\WOW6432Node\EA Games",
                @"SOFTWARE\EA Games",
                @"SOFTWARE\WOW6432Node\Electronic Arts",
                @"SOFTWARE\Electronic Arts"
            };

            foreach (string rootPath in eaRoots)
            {
                try
                {
                    using RegistryKey? eaRoot = Registry.LocalMachine.OpenSubKey(rootPath);
                    if (eaRoot == null) continue;

                    foreach (string subName in eaRoot.GetSubKeyNames())
                    {
                        try
                        {
                            using RegistryKey? gameKey = eaRoot.OpenSubKey(subName);
                            if (gameKey == null) continue;

                            string? installDir = (gameKey.GetValue("Install Dir") ?? gameKey.GetValue("InstallDir") ?? gameKey.GetValue("InstallLocation")) as string;
                            if (string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                                continue;

                            string name = (gameKey.GetValue("DisplayName") as string)?.Trim() ?? subName.Trim();
                            if (IsIgnoredLauncherApp(name)) continue;

                            var profile = BuildGameProfile(name, installDir.TrimEnd('\\'), GameLauncherSource.EaApp, null);
                            if (!string.IsNullOrWhiteSpace(profile.ExecutablePath))
                                results.Add(profile);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.LogError(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                }
            }

            string[] uninstallRoots =
            {
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (string rootPath in uninstallRoots)
            {
                try
                {
                    using RegistryKey? uninstall = Registry.LocalMachine.OpenSubKey(rootPath);
                    if (uninstall == null) continue;

                    foreach (string subName in uninstall.GetSubKeyNames())
                    {
                        try
                        {
                            using RegistryKey? appKey = uninstall.OpenSubKey(subName);
                            if (appKey == null) continue;

                            string? publisher = appKey.GetValue("Publisher") as string;
                            if (string.IsNullOrWhiteSpace(publisher) ||
                                !publisher.Contains("Electronic Arts", StringComparison.OrdinalIgnoreCase))
                                continue;

                            string? name = appKey.GetValue("DisplayName") as string;
                            string? installDir = appKey.GetValue("InstallLocation") as string;

                            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
                                continue;

                            if (IsIgnoredLauncherApp(name)) continue;

                            var profile = BuildGameProfile(name.Trim(), installDir.TrimEnd('\\'), GameLauncherSource.EaApp, null);
                            if (!string.IsNullOrWhiteSpace(profile.ExecutablePath))
                                results.Add(profile);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.LogError(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                }
            }

            return results;
        }

        private static bool IsIgnoredLauncherApp(string name)
        {
            return IgnoredLauncherAppNames.Any(n => name.Contains(n, StringComparison.OrdinalIgnoreCase));
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
                    string resolved = JsonProfilePackManager.ResolvePackConfigPath(profile, matchedPack);
                    if (!string.IsNullOrWhiteSpace(resolved))
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

                var subCandidates = EnumerateExesShallow(installDir, maxDepth: 2)
                    .Where(p => !IsIgnoredExe(p) &&
                                !Path.GetFileName(p).Equals("start_protected_game.exe", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p => new FileInfo(p).Length)
                    .ToList();

                if (subCandidates.Count > 0)
                {
                    targetExe = subCandidates[0];
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

        private static IEnumerable<string> EnumerateExesShallow(string rootDir, int maxDepth)
        {
            var queue = new Queue<(string Dir, int Depth)>();
            queue.Enqueue((rootDir, 0));

            while (queue.Count > 0)
            {
                var (currentDir, depth) = queue.Dequeue();

                string[] files;
                try { files = Directory.GetFiles(currentDir, "*.exe"); }
                catch { continue; }

                foreach (string f in files)
                    yield return f;

                if (depth >= maxDepth) continue;

                string[] subDirs;
                try { subDirs = Directory.GetDirectories(currentDir); }
                catch { continue; }

                foreach (string sub in subDirs)
                    queue.Enqueue((sub, depth + 1));
            }
        }

        public static string ResolveConfigDirectory(string gameName, string installDir, GameEngineType engine, string targetExe)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string exeStem = string.IsNullOrWhiteSpace(targetExe)
                ? string.Empty
                : Path.GetFileNameWithoutExtension(targetExe).Replace("-Win64-Shipping", "", StringComparison.OrdinalIgnoreCase);

            string cleanName = Regex.Replace(gameName ?? string.Empty, @"[™®©:]", "").Trim();
            var candidateFolderNames = new[]
            {
                exeStem,
                cleanName,
                cleanName.Replace(" ", ""),
                gameName ?? string.Empty
            }
            .Where(s => !string.IsNullOrWhiteSpace(s) && s.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

            if (engine == GameEngineType.UnrealEngine)
            {
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

            if (engine == GameEngineType.Unity)
            {
                string localLow = UnityEngineModule.ResolveLocalLowConfigDirectory(
                    installDir, string.IsNullOrWhiteSpace(targetExe) ? null : Path.GetFileName(targetExe));
                if (!string.IsNullOrWhiteSpace(localLow))
                    return localLow;

                string installCfg = Path.Combine(installDir, "cfg");
                if (Directory.Exists(installCfg) && HasConfigFiles(installCfg))
                    return installCfg;
            }

            if (engine == GameEngineType.Source)
            {
                string sourceCfg = SourceEngineModule.ResolveCfgDirectory(installDir);
                if (!string.IsNullOrWhiteSpace(sourceCfg))
                    return sourceCfg;
            }

            string[] baseRoots =
            {
                Path.Combine(myDocs, "My Games"),
                myDocs,
                Path.Combine(userProfile, "Saved Games"),
                localAppData,
                appData
            };

            string[] commonSubDirs = { "settings", "config", "cfg", "Saved" };

            foreach (string root in baseRoots.Where(Directory.Exists))
            {
                foreach (string folderName in candidateFolderNames)
                {
                    string basePath = Path.Combine(root, folderName);
                    if (!Directory.Exists(basePath)) continue;

                    foreach (string sub in commonSubDirs)
                    {
                        string subPath = Path.Combine(basePath, sub);
                        if (Directory.Exists(subPath) && HasConfigFiles(subPath))
                            return subPath;
                    }

                    if (HasConfigFiles(basePath))
                        return basePath;
                }
            }

            return string.Empty;
        }

        private static bool HasConfigFiles(string directory)
        {
            try
            {
                return Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
                    .Any(f => ConfigExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
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