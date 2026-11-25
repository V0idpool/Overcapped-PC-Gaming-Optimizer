using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.GUI;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Models;

namespace Overcapped_Optimizer
{
    public partial class ProfilePackBuilder : Form
    {
        private const string UniversalScopeLabel = "Universal Settings (All Presets)";
        private const string TokenPlaceholder = "-- Select Token --";

        private GameProfilePack _pack = new();
        private bool _isSyncingUi;
        private string? _lastBrowsedFullConfigPath;

        public bool PacksChanged { get; private set; }

        public ProfilePackBuilder(GameProfile? initialGame = null)
        {
            InitializeComponent();
            StyleDataGrid();
            PopulateDynamicTokens();
            PopulateLibraryGames(initialGame);

            if (initialGame != null)
            {
                var existingPack = JsonProfilePackManager.FindMatchingPack(initialGame);
                if (existingPack != null)
                {
                    LoadPackIntoEditor(JsonProfilePackManager.ClonePack(existingPack));
                }
                else
                {
                    CreateBlankPackForGame(initialGame);
                }
            }
            else
            {
                CreateBlankPackForGame(null);
            }
        }

        private void StyleDataGrid()
        {
            dgvSettings.DefaultCellStyle.BackColor = Color.FromArgb(22, 22, 29);
            dgvSettings.DefaultCellStyle.ForeColor = Color.FromArgb(242, 242, 246);
            dgvSettings.DefaultCellStyle.SelectionBackColor = Color.FromArgb(66, 22, 32);
            dgvSettings.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvSettings.DefaultCellStyle.Font = new Font("Segoe UI", 8.75F);
            dgvSettings.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvSettings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(18, 18, 25);

            dgvSettings.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 28, 38);
            dgvSettings.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(242, 242, 246);
            dgvSettings.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvSettings.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(28, 28, 38);
            dgvSettings.RowTemplate.Height = 26;

            dgvSettings.EditingControlShowing += (_, e) =>
            {
                if (e.Control is TextBox tb)
                {
                    tb.BackColor = Color.FromArgb(30, 30, 40);
                    tb.ForeColor = Color.White;
                    tb.BorderStyle = BorderStyle.None;
                }
            };

