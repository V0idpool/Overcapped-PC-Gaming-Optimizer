using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.Helpers;

namespace Overcapped_Optimizer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            AppSettings.Load();

            bool skipAutoElevate = Array.Exists(
                args, a => a.Equals("--no-elevate", StringComparison.OrdinalIgnoreCase));

            if (AppSettings.Current.StartAsAdmin && !skipAutoElevate && !Optimizer.IsAdministrator())
            {
                if (TryRestartAsAdministrator(args))
                    return;
            }

            Application.Run(new MainFrm());
        }

        private static bool TryRestartAsAdministrator(string[] args)
        {
            try
            {
                string exePath = !string.IsNullOrWhiteSpace(Environment.ProcessPath)
                    ? Environment.ProcessPath
                    : Application.ExecutablePath;

                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = AppContext.BaseDirectory,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                if (args.Length > 0)
                {
                    psi.Arguments = string.Join(" ",
                        Array.ConvertAll(args, a => a.Contains(' ') ? $"\"{a}\"" : a));
                }

                Process.Start(psi);
                return true;
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223) // User canceled UAC prompt
            {
                ExceptionHandler.LogMessage("[Startup] UAC prompt canceled by user, continued in standard mode.");
                return false;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                return false;
            }
        }
    }
}