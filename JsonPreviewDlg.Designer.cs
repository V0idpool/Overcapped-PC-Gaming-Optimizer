using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class JsonPreviewDlg
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
            txtJson = new TextBox();
            lblFileInfo = new Label();
            btnCopy = new ROModernButton();
            btnClose = new ROModernButton();
            roThemeContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(txtJson);
            roThemeContainer1.Controls.Add(lblFileInfo);
            roThemeContainer1.Controls.Add(btnCopy);
            roThemeContainer1.Controls.Add(btnClose);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(640, 520);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterParent;
            roThemeContainer1.SubHeader = "";
            roThemeContainer1.TabIndex = 0;
            roThemeContainer1.Text = "JSON Preview";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // txtJson
            // 
            txtJson.BackColor = Color.FromArgb(18, 18, 24);
            txtJson.BorderStyle = BorderStyle.FixedSingle;
            txtJson.Font = new Font("Consolas", 9F);
            txtJson.ForeColor = Color.FromArgb(228, 228, 238);
            txtJson.Location = new Point(14, 56);
            txtJson.Multiline = true;
            txtJson.Name = "txtJson";
            txtJson.ReadOnly = true;
            txtJson.ScrollBars = ScrollBars.Both;
            txtJson.Size = new Size(612, 404);
            txtJson.TabIndex = 1;
            txtJson.WordWrap = false;
            // 
            // lblFileInfo
            // 
            lblFileInfo.AutoEllipsis = true;
            lblFileInfo.Font = new Font("Segoe UI", 8.25F);
            lblFileInfo.ForeColor = Color.FromArgb(162, 162, 176);
            lblFileInfo.Location = new Point(14, 478);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(380, 18);
            lblFileInfo.TabIndex = 2;
            lblFileInfo.Text = "pack.json";
            // 
            // btnCopy
            // 
            btnCopy.AccentColor = Color.FromArgb(214, 28, 52);
            btnCopy.BackColor = Color.Transparent;
            btnCopy.CornerRadius = 5;
            btnCopy.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnCopy.ForeColor = Color.White;
            btnCopy.IsPrimaryAction = true;
            btnCopy.Location = new Point(406, 470);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(112, 34);
            btnCopy.TabIndex = 3;
            btnCopy.Text = "Copy JSON";
            btnCopy.Click += btnCopy_Click;
            // 
            // btnClose
            // 
            btnClose.AccentColor = Color.FromArgb(214, 28, 52);
            btnClose.BackColor = Color.Transparent;
            btnClose.CornerRadius = 5;
            btnClose.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.IsPrimaryAction = false;
            btnClose.Location = new Point(526, 470);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 34);
            btnClose.TabIndex = 4;
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // JsonPreviewDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(640, 520);
            Controls.Add(roThemeContainer1);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "JsonPreviewDlg";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "JSON Preview";
            roThemeContainer1.ResumeLayout(false);
            roThemeContainer1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ROThemeContainer roThemeContainer1;
        private TextBox txtJson;
        private Label lblFileInfo;
        private ROModernButton btnCopy;
        private ROModernButton btnClose;
    }
}