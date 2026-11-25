using System;
using System.Drawing;
using System.Windows.Forms;

namespace Overcapped_Optimizer.Forms
{
    public partial class DashboardOpacityOverlay : Form
    {
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int MinOpacityPercent = 20;
        private const int MaxOpacityPercent = 100;

        private Form? _attachedOwner;
        private int _opacityPercent = 85;
        private bool _isDragging;

        public event Action<int>? OpacityValueChanged;
        public event Action<int>? OpacityCommitted;

        public DashboardOpacityOverlay()
        {
            InitializeComponent();
            UpdateTrackVisual();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                return cp;
            }
        }

        public void ShowFor(Form owner, int initialPercent)
        {
            _opacityPercent = Math.Clamp(initialPercent, MinOpacityPercent, MaxOpacityPercent);
            UpdateTrackVisual();

            if (_attachedOwner != owner)
            {
                DetachEvents();
                _attachedOwner = owner;
                _attachedOwner.LocationChanged += Owner_BoundsChanged;
                _attachedOwner.SizeChanged += Owner_BoundsChanged;
                _attachedOwner.VisibleChanged += Owner_VisibleChanged;
            }

            Reposition();
            if (!Visible) Show();
            TopMost = true;
        }

        public void DetachAndHide()
        {
            _isDragging = false;
            DetachEvents();
            if (Visible) Hide();
        }

        private void DetachEvents()
        {
            if (_attachedOwner != null)
            {
                _attachedOwner.LocationChanged -= Owner_BoundsChanged;
                _attachedOwner.SizeChanged -= Owner_BoundsChanged;
                _attachedOwner.VisibleChanged -= Owner_VisibleChanged;
                _attachedOwner = null;
            }
        }

        private void Owner_BoundsChanged(object? sender, EventArgs e)
        {
            if (Visible) Reposition();
        }

        private void Owner_VisibleChanged(object? sender, EventArgs e)
        {
            if (_attachedOwner != null && !_attachedOwner.Visible)
                DetachAndHide();
        }

        public void Reposition()
        {
            if (_attachedOwner == null || _attachedOwner.IsDisposed) return;

            Rectangle workArea = Screen.FromControl(_attachedOwner).WorkingArea;
            const int gap = 6;

            int x = _attachedOwner.Left + (_attachedOwner.Width - Width) / 2;
            int y = _attachedOwner.Bottom + gap;

            if (y + Height > workArea.Bottom)
                y = _attachedOwner.Top - Height - gap;

            x = Math.Clamp(x, workArea.Left, Math.Max(workArea.Left, workArea.Right - Width));
            y = Math.Clamp(y, workArea.Top, Math.Max(workArea.Top, workArea.Bottom - Height));

            Location = new Point(x, y);
        }

        private void UpdateTrackVisual()
        {
            float ratio = (float)(_opacityPercent - MinOpacityPercent) / (MaxOpacityPercent - MinOpacityPercent);
            pnlFill.Width = Math.Max(4, (int)Math.Round(pnlTrack.Width * ratio));
            lblPercent.Text = $"{_opacityPercent}%";
        }

        private void UpdateFromCursor(bool commit)
        {
            int localX = pnlTrack.PointToClient(Cursor.Position).X;
            float ratio = Math.Clamp((float)localX / Math.Max(1, pnlTrack.Width), 0f, 1f);
            int nextPercent = MinOpacityPercent + (int)Math.Round(ratio * (MaxOpacityPercent - MinOpacityPercent));

            if (nextPercent != _opacityPercent)
            {
                _opacityPercent = nextPercent;
                UpdateTrackVisual();
                OpacityValueChanged?.Invoke(_opacityPercent);
            }

            if (commit)
                OpacityCommitted?.Invoke(_opacityPercent);
        }

        private void Track_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _isDragging = true;
            if (sender is Control ctrl)
                ctrl.Capture = true;

            UpdateFromCursor(commit: false);
        }

        private void Track_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_isDragging)
                UpdateFromCursor(commit: false);
        }

        private void Track_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!_isDragging || e.Button != MouseButtons.Left) return;

            _isDragging = false;
            if (sender is Control ctrl)
                ctrl.Capture = false;

            UpdateFromCursor(commit: true);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DetachEvents();
            base.OnFormClosed(e);
        }
    }
}