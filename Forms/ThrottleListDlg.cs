using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Overcapped_Optimizer.Core;

namespace Overcapped_Optimizer
{
    public partial class ThrottleListDlg : Form
    {
        private readonly List<string> _workingList = new();

        public IReadOnlyList<string> UpdatedApps => _workingList;

        public ThrottleListDlg()
        {
            InitializeComponent();
        }

        public ThrottleListDlg(IEnumerable<string> currentApps) : this()
        {
            _workingList.AddRange(currentApps
                .Select(AppSettings.NormalizeProcessName)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase));

            PopulateRunningProcesses();
            RefreshList();
        }

        private void PopulateRunningProcesses()
        {
            cmbRunning.Items.Clear();
            int ownPid = Environment.ProcessId;
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.Id == ownPid) continue;
                    string name = p.ProcessName;
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    if (p.MainWindowHandle != IntPtr.Zero ||
                        name.Contains("wallpaper", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("overwolf", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("spotify", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("icue", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("lghub", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("razer", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("discord", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("signalrgb", StringComparison.OrdinalIgnoreCase))
                    {
                        names.Add(name);
                    }
                }
                catch { }
                finally { p.Dispose(); }
            }

            foreach (string n in names.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                cmbRunning.Items.Add($"{n}.exe");

            if (cmbRunning.Items.Count > 0)
                cmbRunning.SelectedIndex = 0;
        }

        private void AddProcessName(string raw)
        {
            string normalized = AppSettings.NormalizeProcessName(raw);
            if (string.IsNullOrWhiteSpace(normalized)) return;

            if (!_workingList.Any(x => x.Equals(normalized, StringComparison.OrdinalIgnoreCase)))
            {
                _workingList.Add(normalized);
                _workingList.Sort(StringComparer.OrdinalIgnoreCase);
                RefreshList();
            }
        }

        private void RefreshList()
        {
            lstApps.BeginUpdate();
            lstApps.Items.Clear();
            foreach (string proc in _workingList)
                lstApps.Items.Add($"{proc}.exe");
            lstApps.EndUpdate();
        }

        private void RemoveSelected()
        {
            if (lstApps.SelectedIndices.Count == 0) return;

            var toRemove = lstApps.SelectedItems
                .Cast<object>()
                .Select(o => AppSettings.NormalizeProcessName(o.ToString() ?? string.Empty))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _workingList.RemoveAll(x => toRemove.Contains(x));
            RefreshList();
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            AddProcessName(txtNewApp.Text);
            txtNewApp.Text = string.Empty;
        }

        private void btnBrowse_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Applications (*.exe)|*.exe",
                Title = "Select a background application to throttle while gaming"
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
                AddProcessName(ofd.FileName);
        }

        private void btnAddRunning_Click(object? sender, EventArgs e)
        {
            if (cmbRunning.SelectedItem is string item)
                AddProcessName(item);
        }

        private void btnRemove_Click(object? sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            _workingList.Clear();
            _workingList.AddRange(AppSettings.DefaultThrottledApps);
            _workingList.Sort(StringComparer.OrdinalIgnoreCase);
            RefreshList();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lstApps_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveSelected();
        }

        private void lstApps_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstApps.Items.Count) return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color bg = selected ? Color.FromArgb(48, 22, 28) : Color.FromArgb(22, 22, 29);
            Color fg = selected ? Color.White : Color.FromArgb(242, 242, 246);

            using (var bgBrush = new SolidBrush(bg))
                e.Graphics.FillRectangle(bgBrush, e.Bounds);

            if (selected)
            {
                using var bar = new SolidBrush(Color.FromArgb(214, 28, 52));
                e.Graphics.FillRectangle(bar, e.Bounds.Left, e.Bounds.Top, 3, e.Bounds.Height);
            }

            string text = lstApps.Items[e.Index]?.ToString() ?? string.Empty;
            TextRenderer.DrawText(
                e.Graphics,
                text,
                Font,
                new Rectangle(e.Bounds.Left + 10, e.Bounds.Top, e.Bounds.Width - 14, e.Bounds.Height),
                fg,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}