using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Core
{
    public static class UnrealEngineModule
    {
        public static string[] AvailablePresets => new[]
        {
            "Performance (Max FPS)",
            "Balanced (Quality & Speed)",
            "Recommended (Optimized)",
            "Ultra (Maximum Visuals)",
            "Handheld/SteamDeck"
        };

        public static void ApplyProfile(GameProfile game)
        {
            if (string.IsNullOrWhiteSpace(game.ConfigDirectory) || !Directory.Exists(game.ConfigDirectory))
                throw new DirectoryNotFoundException("Unreal Engine Saved\\Config\\Windows directory not found. Please select the game's config folder first.");

            double vramGb = HardwareDetector.GetGpuVramInGB();
            int poolSizeMb = CalculateTexturePoolSizeMb(vramGb);

            var systemSettings = BuildEngineSystemSettings(game.SelectedPreset, game.EnableLumen, game.EnableFog, poolSizeMb);
            var inputSettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["bEnableMouseSmoothing"] = "False",
                ["bViewAccelerationEnabled"] = "False"
            };

            string engineIniPath = Path.Combine(game.ConfigDirectory, "Engine.ini");
            BackupFile(engineIniPath);
            MergeIniSections(engineIniPath, new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["SystemSettings"] = systemSettings,
                ["/Script/Engine.InputSettings"] = inputSettings
            });

            var scalability = BuildScalabilitySettings(game.SelectedPreset, game.EnableLumen, vramGb);
            string ueScreenMode = game.ScreenMode == 1 ? "0" : "1";
            var userSettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["bUseVSync"] = "False",
                ["FullscreenMode"] = ueScreenMode,
                ["LastConfirmedFullscreenMode"] = ueScreenMode,
                ["FrameRateLimit"] = game.FpsLimit <= 0
                    ? "0.000000"
                    : game.FpsLimit.ToString("0.000000", CultureInfo.InvariantCulture)
            };

            string gusIniPath = Path.Combine(game.ConfigDirectory, "GameUserSettings.ini");
            BackupFile(gusIniPath);

            string gusSectionName = FindGameUserSettingsSection(gusIniPath);
            MergeIniSections(gusIniPath, new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["ScalabilityGroups"] = scalability,
                [gusSectionName] = userSettings
            });
        }

        public static int CalculateTexturePoolSizeMb(double vramGb)
        {
            if (vramGb >= 20) return 14336;
            if (vramGb >= 16) return 11264;
            if (vramGb >= 12) return 8192;
            if (vramGb >= 10) return 6144;
            if (vramGb >= 8) return 5120;
            if (vramGb >= 6) return 3584;
            return 2560;
        }

        private static Dictionary<string, string> BuildEngineSystemSettings(
            string preset,
            bool enableLumen,
            bool enableFog,
            int poolSizeMb)
        {
            var s = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["r.Streaming.PoolSize"] = poolSizeMb.ToString(CultureInfo.InvariantCulture),
                ["r.Streaming.UseNewMetrics"] = "1",
                ["r.Streaming.StressTest.ExtraAsyncLatency"] = "0",
                ["r.PSOPrecaching"] = "1",
                ["r.PSOPrecache.Components"] = "1",
                ["r.PSOPrecache.Resources"] = "1",
                ["fx.NiagaraAllowRuntimeScalabilityChanges"] = "1",
                ["r.Occlusion.SingleRHIThreadStall"] = "1",
                ["r.Shadow.DetectVertexShaderLayerAtRuntime"] = enableLumen ? "1" : "0",
                ["D3D12.Bindless.ResourceDescriptorHeapSize"] = "100000",
                ["D3D12.Bindless.SamplerDescriptorHeapSize"] = "2048",
                ["r.Shaders.RemoveUnusedInterpolators"] = enableLumen ? "1" : "0",
                ["r.D3D.RemoveUnusedInterpolators"] = enableLumen ? "1" : "0",
                ["r.Shaders.Optimize"] = "1",
                ["r.UseShaderCaching"] = "1",
                ["r.Shaders.Validation"] = "0",
                ["r.CookOutUnusedDetailModeComponents"] = "1",
                ["r.DontLimitOnBattery"] = "1",
                ["r.ShaderComplexity.CacheShaders"] = "1",
                ["r.ShaderPipelineCache.ReportPSO"] = "0",
                ["r.ShaderPipelineCache.LogPSO"] = "0",
                ["r.AOAsyncBuildQueue"] = "1",
                ["r.AsyncPipelineCompile"] = "1",
                ["r.UseFastIntersect"] = "1",
                ["r.UniformBufferPooling"] = "1",
                ["r.Renderer.UseGPUInstancing"] = "1",
                ["r.Bloom.AsyncCompute"] = "1",
                ["r.ParallelRendering"] = "1",
                ["r.ParallelShadows"] = "1",
                ["r.ParallelVelocity"] = "1",
                ["r.ParallelTranslucency"] = "1",
                ["r.ParallelPhysicsScene"] = "1",
                ["r.ParallelSceneCapture"] = "1",
                ["r.ParallelMeshProcessing"] = "1",
                ["p.Chaos.Spring.ISPC"] = "1",
                ["p.Chaos.VisualDebuggerEnable"] = "0",
                ["gc.CreateGCClusters"] = "1",
                ["gc.AllowParallelGC"] = "1",
                ["gc.AssetClustreringEnabled"] = "1",
                ["vm.OptimizeVMByteCode"] = "1",
                ["s.MaxIncomingRequestsToStall"] = "0",
                ["s.MaxReadyRequestsToStallMB"] = "0",
                ["s.MinBulkDataSizeForAsyncLoading"] = "1024",
                ["r.RHICmdCollectRHIThreadStatsFromHighLevel"] = "0",
                ["r.GPUCrashDebugging"] = "0",
                ["r.GPUCrashDump"] = "0",
                ["r.GPUCrashDebugging.Aftermath.Markers"] = "0",
                ["r.GPUCrashDebugging.Aftermath.Callstack"] = "0",
                ["r.GPUCrashDebugging.Aftermath.TrackAll"] = "0",
                ["r.GPUCrash.DataDepth"] = "0",
                ["r.GPUCrash.Collectionenable"] = "0",
                ["r.D3D11.GPUCrashDebuggingMode"] = "0",
                ["r.D3D12.GPUCrashDebuggingMode"] = "0",
                ["r.ShaderLibrary.PrintExtendedStats"] = "0",
                ["r.CompileShadersForDevelopment"] = "0",
                ["r.ShaderDrawDebug"] = "0",
                ["memory.logGenericPlatformMemoryStats"] = "0",
                ["fx.ParticlePerfStats.Enabled"] = "False",
                ["fx.EnableCircularAnimTrailDump"] = "0"
            };

            bool fogOn = enableFog && preset is "Recommended (Optimized)" or "Ultra (Maximum Visuals)" or "Balanced (Quality & Speed)";
            s["r.Fog"] = fogOn ? "1" : "0";
            s["r.VolumetricFog"] = (enableFog && preset is "Recommended (Optimized)" or "Ultra (Maximum Visuals)") ? "1" : "0";

            if (!enableLumen)
            {
                s["r.Lumen.DiffuseIndirect.Allow"] = "0";
                s["r.Lumen.Reflections.Allow"] = "0";
            }

            return s;
        }

        private static Dictionary<string, string> BuildScalabilitySettings(
            string preset,
            bool enableLumen,
            double vramGb)
        {
            var sg = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            switch (preset)
            {
                case "Performance (Max FPS)":
                    sg["sg.ViewDistanceQuality"] = "0";
                    sg["sg.AntiAliasingQuality"] = "0";
                    sg["sg.ShadowQuality"] = "0";
                    sg["sg.GlobalIlluminationQuality"] = enableLumen ? "1" : "0";
                    sg["sg.ReflectionQuality"] = "0";
                    sg["sg.PostProcessQuality"] = "0";
                    sg["sg.TextureQuality"] = vramGb >= 8 ? "1" : "0";
                    sg["sg.EffectsQuality"] = "0";
                    sg["sg.FoliageQuality"] = "0";
                    sg["sg.ShadingQuality"] = "0";
                    sg["sg.LandscapeQuality"] = "0";
                    break;

                case "Balanced (Quality & Speed)":
                    sg["sg.ViewDistanceQuality"] = "1";
                    sg["sg.AntiAliasingQuality"] = "1";
                    sg["sg.ShadowQuality"] = "1";
                    sg["sg.GlobalIlluminationQuality"] = enableLumen ? "1" : "0";
                    sg["sg.ReflectionQuality"] = "1";
                    sg["sg.PostProcessQuality"] = "1";
                    sg["sg.TextureQuality"] = vramGb >= 12 ? "2" : "1";
                    sg["sg.EffectsQuality"] = "1";
                    sg["sg.FoliageQuality"] = "1";
                    sg["sg.ShadingQuality"] = "1";
                    sg["sg.LandscapeQuality"] = "1";
                    break;

                case "Ultra (Maximum Visuals)":
                    sg["sg.ViewDistanceQuality"] = "3";
                    sg["sg.AntiAliasingQuality"] = "3";
                    sg["sg.ShadowQuality"] = "3";
                    sg["sg.GlobalIlluminationQuality"] = enableLumen ? "3" : "0";
                    sg["sg.ReflectionQuality"] = "3";
                    sg["sg.PostProcessQuality"] = "3";
                    sg["sg.TextureQuality"] = "3";
                    sg["sg.EffectsQuality"] = "3";
                    sg["sg.FoliageQuality"] = "3";
                    sg["sg.ShadingQuality"] = "3";
                    sg["sg.LandscapeQuality"] = "3";
                    break;

                case "Handheld/SteamDeck":
                    sg["sg.ViewDistanceQuality"] = "1";
                    sg["sg.AntiAliasingQuality"] = "1";
                    sg["sg.ShadowQuality"] = "1";
                    sg["sg.GlobalIlluminationQuality"] = "0";
                    sg["sg.ReflectionQuality"] = "1";
                    sg["sg.PostProcessQuality"] = "1";
                    sg["sg.TextureQuality"] = "1";
                    sg["sg.EffectsQuality"] = "1";
                    sg["sg.FoliageQuality"] = "0";
                    sg["sg.ShadingQuality"] = "1";
                    sg["sg.LandscapeQuality"] = "1";
                    break;

                case "Recommended (Optimized)":
                default:
                    sg["sg.ViewDistanceQuality"] = "3";
                    sg["sg.AntiAliasingQuality"] = "3";
                    sg["sg.ShadowQuality"] = "2";
                    sg["sg.GlobalIlluminationQuality"] = enableLumen ? "2" : "0";
                    sg["sg.ReflectionQuality"] = "2";
                    sg["sg.PostProcessQuality"] = "2";
                    sg["sg.TextureQuality"] = vramGb < 12 ? "1" : "3";
                    sg["sg.EffectsQuality"] = "2";
                    sg["sg.FoliageQuality"] = "3";
                    sg["sg.ShadingQuality"] = "2";
                    sg["sg.LandscapeQuality"] = "3";
                    break;
            }

            return sg;
        }

        private static string FindGameUserSettingsSection(string filePath)
        {
            if (File.Exists(filePath))
            {
                foreach (string line in File.ReadAllLines(filePath))
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("[/") && trimmed.EndsWith("GameUserSettings]", StringComparison.OrdinalIgnoreCase))
                        return trimmed.Trim('[', ']');
                }
            }
            return "/Script/Engine.GameUserSettings";
        }

        private static void BackupFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            string backupPath = filePath + ".universal.bak";
            if (!File.Exists(backupPath))
                File.Copy(filePath, backupPath, overwrite: false);
        }

        public static void MergeIniSections(
            string filePath,
            Dictionary<string, Dictionary<string, string>> sectionsToWrite)
        {
            List<string> lines = File.Exists(filePath)
                ? File.ReadAllLines(filePath).ToList()
                : new List<string>();

            foreach (var sectionEntry in sectionsToWrite)
            {
                string sectionHeader = $"[{sectionEntry.Key}]";
                int sectionStart = lines.FindIndex(l => l.Trim().Equals(sectionHeader, StringComparison.OrdinalIgnoreCase));

                if (sectionStart == -1)
                {
                    if (lines.Count > 0 && !string.IsNullOrWhiteSpace(lines[^1]))
                        lines.Add(string.Empty);

                    lines.Add(sectionHeader);
                    foreach (var kvp in sectionEntry.Value)
                        lines.Add($"{kvp.Key}={kvp.Value}");
                    continue;
                }

                int sectionEnd = lines.Count;
                for (int i = sectionStart + 1; i < lines.Count; i++)
                {
                    string trimmed = lines[i].Trim();
                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        sectionEnd = i;
                        break;
                    }
                }

                var remaining = new Dictionary<string, string>(sectionEntry.Value, StringComparer.OrdinalIgnoreCase);

                for (int i = sectionStart + 1; i < sectionEnd; i++)
                {
                    string trimmed = lines[i].Trim();
                    int eq = trimmed.IndexOf('=');
                    if (eq <= 0 || trimmed.StartsWith(";")) continue;

                    string key = trimmed.Substring(0, eq).Trim();
                    if (remaining.TryGetValue(key, out string? newVal))
                    {
                        lines[i] = $"{key}={newVal}";
                        remaining.Remove(key);
                    }
                }

                if (remaining.Count > 0)
                {
                    int insertAt = sectionEnd;
                    while (insertAt > sectionStart + 1 && string.IsNullOrWhiteSpace(lines[insertAt - 1]))
                        insertAt--;

                    foreach (var kvp in remaining)
                    {
                        lines.Insert(insertAt, $"{kvp.Key}={kvp.Value}");
                        insertAt++;
                    }
                }
            }

            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllLines(filePath, lines);
        }
    }
}