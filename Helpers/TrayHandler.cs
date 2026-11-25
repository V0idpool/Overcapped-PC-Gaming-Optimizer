using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Overcapped_Optimizer;
using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Helpers
{
    public static class TrayHandler
    {
        private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
        private const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        private const uint TOKEN_DUPLICATE = 0x0002;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_ADJUST_DEFAULT = 0x0080;
        private const uint TOKEN_ADJUST_SESSIONID = 0x0100;
        private const int SecurityImpersonation = 2;
        private const int TokenPrimary = 1;

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            uint dwDesiredAccess,
            IntPtr lpTokenAttributes,
            int ImpersonationLevel,
            int TokenType,
            out IntPtr phNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateProcessWithTokenW(
            IntPtr hToken,
            uint dwLogonFlags,
            string? lpApplicationName,
            string? lpCommandLine,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string? lpCurrentDirectory,
            ref STARTUPINFOW lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFOW
        {
            public int cb;
            public string? lpReserved;
            public string? lpDesktop;
            public string? lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        public static void HideFormToTray()
        {
            if (MainFrm.Instance == null || MainFrm.Instance.sysTrayIcon == null) return;

            MainFrm.Instance.ExitHotkeyOverlayMode();
            MainFrm.Instance.Hide();
            MainFrm.Instance.ShowInTaskbar = false;
            MainFrm.Instance.sysTrayIcon.Visible = true;
            MainFrm.Instance.sysTrayIcon.BalloonTipTitle = "Overcapped Optimizer";
            MainFrm.Instance.sysTrayIcon.BalloonTipText = "Monitoring games and running in the background...";
            MainFrm.Instance.sysTrayIcon.BalloonTipIcon = ToolTipIcon.Info;
            MainFrm.Instance.sysTrayIcon.ShowBalloonTip(3000);
        }

        public static void ShowFormFromTray()
        {
            if (MainFrm.Instance == null) return;

            MainFrm.Instance.ExitHotkeyOverlayMode();
            MainFrm.Instance.Show();
            MainFrm.Instance.WindowState = FormWindowState.Normal;
            MainFrm.Instance.ShowInTaskbar = true;
            if (MainFrm.Instance.sysTrayIcon != null)
                MainFrm.Instance.sysTrayIcon.Visible = false;
            MainFrm.Instance.BringToFront();
            MainFrm.Instance.Activate();
        }

        public static void LaunchGame(GameProfile? game)
        {
            if (game == null)
            {
                MessageBox.Show("Select or scan a game on the Dashboard first.", "No Game Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(game.ProcessNameWithoutExtension) &&
                Process.GetProcessesByName(game.ProcessNameWithoutExtension).Length > 0)
            {
                MessageBox.Show($"{game.Name} is already running.", "Game Already Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (game.Launcher == GameLauncherSource.Steam)
                {
                    string? appId = !string.IsNullOrWhiteSpace(game.LauncherAppId)
                        ? game.LauncherAppId
                        : GameScanner.TryResolveSteamAppId(game.InstallDirectory);

                    if (!string.IsNullOrWhiteSpace(appId))
                    {
                        if (string.IsNullOrWhiteSpace(game.LauncherAppId))
                        {
                            game.LauncherAppId = appId;
                            AppSettings.Save();
                        }

                        string args = game.LaunchArguments?.Trim() ?? string.Empty;
                        if (!string.IsNullOrEmpty(args))
                        {
                            string? steamDir = GameScanner.GetSteamInstallPath();
                            string steamExe = !string.IsNullOrWhiteSpace(steamDir) ? Path.Combine(steamDir, "steam.exe") : string.Empty;

                            if (File.Exists(steamExe))
                            {
                                LaunchProcessStandardUser(steamExe, steamDir, $"-applaunch {appId} {args}");
                                return;
                            }
                        }

                        LaunchProcessStandardUser($"steam://rungameid/{appId}");
                        return;
                    }
                }

                if (game.Launcher == GameLauncherSource.Epic && !string.IsNullOrWhiteSpace(game.LauncherAppId))
                {
                    string epicUri = $"com.epicgames.launcher://apps/{Uri.EscapeDataString(game.LauncherAppId)}?action=launch&silent=true";
                    LaunchProcessStandardUser(epicUri);
                    return;
                }

                string exeToLaunch = !string.IsNullOrWhiteSpace(game.LaunchExecutablePath) && File.Exists(game.LaunchExecutablePath)
                    ? game.LaunchExecutablePath
                    : game.ExecutablePath;

                if (string.IsNullOrWhiteSpace(exeToLaunch) || !File.Exists(exeToLaunch))
                {
                    MessageBox.Show($"Could not find executable for {game.Name}:\n{exeToLaunch}", "Executable Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string workingDir = Path.GetDirectoryName(exeToLaunch) ?? game.InstallDirectory;
                LaunchProcessStandardUser(exeToLaunch, workingDir, game.LaunchArguments);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                MessageBox.Show($"Failed to launch {game.Name}: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void LaunchProcessStandardUser(string targetPathOrUri, string? workingDirectory = null, string? arguments = null)
        {
            string safeArgs = arguments?.Trim() ?? string.Empty;

            if (!Optimizer.IsAdministrator())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = targetPathOrUri,
                    Arguments = safeArgs,
                    WorkingDirectory = !string.IsNullOrWhiteSpace(workingDirectory) && Directory.Exists(workingDirectory)
                        ? workingDirectory
                        : (File.Exists(targetPathOrUri) ? Path.GetDirectoryName(targetPathOrUri) ?? "" : ""),
                    UseShellExecute = true
                });
                return;
            }

            if (!File.Exists(targetPathOrUri))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"\"{targetPathOrUri}\"")
                {
                    UseShellExecute = true
                });
                return;
            }

            string cwd = !string.IsNullOrWhiteSpace(workingDirectory) && Directory.Exists(workingDirectory)
                ? workingDirectory
                : Path.GetDirectoryName(targetPathOrUri) ?? AppContext.BaseDirectory;

            if (TryLaunchViaShellToken(targetPathOrUri, cwd, safeArgs))
                return;

            if (!string.IsNullOrEmpty(safeArgs))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = targetPathOrUri,
                    Arguments = safeArgs,
                    WorkingDirectory = cwd,
                    UseShellExecute = true
                });
                return;
            }

            Process.Start(new ProcessStartInfo("explorer.exe", $"\"{targetPathOrUri}\"")
            {
                UseShellExecute = true
            });
        }

        private static bool TryLaunchViaShellToken(string exePath, string workingDirectory, string arguments)
        {
            IntPtr shellProcess = IntPtr.Zero;
            IntPtr shellToken = IntPtr.Zero;
            IntPtr primaryToken = IntPtr.Zero;

            try
            {
                IntPtr shellWnd = GetShellWindow();
                if (shellWnd == IntPtr.Zero) return false;

                GetWindowThreadProcessId(shellWnd, out uint shellPid);
                if (shellPid == 0) return false;

                shellProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, shellPid);
                if (shellProcess == IntPtr.Zero) return false;

                if (!OpenProcessToken(shellProcess, TOKEN_DUPLICATE | TOKEN_QUERY, out shellToken))
                    return false;

                uint access = TOKEN_QUERY | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID;
                if (!DuplicateTokenEx(shellToken, access, IntPtr.Zero, SecurityImpersonation, TokenPrimary, out primaryToken))
                    return false;

                string cmdLine = string.IsNullOrWhiteSpace(arguments)
                    ? $"\"{exePath}\""
                    : $"\"{exePath}\" {arguments}";

                var si = new STARTUPINFOW { cb = Marshal.SizeOf<STARTUPINFOW>() };
                bool created = CreateProcessWithTokenW(
                    primaryToken,
                    0,
                    exePath,
                    cmdLine,
                    0,
                    IntPtr.Zero,
                    workingDirectory,
                    ref si,
                    out PROCESS_INFORMATION pi);

                if (created)
                {
                    if (pi.hThread != IntPtr.Zero) CloseHandle(pi.hThread);
                    if (pi.hProcess != IntPtr.Zero) CloseHandle(pi.hProcess);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (primaryToken != IntPtr.Zero) CloseHandle(primaryToken);
                if (shellToken != IntPtr.Zero) CloseHandle(shellToken);
                if (shellProcess != IntPtr.Zero) CloseHandle(shellProcess);
            }
        }
    }
}