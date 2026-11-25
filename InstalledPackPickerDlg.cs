using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Overcapped_Optimizer.Core;

namespace Overcapped_Optimizer
{
    public partial class InstalledPackPickerDlg : Form
    {
        private readonly List<GameProfilePack> _packs = new();

        public GameProfilePack? SelectedPack =>
            lstPacks.SelectedIndex >= 0 && lstPacks.SelectedIndex < _packs.Count
                ? _packs[lstPacks.SelectedIndex]
                : null;

        public InstalledPackPickerDlg()
        {
            InitializeComponent();
        }

        public InstalledPackPickerDlg(IEnumerable<GameProfilePack> packs) : this()
        {
            _packs.AddRange(packs.OrderBy(p => p.GameName, StringComparer.OrdinalIgnoreCase));

            lstPacks.BeginUpdate();
            lstPacks.Items.Clear();
            foreach (GameProfilePack p in _packs)
                lstPacks.Items.Add($"{p.GameName} ({p.PackId}.json | {p.MatchExecutable})");
            lstPacks.EndUpdate();

            if (lstPacks.Items.Count > 0)
                lstPacks.SelectedIndex = 0;
        }

        private void lstPacks_DoubleClick(object? sender, EventArgs e)
        {
            if (lstPacks.SelectedIndex >= 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void lstPacks_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstPacks.Items.Count) return;

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

            string text = lstPacks.Items[e.Index]?.ToString() ?? string.Empty;
            TextRenderer.DrawText(
                e.Graphics,
                text,
                Font,
                new Rectangle(e.Bounds.Left + 10, e.Bounds.Top, e.Bounds.Width - 14, e.Bounds.Height),
                fg,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void btnLoad_Click(object? sender, EventArgs e)
        {
            if (lstPacks.SelectedIndex < 0) return;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}