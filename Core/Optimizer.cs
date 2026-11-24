using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Overcapped_Optimizer;
using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public sealed class PingTestProgress
    {
        public int Completed { get; init; }
        public int Total { get; init; }
        public double AvgPingMs { get; init; }
        public double MinPingMs { get; init; }
        public double MaxPingMs { get; init; }
        public double JitterMs { get; init; }
        public double PacketLossPercent { get; init; }
        public int Sent { get; init; }
        public int Lost { get; init; }
        public string ProbeMode { get; init; } = "ICMP";
    }

    public static class Optimizer
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string? lpSystemName, string lpName, out LUID lpLuid);
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder? strReturn, int iReturnLength, IntPtr hwndCallback);
        [DllImport("ntdll.dll")]
        private static extern int NtSetSystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength);
        [DllImport("ntdll.dll")]
        private static extern int NtQuerySystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength, out int ReturnLength);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetLogicalProcessorInformation(IntPtr Buffer, ref uint ReturnLength);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);
        [DllImport("powrprof.dll")]
        private static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, out IntPtr ActivePolicyGuid);
        [DllImport("powrprof.dll")]
        private static extern uint PowerSetActiveScheme(IntPtr UserRootPowerKey, ref Guid SchemeGuid);

        private const int RelationProcessorCore = 0;
        private const int SystemMemoryListInformation = 80;
        private static readonly Guid HighPerformancePlanGuid = new("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
        private static readonly Guid UltimatePerformancePlanGuid = new("e9a42b02-d5df-448d-aa00-03f14749eb61");

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION
        {
            public UIntPtr ProcessorMask;
            public int Relationship;
            public long UnionPad1;
            public long UnionPad2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID { public uint LowPart; public int HighPart; }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_PRIVILEGES { public uint PrivilegeCount; public LUID Luid; public uint Attributes; }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength, dwMemoryLoad;
            public ulong ullTotalPhys, ullAvailPhys, ullTotalPageFile, ullAvailPageFile, ullTotalVirtual, ullAvailVirtual, ullAvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_MEMORY_LIST_INFORMATION
        {
            public UIntPtr ZeroPageCount, FreePageCount, ModifiedPageCount, ModifiedNoWritePageCount, BadPageCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public UIntPtr[] PageCountByPriority;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public UIntPtr[] RepurposedPagesByPriority;
            public UIntPtr ModifiedPageCountPageFile;
        }

        private static System.Timers.Timer? _watchdogTimer;
        private static readonly object WatchdogLock = new();
        private static long _cachedPhysicalCoreMask;
        private static Guid? _previousPowerPlanGuid;
        private static DateTime _lastSmartFlushUtc = DateTime.MinValue;
        private static readonly Dictionary<int, ProcessPriorityClass> ThrottledProcesses = new();
        // TODO: Add the ability for users to add exe's to the apps to throttle list via the UI.
        private static readonly string[] BackgroundAppsToThrottle =
        {
            "chrome", "msedge", "firefox", "brave", "opera", "opera_gx",
            "steamwebhelper", "epicgameslauncher"
        };

        public static string? ActiveDetectedGameName { get; private set; }
        public static event Action<string?>? ActiveGameChanged;
        private static readonly HashSet<int> PinnedProcessIds = new();

        public static bool IsAdministrator()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsNvidiaGpu(string gpuName) =>
            !string.IsNullOrWhiteSpace(gpuName) &&
            (gpuName.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) ||
             gpuName.Contains("GeForce", StringComparison.OrdinalIgnoreCase) ||
             gpuName.Contains("RTX", StringComparison.OrdinalIgnoreCase) ||
             gpuName.Contains("GTX", StringComparison.OrdinalIgnoreCase));

        public static void RestartAsAdministrator()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Application.StartupPath,
                    FileName = Application.ExecutablePath,
                    Verb = "runas"
                });
                Application.Exit();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Administrator elevation was canceled.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static long GetPhysicalCoreMask()
        {
            if (_cachedPhysicalCoreMask != 0) return _cachedPhysicalCoreMask;

            uint returnLength = 0;
            GetLogicalProcessorInformation(IntPtr.Zero, ref returnLength);
            if (returnLength == 0) return 0;

            IntPtr buffer = Marshal.AllocHGlobal((int)returnLength);
            try
            {
                if (GetLogicalProcessorInformation(buffer, ref returnLength))
                {
                    int size = Marshal.SizeOf<SYSTEM_LOGICAL_PROCESSOR_INFORMATION>();
                    int count = (int)returnLength / size;
                    long physicalMask = 0;

                    for (int i = 0; i < count; i++)
                    {
                        IntPtr itemAddr = new IntPtr(buffer.ToInt64() + (i * size));
                        var info = Marshal.PtrToStructure<SYSTEM_LOGICAL_PROCESSOR_INFORMATION>(itemAddr);

                        if (info.Relationship == RelationProcessorCore)
                        {
                            long coreMask = (long)info.ProcessorMask;
                            physicalMask |= (coreMask & -coreMask);
                        }
                    }

                    _cachedPhysicalCoreMask = physicalMask;
                    return physicalMask;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

            return 0;
        }

        private static void ApplyProcessAffinity(Process p, bool physicalOnly)
        {
            long targetMask = physicalOnly ? GetPhysicalCoreMask() : 0;
            if (targetMask == 0)
                targetMask = Environment.ProcessorCount >= 64 ? -1L : (1L << Environment.ProcessorCount) - 1;

            if ((long)p.ProcessorAffinity != targetMask)
                p.ProcessorAffinity = (IntPtr)targetMask;
        }

        public static void ApplyAllPerGameOptimization(GameProfile game)
        {
            if (IsAdministrator())
            {
                ApplyIfeoPriority(game.ExecutableName, game.HighPriorityIfeo);
            }

            ApplyGpuPreference(game.ExecutablePath, game.HighPerfGpuBinding);
        }

        public static void ApplyIfeoPriority(string executableName, bool high)
        {
            if (!IsAdministrator() || string.IsNullOrWhiteSpace(executableName)) return;

            string safeExe = Path.GetFileName(executableName);
            string perfKeyPath = $@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\{safeExe}\PerfOptions";

            try
            {
                if (high)
                {
                    using RegistryKey? key = Registry.LocalMachine.CreateSubKey(perfKeyPath, true);
                    key?.SetValue("CpuPriorityClass", 3, RegistryValueKind.DWord);
                }
                else
                {
                    using RegistryKey? key = Registry.LocalMachine.OpenSubKey(perfKeyPath, true);
                    key?.DeleteValue("CpuPriorityClass", false);
                }
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
        }

        public static void ApplyGpuPreference(string executablePath, bool highPerformance)
        {
            if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath)) return;

            const string gpuPrefPath = @"Software\Microsoft\DirectX\UserGpuPreferences";
            try
            {
                using RegistryKey? key = Registry.CurrentUser.CreateSubKey(gpuPrefPath, true);
                if (key == null) return;

                if (highPerformance)
                    key.SetValue(executablePath, "GpuPreference=2;", RegistryValueKind.String);
                else
                    key.DeleteValue(executablePath, false);
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
        }

        public static void ApplyQosDscpPolicy(string executableName, bool enable)
        {
            if (!IsAdministrator() || string.IsNullOrWhiteSpace(executableName)) return;

            string safeExe = Path.GetFileName(executableName);
            string policyName = $"OvercappedOptimizer_{Path.GetFileNameWithoutExtension(safeExe)}";
            const string qosRoot = @"SOFTWARE\Policies\Microsoft\Windows\QoS";

            try
            {
                if (enable)
                {
                    using RegistryKey? key = Registry.LocalMachine.CreateSubKey($@"{qosRoot}\{policyName}", true);
                    if (key == null) return;

                    key.SetValue("Version", "1.0", RegistryValueKind.String);
                    key.SetValue("Application Name", safeExe, RegistryValueKind.String);
                    key.SetValue("Protocol", "UDP", RegistryValueKind.String);
                    key.SetValue("Local Port", "*", RegistryValueKind.String);
                    key.SetValue("Local IP", "*", RegistryValueKind.String);
                    key.SetValue("Local IP Prefix Length", "*", RegistryValueKind.String);
                    key.SetValue("Remote Port", "*", RegistryValueKind.String);
                    key.SetValue("Remote IP", "*", RegistryValueKind.String);
                    key.SetValue("Remote IP Prefix Length", "*", RegistryValueKind.String);
                    key.SetValue("DSCP Value", "46", RegistryValueKind.String);
                    key.SetValue("Throttle Rate", "-1", RegistryValueKind.String);
                }
                else
                {
                    using RegistryKey? rootKey = Registry.LocalMachine.OpenSubKey(qosRoot, true);
                    rootKey?.DeleteSubKeyTree(policyName, false);
                }
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
        }

        public static string ApplyGameConfigPreset(GameProfile game)
        {
            var pack = JsonProfilePackManager.FindMatchingPack(game);
            if (pack != null)
            {
                JsonProfilePackManager.ApplyPack(game, pack);
                return $"Applied JSON Pack '{pack.GameName}' ({game.SelectedPreset}).";
            }

            if (game.Engine == GameEngineType.UnrealEngine)
            {
                UnrealEngineModule.ApplyProfile(game);
                return $"Applied Unreal Engine preset '{game.SelectedPreset}' to Engine.ini & GameUserSettings.ini.";
            }

            return "Applied system optimizations (no engine config pack required for this title).";
        }

        public static void SyncWatchdog()
        {
            bool needsWatchdog = AppSettings.Current.AutoPowerPlan
                              || AppSettings.Current.ThrottleBackgroundApps
                              || AppSettings.Current.PCoresOnly;

            lock (WatchdogLock)
            {
                if (!AppSettings.Current.AutoPowerPlan) RestorePreviousPowerPlan();
                if (!AppSettings.Current.ThrottleBackgroundApps) RestoreThrottledProcesses();
                if (!AppSettings.Current.PCoresOnly) RestorePinnedProcesses();
            }

            if (needsWatchdog)
            {
                if (_watchdogTimer == null)
                {
                    _watchdogTimer = new System.Timers.Timer(5000);
                    _watchdogTimer.Elapsed += (_, _) => WatchdogTick();
                    _watchdogTimer.AutoReset = true;
                    _watchdogTimer.Start();
                }
                Task.Run(WatchdogTick);
            }
            else if (_watchdogTimer != null)
            {
                _watchdogTimer.Stop();
                _watchdogTimer.Dispose();
                _watchdogTimer = null;

                if (ActiveDetectedGameName != null)
                {
                    ActiveDetectedGameName = null;
                    ActiveGameChanged?.Invoke(null);
                }
            }
        }

        private static void WatchdogTick()
        {
            lock (WatchdogLock)
            {
                try
                {
                    var monitoredGames = AppSettings.Current.Games
                        .Where(g => !string.IsNullOrWhiteSpace(g.ProcessNameWithoutExtension))
                        .ToList();

                    string? runningGameName = null;

                    foreach (GameProfile game in monitoredGames)
                    {
                        Process[] procs = Process.GetProcessesByName(game.ProcessNameWithoutExtension);
                        bool isRunning = procs.Length > 0;

                        foreach (Process p in procs)
                        {
                            try
                            {
                                if (AppSettings.Current.PCoresOnly)
                                {
                                    ApplyProcessAffinity(p, true);
                                    PinnedProcessIds.Add(p.Id);
                                }
                            }
                            catch (System.ComponentModel.Win32Exception) { }
                            catch (InvalidOperationException) { }
                            finally
                            {
                                p.Dispose();
                            }
                        }

                        if (isRunning && runningGameName == null)
                        {
                            runningGameName = game.Name;
                        }
                    }

                    if (runningGameName != null)
                    {
                        if (AppSettings.Current.AutoPowerPlan) ActivateGamingPowerPlan();
                        if (AppSettings.Current.ThrottleBackgroundApps) ThrottleBackgroundProcesses();
                    }
                    else
                    {
                        RestorePreviousPowerPlan();
                        RestoreThrottledProcesses();
                        PinnedProcessIds.Clear();
                    }

                    if (!string.Equals(ActiveDetectedGameName, runningGameName, StringComparison.Ordinal))
                    {
                        ActiveDetectedGameName = runningGameName;
                        ActiveGameChanged?.Invoke(runningGameName);
                    }
                }
                catch (Exception ex) { ExceptionHandler.LogError(ex); }
            }
        }

        private static void RestorePinnedProcesses()
        {
            if (PinnedProcessIds.Count == 0) return;

            foreach (int pid in PinnedProcessIds.ToList())
            {
                try
                {
                    using Process p = Process.GetProcessById(pid);
                    if (!p.HasExited)
                        ApplyProcessAffinity(p, false);
                }
                catch { }
            }

            PinnedProcessIds.Clear();
        }

        private static void ActivateGamingPowerPlan()
        {
            if (_previousPowerPlanGuid.HasValue) return;

            try
            {
                if (PowerGetActiveScheme(IntPtr.Zero, out IntPtr activePtr) == 0 && activePtr != IntPtr.Zero)
                {
                    Guid current = Marshal.PtrToStructure<Guid>(activePtr);
                    LocalFree(activePtr);

                    if (current == UltimatePerformancePlanGuid || current == HighPerformancePlanGuid)
                        return;

                    Guid target = UltimatePerformancePlanGuid;
                    if (PowerSetActiveScheme(IntPtr.Zero, ref target) != 0)
                    {
                        target = HighPerformancePlanGuid;
                        if (PowerSetActiveScheme(IntPtr.Zero, ref target) != 0) return;
                    }

                    _previousPowerPlanGuid = current;
                }
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
        }

        private static void RestorePreviousPowerPlan()
        {
            if (!_previousPowerPlanGuid.HasValue) return;
            try
            {
                Guid restore = _previousPowerPlanGuid.Value;
                PowerSetActiveScheme(IntPtr.Zero, ref restore);
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
            finally { _previousPowerPlanGuid = null; }
        }

        private static void ThrottleBackgroundProcesses()
        {
            foreach (string procName in BackgroundAppsToThrottle)
            {
                foreach (Process p in Process.GetProcessesByName(procName))
                {
                    try
                    {
                        if (!ThrottledProcesses.ContainsKey(p.Id))
                        {
                            ProcessPriorityClass current = p.PriorityClass;
                            if (current is ProcessPriorityClass.Normal or ProcessPriorityClass.AboveNormal)
                            {
                                ThrottledProcesses[p.Id] = current;
                                p.PriorityClass = ProcessPriorityClass.BelowNormal;
                            }
                        }
                    }
                    catch { }
                    finally { p.Dispose(); }
                }
            }
        }

        private static void RestoreThrottledProcesses()
        {
            if (ThrottledProcesses.Count == 0) return;
            foreach (var kvp in ThrottledProcesses.ToList())
            {
                try
                {
                    using Process p = Process.GetProcessById(kvp.Key);
                    if (!p.HasExited && p.PriorityClass == ProcessPriorityClass.BelowNormal)
                        p.PriorityClass = kvp.Value;
                }
                catch { }
            }
            ThrottledProcesses.Clear();
        }

        public static void RestoreAllRuntimeStates()
        {
            lock (WatchdogLock)
            {
                RestorePreviousPowerPlan();
                RestoreThrottledProcesses();
                RestorePinnedProcesses();
            }
        }

        public static void ApplyGameModeAndDvr(bool optimize)
        {
            try
            {
                using (RegistryKey? gameBarKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar", true))
                {
                    if (optimize) gameBarKey?.SetValue("AutoGameModeEnabled", 1, RegistryValueKind.DWord);
                }

                using (RegistryKey? dvrKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", true))
                {
                    dvrKey?.SetValue("AppCaptureEnabled", optimize ? 0 : 1, RegistryValueKind.DWord);
                }

                using (RegistryKey? storeKey = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore", true))
                {
                    storeKey?.SetValue("GameDVR_Enabled", optimize ? 0 : 1, RegistryValueKind.DWord);
                }
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
        }

        public static void ApplyFSOState(bool disableFSO)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore", true);
                if (key == null) return;
                int target = disableFSO ? 2 : 0;
                key.SetValue("GameDVR_FSEBehaviorMode", target, RegistryValueKind.DWord);
                key.SetValue("GameDVR_HonorUserFSEBehaviorMode", disableFSO ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue("GameDVR_FSEBehavior", target, RegistryValueKind.DWord);
                key.SetValue("GameDVR_DXGIHonorFSEWindowsCompatible", disableFSO ? 1 : 0, RegistryValueKind.DWord);
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
        }

        private static void PlayFlushSound()
        {
            try
            {
                if (!AppSettings.Current.AutoFlushSfx) return;

                string tempSoundPath = Path.Combine(Path.GetTempPath(), "OvercappedOptimizerTemp", "OvercappedOptimizer_flush.mp3");
                if (!File.Exists(tempSoundPath))
                {
                    string? dir = Path.GetDirectoryName(tempSoundPath);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                    EmbedResources.SaveToDisk("flush.mp3", tempSoundPath);
                }

                if (File.Exists(tempSoundPath))
                {
                    mciSendString("close ramflush", null, 0, IntPtr.Zero);
                    mciSendString($"open \"{tempSoundPath}\" type mpegvideo alias ramflush", null, 0, IntPtr.Zero);
                    mciSendString("play ramflush from 0", null, 0, IntPtr.Zero);
                }
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }
        }

        public static void InitializeAutoFlushTimer()
        {
            if (MainFrm.Instance == null) return;

            if (MainFrm.Instance.autoFlushTimer != null)
            {
                MainFrm.Instance.autoFlushTimer.Stop();
                MainFrm.Instance.autoFlushTimer.Dispose();
                MainFrm.Instance.autoFlushTimer = null;
            }

            if (!AppSettings.Current.AutoFlushEnabled) return;

            int intervalMs;
            if (AppSettings.Current.SmartFlushEnabled)
            {
                intervalMs = 30 * 1000;
            }
            else if (AppSettings.Current.AutoFlushInterval > 0)
            {
                intervalMs = AppSettings.Current.AutoFlushUnit.Equals("Hours", StringComparison.OrdinalIgnoreCase)
                    ? AppSettings.Current.AutoFlushInterval * 60 * 60 * 1000
                    : AppSettings.Current.AutoFlushInterval * 60 * 1000;
            }
            else
            {
                return;
            }

            MainFrm.Instance.autoFlushTimer = new System.Windows.Forms.Timer { Interval = intervalMs };
            MainFrm.Instance.autoFlushTimer.Tick += (_, _) => CheckSmartFlushTick();
            MainFrm.Instance.autoFlushTimer.Start();
        }

        public static bool TryGetMemoryStats(out double freeGb, out double standbyGb, out double availPhysGb)
        {
            freeGb = standbyGb = availPhysGb = 0;
            var memStatus = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            if (!GlobalMemoryStatusEx(ref memStatus)) return false;

            availPhysGb = memStatus.ullAvailPhys / (1024.0 * 1024.0 * 1024.0);
            int structSize = Marshal.SizeOf<SYSTEM_MEMORY_LIST_INFORMATION>();
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            try
            {
                if (NtQuerySystemInformation(SystemMemoryListInformation, buffer, structSize, out _) == 0)
                {
                    var listInfo = Marshal.PtrToStructure<SYSTEM_MEMORY_LIST_INFORMATION>(buffer);
                    double pageSize = Environment.SystemPageSize;
                    ulong freePages = listInfo.ZeroPageCount.ToUInt64() + listInfo.FreePageCount.ToUInt64();
                    ulong standbyPages = 0;
                    if (listInfo.PageCountByPriority != null)
                        for (int i = 0; i < listInfo.PageCountByPriority.Length; i++)
                            standbyPages += listInfo.PageCountByPriority[i].ToUInt64();

                    freeGb = (freePages * pageSize) / (1024.0 * 1024.0 * 1024.0);
                    standbyGb = (standbyPages * pageSize) / (1024.0 * 1024.0 * 1024.0);
                    return true;
                }
            }
            catch { }
            finally { Marshal.FreeHGlobal(buffer); }

            freeGb = availPhysGb;
            return true;
        }

        public static bool FlushStandbyList(bool silent = false)
        {
            if (!IsAdministrator()) return false;

            try
            {
                if (!OpenProcessToken(Process.GetCurrentProcess().Handle, 0x0020 | 0x0008, out IntPtr tokenHandle))
                    return false;

                if (!LookupPrivilegeValue(null, "SeProfileSingleProcessPrivilege", out LUID luid))
                    return false;

                TOKEN_PRIVILEGES tp = new() { PrivilegeCount = 1, Luid = luid, Attributes = 0x00000002 };
                AdjustTokenPrivileges(tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);

                IntPtr pCommand = Marshal.AllocHGlobal(sizeof(int));
                try
                {
                    Marshal.WriteInt32(pCommand, 4);
                    NtSetSystemInformation(SystemMemoryListInformation, pCommand, sizeof(int));
                }
                finally { Marshal.FreeHGlobal(pCommand); }

                PlayFlushSound();

                if (!silent)
                    MessageBox.Show("RAM standby list flush complete!", "Overcapped Optimizer", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                return false;
            }
        }

        public static void CheckSmartFlushTick()
        {
            if (!AppSettings.Current.AutoFlushEnabled) return;

            if (!AppSettings.Current.SmartFlushEnabled)
            {
                FlushStandbyList(silent: true);
                return;
            }

            if ((DateTime.UtcNow - _lastSmartFlushUtc).TotalSeconds < 60) return;

            if (TryGetMemoryStats(out double freeGb, out double standbyGb, out double availPhysGb))
            {
                double threshold = Math.Max(1, AppSettings.Current.SmartFlushThresholdGB);
                if ((freeGb < threshold && standbyGb >= 0.5) || (standbyGb == 0 && availPhysGb < threshold))
                {
                    _lastSmartFlushUtc = DateTime.UtcNow;
                    FlushStandbyList(silent: true);
                }
            }
        }

        public static (int DeletedFiles, double FreedMB, int SkippedFiles) ClearShaderCaches()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string[] cacheDirs =
            {
                Path.Combine(localAppData, "D3DSCache"),
                Path.Combine(localAppData, "NVIDIA", "DXCache"),
                Path.Combine(localAppData, "NVIDIA", "GLCache"),
                Path.Combine(localAppData, "AMD", "DxCache"),
                Path.Combine(localAppData, "AMD", "GLCache")
            };

            long freedBytes = 0;
            int deleted = 0, skipped = 0;

            foreach (string dir in cacheDirs.Where(Directory.Exists))
            {
                foreach (FileInfo file in new DirectoryInfo(dir).EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    try { long len = file.Length; file.Delete(); freedBytes += len; deleted++; }
                    catch { skipped++; }
                }
            }

            return (deleted, freedBytes / 1024.0 / 1024.0, skipped);
        }

        public static int ApplyNicPowerOptimization(bool disablePowerSaving)
        {
            if (!IsAdministrator())
                throw new UnauthorizedAccessException("Administrator privileges are required.");

            var activeEthernetIds = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet && n.OperationalStatus == OperationalStatus.Up)
                .Select(n => n.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (activeEthernetIds.Count == 0) return 0;

            const string nicClassPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
            string[] eeeNames = { "*EEE", "EEE", "EEELinkAdvertisement", "EnableGreenEthernet", "PowerSavingMode", "AdvancedEEE", "GigaLite", "ULPMode" };
            int updated = 0;

            using RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(nicClassPath, true);
            if (classKey == null) return 0;

            foreach (string sub in classKey.GetSubKeyNames())
            {
                if (!int.TryParse(sub, out _)) continue;
                using RegistryKey? adapterKey = classKey.OpenSubKey(sub, true);
                if (adapterKey == null) continue;

                string? id = adapterKey.GetValue("NetCfgInstanceId") as string;
                if (string.IsNullOrEmpty(id) || !activeEthernetIds.Contains(id)) continue;

                adapterKey.SetValue("PnPCapabilities", disablePowerSaving ? 24 : 0, RegistryValueKind.DWord);
                foreach (string name in eeeNames)
                {
                    object? val = adapterKey.GetValue(name);
                    if (val is int) adapterKey.SetValue(name, disablePowerSaving ? 0 : 1, RegistryValueKind.DWord);
                    else if (val != null) adapterKey.SetValue(name, disablePowerSaving ? "0" : "1", RegistryValueKind.String);
                }
                updated++;
            }

            return updated;
        }

        public static bool FlushDnsCache(out string output)
        {
            output = string.Empty;
            try
            {
                using Process proc = new()
                {
                    StartInfo = new ProcessStartInfo("ipconfig", "/flushdns")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                output = proc.StandardOutput.ReadToEnd().Trim();
                proc.WaitForExit(5000);
                return proc.ExitCode == 0;
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); output = ex.Message; return false; }
        }

        private static readonly byte[] A2SInfoPayload =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x54,
            (byte)'S', (byte)'o', (byte)'u', (byte)'r', (byte)'c', (byte)'e', (byte)' ',
            (byte)'E', (byte)'n', (byte)'g', (byte)'i', (byte)'n', (byte)'e', (byte)' ',
            (byte)'Q', (byte)'u', (byte)'e', (byte)'r', (byte)'y', 0x00
        };

        public static async Task<PingTestProgress> RunPingTestAsync(
            string rawHost, int count, IProgress<PingTestProgress> progress, CancellationToken ct)
        {
            string cleaned = rawHost.Trim();
            if (cleaned.StartsWith("client.connect ", StringComparison.OrdinalIgnoreCase)) cleaned = cleaned[15..].Trim();
            else if (cleaned.StartsWith("connect ", StringComparison.OrdinalIgnoreCase)) cleaned = cleaned[8..].Trim();

            int? specifiedPort = null;
            string host = cleaned;
            int colon = cleaned.LastIndexOf(':');
            if (colon > 0 && int.TryParse(cleaned[(colon + 1)..], out int p) && p is >= 1 and <= 65535)
            {
                host = cleaned[..colon].Trim();
                specifiedPort = p;
            }

            if (!IPAddress.TryParse(host, out IPAddress? targetIp))
            {
                IPAddress[] addrs = await Dns.GetHostAddressesAsync(host, ct);
                targetIp = addrs.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                           ?? addrs.FirstOrDefault()
                           ?? throw new Exception($"Could not resolve host '{host}'.");
            }

            int? a2sPort = null;
            if (specifiedPort.HasValue)
            {
                a2sPort = await DiscoverPortAsync(targetIp, specifiedPort, ct);
            }
            else
            {
                bool icmpOk = false;
                try { using var qp = new Ping(); icmpOk = (await qp.SendPingAsync(targetIp, 800)).Status == IPStatus.Success; } catch { }
                if (!icmpOk) a2sPort = await DiscoverPortAsync(targetIp, null, ct);
            }

            string mode = a2sPort.HasValue ? $"UDP A2S :{a2sPort.Value}" : "ICMP";
            var rtts = new List<double>(count);
            int sent = 0, lost = 0;
            using var ping = a2sPort.HasValue ? null : new Ping();
            PingTestProgress latest = new() { Total = count, ProbeMode = mode };

            for (int i = 1; i <= count; i++)
            {
                ct.ThrowIfCancellationRequested();
                sent++;

                if (a2sPort.HasValue)
                {
                    double? rtt = await SendProbeAsync(targetIp, a2sPort.Value, 2000, ct);
                    if (rtt.HasValue) rtts.Add(rtt.Value); else lost++;
                }
                else
                {
                    try
                    {
                        PingReply reply = await ping!.SendPingAsync(targetIp, 2000);
                        if (reply.Status == IPStatus.Success) rtts.Add(reply.RoundtripTime); else lost++;
                    }
                    catch { lost++; }
                }

                double jitter = 0;
                if (rtts.Count >= 2)
                {
                    double sum = 0;
                    for (int j = 1; j < rtts.Count; j++) sum += Math.Abs(rtts[j] - rtts[j - 1]);
                    jitter = sum / (rtts.Count - 1);
                }

                latest = new PingTestProgress
                {
                    Completed = i,
                    Total = count,
                    Sent = sent,
                    Lost = lost,
                    ProbeMode = mode,
                    AvgPingMs = rtts.Count > 0 ? rtts.Average() : 0,
                    MinPingMs = rtts.Count > 0 ? rtts.Min() : 0,
                    MaxPingMs = rtts.Count > 0 ? rtts.Max() : 0,
                    JitterMs = jitter,
                    PacketLossPercent = (lost * 100.0) / sent
                };
                progress?.Report(latest);
                if (i < count) await Task.Delay(a2sPort.HasValue ? 500 : 250, ct);
            }
            return latest;
        }

        private static async Task<int?> DiscoverPortAsync(IPAddress ip, int? port, CancellationToken ct)
        {
            var candidates = port.HasValue
                ? new[] { port.Value, port.Value + 2, port.Value + 1, port.Value - 1, 27015 }
                : new[] { 28017, 28015, 27015 };

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var tasks = candidates.Where(p => p is >= 1 and <= 65535).Distinct()
                .Select(async p => (await SendProbeAsync(ip, p, 1200, cts.Token)).HasValue ? (int?)p : null).ToList();

            while (tasks.Count > 0)
            {
                var finished = await Task.WhenAny(tasks);
                tasks.Remove(finished);
                int? found = await finished;
                if (found.HasValue) { cts.Cancel(); return found.Value; }
            }
            return null;
        }

        private static async Task<double?> SendProbeAsync(IPAddress ip, int port, int timeoutMs, CancellationToken ct)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(timeoutMs);
            try
            {
                using var udp = new UdpClient(ip.AddressFamily);
                udp.Connect(ip, port);
                var sw = Stopwatch.StartNew();
                await udp.SendAsync(A2SInfoPayload, A2SInfoPayload.Length);
                UdpReceiveResult res = await udp.ReceiveAsync(cts.Token);
                sw.Stop();
                if (res.Buffer.Length >= 5 && res.Buffer[0] == 0xFF && res.Buffer[1] == 0xFF &&
                    res.Buffer[2] == 0xFF && res.Buffer[3] == 0xFF &&
                    (res.Buffer[4] == 0x49 || res.Buffer[4] == 0x41 || res.Buffer[4] == 0x6D))
                    return sw.Elapsed.TotalMilliseconds;
            }
            catch { }
            return null;
        }
    }
}