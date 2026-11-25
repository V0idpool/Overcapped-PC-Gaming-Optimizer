using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class ThrottleListDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThrottleListDlg));
            lblHeader = new Label();
            lblSub = new Label();
            txtNewApp = new ROTextBox();
            btnAdd = new ROModernButton();
            btnBrowse = new ROModernButton();
            lblRunning = new Label();
            cmbRunning = new ROComboBox();
            btnAddRunning = new ROModernButton();
            lstApps = new ListBox();
            btnRemove = new ROModernButton();
            btnReset = new ROModernButton();
            btnCancel = new ROModernButton();
            btnSave = new ROModernButton();
            roThemeContainer1 = new ROThemeContainer();
            roThemeContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // lblHeader
            // 
            lblHeader.AutoSize = true;
            lblHeader.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(242, 242, 246);
            lblHeader.Location = new Point(16, 57);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(252, 19);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "Manage Throttled Background Apps";
            // 
            // lblSub
            // 
            lblSub.Font = new Font("Segoe UI", 8.25F);
            lblSub.ForeColor = Color.FromArgb(162, 162, 176);
            lblSub.Location = new Point(16, 81);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(464, 30);
            lblSub.TabIndex = 1;
            lblSub.Text = "Lowers CPU priority for these apps while you play so they don't steal FPS, then restores them when you exit. (e.g. Spotify, Wallpaper Engine, Overwolf, RGB suites).\r\n";
            // 
            // txtNewApp
            // 
            txtNewApp.BackColor = Color.Transparent;
            txtNewApp.Location = new Point(16, 119);
            txtNewApp.MaxLength = 128;
            txtNewApp.Multiline = false;
            txtNewApp.Name = "txtNewApp";
            txtNewApp.ReadOnly = false;
            txtNewApp.Size = new Size(276, 32);
            txtNewApp.TabIndex = 2;
            txtNewApp.TextAlign = HorizontalAlignment.Left;
            txtNewApp.UseSystemPasswordChar = false;
            // 
            // btnAdd
            // 
            btnAdd.AccentColor = Color.FromArgb(214, 28, 52);
            btnAdd.BackColor = Color.Transparent;
            btnAdd.CornerRadius = 5;
            btnAdd.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnAdd.ForeColor = Color.FromArgb(242, 242, 246);
            btnAdd.IsPrimaryAction = true;
            btnAdd.Location = new Point(300, 119);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(76, 32);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "Add";
            btnAdd.Click += btnAdd_Click;
            // 
            // btnBrowse
            // 
            btnBrowse.AccentColor = Color.FromArgb(214, 28, 52);
            btnBrowse.BackColor = Color.Transparent;
            btnBrowse.CornerRadius = 5;
            btnBrowse.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnBrowse.ForeColor = Color.FromArgb(242, 242, 246);
            btnBrowse.IsPrimaryAction = false;
            btnBrowse.Location = new Point(384, 119);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(96, 32);
            btnBrowse.TabIndex = 4;
            btnBrowse.Text = "Browse...";
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lblRunning
            // 
            lblRunning.AutoSize = true;
            lblRunning.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblRunning.ForeColor = Color.FromArgb(162, 162, 176);
            lblRunning.Location = new Point(16, 159);
            lblRunning.Name = "lblRunning";
            lblRunning.Size = new Size(130, 13);
            lblRunning.TabIndex = 5;
            lblRunning.Text = "Quick-add running app:";
            // 
            // cmbRunning
            // 
            cmbRunning.BackColor = Color.FromArgb(22, 22, 29);
            cmbRunning.DrawMode = DrawMode.OwnerDrawFixed;
            cmbRunning.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRunning.Font = new Font("Segoe UI", 8.75F);
            cmbRunning.ForeColor = Color.FromArgb(242, 242, 246);
            cmbRunning.ItemHeight = 24;
            cmbRunning.Location = new Point(16, 177);
            cmbRunning.Name = "cmbRunning";
            cmbRunning.Size = new Size(360, 30);
            cmbRunning.TabIndex = 6;
            // 
            // btnAddRunning
            // 
            btnAddRunning.AccentColor = Color.FromArgb(214, 28, 52);
            btnAddRunning.BackColor = Color.Transparent;
            btnAddRunning.CornerRadius = 5;
            btnAddRunning.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnAddRunning.ForeColor = Color.FromArgb(242, 242, 246);
            btnAddRunning.IsPrimaryAction = false;
            btnAddRunning.Location = new Point(384, 177);
            btnAddRunning.Name = "btnAddRunning";
            btnAddRunning.Size = new Size(96, 30);
            btnAddRunning.TabIndex = 7;
            btnAddRunning.Text = "Add App";
            btnAddRunning.Click += btnAddRunning_Click;
            // 
            // lstApps
            // 
            lstApps.BackColor = Color.FromArgb(22, 22, 29);
            lstApps.BorderStyle = BorderStyle.FixedSingle;
            lstApps.DrawMode = DrawMode.OwnerDrawFixed;
            lstApps.ForeColor = Color.FromArgb(242, 242, 246);
            lstApps.IntegralHeight = false;
            lstApps.ItemHeight = 26;
            lstApps.Location = new Point(16, 219);
            lstApps.Name = "lstApps";
            lstApps.SelectionMode = SelectionMode.MultiExtended;
            lstApps.Size = new Size(464, 172);
            lstApps.TabIndex = 8;
            lstApps.DrawItem += lstApps_DrawItem;
            lstApps.KeyDown += lstApps_KeyDown;
            // 
            // btnRemove
            // 
            btnRemove.AccentColor = Color.FromArgb(244, 63, 94);
            btnRemove.BackColor = Color.Transparent;
            btnRemove.CornerRadius = 5;
            btnRemove.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnRemove.ForeColor = Color.FromArgb(242, 242, 246);
            btnRemove.IsPrimaryAction = false;
            btnRemove.Location = new Point(16, 401);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(140, 32);
            btnRemove.TabIndex = 9;
            btnRemove.Text = "Remove Selected";
            btnRemove.Click += btnRemove_Click;
            // 
            // btnReset
            // 
            btnReset.AccentColor = Color.FromArgb(214, 28, 52);
            btnReset.BackColor = Color.Transparent;
            btnReset.CornerRadius = 5;
            btnReset.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnReset.ForeColor = Color.FromArgb(242, 242, 246);
            btnReset.IsPrimaryAction = false;
            btnReset.Location = new Point(164, 401);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(124, 32);
            btnReset.TabIndex = 10;
            btnReset.Text = "Reset Defaults";
            btnReset.Click += btnReset_Click;
            // 
            // btnCancel
            // 
            btnCancel.AccentColor = Color.FromArgb(214, 28, 52);
            btnCancel.BackColor = Color.Transparent;
            btnCancel.CornerRadius = 5;
            btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(242, 242, 246);
            btnCancel.IsPrimaryAction = false;
            btnCancel.Location = new Point(296, 441);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 34);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.AccentColor = Color.FromArgb(214, 28, 52);
            btnSave.BackColor = Color.Transparent;
            btnSave.CornerRadius = 5;
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.ForeColor = Color.FromArgb(242, 242, 246);
            btnSave.IsPrimaryAction = true;
            btnSave.Location = new Point(392, 441);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 34);
            btnSave.TabIndex = 12;
            btnSave.Text = "Save List";
            btnSave.Click += btnSave_Click;
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(lblHeader);
            roThemeContainer1.Controls.Add(lstApps);
            roThemeContainer1.Controls.Add(lblSub);
            roThemeContainer1.Controls.Add(btnSave);
            roThemeContainer1.Controls.Add(txtNewApp);
            roThemeContainer1.Controls.Add(btnCancel);
            roThemeContainer1.Controls.Add(btnAdd);
            roThemeContainer1.Controls.Add(btnReset);
            roThemeContainer1.Controls.Add(btnBrowse);
            roThemeContainer1.Controls.Add(btnRemove);
            roThemeContainer1.Controls.Add(lblRunning);
            roThemeContainer1.Controls.Add(btnAddRunning);
            roThemeContainer1.Controls.Add(cmbRunning);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(496, 487);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterParent;
            roThemeContainer1.SubHeader = "";
            roThemeContainer1.TabIndex = 13;
            roThemeContainer1.Text = "Background Apps List";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // ThrottleListDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(496, 487);
            Controls.Add(roThemeContainer1);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ThrottleListDlg";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Background Apps List";
            roThemeContainer1.ResumeLayout(false);
            roThemeContainer1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblHeader;
        private Label lblSub;
        private ROTextBox txtNewApp;
        private ROModernButton btnAdd;
        private ROModernButton btnBrowse;
        private Label lblRunning;
        private ROComboBox cmbRunning;
        private ROModernButton btnAddRunning;
        private ListBox lstApps;
        private ROModernButton btnRemove;
        private ROModernButton btnReset;
        private ROModernButton btnCancel;
        private ROModernButton btnSave;
        private ROThemeContainer roThemeContainer1;
    }
}