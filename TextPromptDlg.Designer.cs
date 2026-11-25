using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class TextPromptDlg
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
            lblMessage = new Label();
            txtInput = new ROTextBox();
            btnCancel = new ROModernButton();
            btnOk = new ROModernButton();
            roThemeContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(lblMessage);
            roThemeContainer1.Controls.Add(txtInput);
            roThemeContainer1.Controls.Add(btnCancel);
            roThemeContainer1.Controls.Add(btnOk);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(420, 192);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterParent;
            roThemeContainer1.SubHeader = "";
            roThemeContainer1.TabIndex = 0;
            roThemeContainer1.Text = "Enter Value";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // lblMessage
            // 
            lblMessage.Font = new Font("Segoe UI", 8.75F);
            lblMessage.ForeColor = Color.FromArgb(200, 200, 214);
            lblMessage.Location = new Point(14, 56);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(392, 32);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "Enter a value:";
            // 
            // txtInput
            // 
            txtInput.BackColor = Color.Transparent;
            txtInput.Location = new Point(14, 94);
            txtInput.MaxLength = 256;
            txtInput.Multiline = false;
            txtInput.Name = "txtInput";
            txtInput.ReadOnly = false;
            txtInput.Size = new Size(392, 32);
            txtInput.TabIndex = 2;
            txtInput.TextAlign = HorizontalAlignment.Left;
            txtInput.UseSystemPasswordChar = false;
            // 
            // btnCancel
            // 
            btnCancel.AccentColor = Color.FromArgb(214, 28, 52);
            btnCancel.BackColor = Color.Transparent;
            btnCancel.CornerRadius = 5;
            btnCancel.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.IsPrimaryAction = false;
            btnCancel.Location = new Point(226, 142);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(86, 34);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOk
            // 
            btnOk.AccentColor = Color.FromArgb(214, 28, 52);
            btnOk.BackColor = Color.Transparent;
            btnOk.CornerRadius = 5;
            btnOk.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnOk.ForeColor = Color.White;
            btnOk.IsPrimaryAction = true;
            btnOk.Location = new Point(320, 142);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(86, 34);
            btnOk.TabIndex = 4;
            btnOk.Text = "OK";
            btnOk.Click += btnOk_Click;
            // 
            // TextPromptDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(420, 192);
            Controls.Add(roThemeContainer1);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TextPromptDlg";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enter Value";
            roThemeContainer1.ResumeLayout(false);
            roThemeContainer1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ROThemeContainer roThemeContainer1;
        private Label lblMessage;
        private ROTextBox txtInput;
        private ROModernButton btnCancel;
        private ROModernButton btnOk;
    }
}