using System;
using System.Windows.Forms;
using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer
{
    public partial class PerGameOptimizationsDlg : Form
    {
        public bool ResultMonitorGame => toggleMonitorGame.Checked;
        public AutoOptimizeOverride ResultPCoresOverride => (AutoOptimizeOverride)Math.Clamp(cmbPCores.SelectedIndex, 0, 2);
        public AutoOptimizeOverride ResultAutoPowerPlanOverride => (AutoOptimizeOverride)Math.Clamp(cmbPowerPlan.SelectedIndex, 0, 2);
        public AutoOptimizeOverride ResultThrottleAppsOverride => (AutoOptimizeOverride)Math.Clamp(cmbThrottleApps.SelectedIndex, 0, 2);
        public string ResultLaunchArguments => txtLaunchArgs.Text.Trim();

        public PerGameOptimizationsDlg()
        {
            InitializeComponent();
        }

        public PerGameOptimizationsDlg(GameProfile game) : this()
        {
            Text = $"Per-Game Auto Optimizations: {game.Name}";
            lblHeader.Text = $"Auto Optimizations for: {game.Name}";
            lblSub.Text = $"Process: {game.ExecutableName} | Override global System Optimization settings for this game only.";
            txtLaunchArgs.Text = game.LaunchArguments ?? string.Empty;
            toggleMonitorGame.Checked = game.MonitorGame;

            PopulateRuleComboBox(cmbPCores, AppSettings.Current.PCoresOnly, game.PCoresOverride);
            PopulateRuleComboBox(cmbPowerPlan, AppSettings.Current.AutoPowerPlan, game.AutoPowerPlanOverride);
            PopulateRuleComboBox(cmbThrottleApps, AppSettings.Current.ThrottleBackgroundApps, game.ThrottleAppsOverride);

            UpdateDropdownEnabledState();
        }

        private static void PopulateRuleComboBox(ComboBox cmb, bool globalState, AutoOptimizeOverride currentRule)
        {
            cmb.Items.Clear();
            cmb.Items.Add($"Use Global ({(globalState ? "ON" : "OFF")})");
            cmb.Items.Add("Always ON");
            cmb.Items.Add("Always OFF");
            cmb.SelectedIndex = Math.Clamp((int)currentRule, 0, 2);
        }

        private void UpdateDropdownEnabledState()
        {
            bool enabled = toggleMonitorGame.Checked;
            cmbPCores.Enabled = enabled;
            cmbPowerPlan.Enabled = enabled;
            cmbThrottleApps.Enabled = enabled;
        }

        private void toggleMonitorGame_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateDropdownEnabledState();
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            toggleMonitorGame.Checked = true;
            if (cmbPCores.Items.Count > 0) cmbPCores.SelectedIndex = 0;
            if (cmbPowerPlan.Items.Count > 0) cmbPowerPlan.SelectedIndex = 0;
            if (cmbThrottleApps.Items.Count > 0) cmbThrottleApps.SelectedIndex = 0;
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}