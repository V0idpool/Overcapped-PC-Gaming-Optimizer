using System;

namespace Overcapped_Optimizer.Models
{
    public enum GameEngineType
    {
        Generic = 0,
        UnrealEngine = 1,
        Unity = 2,
        Source = 3
    }

    public enum GameLauncherSource
    {
        Manual = 0,
        Steam = 1,
        Epic = 2,
        Gog = 3,
        Xbox = 4,
        BattleNet = 5,
        EaApp = 6
    }

    public enum AutoOptimizeOverride
    {
        UseGlobal = 0,
        ForceOn = 1,
        ForceOff = 2
    }

    public sealed class GameProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = string.Empty;
        public string ExecutableName { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public string LaunchExecutablePath { get; set; } = string.Empty;
        public string InstallDirectory { get; set; } = string.Empty;
        public GameLauncherSource Launcher { get; set; } = GameLauncherSource.Manual;
        public GameEngineType Engine { get; set; } = GameEngineType.Generic;
        public string ConfigDirectory { get; set; } = string.Empty;
        public string LaunchArguments { get; set; } = string.Empty;
        public string? JsonPackId { get; set; }
        public bool MonitorGame { get; set; } = true;
        public AutoOptimizeOverride PCoresOverride { get; set; } = AutoOptimizeOverride.UseGlobal;
        public AutoOptimizeOverride AutoPowerPlanOverride { get; set; } = AutoOptimizeOverride.UseGlobal;
        public AutoOptimizeOverride ThrottleAppsOverride { get; set; } = AutoOptimizeOverride.UseGlobal;
        public bool HighPriorityIfeo { get; set; } = true;
        public bool HighPerfGpuBinding { get; set; } = true;
        public bool QosDscpEnabled { get; set; } = false;
        public string SelectedPreset { get; set; } = "Recommended (Optimized)";
        public bool EnableLumen { get; set; } = true;
        public bool EnableFog { get; set; } = false;
        public int FpsLimit { get; set; } = 0;
        public int MenuFpsLimit { get; set; } = 60;
        public int ScreenMode { get; set; } = 1;

        public string ProcessNameWithoutExtension =>
            string.IsNullOrWhiteSpace(ExecutableName)
                ? string.Empty
                : System.IO.Path.GetFileNameWithoutExtension(ExecutableName);

        public bool HasCustomAutoOptimizes =>
            !MonitorGame ||
            PCoresOverride != AutoOptimizeOverride.UseGlobal ||
            AutoPowerPlanOverride != AutoOptimizeOverride.UseGlobal ||
            ThrottleAppsOverride != AutoOptimizeOverride.UseGlobal;

        public static bool ResolveAutoOptimize(AutoOptimizeOverride rule, bool globalValue)
        {
            return rule switch
            {
                AutoOptimizeOverride.ForceOn => true,
                AutoOptimizeOverride.ForceOff => false,
                _ => globalValue
            };
        }

        public override string ToString() => $"{Name} [{Engine}]";
    }
}