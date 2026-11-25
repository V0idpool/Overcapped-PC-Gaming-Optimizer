using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class InstalledPackPickerDlg
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
            lstPacks = new ListBox();
            btnCancel = new ROModernButton();
            btnLoad = new ROModernButton();
            roThemeContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(lblPrompt);
            roThemeContainer1.Controls.Add(lstPacks);
            roThemeContainer1.Controls.Add(btnCancel);
            roThemeContainer1.Controls.Add(btnLoad);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(440, 366);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterParent;
            roThemeContainer1.SubHeader = "";
            roThemeContainer1.TabIndex = 0;
            roThemeContainer1.Text = "Edit Installed JSON Pack";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // lblPrompt
            // 
            lblPrompt.AutoSize = true;
            lblPrompt.Font = new Font("Segoe UI", 8.75F);
            lblPrompt.ForeColor = Color.FromArgb(162, 162, 176);
            lblPrompt.Location = new Point(14, 56);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(315, 15);
            lblPrompt.TabIndex = 1;
            lblPrompt.Text = "Select an installed pack from your Profiles folder to edit:";
            // 
            // lstPacks
            // 
            lstPacks.BackColor = Color.FromArgb(22, 22, 29);
            lstPacks.BorderStyle = BorderStyle.FixedSingle;
            lstPacks.DrawMode = DrawMode.OwnerDrawFixed;
            lstPacks.Font = new Font("Segoe UI", 9F);
            lstPacks.ForeColor = Color.FromArgb(242, 242, 246);
            lstPacks.IntegralHeight = false;
            lstPacks.ItemHeight = 26;
            lstPacks.Location = new Point(14, 80);
            lstPacks.Name = "lstPacks";
            lstPacks.Size = new Size(412, 222);
            lstPacks.TabIndex = 2;
            lstPacks.DoubleClick += lstPacks_DoubleClick;
            lstPacks.DrawItem += lstPacks_DrawItem;
            // 
            // btnCancel
            // 
            btnCancel.AccentColor = Color.FromArgb(214, 28, 52);
            btnCancel.BackColor = Color.Transparent;
            btnCancel.CornerRadius = 5;
            btnCancel.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.IsPrimaryAction = false;
            btnCancel.Location = new Point(242, 316);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 34);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnLoad
            // 
            btnLoad.AccentColor = Color.FromArgb(214, 28, 52);
            btnLoad.BackColor = Color.Transparent;
            btnLoad.CornerRadius = 5;
            btnLoad.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnLoad.ForeColor = Color.White;
            btnLoad.IsPrimaryAction = true;
            btnLoad.Location = new Point(338, 316);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(88, 34);
            btnLoad.TabIndex = 4;
            btnLoad.Text = "Load Pack";
            btnLoad.Click += btnLoad_Click;
            // 
            // InstalledPackPickerDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(440, 366);
            Controls.Add(roThemeContainer1);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InstalledPackPickerDlg";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Installed JSON Pack";
            roThemeContainer1.ResumeLayout(false);
            roThemeContainer1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ROThemeContainer roThemeContainer1;
        private Label lblPrompt;
        private ListBox lstPacks;
        private ROModernButton btnCancel;
        private ROModernButton btnLoad;
    }
}