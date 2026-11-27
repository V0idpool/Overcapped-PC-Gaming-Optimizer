using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Overcapped_Optimizer.Helpers;

namespace Overcapped_Optimizer.Core
{
    public sealed class RegistryValueSnapshot
    {
        public string Hive { get; set; } = string.Empty;
        public string SubKey { get; set; } = string.Empty;
        public string ValueName { get; set; } = string.Empty;
        public bool KeyExisted { get; set; }
        public bool ValueExisted { get; set; }
        public string ValueKind { get; set; } = string.Empty;
        public string? SerializedValue { get; set; }
        public DateTime CapturedAtUtc { get; set; }
    }

    public sealed class SnapshotRestoreResult
    {
        public int RestoredUserCount { get; set; }
        public int RestoredMachineCount { get; set; }
        public int SkippedAdminCount { get; set; }
        public int FailedCount { get; set; }
        public HashSet<string> RestoredCompositeKeys { get; } = new(StringComparer.OrdinalIgnoreCase);

        public int TotalRestored => RestoredUserCount + RestoredMachineCount;
    }

    public static class RegistrySnapshotStore
    {
        private static readonly object SyncLock = new();
        private static Dictionary<string, RegistryValueSnapshot>? _entries;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static string BuildCompositeKey(RegistryHive hive, string subKey, string valueName) =>
            $"{HiveToCode(hive)}|{NormalizeSubKey(subKey)}|{valueName}";

        public static int PendingCount
        {
            get
            {
                lock (SyncLock)
                {
                    EnsureLoadedLocked();
                    return _entries!.Count;
                }
            }
        }

