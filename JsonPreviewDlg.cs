using Overcapped_Optimizer.GUI;
using System;
using System.Windows.Forms;

namespace Overcapped_Optimizer
{
    public partial class JsonPreviewDlg : Form
    {
        private string _jsonText = string.Empty;

        public JsonPreviewDlg()
        {
            InitializeComponent();
        }

        public JsonPreviewDlg(string fileName, string json) : this()
        {
            _jsonText = json ?? string.Empty;
            Text = $"JSON Preview: {fileName}";
            roThemeContainer1.SubHeader = fileName;
            txtJson.Text = _jsonText;
            txtJson.SelectionStart = 0;
            txtJson.SelectionLength = 0;

            int lines = string.IsNullOrEmpty(_jsonText) ? 0 : _jsonText.Split('\n').Length;
            lblFileInfo.Text = $"{fileName}  |  {lines} lines";
        }

        private void btnCopy_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_jsonText)) return;
            Clipboard.SetText(_jsonText);
            btnCopy.Text = "Copied!";
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}