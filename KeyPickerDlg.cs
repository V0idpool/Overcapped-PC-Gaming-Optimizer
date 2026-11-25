using Overcapped_Optimizer.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Overcapped_Optimizer
{
    public partial class KeyPickerDlg : Form
    {
        private readonly List<KeyValuePair<string, string>> _ordered = new();

        public Dictionary<string, string> SelectedEntries { get; } = new(StringComparer.OrdinalIgnoreCase);

        public KeyPickerDlg()
        {
            InitializeComponent();
        }

        public KeyPickerDlg(Dictionary<string, string> parsed, string fileName, string targetScopeName) : this()
        {
            Text = $"Pull Keys from {fileName}";
            roThemeContainer1.SubHeader = fileName;
            lblPrompt.Text = $"Found {parsed.Count} keys in {fileName}. Check the keys you want to pull into '{targetScopeName}':";

            _ordered.AddRange(parsed.OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase));

            clbKeys.BeginUpdate();
            clbKeys.Items.Clear();
            foreach (var kvp in _ordered)
                clbKeys.Items.Add($"{kvp.Key} = {kvp.Value}", false);
            clbKeys.EndUpdate();
        }

        private void btnSelectAll_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < clbKeys.Items.Count; i++)
                clbKeys.SetItemChecked(i, true);
        }

        private void btnClear_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < clbKeys.Items.Count; i++)
                clbKeys.SetItemChecked(i, false);
        }

        private void btnImport_Click(object? sender, EventArgs e)
        {
            SelectedEntries.Clear();
            foreach (int idx in clbKeys.CheckedIndices)
            {
                var kvp = _ordered[idx];
                SelectedEntries[kvp.Key] = kvp.Value;
            }

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