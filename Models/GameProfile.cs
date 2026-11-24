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
        Epic = 2
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
        public string? JsonPackId { get; set; }
        public bool MonitorGame { get; set; } = true;
        public bool HighPriorityIfeo { get; set; } = true;
        public bool HighPerfGpuBinding { get; set; } = true;
        public bool QosDscpEnabled { get; set; } = false;
        public string SelectedPreset { get; set; } = "Recommended (Optimized)";
        public bool EnableLumen { get; set; } = true;
        public bool EnableFog { get; set; } = false;
        public int FpsLimit { get; set; } = 0;
        public int MenuFpsLimit { get; set; } = 60;
        public int ScreenMode { get; set; } = 1;

        public string ProcessNameWithoutExtension => string.IsNullOrWhiteSpace(ExecutableName) ? string.Empty : System.IO.Path.GetFileNameWithoutExtension(ExecutableName);

        public override string ToString() => $"{Name} [{Engine}]";
    }
}