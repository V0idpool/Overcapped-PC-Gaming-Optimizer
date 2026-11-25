using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class KeyPickerDlg
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            roThemeContainer1 = new ROThemeContainer();
            lblPrompt = new Label();
            clbKeys = new CheckedListBox();
            btnSelectAll = new ROModernButton();
            btnClear = new ROModernButton();
            btnCancel = new ROModernButton();
            btnImport = new ROModernButton();
            roThemeContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(lblPrompt);
            roThemeContainer1.Controls.Add(clbKeys);
            roThemeContainer1.Controls.Add(btnSelectAll);
            roThemeContainer1.Controls.Add(btnClear);
            roThemeContainer1.Controls.Add(btnCancel);
            roThemeContainer1.Controls.Add(btnImport);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(536, 466);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterParent;
            roThemeContainer1.SubHeader = "";
            roThemeContainer1.TabIndex = 0;
            roThemeContainer1.Text = "Pull Keys from Config";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // lblPrompt
            // 
            lblPrompt.Font = new Font("Segoe UI", 8.5F);
            lblPrompt.ForeColor = Color.FromArgb(162, 162, 176);
            lblPrompt.Location = new Point(14, 56);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(508, 30);
            lblPrompt.TabIndex = 1;
            lblPrompt.Text = "Check the keys you want to pull into your profile pack:";
            // 
            // clbKeys
            // 
            clbKeys.BackColor = Color.FromArgb(22, 22, 29);
            clbKeys.BorderStyle = BorderStyle.FixedSingle;
            clbKeys.CheckOnClick = true;
            clbKeys.Font = new Font("Consolas", 8.75F);
            clbKeys.ForeColor = Color.FromArgb(242, 242, 246);
            clbKeys.FormattingEnabled = true;
            clbKeys.Location = new Point(14, 90);
            clbKeys.Name = "clbKeys";
            clbKeys.Size = new Size(508, 314);
            clbKeys.TabIndex = 2;
            // 
            // btnSelectAll
            // 
            btnSelectAll.AccentColor = Color.FromArgb(214, 28, 52);
            btnSelectAll.BackColor = Color.Transparent;
            btnSelectAll.CornerRadius = 5;
            btnSelectAll.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnSelectAll.ForeColor = Color.White;
            btnSelectAll.IsPrimaryAction = false;
            btnSelectAll.Location = new Point(14, 416);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(92, 34);
            btnSelectAll.TabIndex = 3;
            btnSelectAll.Text = "Select All";
            btnSelectAll.Click += btnSelectAll_Click;
            // 
            // btnClear
            // 
            btnClear.AccentColor = Color.FromArgb(214, 28, 52);
            btnClear.BackColor = Color.Transparent;
            btnClear.CornerRadius = 5;
            btnClear.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnClear.ForeColor = Color.White;
            btnClear.IsPrimaryAction = false;
            btnClear.Location = new Point(114, 416);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(76, 34);
            btnClear.TabIndex = 4;
            btnClear.Text = "Clear";
            btnClear.Click += btnClear_Click;
            // 
            // btnCancel
            // 
            btnCancel.AccentColor = Color.FromArgb(214, 28, 52);
            btnCancel.BackColor = Color.Transparent;
            btnCancel.CornerRadius = 5;
            btnCancel.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.IsPrimaryAction = false;
            btnCancel.Location = new Point(320, 416);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 34);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnImport
            // 
            btnImport.AccentColor = Color.FromArgb(214, 28, 52);
            btnImport.BackColor = Color.Transparent;
            btnImport.CornerRadius = 5;
            btnImport.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnImport.ForeColor = Color.White;
            btnImport.IsPrimaryAction = true;
            btnImport.Location = new Point(416, 416);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(106, 34);
            btnImport.TabIndex = 6;
            btnImport.Text = "Pull Checked";
            btnImport.Click += btnImport_Click;
            // 
            // KeyPickerDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(536, 466);
            Controls.Add(roThemeContainer1);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "KeyPickerDlg";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pull Keys from Config";
            roThemeContainer1.ResumeLayout(false);
            roThemeContainer1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ROThemeContainer roThemeContainer1;
        private Label lblPrompt;
        private CheckedListBox clbKeys;
        private ROModernButton btnSelectAll;
        private ROModernButton btnClear;
        private ROModernButton btnCancel;
        private ROModernButton btnImport;
    }
}