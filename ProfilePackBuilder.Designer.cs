using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class ProfilePackBuilder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilePackBuilder));
            btnLoadInstalledPack = new ROModernButton();
            btnImportJson = new ROModernButton();
            btnNewPack = new ROModernButton();
            grpPackMeta = new ROGroupBox();
            lblPickGame = new Label();
            cmbLibraryGame = new ROComboBox();
            lblPackId = new Label();
            txtPackId = new ROTextBox();
            lblMatchExe = new Label();
            txtMatchExe = new ROTextBox();
            lblGameName = new Label();
            txtGameName = new ROTextBox();
            lblConfigFormat = new Label();
            cmbConfigFormat = new ROComboBox();
            lblIniSection = new Label();
            txtIniSection = new ROTextBox();
            lblConfigPath = new Label();
            txtConfigPath = new ROTextBox();
            btnBrowseConfig = new ROModernButton();
            btnReadConfigKeys = new ROModernButton();
            grpPackSettings = new ROGroupBox();
            lblScope = new Label();
            cmbScope = new ROComboBox();
            btnAddPreset = new ROModernButton();
            btnRenamePreset = new ROModernButton();
            btnDeletePreset = new ROModernButton();
            lblEntryCount = new Label();
            lblKey = new Label();
            txtKey = new ROTextBox();
            lblValue = new Label();
            txtValue = new ROTextBox();
            lblToken = new Label();
            cmbDynamicToken = new ROComboBox();
            btnAddPair = new ROModernButton();
            dgvSettings = new DataGridView();
            colKey = new DataGridViewTextBoxColumn();
            colValue = new DataGridViewTextBoxColumn();
            btnRemoveSelectedKey = new ROModernButton();
            btnPreviewJson = new ROModernButton();
            btnClose = new ROModernButton();
            btnExportJson = new ROModernButton();
            btnSaveToLibrary = new ROModernButton();
            roThemeContainer1 = new ROThemeContainer();
            grpPackMeta.SuspendLayout();
            grpPackSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSettings).BeginInit();
            roThemeContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // btnLoadInstalledPack
            // 
            btnLoadInstalledPack.AccentColor = Color.FromArgb(214, 28, 52);
            btnLoadInstalledPack.BackColor = Color.Transparent;
            btnLoadInstalledPack.CornerRadius = 5;
            btnLoadInstalledPack.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnLoadInstalledPack.ForeColor = Color.FromArgb(242, 242, 246);
            btnLoadInstalledPack.IsPrimaryAction = false;
            btnLoadInstalledPack.Location = new Point(14, 44);
            btnLoadInstalledPack.Name = "btnLoadInstalledPack";
            btnLoadInstalledPack.Size = new Size(152, 32);
            btnLoadInstalledPack.TabIndex = 1;
            btnLoadInstalledPack.Text = "Edit Installed Pack...";
            btnLoadInstalledPack.Click += btnLoadInstalledPack_Click;
            // 
            // btnImportJson
            // 
            btnImportJson.AccentColor = Color.FromArgb(214, 28, 52);
            btnImportJson.BackColor = Color.Transparent;
            btnImportJson.CornerRadius = 5;
            btnImportJson.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnImportJson.ForeColor = Color.FromArgb(242, 242, 246);
            btnImportJson.IsPrimaryAction = false;
            btnImportJson.Location = new Point(174, 44);
            btnImportJson.Name = "btnImportJson";
            btnImportJson.Size = new Size(146, 32);
            btnImportJson.TabIndex = 2;
            btnImportJson.Text = "Import .json...";
            btnImportJson.Click += btnImportJson_Click;
            // 
            // btnNewPack
            // 
            btnNewPack.AccentColor = Color.FromArgb(214, 28, 52);
            btnNewPack.BackColor = Color.Transparent;
            btnNewPack.CornerRadius = 5;
            btnNewPack.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnNewPack.ForeColor = Color.FromArgb(242, 242, 246);
            btnNewPack.IsPrimaryAction = false;
            btnNewPack.Location = new Point(328, 44);
            btnNewPack.Name = "btnNewPack";
            btnNewPack.Size = new Size(142, 32);
            btnNewPack.TabIndex = 3;
            btnNewPack.Text = "New Blank Pack";
            btnNewPack.Click += btnNewPack_Click;
            // 
            // grpPackMeta
            // 
            grpPackMeta.BackColor = Color.Transparent;
            grpPackMeta.Controls.Add(btnLoadInstalledPack);
            grpPackMeta.Controls.Add(lblPickGame);
            grpPackMeta.Controls.Add(btnImportJson);
            grpPackMeta.Controls.Add(cmbLibraryGame);
            grpPackMeta.Controls.Add(btnNewPack);
            grpPackMeta.Controls.Add(lblPackId);
            grpPackMeta.Controls.Add(txtPackId);
            grpPackMeta.Controls.Add(lblMatchExe);
            grpPackMeta.Controls.Add(txtMatchExe);
            grpPackMeta.Controls.Add(lblGameName);
            grpPackMeta.Controls.Add(txtGameName);
            grpPackMeta.Controls.Add(lblConfigFormat);
            grpPackMeta.Controls.Add(cmbConfigFormat);
            grpPackMeta.Controls.Add(lblIniSection);
            grpPackMeta.Controls.Add(txtIniSection);
            grpPackMeta.Controls.Add(lblConfigPath);
            grpPackMeta.Controls.Add(txtConfigPath);
            grpPackMeta.Controls.Add(btnBrowseConfig);
            grpPackMeta.Controls.Add(btnReadConfigKeys);
            grpPackMeta.Font = new Font("Segoe UI", 9.5F);
            grpPackMeta.Location = new Point(16, 54);
            grpPackMeta.Name = "grpPackMeta";
            grpPackMeta.Padding = new Padding(14, 44, 14, 14);
            grpPackMeta.Size = new Size(828, 251);
            grpPackMeta.SubTitle = "Pick an installed game or fill in metadata manually, then point to its .ini or .cfg file";
            grpPackMeta.TabIndex = 4;
            grpPackMeta.Title = "Pack Metadata & Target Config";
            // 
            // lblPickGame
            // 
            lblPickGame.AutoSize = true;
            lblPickGame.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblPickGame.ForeColor = Color.FromArgb(162, 162, 176);
            lblPickGame.Location = new Point(14, 85);
            lblPickGame.Name = "lblPickGame";
            lblPickGame.Size = new Size(122, 13);
            lblPickGame.TabIndex = 0;
            lblPickGame.Text = "Auto-Fill from Library:";
            // 
            // cmbLibraryGame
            // 
            cmbLibraryGame.BackColor = Color.FromArgb(22, 22, 29);
            cmbLibraryGame.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLibraryGame.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLibraryGame.Font = new Font("Segoe UI", 8.75F);
            cmbLibraryGame.ForeColor = Color.FromArgb(242, 242, 246);
            cmbLibraryGame.ItemHeight = 24;
            cmbLibraryGame.Location = new Point(14, 101);
            cmbLibraryGame.Name = "cmbLibraryGame";
            cmbLibraryGame.Size = new Size(392, 30);
            cmbLibraryGame.TabIndex = 1;
            cmbLibraryGame.SelectedIndexChanged += cmbLibraryGame_SelectedIndexChanged;
            // 
            // lblPackId
            // 
            lblPackId.AutoSize = true;
            lblPackId.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblPackId.ForeColor = Color.FromArgb(162, 162, 176);
            lblPackId.Location = new Point(418, 85);
            lblPackId.Name = "lblPackId";
            lblPackId.Size = new Size(104, 13);
            lblPackId.TabIndex = 2;
            lblPackId.Text = "Pack ID (filename):";
            // 
            // txtPackId
            // 
            txtPackId.BackColor = Color.Transparent;
            txtPackId.Location = new Point(418, 101);
            txtPackId.MaxLength = 128;
            txtPackId.Multiline = false;
            txtPackId.Name = "txtPackId";
            txtPackId.ReadOnly = false;
            txtPackId.Size = new Size(186, 32);
            txtPackId.TabIndex = 3;
            txtPackId.TextAlign = HorizontalAlignment.Left;
            txtPackId.UseSystemPasswordChar = false;
            // 
            // lblMatchExe
            // 
            lblMatchExe.AutoSize = true;
            lblMatchExe.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblMatchExe.ForeColor = Color.FromArgb(162, 162, 176);
            lblMatchExe.Location = new Point(614, 85);
            lblMatchExe.Name = "lblMatchExe";
            lblMatchExe.Size = new Size(134, 13);
            lblMatchExe.TabIndex = 4;
            lblMatchExe.Text = "Match Executable (.exe):";
            // 
            // txtMatchExe
            // 
            txtMatchExe.BackColor = Color.Transparent;
            txtMatchExe.Location = new Point(614, 101);
            txtMatchExe.MaxLength = 256;
            txtMatchExe.Multiline = false;
            txtMatchExe.Name = "txtMatchExe";
            txtMatchExe.ReadOnly = false;
            txtMatchExe.Size = new Size(200, 32);
            txtMatchExe.TabIndex = 5;
            txtMatchExe.TextAlign = HorizontalAlignment.Left;
            txtMatchExe.UseSystemPasswordChar = false;
            // 
            // lblGameName
            // 
            lblGameName.AutoSize = true;
            lblGameName.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblGameName.ForeColor = Color.FromArgb(162, 162, 176);
            lblGameName.Location = new Point(14, 137);
            lblGameName.Name = "lblGameName";
            lblGameName.Size = new Size(74, 13);
            lblGameName.TabIndex = 6;
            lblGameName.Text = "Game Name:";
            // 
            // txtGameName
            // 
            txtGameName.BackColor = Color.Transparent;
            txtGameName.Location = new Point(14, 153);
            txtGameName.MaxLength = 256;
            txtGameName.Multiline = false;
            txtGameName.Name = "txtGameName";
            txtGameName.ReadOnly = false;
            txtGameName.Size = new Size(260, 32);
            txtGameName.TabIndex = 7;
            txtGameName.TextAlign = HorizontalAlignment.Left;
            txtGameName.UseSystemPasswordChar = false;
            // 
            // lblConfigFormat
            // 
            lblConfigFormat.AutoSize = true;
            lblConfigFormat.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblConfigFormat.ForeColor = Color.FromArgb(162, 162, 176);
            lblConfigFormat.Location = new Point(284, 137);
            lblConfigFormat.Name = "lblConfigFormat";
            lblConfigFormat.Size = new Size(85, 13);
            lblConfigFormat.TabIndex = 8;
            lblConfigFormat.Text = "Config Format:";
            // 
            // cmbConfigFormat
            // 
            cmbConfigFormat.BackColor = Color.FromArgb(22, 22, 29);
            cmbConfigFormat.DrawMode = DrawMode.OwnerDrawFixed;
            cmbConfigFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbConfigFormat.Font = new Font("Segoe UI", 8.75F);
            cmbConfigFormat.ForeColor = Color.FromArgb(242, 242, 246);
            cmbConfigFormat.ItemHeight = 24;
            cmbConfigFormat.Items.AddRange(new object[] { "SpaceDelimited", "Ini", "KeyValueEquals", "Json" });
            cmbConfigFormat.Location = new Point(284, 153);
            cmbConfigFormat.Name = "cmbConfigFormat";
            cmbConfigFormat.Size = new Size(174, 30);
            cmbConfigFormat.TabIndex = 9;
            cmbConfigFormat.SelectedIndexChanged += cmbConfigFormat_SelectedIndexChanged;
            // 
            // lblIniSection
            // 
            lblIniSection.AutoSize = true;
            lblIniSection.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblIniSection.ForeColor = Color.FromArgb(162, 162, 176);
            lblIniSection.Location = new Point(468, 137);
            lblIniSection.Name = "lblIniSection";
            lblIniSection.Size = new Size(146, 13);
            lblIniSection.TabIndex = 10;
            lblIniSection.Text = "INI Section (for Ini format):";
            // 
            // txtIniSection
            // 
            txtIniSection.BackColor = Color.Transparent;
            txtIniSection.Location = new Point(468, 153);
            txtIniSection.MaxLength = 256;
            txtIniSection.Multiline = false;
            txtIniSection.Name = "txtIniSection";
            txtIniSection.ReadOnly = false;
            txtIniSection.Size = new Size(346, 32);
            txtIniSection.TabIndex = 11;
            txtIniSection.Text = "SystemSettings";
            txtIniSection.TextAlign = HorizontalAlignment.Left;
            txtIniSection.UseSystemPasswordChar = false;
            // 
            // lblConfigPath
            // 
            lblConfigPath.AutoSize = true;
            lblConfigPath.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblConfigPath.ForeColor = Color.FromArgb(162, 162, 176);
            lblConfigPath.Location = new Point(14, 189);
            lblConfigPath.Name = "lblConfigPath";
            lblConfigPath.Size = new Size(419, 13);
            lblConfigPath.TabIndex = 12;
            lblConfigPath.Text = "Config File Path (relative to install dir, or %LOCALAPPDATA% / {DOCUMENTS}):";
            // 
            // txtConfigPath
            // 
            txtConfigPath.BackColor = Color.Transparent;
            txtConfigPath.Location = new Point(14, 205);
            txtConfigPath.MaxLength = 1024;
            txtConfigPath.Multiline = false;
            txtConfigPath.Name = "txtConfigPath";
            txtConfigPath.ReadOnly = false;
            txtConfigPath.Size = new Size(548, 32);
            txtConfigPath.TabIndex = 13;
            txtConfigPath.TextAlign = HorizontalAlignment.Left;
            txtConfigPath.UseSystemPasswordChar = false;
            // 
            // btnBrowseConfig
            // 
            btnBrowseConfig.AccentColor = Color.FromArgb(214, 28, 52);
            btnBrowseConfig.BackColor = Color.Transparent;
            btnBrowseConfig.CornerRadius = 5;
            btnBrowseConfig.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnBrowseConfig.ForeColor = Color.FromArgb(242, 242, 246);
            btnBrowseConfig.IsPrimaryAction = false;
            btnBrowseConfig.Location = new Point(570, 205);
            btnBrowseConfig.Name = "btnBrowseConfig";
            btnBrowseConfig.Size = new Size(118, 30);
            btnBrowseConfig.TabIndex = 14;
            btnBrowseConfig.Text = "Browse File...";
            btnBrowseConfig.Click += btnBrowseConfig_Click;
            // 
            // btnReadConfigKeys
            // 
            btnReadConfigKeys.AccentColor = Color.FromArgb(214, 28, 52);
            btnReadConfigKeys.BackColor = Color.Transparent;
            btnReadConfigKeys.CornerRadius = 5;
            btnReadConfigKeys.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnReadConfigKeys.ForeColor = Color.FromArgb(242, 242, 246);
            btnReadConfigKeys.IsPrimaryAction = false;
            btnReadConfigKeys.Location = new Point(694, 205);
            btnReadConfigKeys.Name = "btnReadConfigKeys";
            btnReadConfigKeys.Size = new Size(120, 30);
            btnReadConfigKeys.TabIndex = 15;
            btnReadConfigKeys.Text = "Pull Keys...";
            btnReadConfigKeys.Click += btnReadConfigKeys_Click;
            // 
            // grpPackSettings
            // 
            grpPackSettings.BackColor = Color.Transparent;
            grpPackSettings.Controls.Add(lblScope);
            grpPackSettings.Controls.Add(cmbScope);
            grpPackSettings.Controls.Add(btnAddPreset);
            grpPackSettings.Controls.Add(btnRenamePreset);
            grpPackSettings.Controls.Add(btnDeletePreset);
            grpPackSettings.Controls.Add(lblEntryCount);
            grpPackSettings.Controls.Add(lblKey);
            grpPackSettings.Controls.Add(txtKey);
            grpPackSettings.Controls.Add(lblValue);
            grpPackSettings.Controls.Add(txtValue);
            grpPackSettings.Controls.Add(lblToken);
            grpPackSettings.Controls.Add(cmbDynamicToken);
            grpPackSettings.Controls.Add(btnAddPair);
            grpPackSettings.Controls.Add(dgvSettings);
            grpPackSettings.Font = new Font("Segoe UI", 9.5F);
            grpPackSettings.Location = new Point(16, 313);
            grpPackSettings.Name = "grpPackSettings";
            grpPackSettings.Padding = new Padding(14, 44, 14, 14);
            grpPackSettings.Size = new Size(828, 316);
            grpPackSettings.SubTitle = "Edit Universal Settings (applied to all presets) or preset-specific key/value overrides";
            grpPackSettings.TabIndex = 5;
            grpPackSettings.Title = "Settings & Presets Editor";
            // 
            // lblScope
            // 
            lblScope.AutoSize = true;
            lblScope.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblScope.ForeColor = Color.FromArgb(242, 242, 246);
            lblScope.Location = new Point(14, 51);
            lblScope.Name = "lblScope";
            lblScope.Size = new Size(48, 15);
            lblScope.TabIndex = 0;
            lblScope.Text = "Editing:";
            // 
            // cmbScope
            // 
            cmbScope.BackColor = Color.FromArgb(22, 22, 29);
            cmbScope.DrawMode = DrawMode.OwnerDrawFixed;
            cmbScope.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScope.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            cmbScope.ForeColor = Color.FromArgb(242, 242, 246);
            cmbScope.ItemHeight = 24;
            cmbScope.Location = new Point(70, 44);
            cmbScope.Name = "cmbScope";
            cmbScope.Size = new Size(342, 30);
            cmbScope.TabIndex = 1;
            cmbScope.SelectedIndexChanged += cmbScope_SelectedIndexChanged;
            // 
            // btnAddPreset
            // 
            btnAddPreset.AccentColor = Color.FromArgb(214, 28, 52);
            btnAddPreset.BackColor = Color.Transparent;
            btnAddPreset.CornerRadius = 5;
            btnAddPreset.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnAddPreset.ForeColor = Color.FromArgb(242, 242, 246);
            btnAddPreset.IsPrimaryAction = false;
            btnAddPreset.Location = new Point(420, 44);
            btnAddPreset.Name = "btnAddPreset";
            btnAddPreset.Size = new Size(106, 30);
            btnAddPreset.TabIndex = 2;
            btnAddPreset.Text = "+ New Preset";
            btnAddPreset.Click += btnAddPreset_Click;
            // 
            // btnRenamePreset
            // 
            btnRenamePreset.AccentColor = Color.FromArgb(214, 28, 52);
            btnRenamePreset.BackColor = Color.Transparent;
            btnRenamePreset.CornerRadius = 5;
            btnRenamePreset.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnRenamePreset.ForeColor = Color.FromArgb(242, 242, 246);
            btnRenamePreset.IsPrimaryAction = false;
            btnRenamePreset.Location = new Point(532, 44);
            btnRenamePreset.Name = "btnRenamePreset";
            btnRenamePreset.Size = new Size(92, 30);
            btnRenamePreset.TabIndex = 3;
            btnRenamePreset.Text = "Rename";
            btnRenamePreset.Click += btnRenamePreset_Click;
            // 
            // btnDeletePreset
            // 
            btnDeletePreset.AccentColor = Color.FromArgb(244, 63, 94);
            btnDeletePreset.BackColor = Color.Transparent;
            btnDeletePreset.CornerRadius = 5;
            btnDeletePreset.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnDeletePreset.ForeColor = Color.FromArgb(242, 242, 246);
            btnDeletePreset.IsPrimaryAction = false;
            btnDeletePreset.Location = new Point(630, 44);
            btnDeletePreset.Name = "btnDeletePreset";
            btnDeletePreset.Size = new Size(92, 30);
            btnDeletePreset.TabIndex = 4;
            btnDeletePreset.Text = "Delete";
            btnDeletePreset.Click += btnDeletePreset_Click;
            // 
            // lblEntryCount
            // 
            lblEntryCount.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblEntryCount.ForeColor = Color.FromArgb(162, 162, 176);
            lblEntryCount.Location = new Point(726, 51);
            lblEntryCount.Name = "lblEntryCount";
            lblEntryCount.Size = new Size(88, 18);
            lblEntryCount.TabIndex = 5;
            lblEntryCount.Text = "0 keys";
            lblEntryCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblKey
            // 
            lblKey.AutoSize = true;
            lblKey.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblKey.ForeColor = Color.FromArgb(162, 162, 176);
            lblKey.Location = new Point(14, 82);
            lblKey.Name = "lblKey";
            lblKey.Size = new Size(103, 13);
            lblKey.TabIndex = 6;
            lblKey.Text = "Setting Key / CVar:";
            // 
            // txtKey
            // 
            txtKey.BackColor = Color.Transparent;
            txtKey.Location = new Point(14, 98);
            txtKey.MaxLength = 256;
            txtKey.Multiline = false;
            txtKey.Name = "txtKey";
            txtKey.ReadOnly = false;
            txtKey.Size = new Size(256, 32);
            txtKey.TabIndex = 7;
            txtKey.TextAlign = HorizontalAlignment.Left;
            txtKey.UseSystemPasswordChar = false;
            // 
            // lblValue
            // 
            lblValue.AutoSize = true;
            lblValue.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblValue.ForeColor = Color.FromArgb(162, 162, 176);
            lblValue.Location = new Point(278, 82);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(38, 13);
            lblValue.TabIndex = 8;
            lblValue.Text = "Value:";
            // 
            // txtValue
            // 
            txtValue.BackColor = Color.Transparent;
            txtValue.Location = new Point(278, 98);
            txtValue.MaxLength = 512;
            txtValue.Multiline = false;
            txtValue.Name = "txtValue";
            txtValue.ReadOnly = false;
            txtValue.Size = new Size(200, 32);
            txtValue.TabIndex = 9;
            txtValue.TextAlign = HorizontalAlignment.Left;
            txtValue.UseSystemPasswordChar = false;
            // 
            // lblToken
            // 
            lblToken.AutoSize = true;
            lblToken.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblToken.ForeColor = Color.FromArgb(162, 162, 176);
            lblToken.Location = new Point(486, 82);
            lblToken.Name = "lblToken";
            lblToken.Size = new Size(121, 13);
            lblToken.TabIndex = 10;
            lblToken.Text = "Insert Dynamic Token:";
            // 
            // cmbDynamicToken
            // 
            cmbDynamicToken.BackColor = Color.FromArgb(22, 22, 29);
            cmbDynamicToken.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDynamicToken.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDynamicToken.Font = new Font("Segoe UI", 8.5F);
            cmbDynamicToken.ForeColor = Color.FromArgb(242, 242, 246);
            cmbDynamicToken.ItemHeight = 24;
            cmbDynamicToken.Location = new Point(486, 98);
            cmbDynamicToken.Name = "cmbDynamicToken";
            cmbDynamicToken.Size = new Size(174, 30);
            cmbDynamicToken.TabIndex = 11;
            cmbDynamicToken.SelectedIndexChanged += cmbDynamicToken_SelectedIndexChanged;
            // 
            // btnAddPair
            // 
            btnAddPair.AccentColor = Color.FromArgb(214, 28, 52);
            btnAddPair.BackColor = Color.Transparent;
            btnAddPair.CornerRadius = 5;
            btnAddPair.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnAddPair.ForeColor = Color.FromArgb(242, 242, 246);
            btnAddPair.IsPrimaryAction = true;
            btnAddPair.Location = new Point(668, 98);
            btnAddPair.Name = "btnAddPair";
            btnAddPair.Size = new Size(146, 30);
            btnAddPair.TabIndex = 12;
            btnAddPair.Text = "Add / Update Key";
            btnAddPair.Click += btnAddPair_Click;
            // 
            // dgvSettings
            // 
            dgvSettings.AllowUserToAddRows = false;
            dgvSettings.AllowUserToDeleteRows = false;
            dgvSettings.AllowUserToResizeRows = false;
            dgvSettings.BackgroundColor = Color.FromArgb(15, 15, 20);
            dgvSettings.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvSettings.ColumnHeadersHeight = 28;
            dgvSettings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSettings.Columns.AddRange(new DataGridViewColumn[] { colKey, colValue });
            dgvSettings.EnableHeadersVisualStyles = false;
            dgvSettings.GridColor = Color.FromArgb(36, 36, 48);
            dgvSettings.Location = new Point(14, 136);
            dgvSettings.Name = "dgvSettings";
            dgvSettings.RowHeadersVisible = false;
            dgvSettings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSettings.Size = new Size(800, 166);
            dgvSettings.TabIndex = 13;
            dgvSettings.CellEndEdit += dgvSettings_CellEndEdit;
            dgvSettings.SelectionChanged += dgvSettings_SelectionChanged;
            // 
            // colKey
            // 
            colKey.HeaderText = "Setting Key / CVar";
            colKey.Name = "colKey";
            colKey.Width = 420;
            // 
            // colValue
            // 
            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colValue.HeaderText = "Value";
            colValue.Name = "colValue";
            // 
            // btnRemoveSelectedKey
            // 
            btnRemoveSelectedKey.AccentColor = Color.FromArgb(244, 63, 94);
            btnRemoveSelectedKey.BackColor = Color.Transparent;
            btnRemoveSelectedKey.CornerRadius = 5;
            btnRemoveSelectedKey.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnRemoveSelectedKey.ForeColor = Color.FromArgb(242, 242, 246);
            btnRemoveSelectedKey.IsPrimaryAction = false;
            btnRemoveSelectedKey.Location = new Point(16, 639);
            btnRemoveSelectedKey.Name = "btnRemoveSelectedKey";
            btnRemoveSelectedKey.Size = new Size(152, 36);
            btnRemoveSelectedKey.TabIndex = 6;
            btnRemoveSelectedKey.Text = "Remove Selected";
            btnRemoveSelectedKey.Click += btnRemoveSelectedKey_Click;
            // 
            // btnPreviewJson
            // 
            btnPreviewJson.AccentColor = Color.FromArgb(214, 28, 52);
            btnPreviewJson.BackColor = Color.Transparent;
            btnPreviewJson.CornerRadius = 5;
            btnPreviewJson.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnPreviewJson.ForeColor = Color.FromArgb(242, 242, 246);
            btnPreviewJson.IsPrimaryAction = false;
            btnPreviewJson.Location = new Point(176, 639);
            btnPreviewJson.Name = "btnPreviewJson";
            btnPreviewJson.Size = new Size(124, 36);
            btnPreviewJson.TabIndex = 7;
            btnPreviewJson.Text = "Preview JSON";
            btnPreviewJson.Click += btnPreviewJson_Click;
            // 
            // btnClose
            // 
            btnClose.AccentColor = Color.FromArgb(214, 28, 52);
            btnClose.BackColor = Color.Transparent;
            btnClose.CornerRadius = 5;
            btnClose.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnClose.ForeColor = Color.FromArgb(242, 242, 246);
            btnClose.IsPrimaryAction = false;
            btnClose.Location = new Point(430, 639);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(96, 36);
            btnClose.TabIndex = 8;
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // btnExportJson
            // 
            btnExportJson.AccentColor = Color.FromArgb(214, 28, 52);
            btnExportJson.BackColor = Color.Transparent;
            btnExportJson.CornerRadius = 5;
            btnExportJson.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportJson.ForeColor = Color.FromArgb(242, 242, 246);
            btnExportJson.IsPrimaryAction = false;
            btnExportJson.Location = new Point(534, 639);
            btnExportJson.Name = "btnExportJson";
            btnExportJson.Size = new Size(144, 36);
            btnExportJson.TabIndex = 9;
            btnExportJson.Text = "Export .json...";
            btnExportJson.Click += btnExportJson_Click;
            // 
            // btnSaveToLibrary
            // 
            btnSaveToLibrary.AccentColor = Color.FromArgb(214, 28, 52);
            btnSaveToLibrary.BackColor = Color.Transparent;
            btnSaveToLibrary.CornerRadius = 5;
            btnSaveToLibrary.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSaveToLibrary.ForeColor = Color.FromArgb(242, 242, 246);
            btnSaveToLibrary.IsPrimaryAction = true;
            btnSaveToLibrary.Location = new Point(686, 639);
            btnSaveToLibrary.Name = "btnSaveToLibrary";
            btnSaveToLibrary.Size = new Size(158, 36);
            btnSaveToLibrary.TabIndex = 10;
            btnSaveToLibrary.Text = "Save to Overcapped";
            btnSaveToLibrary.Click += btnSaveToLibrary_Click;
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(btnSaveToLibrary);
            roThemeContainer1.Controls.Add(btnExportJson);
            roThemeContainer1.Controls.Add(btnClose);
            roThemeContainer1.Controls.Add(grpPackMeta);
            roThemeContainer1.Controls.Add(btnPreviewJson);
            roThemeContainer1.Controls.Add(grpPackSettings);
            roThemeContainer1.Controls.Add(btnRemoveSelectedKey);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(860, 687);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterParent;
            roThemeContainer1.SubHeader = "";
            roThemeContainer1.TabIndex = 11;
            roThemeContainer1.Text = "Profile Pack Builder";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // ProfilePackBuilder
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(860, 687);
            Controls.Add(roThemeContainer1);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProfilePackBuilder";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Overcapped Optimizer | Profile Pack Builder";
            grpPackMeta.ResumeLayout(false);
            grpPackMeta.PerformLayout();
            grpPackSettings.ResumeLayout(false);
            grpPackSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSettings).EndInit();
            roThemeContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ROModernButton btnLoadInstalledPack;
        private ROModernButton btnImportJson;
        private ROModernButton btnNewPack;
        private ROGroupBox grpPackMeta;
        private Label lblPickGame;
        private ROComboBox cmbLibraryGame;
        private Label lblPackId;
        private ROTextBox txtPackId;
        private Label lblMatchExe;
        private ROTextBox txtMatchExe;
        private Label lblGameName;
        private ROTextBox txtGameName;
        private Label lblConfigFormat;
        private ROComboBox cmbConfigFormat;
        private Label lblIniSection;
        private ROTextBox txtIniSection;
        private Label lblConfigPath;
        private ROTextBox txtConfigPath;
        private ROModernButton btnBrowseConfig;
        private ROModernButton btnReadConfigKeys;
        private ROGroupBox grpPackSettings;
        private Label lblScope;
        private ROComboBox cmbScope;
        private ROModernButton btnAddPreset;
        private ROModernButton btnRenamePreset;
        private ROModernButton btnDeletePreset;
        private Label lblEntryCount;
        private Label lblKey;
        private ROTextBox txtKey;
        private Label lblValue;
        private ROTextBox txtValue;
        private Label lblToken;
        private ROComboBox cmbDynamicToken;
        private ROModernButton btnAddPair;
        private DataGridView dgvSettings;
        private DataGridViewTextBoxColumn colKey;
        private DataGridViewTextBoxColumn colValue;
        private ROModernButton btnRemoveSelectedKey;
        private ROModernButton btnPreviewJson;
        private ROModernButton btnClose;
        private ROModernButton btnExportJson;
        private ROModernButton btnSaveToLibrary;
        private ROThemeContainer roThemeContainer1;
    }
}