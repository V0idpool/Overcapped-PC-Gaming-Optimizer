using Overcapped_Optimizer.GUI;
using System;
using System.Windows.Forms;

namespace Overcapped_Optimizer
{
    public partial class TextPromptDlg : Form
    {
        public string InputText => txtInput.Text;

        public TextPromptDlg()
        {
            InitializeComponent();
        }

        public TextPromptDlg(string title, string message, string defaultValue = "") : this()
        {
            Text = title;
            roThemeContainer1.Text = title;
            lblMessage.Text = message;
            txtInput.Text = defaultValue;
        }

        private void btnOk_Click(object? sender, EventArgs e)
        {
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