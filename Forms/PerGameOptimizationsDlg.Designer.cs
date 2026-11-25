using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class PerGameOptimizationsDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerGameOptimizationsDlg));
            lblMonitorTitle = new Label();
            lblMonitorDesc = new Label();
            toggleMonitorGame = new ROToggle();
            sep1 = new ROSeparator();
            lblPCores = new Label();
            lblPCoresDesc = new Label();
            cmbPCores = new ROComboBox();
            lblPower = new Label();
            lblPowerDesc = new Label();
            cmbPowerPlan = new ROComboBox();
            lblThrottle = new Label();
            lblThrottleDesc = new Label();
            cmbThrottleApps = new ROComboBox();
            lblLaunchArgs = new Label();
            txtLaunchArgs = new ROTextBox();
            sepBottom = new ROSeparator();
            btnReset = new ROModernButton();
            btnCancel = new ROModernButton();
            btnSave = new ROModernButton();
            roThemeContainer1 = new ROThemeContainer();
            lblHeader = new Label();
            lblSub = new Label();
            sepTop = new ROSeparator();
            roThemeContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // lblMonitorTitle
            // 
            lblMonitorTitle.AutoSize = true;
            lblMonitorTitle.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            lblMonitorTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblMonitorTitle.Location = new Point(20, 117);
            lblMonitorTitle.Name = "lblMonitorTitle";
            lblMonitorTitle.Size = new Size(242, 17);
            lblMonitorTitle.TabIndex = 3;
            lblMonitorTitle.Text = "Auto-Optimize When This Game Runs";
            // 
            // lblMonitorDesc
            // 
            lblMonitorDesc.AutoSize = true;
            lblMonitorDesc.Font = new Font("Segoe UI", 8F);
            lblMonitorDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblMonitorDesc.Location = new Point(20, 136);
            lblMonitorDesc.Name = "lblMonitorDesc";
            lblMonitorDesc.Size = new Size(399, 13);
            lblMonitorDesc.TabIndex = 4;
            lblMonitorDesc.Text = "Turn OFF to skip all automatic optimizations whenever this game is running.";
            // 
            // toggleMonitorGame
            // 
            toggleMonitorGame.BackColor = Color.Transparent;
            toggleMonitorGame.Checked = true;
            toggleMonitorGame.Location = new Point(454, 123);
            toggleMonitorGame.Name = "toggleMonitorGame";
            toggleMonitorGame.Size = new Size(46, 22);
            toggleMonitorGame.TabIndex = 5;
            toggleMonitorGame.CheckedChanged += toggleMonitorGame_CheckedChanged;
            // 
            // sep1
            // 
            sep1.BackColor = Color.Transparent;
            sep1.Location = new Point(20, 161);
            sep1.Name = "sep1";
            sep1.Size = new Size(480, 6);
            sep1.TabIndex = 6;
            // 
            // lblPCores
            // 
            lblPCores.AutoSize = true;
            lblPCores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPCores.ForeColor = Color.FromArgb(242, 242, 246);
            lblPCores.Location = new Point(20, 175);
            lblPCores.Name = "lblPCores";
            lblPCores.Size = new Size(191, 15);
            lblPCores.TabIndex = 7;
            lblPCores.Text = "Physical Cores Only (Disable SMT)";
            // 
            // lblPCoresDesc
            // 
            lblPCoresDesc.Font = new Font("Segoe UI", 8F);
            lblPCoresDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblPCoresDesc.Location = new Point(20, 193);
            lblPCoresDesc.Name = "lblPCoresDesc";
            lblPCoresDesc.Size = new Size(276, 30);
            lblPCoresDesc.TabIndex = 8;
            lblPCoresDesc.Text = "Set to Always OFF if this game runs better with all CPU threads enabled.";
            // 
            // cmbPCores
            // 
            cmbPCores.BackColor = Color.FromArgb(22, 22, 29);
            cmbPCores.DrawMode = DrawMode.OwnerDrawFixed;
            cmbPCores.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPCores.Font = new Font("Segoe UI", 8.75F);
            cmbPCores.ForeColor = Color.FromArgb(242, 242, 246);
            cmbPCores.ItemHeight = 24;
            cmbPCores.Location = new Point(302, 181);
            cmbPCores.Name = "cmbPCores";
            cmbPCores.Size = new Size(198, 30);
            cmbPCores.TabIndex = 9;
            // 
            // lblPower
            // 
            lblPower.AutoSize = true;
            lblPower.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPower.ForeColor = Color.FromArgb(242, 242, 246);
            lblPower.Location = new Point(20, 235);
            lblPower.Name = "lblPower";
            lblPower.Size = new Size(188, 15);
            lblPower.TabIndex = 10;
            lblPower.Text = "Auto High / Ultimate Power Plan";
            // 
            // lblPowerDesc
            // 
            lblPowerDesc.Font = new Font("Segoe UI", 8F);
            lblPowerDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblPowerDesc.Location = new Point(20, 253);
            lblPowerDesc.Name = "lblPowerDesc";
            lblPowerDesc.Size = new Size(276, 30);
            lblPowerDesc.TabIndex = 11;
            lblPowerDesc.Text = "Switches Windows to High Performance while playing and restores on exit.";
            // 
            // cmbPowerPlan
            // 
            cmbPowerPlan.BackColor = Color.FromArgb(22, 22, 29);
            cmbPowerPlan.DrawMode = DrawMode.OwnerDrawFixed;
            cmbPowerPlan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPowerPlan.Font = new Font("Segoe UI", 8.75F);
            cmbPowerPlan.ForeColor = Color.FromArgb(242, 242, 246);
            cmbPowerPlan.ItemHeight = 24;
            cmbPowerPlan.Location = new Point(302, 241);
            cmbPowerPlan.Name = "cmbPowerPlan";
            cmbPowerPlan.Size = new Size(198, 30);
            cmbPowerPlan.TabIndex = 12;
            // 
            // lblThrottle
            // 
            lblThrottle.AutoSize = true;
            lblThrottle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblThrottle.ForeColor = Color.FromArgb(242, 242, 246);
            lblThrottle.Location = new Point(20, 295);
            lblThrottle.Name = "lblThrottle";
            lblThrottle.Size = new Size(153, 15);
            lblThrottle.TabIndex = 13;
            lblThrottle.Text = "Throttle Background Apps";
            // 
            // lblThrottleDesc
            // 
            lblThrottleDesc.Font = new Font("Segoe UI", 8F);
            lblThrottleDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblThrottleDesc.Location = new Point(20, 313);
            lblThrottleDesc.Name = "lblThrottleDesc";
            lblThrottleDesc.Size = new Size(276, 30);
            lblThrottleDesc.TabIndex = 14;
            lblThrottleDesc.Text = "Lowers CPU priority of listed background apps while you play.";
            // 
            // cmbThrottleApps
            // 
            cmbThrottleApps.BackColor = Color.FromArgb(22, 22, 29);
            cmbThrottleApps.DrawMode = DrawMode.OwnerDrawFixed;
            cmbThrottleApps.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbThrottleApps.Font = new Font("Segoe UI", 8.75F);
            cmbThrottleApps.ForeColor = Color.FromArgb(242, 242, 246);
            cmbThrottleApps.ItemHeight = 24;
            cmbThrottleApps.Location = new Point(302, 301);
            cmbThrottleApps.Name = "cmbThrottleApps";
            cmbThrottleApps.Size = new Size(198, 30);
            cmbThrottleApps.TabIndex = 15;
            // 
            // sepBottom
            // 
            sepBottom.BackColor = Color.Transparent;
            sepBottom.Location = new Point(16, 415);
            sepBottom.Name = "sepBottom";
            sepBottom.Size = new Size(480, 6);
            sepBottom.TabIndex = 16;
            // 
            // btnReset
            // 
            btnReset.AccentColor = Color.FromArgb(214, 28, 52);
            btnReset.BackColor = Color.Transparent;
            btnReset.CornerRadius = 5;
            btnReset.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnReset.ForeColor = Color.FromArgb(242, 242, 246);
            btnReset.IsPrimaryAction = false;
            btnReset.Location = new Point(16, 434);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(134, 36);
            btnReset.TabIndex = 17;
            btnReset.Text = "Reset to Global";
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
            btnCancel.Location = new Point(304, 434);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 36);
            btnCancel.TabIndex = 18;
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
            btnSave.Location = new Point(400, 434);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(96, 36);
            btnSave.TabIndex = 19;
            btnSave.Text = "Save Rules";
            btnSave.Click += btnSave_Click;
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(lblHeader);
            roThemeContainer1.Controls.Add(lblSub);
            roThemeContainer1.Controls.Add(btnSave);
            roThemeContainer1.Controls.Add(sepTop);
            roThemeContainer1.Controls.Add(btnCancel);
            roThemeContainer1.Controls.Add(lblMonitorTitle);
            roThemeContainer1.Controls.Add(btnReset);
            roThemeContainer1.Controls.Add(lblMonitorDesc);
            roThemeContainer1.Controls.Add(sepBottom);
            roThemeContainer1.Controls.Add(toggleMonitorGame);
            roThemeContainer1.Controls.Add(cmbThrottleApps);
            roThemeContainer1.Controls.Add(sep1);
            roThemeContainer1.Controls.Add(lblThrottleDesc);
            roThemeContainer1.Controls.Add(lblPCores);
            roThemeContainer1.Controls.Add(lblThrottle);
            roThemeContainer1.Controls.Add(lblPCoresDesc);
            roThemeContainer1.Controls.Add(cmbPowerPlan);
            roThemeContainer1.Controls.Add(cmbPCores);
            roThemeContainer1.Controls.Add(lblPowerDesc);
            roThemeContainer1.Controls.Add(lblPower);
            roThemeContainer1.Controls.Add(lblLaunchArgs);
            roThemeContainer1.Controls.Add(txtLaunchArgs);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(512, 484);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterParent;
            roThemeContainer1.SubHeader = "";
            roThemeContainer1.TabIndex = 20;
            roThemeContainer1.Text = "Per-Game Auto Optimizations";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // lblHeader
            // 
            lblHeader.AutoEllipsis = true;
            lblHeader.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(242, 242, 246);
            lblHeader.Location = new Point(20, 53);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(480, 22);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "Per-Game Optimizations";
            // 
            // lblSub
            // 
            lblSub.AutoEllipsis = true;
            lblSub.Font = new Font("Segoe UI", 8.25F);
            lblSub.ForeColor = Color.FromArgb(162, 162, 176);
            lblSub.Location = new Point(20, 77);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(480, 18);
            lblSub.TabIndex = 1;
            lblSub.Text = "Override your System Optimization settings for this game only.";
            // 
            // sepTop
            // 
            sepTop.BackColor = Color.Transparent;
            sepTop.Location = new Point(20, 103);
            sepTop.Name = "sepTop";
            sepTop.Size = new Size(480, 6);
            sepTop.TabIndex = 2;
            // 
            // PerGameOptimizationsDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(512, 484);
            Controls.Add(roThemeContainer1);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PerGameOptimizationsDlg";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Per-Game Auto Optimizations";
            roThemeContainer1.ResumeLayout(false);
            roThemeContainer1.PerformLayout();
            ResumeLayout(false);
            // 
            // lblLaunchArgs
            // 
            lblLaunchArgs.AutoSize = true;
            lblLaunchArgs.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblLaunchArgs.ForeColor = Color.FromArgb(242, 242, 246);
            lblLaunchArgs.Location = new Point(16, 352);
            lblLaunchArgs.Name = "lblLaunchArgs";
            lblLaunchArgs.Size = new Size(340, 15);
            lblLaunchArgs.TabIndex = 16;
            lblLaunchArgs.Text = "Custom Launch Arguments (passed when using LAUNCH GAME):";
            // 
            // txtLaunchArgs
            // 
            txtLaunchArgs.BackColor = Color.Transparent;
            txtLaunchArgs.Location = new Point(16, 371);
            txtLaunchArgs.MaxLength = 512;
            txtLaunchArgs.Multiline = false;
            txtLaunchArgs.Name = "txtLaunchArgs";
            txtLaunchArgs.ReadOnly = false;
            txtLaunchArgs.Size = new Size(480, 32);
            txtLaunchArgs.TabIndex = 17;
            txtLaunchArgs.TextAlign = HorizontalAlignment.Left;
            txtLaunchArgs.UseSystemPasswordChar = false;
        }

        #endregion
        private Label lblMonitorTitle;
        private Label lblMonitorDesc;
        private ROToggle toggleMonitorGame;
        private ROSeparator sep1;
        private Label lblPCores;
        private Label lblPCoresDesc;
        private ROComboBox cmbPCores;
        private Label lblPower;
        private Label lblPowerDesc;
        private ROComboBox cmbPowerPlan;
        private Label lblThrottle;
        private Label lblThrottleDesc;
        private ROComboBox cmbThrottleApps;
        private ROSeparator sepBottom;
        private ROModernButton btnReset;
        private ROModernButton btnCancel;
        private ROModernButton btnSave;
        private ROThemeContainer roThemeContainer1;
        private Label lblHeader;
        private Label lblSub;
        private ROSeparator sepTop;
        private Label lblLaunchArgs;
        private ROTextBox txtLaunchArgs;
    }
}