        public static bool HasPendingMachineEntries
        {
            get
            {
                lock (SyncLock)
                {
                    EnsureLoadedLocked();
                    return _entries!.Values.Any(e => e.Hive.Equals("HKLM", StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        /// <summary>
        /// Captures the current Windows registry state for a value ONLY if no baseline exists yet.
        /// Subsequent calls for the same key/value are no-ops so the original pre-buffed state is preserved.
        /// </summary>
        public static void CaptureIfMissing(RegistryHive hive, string subKey, string valueName)
        {
            string normalizedSubKey = NormalizeSubKey(subKey);
            string compositeKey = BuildCompositeKey(hive, normalizedSubKey, valueName);

            lock (SyncLock)
            {
                EnsureLoadedLocked();
                if (_entries!.ContainsKey(compositeKey))
                    return;

                var snapshot = new RegistryValueSnapshot
                {
                    Hive = HiveToCode(hive),
                    SubKey = normalizedSubKey,
                    ValueName = valueName,
                    CapturedAtUtc = DateTime.UtcNow
                };

                try
                {
                    RegistryKey baseKey = GetBaseKey(hive);
                    using RegistryKey? key = baseKey.OpenSubKey(normalizedSubKey, writable: false);

                    if (key == null)
                    {
                        snapshot.KeyExisted = false;
                        snapshot.ValueExisted = false;
                    }
                    else
                    {
                        snapshot.KeyExisted = true;
                        object? raw = key.GetValue(valueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
                        if (raw == null)
                        {
                            snapshot.ValueExisted = false;
                        }
                        else
                        {
                            RegistryValueKind kind = key.GetValueKind(valueName);
                            snapshot.ValueExisted = true;
                            snapshot.ValueKind = kind.ToString();
                            snapshot.SerializedValue = SerializeValue(raw, kind);
                        }
                    }

                    _entries[compositeKey] = snapshot;
                    SaveLocked();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Replays all captured registry baselines back to Windows.
        /// If isAdmin is false, HKLM entries remain untouched for a future elevated revert.
        /// </summary>
        public static SnapshotRestoreResult RestoreAll(bool isAdmin)
        {
            var result = new SnapshotRestoreResult();

            lock (SyncLock)
            {
                EnsureLoadedLocked();
                if (_entries!.Count == 0)
                    return result;

                var toRemove = new List<string>();
                var keysCreatedByOvercapped = new List<(RegistryHive Hive, string SubKey)>();

                foreach (var kvp in _entries.ToList())
                {
                    RegistryValueSnapshot entry = kvp.Value;
                    bool isMachine = entry.Hive.Equals("HKLM", StringComparison.OrdinalIgnoreCase);

                    if (isMachine && !isAdmin)
                    {
                        result.SkippedAdminCount++;
                        continue;
                    }

                    RegistryHive hive = isMachine ? RegistryHive.LocalMachine : RegistryHive.CurrentUser;

                    try
                    {
                        RegistryKey baseKey = GetBaseKey(hive);

                        if (entry.ValueExisted)
                        {
                            if (!Enum.TryParse(entry.ValueKind, ignoreCase: true, out RegistryValueKind kind))
                                kind = RegistryValueKind.String;

                            object? restoredValue = DeserializeValue(entry.SerializedValue, kind);
                            if (restoredValue != null)
                            {
                                using RegistryKey? key = baseKey.CreateSubKey(entry.SubKey, writable: true);
                                key?.SetValue(entry.ValueName, restoredValue, kind);
                            }
                        }
                        else
                        {
                            using (RegistryKey? key = baseKey.OpenSubKey(entry.SubKey, writable: true))
                            {
                                key?.DeleteValue(entry.ValueName, throwOnMissingValue: false);
                            }

                            if (!entry.KeyExisted)
                            {
                                keysCreatedByOvercapped.Add((hive, entry.SubKey));
                            }
                        }

                        toRemove.Add(kvp.Key);
                        result.RestoredCompositeKeys.Add(kvp.Key);

                        if (isMachine) result.RestoredMachineCount++;
                        else result.RestoredUserCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailedCount++;
                        ExceptionHandler.LogError(ex);
                    }
                }

                foreach (var (hive, subKey) in keysCreatedByOvercapped
                    .Distinct()
                    .OrderByDescending(k => k.SubKey.Length))
                {
                    TryDeleteEmptySubKey(hive, subKey);
                }

                foreach (string key in toRemove)
                {
                    _entries.Remove(key);
                }

                SaveLocked();
            }

            return result;
        }

        private static void TryDeleteEmptySubKey(RegistryHive hive, string subKey)
        {
            try
            {
                RegistryKey baseKey = GetBaseKey(hive);
                using (RegistryKey? leaf = baseKey.OpenSubKey(subKey, writable: false))
                {
                    if (leaf == null || leaf.ValueCount > 0 || leaf.SubKeyCount > 0)
                        return;
                }

                int lastSlash = subKey.LastIndexOf('\\');
                if (lastSlash <= 0) return;

                string parentPath = subKey[..lastSlash];
                string leafName = subKey[(lastSlash + 1)..];

                using RegistryKey? parent = baseKey.OpenSubKey(parentPath, writable: true);
                parent?.DeleteSubKey(leafName, throwOnMissingSubKey: false);
            }
            catch { }
        }

        private static void EnsureLoadedLocked()
        {
            if (_entries != null) return;

            _entries = new Dictionary<string, RegistryValueSnapshot>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if (!File.Exists(AppSettings.RegistrySnapshotFilePath))
                    return;

                string json = File.ReadAllText(AppSettings.RegistrySnapshotFilePath);
                var list = JsonSerializer.Deserialize<List<RegistryValueSnapshot>>(json, JsonOptions);
                if (list == null) return;

                foreach (var item in list)
                {
                    if (string.IsNullOrWhiteSpace(item.Hive) || string.IsNullOrWhiteSpace(item.SubKey))
                        continue;

                    RegistryHive hive = item.Hive.Equals("HKLM", StringComparison.OrdinalIgnoreCase)
                        ? RegistryHive.LocalMachine
                        : RegistryHive.CurrentUser;

                    string composite = BuildCompositeKey(hive, item.SubKey, item.ValueName);
                    _entries[composite] = item;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                _entries = new Dictionary<string, RegistryValueSnapshot>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private static void SaveLocked()
        {
            try
            {
                Directory.CreateDirectory(AppSettings.DataDirectory);

                if (_entries == null || _entries.Count == 0)
                {
                    if (File.Exists(AppSettings.RegistrySnapshotFilePath))
                        File.Delete(AppSettings.RegistrySnapshotFilePath);
                    return;
                }

                string tempPath = AppSettings.RegistrySnapshotFilePath + ".tmp";
                string json = JsonSerializer.Serialize(_entries.Values.OrderBy(e => e.Hive).ThenBy(e => e.SubKey).ThenBy(e => e.ValueName).ToList(), JsonOptions);
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, AppSettings.RegistrySnapshotFilePath, overwrite: true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
            }
        }

        private static RegistryKey GetBaseKey(RegistryHive hive) =>
            hive == RegistryHive.LocalMachine ? Registry.LocalMachine : Registry.CurrentUser;

        private static string HiveToCode(RegistryHive hive) =>
            hive == RegistryHive.LocalMachine ? "HKLM" : "HKCU";

        private static string NormalizeSubKey(string subKey) =>
            (subKey ?? string.Empty).Trim().Trim('\\');

        private static string? SerializeValue(object raw, RegistryValueKind kind)
        {
            return kind switch
            {
                RegistryValueKind.DWord => Convert.ToInt32(raw, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
                RegistryValueKind.QWord => Convert.ToInt64(raw, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
                RegistryValueKind.MultiString => JsonSerializer.Serialize(raw as string[] ?? Array.Empty<string>()),
                RegistryValueKind.Binary => Convert.ToBase64String(raw as byte[] ?? Array.Empty<byte>()),
                _ => raw.ToString()
            };
        }

        private static object? DeserializeValue(string? serialized, RegistryValueKind kind)
        {
            if (serialized == null) return null;

            return kind switch
            {
                RegistryValueKind.DWord => int.TryParse(serialized, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i) ? i : 0,
                RegistryValueKind.QWord => long.TryParse(serialized, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l) ? l : 0L,
                RegistryValueKind.MultiString => JsonSerializer.Deserialize<string[]>(serialized) ?? Array.Empty<string>(),
                RegistryValueKind.Binary => Convert.FromBase64String(serialized),
                _ => serialized
            };
        }
    }
}