            dgvSettings.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Delete && !dgvSettings.IsCurrentCellInEditMode)
                {
                    RemoveSelectedRows();
                    e.Handled = true;
                }
            };
        }

        private void PopulateDynamicTokens()
        {
            _isSyncingUi = true;
            try
            {
                cmbDynamicToken.Items.Clear();
                cmbDynamicToken.Items.Add(TokenPlaceholder);
                foreach (string token in JsonProfilePackManager.DynamicTokens)
                    cmbDynamicToken.Items.Add(token);
                cmbDynamicToken.SelectedIndex = 0;
            }
            finally
            {
                _isSyncingUi = false;
            }
        }

        private void PopulateLibraryGames(GameProfile? initialGame)
        {
            _isSyncingUi = true;
            try
            {
                cmbLibraryGame.Items.Clear();
                cmbLibraryGame.Items.Add("(Manual / Custom Game)");

                int selectIdx = 0;
                for (int i = 0; i < AppSettings.Current.Games.Count; i++)
                {
                    GameProfile g = AppSettings.Current.Games[i];
                    cmbLibraryGame.Items.Add(g);
                    if (initialGame != null && g.Id == initialGame.Id)
                        selectIdx = i + 1;
                }

                cmbLibraryGame.SelectedIndex = selectIdx;
            }
            finally
            {
                _isSyncingUi = false;
            }
        }

        private GameProfile? SelectedLibraryGame => cmbLibraryGame.SelectedItem as GameProfile;

        private void CreateBlankPackForGame(GameProfile? game)
        {
            _lastBrowsedFullConfigPath = null;
            bool isUnreal = game?.Engine == GameEngineType.UnrealEngine;

            var pack = new GameProfilePack
            {
                PackId = game != null ? JsonProfilePackManager.SanitizePackId(game.Name) : "custom_game",
                GameName = game?.Name ?? string.Empty,
                MatchExecutable = game?.ExecutableName ?? string.Empty,
                ConfigFormat = isUnreal ? "Ini" : "SpaceDelimited",
                IniSection = "SystemSettings",
                RelativeConfigPath = GuessInitialConfigPath(game, isUnreal),
                UniversalSettings = new Dictionary<string, string>(),
                Presets = new Dictionary<string, Dictionary<string, string>>
                {
                    ["Performance (Max FPS)"] = new(),
                    ["Balanced"] = new(),
                    ["Recommended (Optimized)"] = new(),
                    ["Ultra (Maximum Visuals)"] = new()
                }
            };

            LoadPackIntoEditor(pack);
        }

        private static string GuessInitialConfigPath(GameProfile? game, bool isUnreal)
        {
            if (game == null) return string.Empty;

            // 1) If a matching JSON pack already defines a path, use it
            var existingPack = JsonProfilePackManager.FindMatchingPack(game);
            if (existingPack != null && !string.IsNullOrWhiteSpace(existingPack.RelativeConfigPath))
                return existingPack.RelativeConfigPath;

            // 2) Use saved ConfigDirectory, or resolve it live for games scanned prior to v1.1
            string configDir = game.ConfigDirectory;
            if (string.IsNullOrWhiteSpace(configDir) || !Directory.Exists(configDir))
            {
                configDir = GameScanner.ResolveConfigDirectory(
                    game.Name, game.InstallDirectory, game.Engine, game.ExecutablePath);
            }

            if (!string.IsNullOrWhiteSpace(configDir) && Directory.Exists(configDir))
            {
                if (isUnreal)
                {
                    string gus = Path.Combine(configDir, "GameUserSettings.ini");
                    string eng = Path.Combine(configDir, "Engine.ini");
                    string preferred = File.Exists(gus) ? gus : eng;
                    return JsonProfilePackManager.MakePortableConfigPath(preferred, game.InstallDirectory);
                }

                string? found = Directory.EnumerateFiles(configDir, "*.*")
                    .FirstOrDefault(f =>
                        f.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".ini", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".config", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(found))
                    return JsonProfilePackManager.MakePortableConfigPath(found, game.InstallDirectory);
            }

            return string.Empty;
        }

        private void LoadPackIntoEditor(GameProfilePack pack)
        {
            _pack = pack;
            _pack.UniversalSettings ??= new Dictionary<string, string>();
            _pack.Presets ??= new Dictionary<string, Dictionary<string, string>>();

            if (_pack.Presets.Count == 0)
                _pack.Presets["Recommended (Optimized)"] = new Dictionary<string, string>();

            _isSyncingUi = true;
            try
            {
                txtPackId.Text = _pack.PackId;
                txtGameName.Text = _pack.GameName;
                txtMatchExe.Text = _pack.MatchExecutable;
                txtConfigPath.Text = _pack.RelativeConfigPath;
                txtIniSection.Text = string.IsNullOrWhiteSpace(_pack.IniSection) ? "SystemSettings" : _pack.IniSection;

                int fmtIdx = -1;
                for (int i = 0; i < cmbConfigFormat.Items.Count; i++)
                {
                    if (string.Equals(cmbConfigFormat.Items[i]?.ToString(), _pack.ConfigFormat, StringComparison.OrdinalIgnoreCase))
                    {
                        fmtIdx = i;
                        break;
                    }
                }
                cmbConfigFormat.SelectedIndex = fmtIdx >= 0 ? fmtIdx : 0;

                UpdateIniSectionVisibility();
                RefreshScopeDropdown(UniversalScopeLabel);
            }
            finally
            {
                _isSyncingUi = false;
            }

            RefreshGridFromActiveScope();
        }

        private void RefreshScopeDropdown(string? preferredSelection)
        {
            bool prevSync = _isSyncingUi;
            _isSyncingUi = true;
            try
            {
                cmbScope.Items.Clear();
                cmbScope.Items.Add(UniversalScopeLabel);
                foreach (string presetName in _pack.Presets.Keys)
                    cmbScope.Items.Add(presetName);

                int idx = !string.IsNullOrWhiteSpace(preferredSelection)
                    ? cmbScope.Items.IndexOf(preferredSelection)
                    : 0;
                cmbScope.SelectedIndex = idx >= 0 ? idx : 0;
                UpdatePresetButtonsState();
            }
            finally
            {
                _isSyncingUi = prevSync;
            }
        }

        private bool IsUniversalScopeSelected => cmbScope.SelectedIndex <= 0;

        private string? SelectedPresetName =>
            IsUniversalScopeSelected ? null : cmbScope.SelectedItem?.ToString();

        private Dictionary<string, string> GetActiveDictionary()
        {
            if (IsUniversalScopeSelected)
                return _pack.UniversalSettings;

            string presetName = SelectedPresetName!;
            if (!_pack.Presets.TryGetValue(presetName, out var dict))
            {
                dict = new Dictionary<string, string>();
                _pack.Presets[presetName] = dict;
            }

            return dict;
        }

        private void UpdatePresetButtonsState()
        {
            bool isPreset = !IsUniversalScopeSelected;
            btnRenamePreset.Enabled = isPreset;
            btnDeletePreset.Enabled = isPreset && _pack.Presets.Count > 1;
        }

        private void UpdateIniSectionVisibility()
        {
            bool isIni = (cmbConfigFormat.SelectedItem?.ToString() ?? "")
                .Equals("Ini", StringComparison.OrdinalIgnoreCase);

            lblIniSection.Enabled = isIni;
            txtIniSection.Enabled = isIni;
        }

        private void RefreshGridFromActiveScope()
        {
            var dict = GetActiveDictionary();

            _isSyncingUi = true;
            try
            {
                dgvSettings.Rows.Clear();
                foreach (var kvp in dict)
                    dgvSettings.Rows.Add(kvp.Key, kvp.Value);

                lblEntryCount.Text = dict.Count == 1 ? "1 key" : $"{dict.Count} keys";
            }
            finally
            {
                _isSyncingUi = false;
            }
        }

        private void SyncMetadataFromInputs()
        {
            _pack.PackId = JsonProfilePackManager.SanitizePackId(
                string.IsNullOrWhiteSpace(txtPackId.Text) ? txtGameName.Text : txtPackId.Text);
            _pack.GameName = txtGameName.Text.Trim();
            _pack.MatchExecutable = Path.GetFileName(txtMatchExe.Text.Trim());
            _pack.ConfigFormat = cmbConfigFormat.SelectedItem?.ToString() ?? "SpaceDelimited";
            _pack.RelativeConfigPath = txtConfigPath.Text.Trim();
            _pack.IniSection = string.IsNullOrWhiteSpace(txtIniSection.Text)
                ? "SystemSettings"
                : txtIniSection.Text.Trim().Trim('[', ']');
        }

        private bool ValidatePack(out string error)
        {
            SyncMetadataFromInputs();

            if (string.IsNullOrWhiteSpace(_pack.GameName))
            {
                error = "Enter a Game Name for this profile pack.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(_pack.MatchExecutable))
            {
                error = "Enter a Match Executable (for example: RustClient.exe or Game-Win64-Shipping.exe).";
                return false;
            }

            if (!_pack.MatchExecutable.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                _pack.MatchExecutable += ".exe";

            int totalKeys = _pack.UniversalSettings.Count + _pack.Presets.Values.Sum(d => d.Count);
            if (totalKeys == 0)
            {
                error = "Add at least one key/value setting in Universal Settings or a Preset before saving/exporting.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private void cmbLibraryGame_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isSyncingUi) return;

            _lastBrowsedFullConfigPath = null;
            GameProfile? game = SelectedLibraryGame;
            if (game == null)
            {
                txtGameName.Text = string.Empty;
                txtPackId.Text = "custom_game";
                txtMatchExe.Text = string.Empty;
                txtConfigPath.Text = string.Empty;
                return;
            }

            txtGameName.Text = game.Name;
            txtPackId.Text = JsonProfilePackManager.SanitizePackId(game.Name);
            txtMatchExe.Text = game.ExecutableName;

            bool isUnreal = game.Engine == GameEngineType.UnrealEngine;
            string guessed = GuessInitialConfigPath(game, isUnreal);

            // Always overwrite txtConfigPath (even with empty string) so stale paths from the previous game never linger
            txtConfigPath.Text = guessed;

            string ext = Path.GetExtension(guessed);
            if (ext.Equals(".json", StringComparison.OrdinalIgnoreCase))
                cmbConfigFormat.SelectedIndex = 3; // Json
            else if (isUnreal || ext.Equals(".ini", StringComparison.OrdinalIgnoreCase))
                cmbConfigFormat.SelectedIndex = 1; // Ini
            else
                cmbConfigFormat.SelectedIndex = 0; // SpaceDelimited
        }

        private void cmbConfigFormat_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateIniSectionVisibility();
        }

        private void btnBrowseConfig_Click(object? sender, EventArgs e)
        {
            GameProfile? game = SelectedLibraryGame;
            string initialDir = string.Empty;

            if (game != null)
            {
                if (!string.IsNullOrWhiteSpace(game.ConfigDirectory) && Directory.Exists(game.ConfigDirectory))
                    initialDir = game.ConfigDirectory;
                else if (!string.IsNullOrWhiteSpace(game.InstallDirectory) && Directory.Exists(game.InstallDirectory))
                    initialDir = game.InstallDirectory;
            }

            using var ofd = new OpenFileDialog
            {
                Filter = "Config Files (*.ini;*.cfg;*.json;*.config;*.txt)|*.ini;*.cfg;*.json;*.config;*.txt|All Files (*.*)|*.*",
                Title = "Select Target Game Config File (.ini, .cfg, or .json)",
                InitialDirectory = Directory.Exists(initialDir) ? initialDir : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            _lastBrowsedFullConfigPath = ofd.FileName;
            txtConfigPath.Text = JsonProfilePackManager.MakePortableConfigPath(ofd.FileName, game?.InstallDirectory);

            string ext = Path.GetExtension(ofd.FileName);
            if (ext.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                cmbConfigFormat.SelectedIndex = 3; // Json
                return;
            }

            bool hasIniSections = false;
            int equalsLines = 0;
            int spaceLines = 0;

            try
            {
                foreach (string rawLine in File.ReadLines(ofd.FileName).Take(60))
                {
                    string line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#") || line.StartsWith("//"))
                        continue;

                    if (line.Length > 2 && line.StartsWith("[") && line.EndsWith("]"))
                        hasIniSections = true;
                    else if (line.Contains('='))
                        equalsLines++;
                    else if (line.Contains(' '))
                        spaceLines++;
                }
            }
            catch { }

            if (ext.Equals(".ini", StringComparison.OrdinalIgnoreCase) || hasIniSections)
            {
                cmbConfigFormat.SelectedIndex = 1; // Ini
                var sections = JsonProfilePackManager.ReadIniSections(ofd.FileName);
                if (sections.Count > 0)
                {
                    string best = sections.FirstOrDefault(s => s.Equals("SystemSettings", StringComparison.OrdinalIgnoreCase))
                               ?? sections.FirstOrDefault(s => s.Contains("GameUserSettings", StringComparison.OrdinalIgnoreCase))
                               ?? sections.FirstOrDefault(s => s.Equals("ScalabilityGroups", StringComparison.OrdinalIgnoreCase))
                               ?? sections[0];
                    txtIniSection.Text = best;
                }
            }
            else if (equalsLines > spaceLines)
            {
                cmbConfigFormat.SelectedIndex = 2; // KeyValueEquals
            }
            else
            {
                cmbConfigFormat.SelectedIndex = 0; // SpaceDelimited
            }
        }

        private void btnReadConfigKeys_Click(object? sender, EventArgs e)
        {
            SyncMetadataFromInputs();

            string? resolvedPath = ResolveBrowsableConfigPath();
            if (string.IsNullOrWhiteSpace(resolvedPath) || !File.Exists(resolvedPath))
            {
                MessageBox.Show(
                    "Point to an existing .ini, .cfg, or .json file using 'Browse File...' first.",
                    "Config File Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var parsed = JsonProfilePackManager.ParseConfigFile(resolvedPath, _pack.ConfigFormat, _pack.IniSection);

            if (parsed.Count == 0)
            {
                string fileName = Path.GetFileName(resolvedPath);
                string hint = fileName.Equals("Engine.ini", StringComparison.OrdinalIgnoreCase)
                    ? "\n\nTip: Fresh Unreal Engine.ini files only contain path mounts by default. Browse to GameUserSettings.ini to pull existing graphics/FPS keys, or add r.* CVars manually."
                    : string.Empty;

                MessageBox.Show(
                    $"No editable key/value pairs were found in {fileName}.{hint}",
                    "Pull Keys",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var selected = ShowKeyPickerDialog(parsed, Path.GetFileName(resolvedPath));
            if (selected == null || selected.Count == 0) return;

            var active = GetActiveDictionary();
            foreach (var kvp in selected)
                active[kvp.Key] = kvp.Value;

            RefreshGridFromActiveScope();
        }

        private string? ResolveBrowsableConfigPath()
        {
            if (!string.IsNullOrWhiteSpace(_lastBrowsedFullConfigPath) && File.Exists(_lastBrowsedFullConfigPath))
                return _lastBrowsedFullConfigPath;

            GameProfile dummy = SelectedLibraryGame ?? new GameProfile();
            string resolved = JsonProfilePackManager.ResolvePackConfigPath(dummy, _pack);
            return File.Exists(resolved) ? resolved : null;
        }

        private void cmbScope_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isSyncingUi) return;
            UpdatePresetButtonsState();
            RefreshGridFromActiveScope();
        }

        private void btnAddPreset_Click(object? sender, EventArgs e)
        {
            string? name = PromptForText("New Preset", "Enter a name for the new preset (e.g. Competitive, Balanced, Ultra):", "Custom Preset");
            if (string.IsNullOrWhiteSpace(name)) return;
            name = name.Trim();

            if (name.Equals(UniversalScopeLabel, StringComparison.OrdinalIgnoreCase) ||
                _pack.Presets.Keys.Any(k => k.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("A preset with that name already exists.", "Duplicate Preset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _pack.Presets[name] = new Dictionary<string, string>();
            RefreshScopeDropdown(name);
            RefreshGridFromActiveScope();
        }

        private void btnRenamePreset_Click(object? sender, EventArgs e)
        {
            string? currentName = SelectedPresetName;
            if (string.IsNullOrWhiteSpace(currentName)) return;

            string? newName = PromptForText("Rename Preset", $"Enter a new name for '{currentName}':", currentName);
            if (string.IsNullOrWhiteSpace(newName)) return;
            newName = newName.Trim();

            if (newName.Equals(currentName, StringComparison.Ordinal)) return;

            if (newName.Equals(UniversalScopeLabel, StringComparison.OrdinalIgnoreCase) ||
                _pack.Presets.Keys.Any(k => k.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("A preset with that name already exists.", "Duplicate Preset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Preserve preset order
            var rebuilt = new Dictionary<string, Dictionary<string, string>>();
            foreach (var kvp in _pack.Presets)
            {
                string key = kvp.Key.Equals(currentName, StringComparison.Ordinal) ? newName : kvp.Key;
                rebuilt[key] = kvp.Value;
            }

            _pack.Presets = rebuilt;
            RefreshScopeDropdown(newName);
            RefreshGridFromActiveScope();
        }

        private void btnDeletePreset_Click(object? sender, EventArgs e)
        {
            string? currentName = SelectedPresetName;
            if (string.IsNullOrWhiteSpace(currentName) || _pack.Presets.Count <= 1) return;

            if (MessageBox.Show($"Delete preset '{currentName}' and its keys?", "Delete Preset",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _pack.Presets.Remove(currentName);
            RefreshScopeDropdown(UniversalScopeLabel);
            RefreshGridFromActiveScope();
        }

        private void cmbDynamicToken_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isSyncingUi) return;

            string? token = cmbDynamicToken.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(token) || token == TokenPlaceholder) return;

            txtValue.Text = token;

            _isSyncingUi = true;
            cmbDynamicToken.SelectedIndex = 0;
            _isSyncingUi = false;
        }

        private void btnAddPair_Click(object? sender, EventArgs e)
        {
            string key = txtKey.Text.Trim();
            string value = txtValue.Text.Trim();

            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("Enter a setting key / CVar name first.", "Missing Key", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dict = GetActiveDictionary();

            // Replace case-insensitively if the key already exists
            string? existingKey = dict.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (existingKey != null && existingKey != key)
                dict.Remove(existingKey);

            dict[key] = value;
            txtKey.Text = string.Empty;
            txtValue.Text = string.Empty;

            RefreshGridFromActiveScope();
        }

        private void dgvSettings_SelectionChanged(object? sender, EventArgs e)
        {
            if (_isSyncingUi || dgvSettings.SelectedRows.Count == 0) return;

            var row = dgvSettings.SelectedRows[0];
            txtKey.Text = row.Cells[0].Value?.ToString() ?? string.Empty;
            txtValue.Text = row.Cells[1].Value?.ToString() ?? string.Empty;
        }

        private void dgvSettings_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isSyncingUi) return;

            var dict = GetActiveDictionary();
            dict.Clear();

            foreach (DataGridViewRow row in dgvSettings.Rows)
            {
                string k = row.Cells[0].Value?.ToString()?.Trim() ?? string.Empty;
                string v = row.Cells[1].Value?.ToString()?.Trim() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(k))
                    dict[k] = v;
            }

            lblEntryCount.Text = dict.Count == 1 ? "1 key" : $"{dict.Count} keys";
        }

        private void btnRemoveSelectedKey_Click(object? sender, EventArgs e) => RemoveSelectedRows();

        private void RemoveSelectedRows()
        {
            if (dgvSettings.SelectedRows.Count == 0) return;

            var dict = GetActiveDictionary();
            foreach (DataGridViewRow row in dgvSettings.SelectedRows)
            {
                string? key = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(key))
                    dict.Remove(key);
            }

            RefreshGridFromActiveScope();
        }

        private void btnLoadInstalledPack_Click(object? sender, EventArgs e)
        {
            JsonProfilePackManager.LoadAllPacks();
            if (JsonProfilePackManager.Packs.Count == 0)
            {
                MessageBox.Show("No installed JSON packs were found yet.", "Installed Packs", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            GameProfilePack? chosen = ShowInstalledPackPickerDialog();
            if (chosen == null) return;

            _lastBrowsedFullConfigPath = null;
            LoadPackIntoEditor(JsonProfilePackManager.ClonePack(chosen));
        }

        private void btnImportJson_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Overcapped JSON Profile Pack (*.json)|*.json",
                Title = "Import JSON Profile Pack to Edit",
                InitialDirectory = Directory.Exists(AppSettings.PacksDirectory)
                    ? AppSettings.PacksDirectory
                    : Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                GameProfilePack imported = JsonProfilePackManager.LoadPackFromFile(ofd.FileName);
                _lastBrowsedFullConfigPath = null;
                LoadPackIntoEditor(imported);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                MessageBox.Show($"Could not import JSON pack:\n{ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewPack_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Start a new blank JSON profile pack? Unsaved changes in the editor will be cleared.",
                "New Blank Pack", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            CreateBlankPackForGame(SelectedLibraryGame);
        }

        private void btnPreviewJson_Click(object? sender, EventArgs e)
        {
            SyncMetadataFromInputs();
            string json = JsonProfilePackManager.SerializePack(_pack);

            using var dlg = new JsonPreviewDlg($"{_pack.PackId}.json", json);
            dlg.ShowDialog(this);
        }

        private void btnExportJson_Click(object? sender, EventArgs e)
        {
            if (!ValidatePack(out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "Overcapped JSON Profile Pack (*.json)|*.json",
                Title = "Export JSON Profile Pack for Sharing",
                FileName = $"{_pack.PackId}.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                JsonProfilePackManager.ExportPackToFile(_pack, sfd.FileName);
                MessageBox.Show(
                    $"Exported JSON pack to:\n{sfd.FileName}\n\nReady to drop straight into Discord!",
                    "Pack Exported",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                MessageBox.Show($"Failed to export JSON pack:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveToLibrary_Click(object? sender, EventArgs e)
        {
            if (!ValidatePack(out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string savedPath = JsonProfilePackManager.SavePackToLibrary(_pack);
                txtPackId.Text = _pack.PackId;

                // Link any matching games in the user's library to this pack
                foreach (GameProfile g in AppSettings.Current.Games)
                {
                    if (!string.IsNullOrWhiteSpace(g.ExecutableName) &&
                        g.ExecutableName.Equals(_pack.MatchExecutable, StringComparison.OrdinalIgnoreCase))
                    {
                        g.JsonPackId = _pack.PackId;
                    }
                }

                AppSettings.Save();
                PacksChanged = true;

                MessageBox.Show(
                    $"Saved '{_pack.GameName}' pack to Overcapped Optimizer!\n\nFile: {savedPath}",
                    "Pack Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                MessageBox.Show($"Failed to save JSON pack:\n{ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private string? PromptForText(string title, string message, string defaultValue)
        {
            using var dlg = new TextPromptDlg(title, message, defaultValue);
            return dlg.ShowDialog(this) == DialogResult.OK ? dlg.InputText : null;
        }

        private GameProfilePack? ShowInstalledPackPickerDialog()
        {
            using var dlg = new InstalledPackPickerDlg(JsonProfilePackManager.Packs);
            return dlg.ShowDialog(this) == DialogResult.OK ? dlg.SelectedPack : null;
        }

        private Dictionary<string, string>? ShowKeyPickerDialog(Dictionary<string, string> parsed, string fileName)
        {
            using var dlg = new KeyPickerDlg(parsed, fileName, cmbScope.Text);
            return dlg.ShowDialog(this) == DialogResult.OK ? dlg.SelectedEntries : null;
        }
    }
}