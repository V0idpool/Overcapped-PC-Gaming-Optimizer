using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Overcapped_Optimizer;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Helpers
{
    public static class TrayHandler
    {
        public static void HideFormToTray()
        {
            if (MainFrm.Instance == null || MainFrm.Instance.sysTrayIcon == null) return;

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

            MainFrm.Instance.Show();
            MainFrm.Instance.WindowState = FormWindowState.Normal;
            MainFrm.Instance.ShowInTaskbar = true;
            if (MainFrm.Instance.sysTrayIcon != null)
                MainFrm.Instance.sysTrayIcon.Visible = false;
            MainFrm.Instance.BringToFront();
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

            string exeToLaunch = !string.IsNullOrWhiteSpace(game.LaunchExecutablePath) && File.Exists(game.LaunchExecutablePath)
                ? game.LaunchExecutablePath
                : game.ExecutablePath;

            if (string.IsNullOrWhiteSpace(exeToLaunch) || !File.Exists(exeToLaunch))
            {
                MessageBox.Show($"Could not find executable for {game.Name}:\n{exeToLaunch}", "Executable Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = exeToLaunch,
                    WorkingDirectory = Path.GetDirectoryName(exeToLaunch) ?? game.InstallDirectory,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                MessageBox.Show($"Failed to launch {game.Name}: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}