using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Overcapped_Optimizer.Helpers
{
    public enum HardwareHealthSeverity
    {
        Ok,
        Info,
        Warning
    }

    public sealed class HardwareHealthWarning
    {
        public string Category { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Details { get; init; } = string.Empty;
        public HardwareHealthSeverity Severity { get; init; } = HardwareHealthSeverity.Warning;
    }

    public static class HardwareDetector
    {
        private const int ENUM_CURRENT_SETTINGS = -1;
        private const uint DISPLAY_DEVICE_ATTACHED_TO_DESKTOP = 0x00000001;
        private const uint DISPLAY_DEVICE_PRIMARY_DEVICE = 0x00000004;
        private static readonly Guid PowerSaverPlanGuid = new("a1841308-3541-4fab-bc81-f71556f20b4a");

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplayDevicesW(string? lpDevice, uint iDevNum, ref DISPLAY_DEVICEW lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplaySettingsW(string? lpszDeviceName, int iModeNum, ref DEVMODEW lpDevMode);

        [DllImport("powrprof.dll")]
        private static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, out IntPtr ActivePolicyGuid);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAY_DEVICEW
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEVMODEW
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;
            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }

        /// <summary>
        /// Grabs the name of the CPU from your system info.
        /// </summary>
        public static string GetCpuName()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("select Name from Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Name"] != null)
                        return obj["Name"].ToString()!.Trim();
                }
            }
            catch { }
            return "N/A";
        }

        /// <summary>
        /// Gets the CPU's clock speed in GHz.
        /// </summary>
        public static double GetCpuSpeedInGhz()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("select MaxClockSpeed from Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["MaxClockSpeed"] != null)
                    {
                        double mhz = Convert.ToDouble(obj["MaxClockSpeed"]);
                        return Math.Round(mhz / 1000.0, 2);
                    }
                }
            }
            catch { }
            return 0.0;
        }

        /// <summary>
        /// Gets the total amount of RAM you have installed, in gigabytes.
        /// </summary>
        public static double GetTotalMemoryInGB()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong totalMemoryKB = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                    return Math.Round(totalMemoryKB / 1024.0 / 1024.0, 1);
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Gets more detailed info about the RAM, like the manufacturer and type (DDR4, DDR5, etc.).
        /// </summary>
        public static string GetRamInfo()
        {
            try
            {
                using ManagementObjectSearcher searcher = new("SELECT * FROM Win32_PhysicalMemory");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string manufacturer = obj["Manufacturer"]?.ToString()?.Trim() ?? "Unknown";
                    string partNumber = obj["PartNumber"]?.ToString()?.Trim() ?? "";

                    string? ramTypeFromPartNumber = GetRamTypeFromPartNumber(partNumber);
                    if (!string.IsNullOrEmpty(ramTypeFromPartNumber))
                    {
                        return $"{manufacturer} {ramTypeFromPartNumber} {partNumber}".Trim();
                    }

                    if (obj["SMBIOSMemoryType"] != null && Convert.ToInt32(obj["SMBIOSMemoryType"]) != 0)
                    {
                        int smbiosMemoryType = Convert.ToInt32(obj["SMBIOSMemoryType"]);
                        return $"{manufacturer} {GetSMBIOSRamTypeString(smbiosMemoryType)} {partNumber}".Trim();
                    }

                    if (obj["MemoryType"] != null)
                    {
                        int memoryType = Convert.ToInt32(obj["MemoryType"]);
                        return $"{manufacturer} {GetWin32RamTypeString(memoryType)} {partNumber}".Trim();
                    }

                    return $"{manufacturer} Unknown {partNumber}".Trim();
                }
            }
            catch { }
            return "N/A";
        }

        /// <summary>
        /// Runs lightweight, non-invasive diagnostics for common PC gaming misconfigurations.
        /// </summary>
        public static List<HardwareHealthWarning> RunHardwareHealthChecks()
        {
            var warnings = new List<HardwareHealthWarning>();

            CheckDisplayRefreshRates(warnings);
            CheckRamConfiguration(warnings);
            CheckActivePowerPlan(warnings);
            CheckSystemDriveSpace(warnings);

            return warnings;
        }

        private static void CheckDisplayRefreshRates(List<HardwareHealthWarning> warnings)
        {
            try
            {
                for (uint devIdx = 0; devIdx < 16; devIdx++)
                {
                    var device = new DISPLAY_DEVICEW { cb = Marshal.SizeOf<DISPLAY_DEVICEW>() };
                    if (!EnumDisplayDevicesW(null, devIdx, ref device, 0))
                        break;

                    if ((device.StateFlags & DISPLAY_DEVICE_ATTACHED_TO_DESKTOP) == 0)
                        continue;

                    var currentMode = new DEVMODEW { dmSize = (ushort)Marshal.SizeOf<DEVMODEW>() };
                    if (!EnumDisplaySettingsW(device.DeviceName, ENUM_CURRENT_SETTINGS, ref currentMode))
                        continue;

                    uint width = currentMode.dmPelsWidth;
                    uint height = currentMode.dmPelsHeight;
                    uint currentHz = currentMode.dmDisplayFrequency;
                    uint maxHz = currentHz;

                    int modeIdx = 0;
                    var testMode = new DEVMODEW { dmSize = (ushort)Marshal.SizeOf<DEVMODEW>() };
                    while (EnumDisplaySettingsW(device.DeviceName, modeIdx++, ref testMode))
                    {
                        if (testMode.dmPelsWidth == width &&
                            testMode.dmPelsHeight == height &&
                            testMode.dmDisplayFrequency > maxHz &&
                            testMode.dmDisplayFrequency <= 1000)
                        {
                            maxHz = testMode.dmDisplayFrequency;
                        }
                    }

                    if (maxHz >= 100 && currentHz > 0 && (maxHz - currentHz) >= 15)
                    {
                        bool isPrimary = (device.StateFlags & DISPLAY_DEVICE_PRIMARY_DEVICE) != 0;
                        string monitorLabel = isPrimary ? "Primary Display" : $"Display {devIdx + 1}";
                        warnings.Add(new HardwareHealthWarning
                        {
                            Category = "Display",
                            Title = $"{monitorLabel} running at {currentHz} Hz (supports {maxHz} Hz)",
                            Details = $"Your {width}x{height} display supports up to {maxHz} Hz but Windows is currently set to {currentHz} Hz. Open Windows Settings -> System -> Display -> Advanced display to switch to {maxHz} Hz.",
                            Severity = HardwareHealthSeverity.Warning
                        });
                    }
                }
            }
            catch { }
        }

        private static void CheckRamConfiguration(List<HardwareHealthWarning> warnings)
        {
            try
            {
                var sticks = new List<(int RatedMhz, int ConfiguredMhz, string Type, ulong CapacityBytes, string PartNumber)>();

                using (var searcher = new ManagementObjectSearcher(
                    "SELECT Speed, ConfiguredClockSpeed, SMBIOSMemoryType, MemoryType, Capacity, PartNumber FROM Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        int rated = obj["Speed"] != null ? Convert.ToInt32(obj["Speed"]) : 0;
                        int configured = obj["ConfiguredClockSpeed"] != null ? Convert.ToInt32(obj["ConfiguredClockSpeed"]) : 0;
                        ulong capacity = obj["Capacity"] != null ? Convert.ToUInt64(obj["Capacity"]) : 0;
                        string part = obj["PartNumber"]?.ToString()?.Trim() ?? string.Empty;

                        string type = GetRamTypeFromPartNumber(part) ?? string.Empty;
                        if (string.IsNullOrEmpty(type) && obj["SMBIOSMemoryType"] != null && Convert.ToInt32(obj["SMBIOSMemoryType"]) != 0)
                            type = GetSMBIOSRamTypeString(Convert.ToInt32(obj["SMBIOSMemoryType"]));
                        else if (string.IsNullOrEmpty(type) && obj["MemoryType"] != null)
                            type = GetWin32RamTypeString(Convert.ToInt32(obj["MemoryType"]));

                        sticks.Add((rated, configured, type, capacity, part));
                    }
                }

                if (sticks.Count == 0) return;

                if (sticks.Count == 1)
                {
                    double gb = Math.Round(sticks[0].CapacityBytes / (1024.0 * 1024.0 * 1024.0), 0);
                    warnings.Add(new HardwareHealthWarning
                    {
                        Category = "Memory",
                        Title = $"Single-channel RAM detected (1x {gb:F0} GB stick)",
                        Details = "Running a single stick of RAM cuts memory bandwidth in half and heavily impacts 1% Low FPS in CPU-bound games. Adding a matched second DIMM enables dual-channel mode.",
                        Severity = HardwareHealthSeverity.Warning
                    });
                }

                int activeMhz = sticks
                    .Select(s => s.ConfiguredMhz > 0 ? s.ConfiguredMhz : s.RatedMhz)
                    .Where(s => s > 0)
                    .DefaultIfEmpty(0)
                    .Min();

                int ratedMhz = sticks
                    .Select(s => s.RatedMhz)
                    .Where(s => s > 0)
                    .DefaultIfEmpty(0)
                    .Max();

                string primaryType = sticks[0].Type;

                if (activeMhz > 0 && ratedMhz > activeMhz + 100)
                {
                    warnings.Add(new HardwareHealthWarning
                    {
                        Category = "Memory",
                        Title = $"RAM running at {activeMhz} MHz (rated {ratedMhz} MHz)",
                        Details = "Your memory is running below its rated clock speed. Reboot into BIOS/UEFI and enable XMP, DOCP, or EXPO to unlock full memory bandwidth and smoother 1% Lows.",
                        Severity = HardwareHealthSeverity.Warning
                    });
                }
                else if (primaryType.Equals("DDR4", StringComparison.OrdinalIgnoreCase) && activeMhz > 0 && activeMhz <= 2400)
                {
                    warnings.Add(new HardwareHealthWarning
                    {
                        Category = "Memory",
                        Title = $"DDR4 running at base JEDEC speed ({activeMhz} MHz)",
                        Details = "Your DDR4 memory is running at default JEDEC speed (2133/2400 MHz). If your kit is rated for 3000+ MHz, enable XMP / DOCP in your motherboard BIOS.",
                        Severity = HardwareHealthSeverity.Warning
                    });
                }
                else if (primaryType.Equals("DDR5", StringComparison.OrdinalIgnoreCase) && activeMhz > 0 && activeMhz <= 4800)
                {
                    warnings.Add(new HardwareHealthWarning
                    {
                        Category = "Memory",
                        Title = $"DDR5 running at base JEDEC speed ({activeMhz} MT/s)",
                        Details = "Your DDR5 kit is running at the 4800 MT/s stock JEDEC profile. If your kit is rated for 5600-6400+ MT/s, enable EXPO or XMP in BIOS.",
                        Severity = HardwareHealthSeverity.Info
                    });
                }

                double totalGb = GetTotalMemoryInGB();
                if (totalGb > 0 && totalGb < 15.0)
                {
                    warnings.Add(new HardwareHealthWarning
                    {
                        Category = "Memory",
                        Title = $"Limited system RAM ({totalGb:F0} GB detected)",
                        Details = "Modern titles (especially Unreal Engine 5 & Rust) easily exceed 12-16 GB. Enable Smart Standby RAM Auto-Flush in the RAM Auto-Flush tab to reduce paging stutters.",
                        Severity = HardwareHealthSeverity.Info
                    });
                }
            }
            catch { }
        }

        private static void CheckActivePowerPlan(List<HardwareHealthWarning> warnings)
        {
            try
            {
                if (PowerGetActiveScheme(IntPtr.Zero, out IntPtr activePtr) == 0 && activePtr != IntPtr.Zero)
                {
                    Guid current = Marshal.PtrToStructure<Guid>(activePtr);
                    LocalFree(activePtr);

                    if (current == PowerSaverPlanGuid)
                    {
                        warnings.Add(new HardwareHealthWarning
                        {
                            Category = "Power",
                            Title = "Windows is set to 'Power Saver' plan",
                            Details = "The Power Saver plan aggressively throttles CPU boost clocks and PCIe power states. Switch to Balanced/High Performance or enable Auto High/Ultimate Power Plan in System Optimization.",
                            Severity = HardwareHealthSeverity.Warning
                        });
                    }
                }
            }
            catch { }
        }

        private static void CheckSystemDriveSpace(List<HardwareHealthWarning> warnings)
        {
            try
            {
                string sysRoot = Path.GetPathRoot(Environment.SystemDirectory) ?? @"C:\";
                DriveInfo drive = new(sysRoot);
                if (drive.IsReady)
                {
                    double freeGb = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    if (freeGb < 15.0)
                    {
                        warnings.Add(new HardwareHealthWarning
                        {
                            Category = "Storage",
                            Title = $"Low free space on {drive.Name.TrimEnd('\\')} ({freeGb:F1} GB free)",
                            Details = "Less than 15 GB remaining on your system drive can starve the Windows pagefile and GPU shader cache, causing hitching and SSD write slowdowns.",
                            Severity = HardwareHealthSeverity.Warning
                        });
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Helper method to parse the part number.
        /// </summary>
        private static string? GetRamTypeFromPartNumber(string partNumber)
        {
            if (partNumber.Contains("DDR5", StringComparison.OrdinalIgnoreCase)) return "DDR5";
            if (partNumber.Contains("DDR4", StringComparison.OrdinalIgnoreCase) || partNumber.Contains("X4M2", StringComparison.OrdinalIgnoreCase)) return "DDR4";
            if (partNumber.Contains("DDR3", StringComparison.OrdinalIgnoreCase)) return "DDR3";
            if (partNumber.Contains("DDR2", StringComparison.OrdinalIgnoreCase)) return "DDR2";
            if (partNumber.Contains("DDR", StringComparison.OrdinalIgnoreCase)) return "DDR";
            return null;
        }

        /// <summary>
        /// This is a helper that translates SMBIOS memory codes into something readable.
        /// </summary>
        private static string GetSMBIOSRamTypeString(int smbiosMemoryType)
        {
            return smbiosMemoryType switch
            {
                24 => "DDR3",
                25 => "DDR4",
                26 => "LPDDR3",
                27 => "LPDDR4",
                28 => "LPDDR5",
                29 => "HBM",
                30 => "HBM2",
                33 => "DDR5",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// A helper to translate Win32 memory codes into a readable format.
        /// </summary>
        private static string GetWin32RamTypeString(int memoryType)
        {
            return memoryType switch
            {
                1 => "Other",
                2 => "Unknown",
                3 => "DRAM",
                4 => "EDRAM",
                5 => "VRAM",
                6 => "SRAM",
                7 => "RAM",
                8 => "ROM",
                9 => "Flash",
                10 => "EEPROM",
                11 => "FEPROM",
                12 => "EPROM",
                13 => "CDRAM",
                14 => "3DRAM",
                15 => "SDRAM",
                16 => "SGRAM",
                17 => "RDRAM",
                18 => "DDR",
                19 => "DDR2",
                20 => "DDR2 FB-DIMM",
                21 => "DDR3",
                22 => "FBD2",
                23 => "DDR4",
                24 => "LPDDR",
                25 => "LPDDR2",
                26 => "LPDDR3",
                27 => "LPDDR4",
                28 => "LPDDR5",
                29 => "HBM",
                30 => "HBM2",
                31 => "DDR5",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Gets the user-friendly name of the discrete graphics card (GPU) with the highest VRAM.
        /// Ignores virtual displays and software adapters.
        /// </summary>
        public static string GetGpuName()
        {
            string bestGpuName = "N/A";
            long maxVramBytes = 0;

            try
            {
                using RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}");
                if (classKey != null)
                {
                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (!int.TryParse(subKeyName, out _)) continue;
                        using RegistryKey? adapterKey = classKey.OpenSubKey(subKeyName);
                        if (adapterKey == null) continue;

                        object? descObj = adapterKey.GetValue("DriverDesc");
                        if (descObj == null) continue;

                        string desc = descObj.ToString() ?? string.Empty;
                        string lowerDesc = desc.ToLowerInvariant();

                        if (lowerDesc.Contains("virtual") || lowerDesc.Contains("remote") ||
                            lowerDesc.Contains("microsoft basic") || lowerDesc.Contains("citrix") ||
                            lowerDesc.Contains("mirage") || lowerDesc.Contains("spacedesk"))
                        {
                            continue;
                        }

                        object? memObj = adapterKey.GetValue("HardwareInformation.qwMemorySize")
                                      ?? adapterKey.GetValue("HardwareInformation.MemorySize");

                        if (memObj != null)
                        {
                            long vramBytes = ParseRegistryMemory(memObj);
                            if (vramBytes >= maxVramBytes)
                            {
                                maxVramBytes = vramBytes;
                                bestGpuName = desc;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting GPU Name from registry: {ex.Message}");
            }

            if (bestGpuName == "N/A")
            {
                try
                {
                    using ManagementObjectSearcher searcher = new("SELECT Name FROM Win32_VideoController");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        if (obj["Name"] != null)
                        {
                            bestGpuName = obj["Name"].ToString() ?? "N/A";
                            break;
                        }
                    }
                }
                catch { }
            }

            return Regex.Replace(bestGpuName, @"\(.*\)", "").Trim();
        }

        /// <summary>
        /// Tries to get the amount of VRAM (video memory) from the Windows Registry.
        /// </summary>
        public static double GetGpuVramInGB()
        {
            double maxVramGb = 0;

            try
            {
                using RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}");
                if (classKey != null)
                {
                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (!int.TryParse(subKeyName, out _)) continue;
                        using RegistryKey? adapterKey = classKey.OpenSubKey(subKeyName);
                        if (adapterKey == null) continue;

                        object? descObj = adapterKey.GetValue("DriverDesc");
                        if (descObj != null)
                        {
                            string lowerDesc = descObj.ToString()!.ToLowerInvariant();
                            if (lowerDesc.Contains("virtual") || lowerDesc.Contains("remote") || lowerDesc.Contains("microsoft basic"))
                            {
                                continue;
                            }
                        }

                        object? memObj = adapterKey.GetValue("HardwareInformation.qwMemorySize")
                                      ?? adapterKey.GetValue("HardwareInformation.MemorySize");

                        if (memObj != null)
                        {
                            long vramBytes = ParseRegistryMemory(memObj);
                            double vramGb = vramBytes / (1024.0 * 1024.0 * 1024.0);
                            if (vramGb > maxVramGb)
                            {
                                maxVramGb = vramGb;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting GPU VRAM: {ex.Message}");
            }

            return Math.Round(maxVramGb, 1);
        }

        /// <summary>
        /// Safely parses registry memory values whether they are stored as integers, longs, or binary byte arrays.
        /// </summary>
        private static long ParseRegistryMemory(object memObj)
        {
            if (memObj == null) return 0;

            if (memObj is byte[] bytes)
            {
                if (bytes.Length == 8)
                    return BitConverter.ToInt64(bytes, 0);
                if (bytes.Length == 4)
                    return BitConverter.ToUInt32(bytes, 0);
                return 0;
            }

            try
            {
                long vramBytes = Convert.ToInt64(memObj);
                if (vramBytes < 0)
                {
                    vramBytes = (uint)Convert.ToInt32(memObj);
                }
                return vramBytes;
            }
            catch
            {
                return 0;
            }
        }

        public static (int Width, int Height, int RefreshHz) GetPrimaryDisplayInfo()
        {
            try
            {
                for (uint devIdx = 0; devIdx < 16; devIdx++)
                {
                    var device = new DISPLAY_DEVICEW { cb = Marshal.SizeOf<DISPLAY_DEVICEW>() };
                    if (!EnumDisplayDevicesW(null, devIdx, ref device, 0))
                        break;

                    if ((device.StateFlags & DISPLAY_DEVICE_ATTACHED_TO_DESKTOP) == 0 ||
                        (device.StateFlags & DISPLAY_DEVICE_PRIMARY_DEVICE) == 0)
                        continue;

                    var mode = new DEVMODEW { dmSize = (ushort)Marshal.SizeOf<DEVMODEW>() };
                    if (EnumDisplaySettingsW(device.DeviceName, ENUM_CURRENT_SETTINGS, ref mode))
                    {
                        return ((int)mode.dmPelsWidth, (int)mode.dmPelsHeight, (int)mode.dmDisplayFrequency);
                    }
                }
            }
            catch { }

            return (0, 0, 0);
        }

        public static string GetGpuDriverVersion()
        {
            string bestVersion = "Unknown";
            long maxVramBytes = 0;

            try
            {
                using RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}");
                if (classKey != null)
                {
                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (!int.TryParse(subKeyName, out _)) continue;
                        using RegistryKey? adapterKey = classKey.OpenSubKey(subKeyName);
                        if (adapterKey == null) continue;

                        string desc = adapterKey.GetValue("DriverDesc")?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(desc)) continue;

                        string lowerDesc = desc.ToLowerInvariant();
                        if (lowerDesc.Contains("virtual") || lowerDesc.Contains("remote") ||
                            lowerDesc.Contains("microsoft basic") || lowerDesc.Contains("citrix") ||
                            lowerDesc.Contains("mirage") || lowerDesc.Contains("spacedesk"))
                        {
                            continue;
                        }

                        object? memObj = adapterKey.GetValue("HardwareInformation.qwMemorySize")
                                      ?? adapterKey.GetValue("HardwareInformation.MemorySize");

                        long vramBytes = memObj != null ? ParseRegistryMemory(memObj) : 0;
                        if (vramBytes >= maxVramBytes)
                        {
                            string rawVer = adapterKey.GetValue("DriverVersion")?.ToString()?.Trim() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(rawVer)) continue;

                            maxVramBytes = vramBytes;
                            string? radeonVer = adapterKey.GetValue("RadeonSoftwareVersion")?.ToString()?.Trim();

                            if (!string.IsNullOrWhiteSpace(radeonVer))
                            {
                                bestVersion = $"Adrenalin {radeonVer} ({rawVer})";
                            }
                            else if (lowerDesc.Contains("nvidia") || lowerDesc.Contains("geforce") || lowerDesc.Contains("rtx") || lowerDesc.Contains("gtx"))
                            {
                                string[] parts = rawVer.Split('.');
                                if (parts.Length == 4 && parts[2].Length >= 1 && parts[3].Length == 4)
                                {
                                    string nvShort = $"{parts[2][^1]}{parts[3][..2]}.{parts[3][2..]}";
                                    bestVersion = $"{nvShort} ({rawVer})";
                                }
                                else
                                {
                                    bestVersion = rawVer;
                                }
                            }
                            else
                            {
                                bestVersion = rawVer;
                            }
                        }
                    }
                }
            }
            catch { }

            return bestVersion;
        }

        public static string GetWindowsVersionString()
        {
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                if (key != null)
                {
                    string product = key.GetValue("ProductName")?.ToString()?.Trim() ?? "Windows";
                    string displayVer = key.GetValue("DisplayVersion")?.ToString()?.Trim() ?? string.Empty;
                    string buildStr = key.GetValue("CurrentBuildNumber")?.ToString()?.Trim()
                                   ?? key.GetValue("CurrentBuild")?.ToString()?.Trim()
                                   ?? string.Empty;
                    string ubr = key.GetValue("UBR")?.ToString()?.Trim() ?? string.Empty;

                    if (int.TryParse(buildStr, out int buildNum) && buildNum >= 22000 &&
                        product.Contains("Windows 10", StringComparison.OrdinalIgnoreCase))
                    {
                        product = product.Replace("Windows 10", "Windows 11", StringComparison.OrdinalIgnoreCase);
                    }

                    string fullBuild = !string.IsNullOrWhiteSpace(ubr) ? $"{buildStr}.{ubr}" : buildStr;
                    string verPart = !string.IsNullOrWhiteSpace(displayVer) ? $" {displayVer}" : string.Empty;
                    string buildPart = !string.IsNullOrWhiteSpace(fullBuild) ? $" (Build {fullBuild})" : string.Empty;

                    return $"{product}{verPart}{buildPart}".Trim();
                }
            }
            catch { }

            return Environment.OSVersion.VersionString;
        }
    }
}