using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.GUI;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer.Forms
{
    public sealed class FpsOverlay : Form
    {
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
        private const uint STILL_ACTIVE = 259;
        private static readonly Color KeyColor = Color.FromArgb(1, 1, 1);

        private static readonly HashSet<string> NonGameProcesses = new(StringComparer.OrdinalIgnoreCase)
        {
            "explorer", "dwm", "chrome", "msedge", "firefox", "brave", "opera", "opera_gx",
            "discord", "steam", "steamwebhelper", "epicgameslauncher", "devenv", "code",
            "obs64", "obs32", "spotify", "applicationframehost", "shellexperiencehost",
            "searchhost", "startmenuexperiencehost", "taskmgr", "windowsterminal", "wt",
            "powershell", "pwsh", "cmd", "notepad", "msiexec", "vlc", "mpc-hc64"
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private void InitializeComponent()
        {
        }

        private readonly EtwFpsMonitor _monitor = new();
        private readonly System.Windows.Forms.Timer _refreshTimer;
        private readonly int _ownPid = Environment.ProcessId;

        private int _currentFps;
        private int _currentOnePercentLow;
        private double _currentMs;
        private bool _etwStarted;
        private bool _dragMode;
        private bool _show1PercentLow = true;
        private double _targetOpacity = 0.75;
        private int _lastCheckedFgPid;
        private string _lastCheckedFgProcName = string.Empty;
        private int _tickCounter;

        private string _lastPosition = "Top Left";
        private int _lastCustomX = 24;
        private int _lastCustomY = 24;

        public bool IsDragMode => _dragMode;
        public bool IsEtwRunning => _etwStarted;
        public event Action<int, int>? CustomPositionSaved;
        public event Action<bool>? DragModeChanged;
        public event Action<FpsSessionSummary>? SessionCompleted;
        private bool _userHidden;

        public FpsOverlay()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Size = new Size(270, 34);
            BackColor = KeyColor;
            TransparencyKey = KeyColor;
            Opacity = _targetOpacity;
            DoubleBuffered = true;

            _refreshTimer = new System.Windows.Forms.Timer { Interval = 250 };
            _refreshTimer.Tick += RefreshTimer_Tick;
        }

        protected override bool ShowWithoutActivation => !_dragMode;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                if (!_dragMode) cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }

        public bool StartOverlay(string position, int customX, int customY, int opacityPercent = 75, bool show1PercentLow = true)
        {
            _userHidden = false;
            _show1PercentLow = show1PercentLow;
            UpdateOverlaySize();
            _etwStarted = _monitor.Start();
            ApplyOpacity(opacityPercent);
            ApplyPosition(position, customX, customY);
            _refreshTimer.Start();

            int fgPid = GetForegroundPid(out _);
            UpdateOverlayVisibility(IsActiveSessionFocused(fgPid));
            return _etwStarted;
        }

        public void StopOverlay()
        {
            _refreshTimer.Stop();
            _monitor.SetSessionPaused(false);
            _monitor.Stop();
            SetDragMode(false);
            if (Visible) Hide();
        }

        public void SetShow1PercentLow(bool show)
        {
            _show1PercentLow = show;
            UpdateOverlaySize();
            ApplyPosition(_lastPosition, _lastCustomX, _lastCustomY);
            if (Visible) Invalidate();
        }

        public void BeginGameSession(GameProfile game, int pid)
        {
            if (!_etwStarted || pid <= 0) return;

            string actualProcName = TryGetProcessName(pid);
            if (string.IsNullOrWhiteSpace(actualProcName))
                actualProcName = game.ProcessNameWithoutExtension;

            _monitor.BeginSession(
                game.Name,
                game.ExecutableName,
                actualProcName,
                game.SelectedPreset,
                game.Engine.ToString(),
                pid,
                game);

            int fgPid = GetForegroundPid(out _);
            bool focused = IsActiveSessionFocused(fgPid);
            _monitor.SetSessionPaused(!focused);
            _monitor.SetTargetProcessId(focused ? fgPid : 0);
            UpdateOverlayVisibility(focused);
        }

        public bool ToggleUserHidden()
        {
            _userHidden = !_userHidden;
            int fgPid = GetForegroundPid(out _);
            UpdateOverlayVisibility(IsActiveSessionFocused(fgPid));
            return _userHidden;
        }

        public FpsSessionSummary? EndGameSession()
        {
            FpsSessionSummary? summary = _monitor.EndSession();
            UpdateOverlayVisibility(false);
            return summary;
        }

        public void ApplyOpacity(int opacityPercent)
        {
            _targetOpacity = Math.Clamp(opacityPercent, 5, 100) / 100.0;
            Opacity = _dragMode ? Math.Max(_targetOpacity, 0.92) : _targetOpacity;
        }

        public void SetDragMode(bool enabled)
        {
            _dragMode = enabled;

            if (enabled && !Visible)
            {
                ApplyPosition(_lastPosition, _lastCustomX, _lastCustomY);
                Show();
            }

            if (IsHandleCreated)
            {
                int exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
                if (enabled)
                    exStyle &= ~(WS_EX_TRANSPARENT | WS_EX_NOACTIVATE);
                else
                    exStyle |= WS_EX_TRANSPARENT | WS_EX_NOACTIVATE;

                SetWindowLong(Handle, GWL_EXSTYLE, exStyle);
            }

            Cursor = enabled ? Cursors.SizeAll : Cursors.Default;
            UpdateOverlaySize();
            ApplyPosition(_lastPosition, _lastCustomX, _lastCustomY);
            Opacity = enabled ? Math.Max(_targetOpacity, 0.92) : _targetOpacity;

            if (!enabled)
            {
                int fgPid = GetForegroundPid(out _);
                UpdateOverlayVisibility(IsActiveSessionFocused(fgPid));
            }

            if (Visible) Invalidate();
            DragModeChanged?.Invoke(enabled);
        }

        private void UpdateOverlaySize()
        {
            if (_dragMode)
                Size = new Size(242, 38);
            else
                Size = _show1PercentLow ? new Size(270, 34) : new Size(164, 34);
        }

        public void ApplyPosition(string position, int customX, int customY)
        {
            _lastPosition = position;
            _lastCustomX = customX;
            _lastCustomY = customY;

            Rectangle area = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
            const int margin = 16;

            Location = position switch
            {
                "Top Right" => new Point(area.Right - Width - margin, area.Top + margin),
                "Top Center" => new Point(area.Left + (area.Width - Width) / 2, area.Top + margin),
                "Bottom Left" => new Point(area.Left + margin, area.Bottom - Height - margin),
                "Bottom Right" => new Point(area.Right - Width - margin, area.Bottom - Height - margin),
                "Custom" => ClampToVirtualScreen(customX, customY),
                _ => new Point(area.Left + margin, area.Top + margin)
            };
        }

        private Point ClampToVirtualScreen(int x, int y)
        {
            Rectangle v = SystemInformation.VirtualScreen;
            int clampedX = Math.Clamp(x, v.Left, Math.Max(v.Left, v.Right - Width));
            int clampedY = Math.Clamp(y, v.Top, Math.Max(v.Top, v.Bottom - Height));
            return new Point(clampedX, clampedY);
        }

        private int GetForegroundPid(out IntPtr fgHwnd)
        {
            fgHwnd = GetForegroundWindow();
            int fgPid = 0;

            if (fgHwnd != IntPtr.Zero)
            {
                GetWindowThreadProcessId(fgHwnd, out uint rawPid);
                if (rawPid != 0 && rawPid != (uint)_ownPid)
                {
                    fgPid = (int)rawPid;
                }
            }

            if (fgPid != _lastCheckedFgPid)
            {
                _lastCheckedFgPid = fgPid;
                _lastCheckedFgProcName = fgPid > 0 ? TryGetProcessName(fgPid) : string.Empty;
            }

            return fgPid;
        }

        private bool IsActiveSessionFocused(int fgPid)
        {
            if (!_monitor.HasActiveSession || fgPid <= 0)
                return false;

            if (fgPid == _monitor.ActiveSessionPid)
                return true;

            if (!string.IsNullOrWhiteSpace(_monitor.ActiveSessionProcessName) &&
                !string.IsNullOrWhiteSpace(_lastCheckedFgProcName) &&
                _lastCheckedFgProcName.Equals(_monitor.ActiveSessionProcessName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private void UpdateOverlayVisibility(bool gameFocused)
        {
            bool shouldShow = _dragMode || (_etwStarted && gameFocused && !_userHidden);

            if (shouldShow)
            {
                if (!Visible)
                {
                    ApplyPosition(_lastPosition, _lastCustomX, _lastCustomY);
                    Show();
                }
            }
            else
            {
                if (Visible)
                {
                    Hide();
                }
            }
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            _tickCounter++;
            int fgPid = GetForegroundPid(out IntPtr fg);

            if (_etwStarted)
            {
                UpdateSessionTracking(fg, fgPid);

                bool activeGameFocused = IsActiveSessionFocused(fgPid);

                if (_monitor.HasActiveSession)
                {
                    if (activeGameFocused)
                    {
                        _monitor.SetTargetProcessId(fgPid);
                        _monitor.SetSessionPaused(false);
                        (_currentFps, _currentMs, _currentOnePercentLow) = _monitor.GetDetailedStats();
                        if (_tickCounter % 8 == 0 && Optimizer.TryGetMemoryStats(out _, out _, out double availPhysGb))
                        {
                            _monitor.RecordRamSample(availPhysGb);
                        }
                    }
                    else
                    {
                        _monitor.SetSessionPaused(true);
                        _monitor.SetTargetProcessId(0);
                        _currentFps = 0;
                        _currentMs = 0;
                        _currentOnePercentLow = 0;
                    }
                }
                else
                {
                    if (fgPid > 0 &&
                        !string.IsNullOrWhiteSpace(_lastCheckedFgProcName) &&
                        !NonGameProcesses.Contains(_lastCheckedFgProcName))
                    {
                        _monitor.SetTargetProcessId(fgPid);
                        (_currentFps, _currentMs, _currentOnePercentLow) = _monitor.GetDetailedStats();
                    }
                    else
                    {
                        _monitor.SetTargetProcessId(0);
                        _currentFps = 0;
                        _currentMs = 0;
                        _currentOnePercentLow = 0;
                    }
                }

                UpdateOverlayVisibility(activeGameFocused);
            }
            else
            {
                UpdateOverlayVisibility(false);
            }

            if (Visible)
            {
                if (!TopMost) TopMost = true;
                Invalidate();
            }
        }

        private void UpdateSessionTracking(IntPtr fgHwnd, int fgPid)
        {
            if (_monitor.HasActiveSession && _tickCounter % 2 == 0)
            {
                int activePid = _monitor.ActiveSessionPid;
                string activeProcName = _monitor.ActiveSessionProcessName;

                if (!IsProcessAlive(activePid, activeProcName))
                {
                    FpsSessionSummary? finished = _monitor.EndSession();
                    if (finished != null)
                        SessionCompleted?.Invoke(finished);
                }
            }

            if (fgPid <= 0 || IsActiveSessionFocused(fgPid))
                return;

            if (string.IsNullOrWhiteSpace(_lastCheckedFgProcName))
                return;

            GameProfile? matchedGame = AppSettings.Current.Games.FirstOrDefault(g =>
                (!string.IsNullOrWhiteSpace(g.ProcessNameWithoutExtension) &&
                 g.ProcessNameWithoutExtension.Equals(_lastCheckedFgProcName, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(g.LaunchExecutablePath) &&
                 Path.GetFileNameWithoutExtension(g.LaunchExecutablePath).Equals(_lastCheckedFgProcName, StringComparison.OrdinalIgnoreCase)));

            if (matchedGame != null)
            {
                if (_monitor.HasActiveSession)
                {
                    FpsSessionSummary? previous = _monitor.EndSession();
                    if (previous != null)
                        SessionCompleted?.Invoke(previous);
                }

                BeginGameSession(matchedGame, fgPid);
            }
            else if (!_monitor.HasActiveSession &&
                     _currentFps >= 10 &&
                     !NonGameProcesses.Contains(_lastCheckedFgProcName) &&
                     (IsWindowFullscreen(fgHwnd) || _currentFps >= 20))
            {
                _monitor.BeginSession(
                    _lastCheckedFgProcName,
                    $"{_lastCheckedFgProcName}.exe",
                    _lastCheckedFgProcName,
                    "Unlisted Game",
                    "Detected Game",
                    fgPid);
                _monitor.SetSessionPaused(false);
            }
        }

        private static string TryGetProcessName(int pid)
        {
            try
            {
                using Process p = Process.GetProcessById(pid);
                return p.ProcessName;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool IsProcessAlive(int pid, string processName)
        {
            if (pid <= 0) return false;

            IntPtr hProc = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, (uint)pid);
            if (hProc != IntPtr.Zero)
            {
                try
                {
                    if (GetExitCodeProcess(hProc, out uint exitCode))
                        return exitCode == STILL_ACTIVE;
                }
                finally
                {
                    CloseHandle(hProc);
                }
            }
            else
            {
                int err = Marshal.GetLastWin32Error();
                if (err == 87)
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(processName))
            {
                Process[] procs = Process.GetProcessesByName(processName);
                bool alive = false;
                foreach (Process p in procs)
                {
                    try
                    {
                        if (p.Id == pid) alive = true;
                    }
                    catch { }
                    finally { p.Dispose(); }
                }
                return alive;
            }

            return false;
        }

        private static bool IsWindowFullscreen(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero || !GetWindowRect(hwnd, out RECT rect))
                return false;

            Rectangle screen = Screen.FromHandle(hwnd).Bounds;
            int w = rect.Right - rect.Left;
            int h = rect.Bottom - rect.Top;
            return w >= screen.Width - 2 && h >= screen.Height - 2;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!_dragMode) return;

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                CustomPositionSaved?.Invoke(Location.X, Location.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                CustomPositionSaved?.Invoke(Location.X, Location.Y);
                SetDragMode(false);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5);
            using (SolidBrush bg = new(ROPalette.BgDeep))
                g.FillPath(bg, path);

            Color accentColor = _dragMode
                ? ROPalette.AccentRustBright
                : (!_etwStarted
                    ? ROPalette.AccentCrimson
                    : (_currentFps >= 60 ? ROPalette.HudGreen : (_currentFps >= 30 ? Color.FromArgb(245, 158, 11) : ROPalette.AccentCrimson)));

            GraphicsState st = g.Save();
            g.SetClip(path);
            using (SolidBrush bar = new(accentColor))
                g.FillRectangle(bar, 0, 0, 4, Height);
            g.Restore(st);

            using (Pen border = new(_dragMode ? ROPalette.AccentRustBright : ROPalette.BorderSubtle, _dragMode ? 1.5f : 1f))
                g.DrawPath(border, path);

            if (_dragMode)
            {
                using Font dragFont = new("Segoe UI", 8.25f, FontStyle.Bold);
                ThemeModule.DrawText(g, "DRAG TO MOVE (RIGHT-CLICK TO LOCK)", dragFont,
                    new Rectangle(10, 0, Width - 14, Height), ROPalette.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                return;
            }

            if (!_etwStarted)
            {
                using Font errFont = new("Segoe UI", 8.5f, FontStyle.Bold);
                ThemeModule.DrawText(g, "FPS: ADMIN REQUIRED", errFont,
                    new Rectangle(12, 0, Width - 16, Height), ROPalette.AccentCrimson,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                return;
            }

            string fpsText = _currentFps > 0 ? $"{_currentFps} FPS" : "-- FPS";
            string msText = _currentFps > 0 ? $"{_currentMs:F1} ms" : "IDLE";

            using Font fpsFont = new("Segoe UI", 9.75f, FontStyle.Bold);
            using Font lowFont = new("Segoe UI", 8.5f, FontStyle.Bold);
            using Font msFont = new("Segoe UI", 8.25f, FontStyle.Regular);

            if (_show1PercentLow)
            {
                string lowText = (_currentFps > 0 && _currentOnePercentLow > 0)
                    ? $"1% LOW: {_currentOnePercentLow}"
                    : "1% LOW: --";

                Color lowColor = (_currentFps > 0 && _currentOnePercentLow > 0)
                    ? (_currentOnePercentLow >= 60 ? ROPalette.HudGreen : (_currentOnePercentLow >= 30 ? Color.FromArgb(245, 158, 11) : ROPalette.AccentCrimson))
                    : ROPalette.TextSecondary;

                ThemeModule.DrawText(g, fpsText, fpsFont, new Rectangle(12, 0, 74, Height), accentColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                ThemeModule.DrawText(g, lowText, lowFont, new Rectangle(88, 0, 102, Height), lowColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                ThemeModule.DrawText(g, msText, msFont, new Rectangle(192, 0, Width - 202, Height), ROPalette.TextSecondary,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }
            else
            {
                ThemeModule.DrawText(g, fpsText, fpsFont, new Rectangle(12, 0, 82, Height), accentColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                ThemeModule.DrawText(g, msText, msFont, new Rectangle(94, 0, Width - 102, Height), ROPalette.TextSecondary,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTimer.Dispose();
                _monitor.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}