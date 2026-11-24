using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;
using Overcapped_Optimizer.Helpers;

namespace Overcapped_Optimizer
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
        private static readonly Color KeyColor = Color.FromArgb(1, 1, 1);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void InitializeComponent()
        {

        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private readonly EtwFpsMonitor _monitor = new();
        private readonly System.Windows.Forms.Timer _refreshTimer;
        private readonly int _ownPid = Environment.ProcessId;

        private int _currentFps;
        private double _currentMs;
        private bool _etwStarted;
        private bool _dragMode;
        private double _targetOpacity = 0.75;

        public bool IsDragMode => _dragMode;
        public event Action<int, int>? CustomPositionSaved;
        public event Action<bool>? DragModeChanged;

        public FpsOverlay()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Size = new Size(164, 34);
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

        public bool StartOverlay(string position, int customX, int customY, int opacityPercent = 75)
        {
            _etwStarted = _monitor.Start();
            ApplyOpacity(opacityPercent);
            ApplyPosition(position, customX, customY);
            _refreshTimer.Start();
            if (!Visible) Show();
            ApplyPosition(position, customX, customY);
            return _etwStarted;
        }

        public void StopOverlay()
        {
            _refreshTimer.Stop();
            _monitor.Stop();
            SetDragMode(false);
            if (Visible) Hide();
        }

        public void ApplyOpacity(int opacityPercent)
        {
            _targetOpacity = Math.Clamp(opacityPercent, 5, 100) / 100.0;
            Opacity = _dragMode ? Math.Max(_targetOpacity, 0.92) : _targetOpacity;
        }

        public void SetDragMode(bool enabled)
        {
            _dragMode = enabled;
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
            Size = enabled ? new Size(242, 38) : new Size(164, 34);
            Opacity = enabled ? Math.Max(_targetOpacity, 0.92) : _targetOpacity;
            Invalidate();
            DragModeChanged?.Invoke(enabled);
        }

        public void ApplyPosition(string position, int customX, int customY)
        {
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

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            IntPtr fg = GetForegroundWindow();
            if (fg != IntPtr.Zero)
            {
                GetWindowThreadProcessId(fg, out uint fgPid);
                if (fgPid != 0 && fgPid != (uint)_ownPid)
                {
                    _monitor.SetTargetProcessId((int)fgPid);
                }
            }

            if (_etwStarted)
            {
                (_currentFps, _currentMs) = _monitor.GetCurrentStats();
            }

            if (!TopMost) TopMost = true;
            Invalidate();
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
            using Font msFont = new("Segoe UI", 8.25f, FontStyle.Regular);

            ThemeModule.DrawText(g, fpsText, fpsFont, new Rectangle(12, 0, 82, Height), accentColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            ThemeModule.DrawText(g, msText, msFont, new Rectangle(94, 0, Width - 102, Height), ROPalette.TextSecondary,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
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