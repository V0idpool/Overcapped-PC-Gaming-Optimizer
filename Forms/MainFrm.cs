using Overcapped_Optimizer.Core;
using Overcapped_Optimizer.Forms;
using Overcapped_Optimizer.Helpers;
using Overcapped_Optimizer.Hotkeys;
using Overcapped_Optimizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Overcapped_Optimizer
{
    public partial class MainFrm : Form
    {
        public static MainFrm? Instance { get; private set; }
        private string GetVersionString() => AppVersion != null ? $"{AppVersion.Major}.{AppVersion.Minor}" : "1.1";
        public System.Windows.Forms.Timer? autoFlushTimer;
        public NotifyIcon? sysTrayIcon;
        private DashboardOpacityOverlay? _dashboardOpacityOverlay;
        private CancellationTokenSource? _pingCts;
        private bool _isLoadingUi;
        private readonly Version? AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public GameProfile? ActiveGame => gameDropdown.SelectedItem as GameProfile;
        private FpsOverlay? _fpsOverlay;
        private List<HardwareHealthWarning> _healthWarnings = new();
        private bool _isCreatingRestorePoint;
        private string? _lastSavedReportPath;
        private const string HotkeyUnboundLabel = "Not Set";
        private readonly HotkeyFilter _hotkeyTracker = new();
        private HotkeyFilter? _activeCaptureFilter;
        private GUI.ROTextBox? _capturingTextBox;
        private GUI.ROModernButton? _capturingButton;
        private string _capturePreviousText = string.Empty;

        public MainFrm()
        {
            InitializeComponent();
            Instance = this;

            menuStrip1.Renderer = new GUI.ROMenuRenderer();
            menuStrip1.ForeColor = GUI.ROPalette.TextPrimary;
            menuStrip1.BackColor = GUI.ROPalette.SurfaceCard;

            cardProfCompetitive.Click += (_, _) => SelectPresetIndexIfValid(0);
            cardProfBalanced.Click += (_, _) => SelectPresetIndexIfValid(1);
            cardProfRecommended.Click += (_, _) => SelectPresetIndexIfValid(2);
            cardProfUltra.Click += (_, _) => SelectPresetIndexIfValid(3);
            profileDropdown.SelectedIndexChanged += (_, _) => SyncProfileCards();
            Optimizer.ActiveGameChanged += GameChanged;
            Optimizer.ActiveGameSessionChanged += GameSessionChanged;
            _hotkeyTracker.HotkeyTriggered += OnPassiveHotkeyTriggered;
        }

        private void MainFrm_Load(object? sender, EventArgs e)
        {
            _isLoadingUi = true;
            try
            {
                string versionString = GetVersionString();
                Text = $"Overcapped Optimizer v{versionString}";
                roThemeContainer1.SubHeader = $"v{versionString}  |  IDLE: NO GAME RUNNING...";

                AppSettings.Load();
                JsonProfilePackManager.LoadAllPacks();

                if (AppSettings.Current.Games.Count == 0)
                {
                    var found = GameScanner.ScanAllInstalledGames();
                    AppSettings.Current.Games.AddRange(found);
                    AppSettings.Save();
                }

                bool isAdmin = Optimizer.IsAdministrator();

                updateToggle.Checked = AppSettings.Current.CheckForUpdates;
                startAsAdminToggle.Checked = AppSettings.Current.StartAsAdmin;
                autoPowerPlanToggle.Checked = AppSettings.Current.AutoPowerPlan;
                gameModeDvrToggle.Checked = AppSettings.Current.GameModeAndDisableDvr;
                disabledFSO.Checked = AppSettings.Current.DisableFSO;
                throttleAppsToggle.Checked = AppSettings.Current.ThrottleBackgroundApps;
                autoFlushChk.Checked = isAdmin && AppSettings.Current.AutoFlushEnabled;
                smartFlushToggle.Checked = AppSettings.Current.SmartFlushEnabled;
                smartFlushThreshold.Value = Math.Clamp(AppSettings.Current.SmartFlushThresholdGB, 1, 16);
                autoFlushinterval.Value = Math.Clamp(AppSettings.Current.AutoFlushInterval, 1, 240);
                autoFlushMinHour.Text = AppSettings.Current.AutoFlushUnit.Equals("Hours", StringComparison.OrdinalIgnoreCase) ? "Hours" : "Minutes";
                autoFlushSound.Checked = AppSettings.Current.AutoFlushSfx;
                disableNicPowerToggle.Checked = AppSettings.Current.DisableNicPowerSaving;
                deliveryOptToggle.Checked = AppSettings.Current.DisableDeliveryOptimization;
                txtServerIp.Text = AppSettings.Current.LastPingServer;
                pCoresToggle.Checked = AppSettings.Current.PCoresOnly;
                fpsOverlayToggle.Checked = isAdmin && AppSettings.Current.FpsOverlayEnabled;
                hotkeysEnabledToggle.Checked = AppSettings.Current.HotkeysEnabled;
                LoadHotkeyFieldsFromSettings();
                Update1PercentLowButtonVisual();

                int posIdx = fpsPositionDropdown.Items.IndexOf(AppSettings.Current.FpsOverlayPosition);
                fpsPositionDropdown.SelectedIndex = posIdx >= 0 ? posIdx : 0;
                string savedOpacity = $"{AppSettings.Current.FpsOverlayOpacity}%";
                int opIdx = fpsOpacityDropdown.Items.IndexOf(savedOpacity);
                fpsOpacityDropdown.SelectedIndex = opIdx >= 0 ? opIdx : 2;
                RefreshGameDropdown(AppSettings.Current.ActiveGameId);
            }
            finally
            {
                _isLoadingUi = false;
            }

            if (AppSettings.Current.CheckForUpdates)
                _ = Updates.CheckForUpdates();

            bool runningAsAdmin = Optimizer.IsAdministrator();
            Optimizer.SyncAutoOptimizer();
            RegisterHotkeys();
            if (runningAsAdmin)
            {
                Optimizer.InitializeAutoFlushTimer();

                if (AppSettings.Current.FpsOverlayEnabled)
                    StartFpsOverlay();
            }
            else if (AppSettings.Current.AutoFlushEnabled || AppSettings.Current.FpsOverlayEnabled)
            {
                ExceptionHandler.LogMessage(
                    "[Startup] Running in standard mode: skipped Auto-Flush, FPS Overlay, & Admin Req. features for this session.");
            }

            try
            {
                if (AppSettings.Current.DisableFSO)
                    Optimizer.ApplyFSOState(true);
                if (AppSettings.Current.GameModeAndDisableDvr)
                    Optimizer.ApplyGameModeAndDvr(true);
            }
            catch (Exception ex) { ExceptionHandler.LogError(ex); }

            lblCPUInfo.Text = $"{HardwareDetector.GetCpuName()} ({HardwareDetector.GetCpuSpeedInGhz()} GHz)";
            lblGPUInfo.Text = $"{HardwareDetector.GetGpuName()} ({HardwareDetector.GetGpuVramInGB():F0}GB VRAM)";
            lblRAMInfo.Text = $"{HardwareDetector.GetRamInfo()} ({HardwareDetector.GetTotalMemoryInGB():F0} GB)";

            _ = RefreshHardwareHealthWarnings();

            ContextMenuStrip trayMenu = new ContextMenuStrip { Renderer = new GUI.ROMenuRenderer() };
            trayMenu.Items.Add("Launch Active Game...", null, (_, _) => TrayHandler.LaunchGame(ActiveGame));
            trayMenu.Items.Add("Open", null, (_, _) => TrayHandler.ShowFormFromTray());
            trayMenu.Items.Add("Exit", null, (_, _) =>
            {
                if (sysTrayIcon != null) sysTrayIcon.Visible = false;
                Optimizer.RestoreAllRuntimeStates();
                Application.Exit();
            });

            sysTrayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Text = "Overcapped Optimizer",
                ContextMenuStrip = trayMenu,
                Visible = false
            };
            sysTrayIcon.DoubleClick += (_, _) => TrayHandler.ShowFormFromTray();

            sysTrayIcon.BalloonTipClicked += (_, _) =>
            {
                if (!string.IsNullOrWhiteSpace(_lastSavedReportPath) && File.Exists(_lastSavedReportPath))
                {
                    Process.Start(new ProcessStartInfo(_lastSavedReportPath) { UseShellExecute = true });
                }
            };
            navDashboard.Selected = true;
        }

        private void MainFrm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _pingCts?.Cancel();
            Optimizer.ActiveGameChanged -= GameChanged;
            Optimizer.ActiveGameSessionChanged -= GameSessionChanged;
            Optimizer.RestoreAllRuntimeStates();
            StopFpsOverlay();
            if (_dashboardOpacityOverlay != null && !_dashboardOpacityOverlay.IsDisposed)
            {
                _dashboardOpacityOverlay.DetachAndHide();
                _dashboardOpacityOverlay.Dispose();
                _dashboardOpacityOverlay = null;
            }
            CancelHotkeyCapture();
            _hotkeyTracker.HotkeyTriggered -= OnPassiveHotkeyTriggered;
            _hotkeyTracker.StopListening();
            _hotkeyTracker.ClearAll();
            AppSettings.Save();
        }

        private async Task RefreshHardwareHealthWarnings()
        {
            lblHealthStatus.Text = "Scanning monitor Hz, RAM speed, power plan & drive...";
            lblHealthStatus.DotColor = GUI.ROPalette.TextSecondary;

            try
            {
                var warnings = await Task.Run(HardwareDetector.RunHardwareHealthChecks);
                if (IsDisposed || !IsHandleCreated) return;

                _healthWarnings = warnings;

                if (warnings.Count == 0)
                {
                    lblHealthStatus.DotColor = GUI.ROPalette.HudGreen;
                    lblHealthStatus.Text = "All checks passed (Monitor Hz, RAM XMP/Channel, Power Plan, Storage)";
                    btnHealthDetails.Text = "Rescan";
                }
                else
                {
                    bool hasWarning = warnings.Any(w => w.Severity == HardwareHealthSeverity.Warning);
                    lblHealthStatus.DotColor = hasWarning ? Color.FromArgb(245, 158, 11) : Color.FromArgb(56, 189, 248);
                    lblHealthStatus.Text = warnings.Count == 1
                        ? $"1 issue: {warnings[0].Title}"
                        : $"{warnings.Count} issues: {warnings[0].Title} (+{warnings.Count - 1} more)";
                    btnHealthDetails.Text = "Details";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                if (!IsDisposed && IsHandleCreated)
                {
                    lblHealthStatus.DotColor = GUI.ROPalette.TextSecondary;
                    lblHealthStatus.Text = "Health scan unavailable";
                }
            }
        }

        private async void btnHealthDetails_Click(object? sender, EventArgs e)
        {
            if (_healthWarnings.Count == 0)
            {
                await RefreshHardwareHealthWarnings();
                if (_healthWarnings.Count == 0)
                {
                    MessageBox.Show(
                        "No hardware misconfigurations detected!\n\nChecked:\n• Display refresh rates vs. maximum supported Hz\n• RAM XMP / DOCP / EXPO speed & dual-channel status\n• Windows Power Saver plan check\n• System drive free space for pagefile & shader cache",
                        "Hardware Health Check",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("Detected Hardware & System Health Notes:");
            sb.AppendLine();

            for (int i = 0; i < _healthWarnings.Count; i++)
            {
                var w = _healthWarnings[i];
                sb.AppendLine($"{i + 1}. [{w.Category}] {w.Title}");
                sb.AppendLine($"   {w.Details}");
                if (i < _healthWarnings.Count - 1) sb.AppendLine();
            }

            sb.AppendLine();
            sb.Append("Re-run hardware health scan now?");

            if (MessageBox.Show(sb.ToString(), "Hardware Health Warnings", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await RefreshHardwareHealthWarnings();
            }
        }

        private async void btnDashRestorePoint_Click(object? sender, EventArgs e)
        {
            if (_isCreatingRestorePoint) return;

            if (!Optimizer.IsAdministrator())
            {
                if (MessageBox.Show(
                    "Creating a Windows System Restore Point requires Administrator privileges.\n\nRestart as Administrator now?",
                    "Administrator Required",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Optimizer.RestartAsAdministrator();
                }
                return;
            }

            if (MessageBox.Show(
                "Create a Windows System Restore Point now so you can safely roll back power, registry, or network optimizations later?",
                "One-Click System Restore Point",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            _isCreatingRestorePoint = true;
            string originalText = btnDashRestorePoint.Text;
            btnDashRestorePoint.Text = "Creating...";
            btnDashRestorePoint.Enabled = false;

            try
            {
                string stamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                var result = await Optimizer.CreateSystemRestorePointAsync($"Overcapped Optimizer ({stamp})");

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "System Restore Point Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (result.ProtectionDisabled)
                {
                    if (MessageBox.Show(
                        $"{result.Message}\n\nOpen Windows System Protection settings now?",
                        "System Protection Disabled",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo("SystemPropertiesProtection.exe") { UseShellExecute = true });
                        }
                        catch (Exception ex) { ExceptionHandler.LogError(ex); }
                    }
                }
                else
                {
                    MessageBox.Show(result.Message, "System Restore Point", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                _isCreatingRestorePoint = false;
                if (!IsDisposed)
                {
                    btnDashRestorePoint.Text = originalText;
                    btnDashRestorePoint.Enabled = true;
                }
            }
        }

        private void GameChanged(string? runningGameName)
        {
            string versionString = GetVersionString();
            if (IsDisposed || !IsHandleCreated) return;
            BeginInvoke(() =>
            {
                roThemeContainer1.SubHeader = string.IsNullOrWhiteSpace(runningGameName)
                    ? $"v{versionString}  |  IDLE: NO GAME RUNNING..."
                    : $"v{versionString}  |  ACTIVE: {runningGameName.ToUpperInvariant()}";
            });
        }

        private void GameSessionChanged(GameProfile? runningGame, int pid)
        {
            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke(() =>
            {
                if (_fpsOverlay == null || _fpsOverlay.IsDisposed || !_fpsOverlay.IsEtwRunning)
                    return;

                FinalizeAndSaveBenchmarkSession();

                if (runningGame != null && pid > 0)
                {
                    _fpsOverlay.BeginGameSession(runningGame, pid);
                }
            });
        }

        private void StartFpsOverlay()
        {
            if (_fpsOverlay == null || _fpsOverlay.IsDisposed)
            {
                _fpsOverlay = new FpsOverlay();
                _fpsOverlay.CustomPositionSaved += OnFpsOverlayCustomPositionSaved;
                _fpsOverlay.DragModeChanged += OnFpsOverlayDragModeChanged;
                _fpsOverlay.SessionCompleted += OnFpsOverlaySessionCompleted;
            }

            bool started = _fpsOverlay.StartOverlay(
                AppSettings.Current.FpsOverlayPosition,
                AppSettings.Current.FpsOverlayCustomX,
                AppSettings.Current.FpsOverlayCustomY,
                AppSettings.Current.FpsOverlayOpacity,
                AppSettings.Current.FpsOverlayShow1PercentLow);

            if (started)
            {
                Optimizer.SyncAutoOptimizer();

                if (Optimizer.ActiveDetectedGame != null && Optimizer.ActiveDetectedGamePid > 0)
                {
                    _fpsOverlay.BeginGameSession(Optimizer.ActiveDetectedGame, Optimizer.ActiveDetectedGamePid);
                }
            }
            else
            {
                MessageBox.Show(
                    "Could not start FPS tracing. Make sure Overcapped Optimizer is running as Administrator.",
                    "FPS Overlay",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void StopFpsOverlay()
        {
            if (_fpsOverlay != null)
            {
                FinalizeAndSaveBenchmarkSession();

                _fpsOverlay.CustomPositionSaved -= OnFpsOverlayCustomPositionSaved;
                _fpsOverlay.DragModeChanged -= OnFpsOverlayDragModeChanged;
                _fpsOverlay.SessionCompleted -= OnFpsOverlaySessionCompleted;
                if (!_fpsOverlay.IsDisposed)
                {
                    _fpsOverlay.StopOverlay();
                    _fpsOverlay.Close();
                    _fpsOverlay.Dispose();
                }
                _fpsOverlay = null;
            }

            btnMoveFpsOverlay.Text = "Move";
        }

        private void RefreshGameDropdown(string? selectGameId)
        {
            _isLoadingUi = true;
            try
            {
                gameDropdown.Items.Clear();
                foreach (GameProfile g in AppSettings.Current.Games)
                    gameDropdown.Items.Add(g);

                if (gameDropdown.Items.Count > 0)
                {
                    int targetIdx = 0;
                    if (!string.IsNullOrWhiteSpace(selectGameId))
                    {
                        int found = AppSettings.Current.Games.FindIndex(g => g.Id == selectGameId);
                        if (found >= 0) targetIdx = found;
                    }
                    gameDropdown.SelectedIndex = targetIdx;
                }
            }
            finally
            {
                _isLoadingUi = false;
            }

            LoadSelectedGameIntoUi();
        }

        private void gameDropdown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isLoadingUi) return;
            AppSettings.Current.ActiveGameId = ActiveGame?.Id;
            AppSettings.Save();
            LoadSelectedGameIntoUi();
        }

        private void LoadSelectedGameIntoUi()
        {
            GameProfile? game = ActiveGame;
            if (game == null)
            {
                lblSelectedGameMeta.Text = "No games loaded. Click Scan Games or Add .exe manually.";
                launchButton1.Text = "LAUNCH GAME";
                launchButton1.SubText = "NO GAME SELECTED";
                grpProfiles.SubTitle = "Select a game on Dashboard to tune its engine config";
                lblQosTitle.Text = "QoS DSCP 46 Tagging (No Active Game)";
                btnGameAutoOptimizes.Text = "Per-Game Optimizations...";
                return;
            }

            _isLoadingUi = true;
            try
            {
                var pack = JsonProfilePackManager.FindMatchingPack(game);
                string modeBadge = pack != null
                    ? $"JSON Pack ({pack.GameName})"
                    : game.Engine switch
                    {
                        GameEngineType.UnrealEngine => "Unreal Engine 4/5 Module",
                        GameEngineType.Unity => "Unity Engine Module",
                        GameEngineType.Source => "Source / Source 2 Module",
                        _ => "Universal Module"
                    };

                lblSelectedGameMeta.Text = $"Launcher: {game.Launcher}  |  Exe: {game.ExecutableName}  |  Engine: {game.Engine}  |  Mode: {modeBadge}";
                launchButton1.Text = "LAUNCH GAME";
                launchButton1.SubText = game.Name.Length > 20 ? game.Name[..20].ToUpperInvariant() + "..." : game.Name.ToUpperInvariant();

                grpProfiles.SubTitle = $"{game.Name} - {modeBadge}";
                txtConfigDir.Text = game.ConfigDirectory;

                SelectDropdownByNumericValue(fpsLimitDropdown, game.FpsLimit, 0);
                screenModeDropdown.SelectedIndex = game.ScreenMode == 0 ? 1 : 0;

                bool isUnreal = game.Engine == GameEngineType.UnrealEngine && pack == null;
                lblLumen.Visible = lumenToggle.Visible = isUnreal;
                lblFog.Visible = fogToggle.Visible = isUnreal;
                lumenToggle.Checked = game.EnableLumen;
                fogToggle.Checked = game.EnableFog;

                profileDropdown.Items.Clear();
                string[] presets = pack != null
                    ? pack.Presets.Keys.ToArray()
                    : game.Engine switch
                    {
                        GameEngineType.UnrealEngine => UnrealEngineModule.AvailablePresets,
                        GameEngineType.Unity => UnityEngineModule.AvailablePresets,
                        GameEngineType.Source => SourceEngineModule.AvailablePresets,
                        _ => new[] { "System Optimizations Only" }
                    };

                foreach (string p in presets) profileDropdown.Items.Add(p);

                int idx = profileDropdown.Items.IndexOf(game.SelectedPreset);
                profileDropdown.SelectedIndex = idx >= 0 ? idx : 0;

                UpdatePresetCardTitles(presets);
                SyncProfileCards();

                gameHighPriorityToggle.Checked = game.HighPriorityIfeo;
                gameGpuBindingToggle.Checked = game.HighPerfGpuBinding;
                lblQosTitle.Text = $"QoS DSCP 46 Tagging for {game.ExecutableName}";
                qosDscpToggle.Checked = game.QosDscpEnabled;
                btnGameAutoOptimizes.Text = game.HasCustomAutoOptimizes ? "Per-Game Optimizations (Custom)..." : "Per-Game Optimizations...";
            }
            finally
            {
                _isLoadingUi = false;
            }
        }

        private void UpdatePresetCardTitles(string[] presets)
        {
            var cards = new[] { cardProfCompetitive, cardProfBalanced, cardProfRecommended, cardProfUltra };
            for (int i = 0; i < cards.Length; i++)
            {
                if (i < presets.Length)
                {
                    cards[i].Visible = true;
                    cards[i].Title = presets[i];
                }
                else
                {
                    cards[i].Visible = false;
                }
            }
        }

        private void SelectPresetIndexIfValid(int index)
        {
            if (index >= 0 && index < profileDropdown.Items.Count)
                profileDropdown.SelectedIndex = index;
        }

        private void SyncProfileCards()
        {
            cardProfCompetitive.Selected = profileDropdown.SelectedIndex == 0;
            cardProfBalanced.Selected = profileDropdown.SelectedIndex == 1;
            cardProfRecommended.Selected = profileDropdown.SelectedIndex == 2;
            cardProfUltra.Selected = profileDropdown.SelectedIndex == 3;
        }

        private void btnScanGames_Click(object? sender, EventArgs e)
        {
            var scanned = GameScanner.ScanAllInstalledGames();
            int added = 0;

            foreach (GameProfile found in scanned)
            {
                var existing = AppSettings.Current.Games.FirstOrDefault(g => g.InstallDirectory.Equals(found.InstallDirectory, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    AppSettings.Current.Games.Add(found);
                    added++;
                }
                else if (string.IsNullOrWhiteSpace(existing.LauncherAppId) && !string.IsNullOrWhiteSpace(found.LauncherAppId))
                {
                    existing.LauncherAppId = found.LauncherAppId;
                }
            }

            AppSettings.Save();
            RefreshGameDropdown(ActiveGame?.Id);

            string breakdown = scanned.Count > 0 ? "\n\nDetected by launcher:\n" + string.Join("\n", scanned.GroupBy(g => g.Launcher).OrderByDescending(g => g.Count()).Select(g => $"• {g.Key}: {g.Count()}")) : string.Empty;

            MessageBox.Show(
                $"Scan complete! Found {scanned.Count} installed games ({added} newly added to library).{breakdown}",
                "Game Scanner",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnAddManualGame_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Game Executables (*.exe)|*.exe",
                Title = "Select a Game Executable (e.g. Game-Win64-Shipping.exe)"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            GameProfile manual = GameScanner.CreateManualProfile(ofd.FileName);
            AppSettings.Current.Games.Add(manual);
            AppSettings.Current.ActiveGameId = manual.Id;
            AppSettings.Save();
            RefreshGameDropdown(manual.Id);
        }

        private void btnRemoveGame_Click(object? sender, EventArgs e)
        {
            GameProfile? game = ActiveGame;
            if (game == null) return;

            if (MessageBox.Show($"Remove '{game.Name}' from Overcapped Optimizer? (This does not uninstall the game.)",
                "Remove Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            AppSettings.Current.Games.Remove(game);
            AppSettings.Current.ActiveGameId = AppSettings.Current.Games.FirstOrDefault()?.Id;
            AppSettings.Save();
            RefreshGameDropdown(AppSettings.Current.ActiveGameId);
        }

        private void btnBrowseConfigDir_Click(object? sender, EventArgs e)
        {
            GameProfile? game = ActiveGame;
            if (game == null) return;

            using var fbd = new FolderBrowserDialog
            {
                Description = $"Select config folder for {game.Name} (e.g. Saved\\Config\\Windows or cfg):",
                SelectedPath = Directory.Exists(txtConfigDir.Text) ? txtConfigDir.Text : game.InstallDirectory
            };

            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                game.ConfigDirectory = fbd.SelectedPath;
                txtConfigDir.Text = fbd.SelectedPath;
                AppSettings.Save();
            }
        }

        private void autoDetectBtn_Click(object? sender, EventArgs e)
        {
            double ramGb = HardwareDetector.GetTotalMemoryInGB();
            double vramGb = HardwareDetector.GetGpuVramInGB();
            double cpuGhz = HardwareDetector.GetCpuSpeedInGhz();

            string recommended;
            if (vramGb >= 12 && ramGb >= 32 && cpuGhz >= 4.0)
                recommended = "Ultra (Maximum Visuals)";
            else if (vramGb >= 8 && ramGb >= 16 && cpuGhz >= 3.0)
                recommended = "Recommended (Optimized)";
            else if (vramGb >= 4 && ramGb >= 8 && cpuGhz >= 2.5)
                recommended = FindMatchingPresetName("Balanced");
            else
                recommended = FindMatchingPresetName("Performance", "Competitive");

            int idx = profileDropdown.Items.IndexOf(recommended);
            if (idx >= 0) profileDropdown.SelectedIndex = idx;

            MessageBox.Show($"Based on your hardware ({vramGb:F0}GB VRAM, {ramGb:F0}GB RAM), we recommend: '{profileDropdown.Text}'.",
                "Profile Recommendation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string FindMatchingPresetName(params string[] prefixes)
        {
            foreach (object item in profileDropdown.Items)
            {
                string s = item.ToString() ?? string.Empty;
                if (prefixes.Any(p => s.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    return s;
            }
            return profileDropdown.Items.Count > 0 ? profileDropdown.Items[0].ToString() ?? "" : "";
        }

        private void optimizeBtn_Click(object? sender, EventArgs e)
        {
            GameProfile? game = ActiveGame;
            if (game == null)
            {
                MessageBox.Show("Select a game on the Dashboard first.", "No Game Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            game.ConfigDirectory = txtConfigDir.Text.Trim();
            game.SelectedPreset = profileDropdown.SelectedItem?.ToString() ?? "Recommended (Optimized)";
            game.FpsLimit = ParseDropdownNumericValue(fpsLimitDropdown, 0);
            game.ScreenMode = screenModeDropdown.SelectedIndex == 1 ? 0 : 1;
            game.EnableLumen = lumenToggle.Checked;
            game.EnableFog = fogToggle.Checked;
            AppSettings.Save();

            try
            {
                string summary = Optimizer.ApplyGameConfigPreset(game);
                MessageBox.Show($"{game.Name}:\n\n{summary}", "Config Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                MessageBox.Show($"Could not apply config:\n{ex.Message}", "Config Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnApplyGame_Click(object? sender, EventArgs e)
        {
            GameProfile? game = ActiveGame;
            if (game == null) return;

            bool needsAdmin = gameHighPriorityToggle.Checked || qosDscpToggle.Checked;
            if (needsAdmin && !Optimizer.IsAdministrator())
            {
                if (MessageBox.Show("CPU High Priority and QoS DSCP 46 require Administrator privileges.\n\nRestart as Administrator now?",
                    "Administrator Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Optimizer.RestartAsAdministrator();
                    return;
                }
            }

            game.HighPriorityIfeo = gameHighPriorityToggle.Checked;
            game.HighPerfGpuBinding = gameGpuBindingToggle.Checked;
            qosDscpToggle.Checked = game.QosDscpEnabled;

            AppSettings.Save();
            Optimizer.ApplyAllPerGameOptimization(game);
            Optimizer.SyncAutoOptimizer();

            MessageBox.Show($"Saved & applied optimizations for {game.Name} ({game.ExecutableName}).",
                "Settings Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveBackupBtn_Click(object? sender, EventArgs e)
        {
            GameProfile? game = ActiveGame;
            if (game == null || string.IsNullOrWhiteSpace(game.ConfigDirectory) || !Directory.Exists(game.ConfigDirectory))
            {
                MessageBox.Show("Set a valid Config Directory for the active game first.", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
            string safeName = string.Concat(game.Name.Split(Path.GetInvalidFileNameChars()));
            string destDir = Path.Combine(AppSettings.Current.BackupsPath, $"{safeName}-{stamp}");
            Directory.CreateDirectory(destDir);

            int copied = 0;
            foreach (string file in Directory.GetFiles(game.ConfigDirectory, "*.*")
                .Where(f => f.EndsWith(".ini", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".config", StringComparison.OrdinalIgnoreCase)))
            {
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), overwrite: true);
                copied++;
            }

            MessageBox.Show($"Backed up {copied} config file(s) to:\n{destDir}", "Backup Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void restoreBackupBtn_Click(object? sender, EventArgs e)
        {
            GameProfile? game = ActiveGame;
            if (game == null || string.IsNullOrWhiteSpace(game.ConfigDirectory) || !Directory.Exists(game.ConfigDirectory))
            {
                MessageBox.Show("Set a valid Config Directory for the active game first.", "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var fbd = new FolderBrowserDialog
            {
                Description = "Select a backup folder to restore config files from:",
                SelectedPath = Directory.Exists(AppSettings.Current.BackupsPath) ? AppSettings.Current.BackupsPath : Application.StartupPath
            };

            if (fbd.ShowDialog() != DialogResult.OK) return;

            int restored = 0;
            foreach (string file in Directory.GetFiles(fbd.SelectedPath, "*.*")
                .Where(f => f.EndsWith(".ini", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase)))
            {
                File.Copy(file, Path.Combine(game.ConfigDirectory, Path.GetFileName(file)), overwrite: true);
                restored++;
            }

            MessageBox.Show($"Restored {restored} config file(s) for {game.Name}.", "Backup Restored", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnOpenPacksFolder_Click(object? sender, EventArgs e)
        {
            Directory.CreateDirectory(AppSettings.PacksDirectory);
            Process.Start(new ProcessStartInfo(AppSettings.PacksDirectory) { UseShellExecute = true });
        }

        private void btnGameAutoOptimizes_Click(object? sender, EventArgs e)
        {
            GameProfile? game = ActiveGame;
            if (game == null)
            {
                MessageBox.Show("Select a game on the Dashboard first.", "No Game Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new PerGameOptimizationsDlg(game);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            game.MonitorGame = dlg.ResultMonitorGame;
            game.PCoresOverride = dlg.ResultPCoresOverride;
            game.AutoPowerPlanOverride = dlg.ResultAutoPowerPlanOverride;
            game.ThrottleAppsOverride = dlg.ResultThrottleAppsOverride;
            game.LaunchArguments = dlg.ResultLaunchArguments;

            AppSettings.Save();
            Optimizer.SyncAutoOptimizer();

            btnGameAutoOptimizes.Text = game.HasCustomAutoOptimizes
                ? "Per-Game Optimizations (Custom)..."
                : "Per-Game Optimizations...";
        }

        private void btnPackBuilder_Click(object? sender, EventArgs e)
        {
            using var builder = new ProfilePackBuilder(ActiveGame);
            builder.ShowDialog(this);

            if (builder.PacksChanged)
            {
                JsonProfilePackManager.LoadAllPacks();
                LoadSelectedGameIntoUi();
            }
        }

        private void btnEditThrottleList_Click(object? sender, EventArgs e)
        {
            using var dlg = new ThrottleListDlg(AppSettings.Current.BackgroundAppsToThrottle);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            AppSettings.Current.BackgroundAppsToThrottle = dlg.UpdatedApps.ToList();
            AppSettings.Save();
            Optimizer.SyncAutoOptimizer();

            MessageBox.Show(
                $"Saved {AppSettings.Current.BackgroundAppsToThrottle.Count} background app(s) to the throttle list.",
                "Background App Throttle List",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void Update1PercentLowButtonVisual()
        {
            btnToggle1PercentLow.Text = AppSettings.Current.FpsOverlayShow1PercentLow ? "1% Low: ON" : "1% Low: OFF";
        }

        private void btnToggle1PercentLow_Click(object? sender, EventArgs e)
        {
            AppSettings.Current.FpsOverlayShow1PercentLow = !AppSettings.Current.FpsOverlayShow1PercentLow;
            AppSettings.Save();
            Update1PercentLowButtonVisual();

            if (_fpsOverlay != null && !_fpsOverlay.IsDisposed)
            {
                _fpsOverlay.SetShow1PercentLow(AppSettings.Current.FpsOverlayShow1PercentLow);
            }
        }

        private void btnOpenBenchmarks_Click(object? sender, EventArgs e)
        {
            Directory.CreateDirectory(AppSettings.BenchmarksDirectory);
            Process.Start(new ProcessStartInfo(AppSettings.BenchmarksDirectory) { UseShellExecute = true });
        }

        private void openLatestBenchmarkToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            string? latest = ReportGenerator.GetLatestReportPath();
            if (string.IsNullOrWhiteSpace(latest) || !File.Exists(latest))
            {
                MessageBox.Show(
                    "No session benchmark reports have been generated yet.\n\nEnable the In-Game FPS Overlay (Admin), launch a monitored game, and an HTML report will be generated automatically when you exit.",
                    "Session Benchmark Reports",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Process.Start(new ProcessStartInfo(latest) { UseShellExecute = true });
        }

        private void saveAdvancedCfgBtn_Click(object? sender, EventArgs e)
        {
            bool isAdmin = Optimizer.IsAdministrator();

            if (autoFlushChk.Checked && !isAdmin)
            {
                if (MessageBox.Show("Auto-Flush requires Administrator privileges.\n\nRestart as Administrator now?",
                    "Administrator Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    AppSettings.Current.AutoFlushEnabled = true;
                    AppSettings.Save();
                    Optimizer.RestartAsAdministrator();
                    return;
                }
                autoFlushChk.Checked = false;
            }

            AppSettings.Current.AutoPowerPlan = autoPowerPlanToggle.Checked;
            AppSettings.Current.GameModeAndDisableDvr = gameModeDvrToggle.Checked;
            AppSettings.Current.DisableFSO = disabledFSO.Checked;
            AppSettings.Current.ThrottleBackgroundApps = throttleAppsToggle.Checked;
            if (isAdmin)
            {
                AppSettings.Current.AutoFlushEnabled = autoFlushChk.Checked;
            }
            AppSettings.Current.SmartFlushEnabled = smartFlushToggle.Checked;
            AppSettings.Current.SmartFlushThresholdGB = (int)smartFlushThreshold.Value;
            AppSettings.Current.AutoFlushInterval = (int)autoFlushinterval.Value;
            AppSettings.Current.AutoFlushUnit = autoFlushMinHour.Text;
            AppSettings.Current.AutoFlushSfx = autoFlushSound.Checked;
            AppSettings.Current.PCoresOnly = pCoresToggle.Checked;

            AppSettings.Save();

            Optimizer.ApplyFSOState(AppSettings.Current.DisableFSO);
            Optimizer.ApplyGameModeAndDvr(AppSettings.Current.GameModeAndDisableDvr);
            Optimizer.SyncAutoOptimizer();
            if (isAdmin)
            {
                Optimizer.InitializeAutoFlushTimer();
            }

            MessageBox.Show("Global settings saved and applied!", "Overcapped Optimizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void flushRamToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (!Optimizer.IsAdministrator())
            {
                if (MessageBox.Show("Flushing the Windows Standby List requires Administrator privileges.\n\nRestart as Administrator now?",
                    "Administrator Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Optimizer.RestartAsAdministrator();
                }
                return;
            }

            Optimizer.FlushStandbyList(silent: false);
        }

        private void clearShaderCacheBtn_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show(
                "This clears DirectX, NVIDIA, and AMD GPU shader caches.\n\nNote: Your first 5 minutes in-game will rebuild shaders.\n\nProceed?",
                "Clear Shader Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var (deleted, freedMb, skipped) = Optimizer.ClearShaderCaches();
            MessageBox.Show(
                $"Shader cache cleanup complete!\n\n- Files deleted: {deleted}\n- Space freed: {freedMb:F2} MB\n- Active/locked files skipped: {skipped}",
                "Shader Cache Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnOpenGameFolder_Click(object? sender, EventArgs e)
        {
            if (ActiveGame == null || !Directory.Exists(ActiveGame.InstallDirectory))
            {
                MessageBox.Show("No valid active game folder selected.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Process.Start(new ProcessStartInfo(ActiveGame.InstallDirectory) { UseShellExecute = true });
        }

        private void btnApplyNetworkOptimzations_Click(object? sender, EventArgs e)
        {
            if (!Optimizer.IsAdministrator())
            {
                if (MessageBox.Show("NIC power optimizations, Delivery Optimization policy, and QoS DSCP policies require Administrator privileges.\n\nRestart as Administrator now?",
                    "Administrator Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Optimizer.RestartAsAdministrator();
                }
                return;
            }

            try
            {
                int adapters = Optimizer.ApplyNicPowerOptimization(disableNicPowerToggle.Checked);
                AppSettings.Current.DisableNicPowerSaving = disableNicPowerToggle.Checked;

                Optimizer.ApplyDeliveryOptimizationState(deliveryOptToggle.Checked);
                AppSettings.Current.DisableDeliveryOptimization = deliveryOptToggle.Checked;

                string qosNote = "No active game selected";
                if (ActiveGame != null)
                {
                    ActiveGame.QosDscpEnabled = qosDscpToggle.Checked;
                    Optimizer.ApplyQosDscpPolicy(ActiveGame.ExecutableName, qosDscpToggle.Checked);
                    qosNote = $"{ActiveGame.ExecutableName}: {(qosDscpToggle.Checked ? "Enabled (DSCP 46)" : "Removed")}";
                }

                AppSettings.Save();
                MessageBox.Show(
                    $"Network optimizations applied!\n\n- Active Ethernet NICs updated: {adapters}\n- Delivery Optimization P2P: {(deliveryOptToggle.Checked ? "Disabled (HTTP Only)" : "Windows Default")}\n- QoS Policy: {qosNote}",
                    "Network Optimizations Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                MessageBox.Show($"Failed to apply network optimizations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFlushDns_Click(object? sender, EventArgs e)
        {
            bool ok = Optimizer.FlushDnsCache(out string output);
            MessageBox.Show(string.IsNullOrWhiteSpace(output) ? (ok ? "DNS cache flushed." : "DNS flush failed.") : output,
                "Flush DNS", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private async void btnRunPingTest_Click(object? sender, EventArgs e)
        {
            if (_pingCts != null) { _pingCts.Cancel(); return; }

            string target = txtServerIp.Text.Trim();
            if (string.IsNullOrWhiteSpace(target)) return;

            AppSettings.Current.LastPingServer = target;
            AppSettings.Save();

            int count = (int)pingCountNumeric.Value;
            _pingCts = new CancellationTokenSource();
            btnRunPingTest.Text = "Stop";
            lblPingAvg.Text = $"Testing {target} (0/{count})...";
            lblPingJitter.Text = "Measuring...";
            lblPacketLoss.Text = "Measuring...";

            var progress = new Progress<PingTestProgress>(p =>
            {
                lblPingAvg.Text = p.Sent > p.Lost
                    ? $"{p.AvgPingMs:F1} ms  (Min: {p.MinPingMs:F0} ms | Max: {p.MaxPingMs:F0} ms)  [{p.ProbeMode}]  {p.Completed}/{p.Total}"
                    : $"No reply yet [{p.ProbeMode}]  {p.Completed}/{p.Total}";

                lblPingJitter.Text = p.Sent - p.Lost >= 2
                    ? $"{p.JitterMs:F1} ms  ({(p.JitterMs < 5 ? "Rock Solid" : p.JitterMs < 15 ? "Good" : "Spiky / Unstable")})"
                    : "Calculating...";

                lblPacketLoss.Text = $"{p.PacketLossPercent:F1}%  ({p.Lost} lost of {p.Sent} sent)";
                lblPacketLoss.DotColor = p.PacketLossPercent == 0 ? GUI.ROPalette.HudGreen : GUI.ROPalette.AccentCrimson;
            });

            try
            {
                await Optimizer.RunPingTest(target, count, progress, _pingCts.Token);
            }
            catch (OperationCanceledException) { lblPingAvg.Text += "  (Stopped)"; }
            catch (Exception ex) { lblPingAvg.Text = $"Error: {ex.Message}"; }
            finally
            {
                _pingCts.Dispose();
                _pingCts = null;
                btnRunPingTest.Text = "Run Test";
            }
        }

        private static void SelectDropdownByNumericValue(ComboBox combo, int value, int fallbackIndex)
        {
            string target = value == 0 ? "Unlimited (0)" : value.ToString(CultureInfo.InvariantCulture);
            int idx = combo.Items.IndexOf(target);
            combo.SelectedIndex = idx >= 0 ? idx : Math.Min(fallbackIndex, combo.Items.Count - 1);
        }

        private static int ParseDropdownNumericValue(ComboBox combo, int fallback)
        {
            string text = combo.SelectedItem?.ToString() ?? string.Empty;
            if (text.StartsWith("Unlimited", StringComparison.OrdinalIgnoreCase)) return 0;
            return int.TryParse(text, out int parsed) ? parsed : fallback;
        }

        private void updateToggle_CheckedChanged(object? sender, EventArgs e)
        {
            if (_isLoadingUi) return;
            AppSettings.Current.CheckForUpdates = updateToggle.Checked;
            AppSettings.Save();
            if (updateToggle.Checked) _ = Updates.CheckForUpdates();
        }

        private void startAsAdminToggle_CheckedChanged(object? sender, EventArgs e)
        {
            if (_isLoadingUi) return;

            AppSettings.Current.StartAsAdmin = startAsAdminToggle.Checked;
            AppSettings.Save();

            if (startAsAdminToggle.Checked && !Optimizer.IsAdministrator())
            {
                if (MessageBox.Show(
                    "Start as Admin is now enabled for future launches.\n\nRestart as Administrator now?",
                    "Start as Admin",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Optimizer.RestartAsAdministrator();
                }
            }
        }

        private void launchButton1_Click(object? sender, EventArgs e)
        {
            TrayHandler.LaunchGame(ActiveGame);
        }

        private void minimizeToTrayToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TrayHandler.HideFormToTray();
        }

        private void exitToolStripMenuItem1_Click(object? sender, EventArgs e)
        {
            StopFpsOverlay();
            Close();
        }

        private void supportToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Join our Discord server for support and community profile packs?",
                "Void Tech Studios Discord", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                Process.Start(new ProcessStartInfo("https://discord.gg/tfwf9Qr7rG") { UseShellExecute = true });
            }
        }

        private void openLogFileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            string path = ExceptionHandler.GetLogFilePath();
            string target = File.Exists(path) ? path : Path.GetDirectoryName(path) ?? Application.StartupPath;
            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
        }

        private void openLogFilePathToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            string dir = Path.GetDirectoryName(ExceptionHandler.GetLogFilePath()) ?? Application.StartupPath;
            Process.Start(new ProcessStartInfo(dir) { UseShellExecute = true });
        }

        private void openSettingsFilePathToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            Directory.CreateDirectory(AppSettings.DataDirectory);
            Process.Start(new ProcessStartInfo(AppSettings.DataDirectory) { UseShellExecute = true });
        }

        private void _lnkWebsite_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://voidtechstudios.us/") { UseShellExecute = true });
        }

        private void FinalizeAndSaveBenchmarkSession()
        {
            if (_fpsOverlay == null || _fpsOverlay.IsDisposed) return;

            FpsSessionSummary? summary = _fpsOverlay.EndGameSession();
            if (summary != null)
                SaveBenchmarkReport(summary);
        }

        private void OnFpsOverlaySessionCompleted(FpsSessionSummary summary)
        {
            SaveBenchmarkReport(summary);
        }

        private void SaveBenchmarkReport(FpsSessionSummary summary)
        {
            if (!AppSettings.Current.AutoSaveSessionBenchmarks) return;

            Task.Run(() =>
            {
                try
                {
                    string reportPath = ReportGenerator.GenerateHtmlReport(summary);
                    _lastSavedReportPath = reportPath;

                    ExceptionHandler.LogMessage(
                        $"[Benchmark] Saved {summary.GameName} report (Avg: {summary.AvgFps:F1} FPS, 1% Low: {summary.OnePercentLowFps:F1} FPS) -> {reportPath}");

                    if (!IsDisposed && IsHandleCreated)
                    {
                        BeginInvoke(() =>
                        {
                            if (sysTrayIcon != null && sysTrayIcon.Visible)
                            {
                                sysTrayIcon.BalloonTipTitle = $"Benchmark Saved: {summary.GameName}";
                                sysTrayIcon.BalloonTipText = $"Avg: {summary.AvgFps:F1} FPS | 1% Low: {summary.OnePercentLowFps:F1} FPS (Click to open)";
                                sysTrayIcon.ShowBalloonTip(5000);
                            }
                            else
                            {
                                var open = MessageBox.Show(
                                    $"Session benchmark report generated and saved for {summary.GameName}!\n\n" +
                                    $"• Average FPS: {summary.AvgFps:F1}\n" +
                                    $"• 1% Low FPS: {summary.OnePercentLowFps:F1}\n" +
                                    $"• Saved to: {reportPath}\n\n" +
                                    "Open the report in your browser now?",
                                    "Benchmark Report Saved",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information);

                                if (open == DialogResult.Yes && File.Exists(reportPath))
                                {
                                    Process.Start(new ProcessStartInfo(reportPath) { UseShellExecute = true });
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                }
            });
        }

        private void OnFpsOverlayCustomPositionSaved(int x, int y)
        {
            AppSettings.Current.FpsOverlayCustomX = x;
            AppSettings.Current.FpsOverlayCustomY = y;
            AppSettings.Current.FpsOverlayPosition = "Custom";
            AppSettings.Save();

            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke(() =>
            {
                _isLoadingUi = true;
                int customIdx = fpsPositionDropdown.Items.IndexOf("Custom");
                if (customIdx >= 0) fpsPositionDropdown.SelectedIndex = customIdx;
                _isLoadingUi = false;

                if (_fpsOverlay == null || !_fpsOverlay.IsDragMode)
                    btnMoveFpsOverlay.Text = "Move";
            });
        }

        private void fpsOverlayToggle_CheckedChanged(object? sender, EventArgs e)
        {
            if (_isLoadingUi) return;

            if (fpsOverlayToggle.Checked)
            {
                if (!Optimizer.IsAdministrator())
                {
                    _isLoadingUi = true;
                    fpsOverlayToggle.Checked = false;
                    _isLoadingUi = false;

                    if (MessageBox.Show(
                        "The zero-injection ETW FPS Overlay requires Administrator privileges to read Windows DXGI frame events.\n\nRestart as Administrator now?",
                        "Administrator Required",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        AppSettings.Current.FpsOverlayEnabled = true;
                        AppSettings.Save();
                        Optimizer.RestartAsAdministrator();
                    }
                    return;
                }

                AppSettings.Current.FpsOverlayEnabled = true;
                AppSettings.Save();
                StartFpsOverlay();
            }
            else
            {
                AppSettings.Current.FpsOverlayEnabled = false;
                AppSettings.Save();
                StopFpsOverlay();
                Optimizer.SyncAutoOptimizer();
            }
        }

        private void fpsPositionDropdown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isLoadingUi) return;

            string selectedPos = fpsPositionDropdown.SelectedItem?.ToString() ?? "Top Left";
            AppSettings.Current.FpsOverlayPosition = selectedPos;
            AppSettings.Save();

            if (_fpsOverlay != null && !_fpsOverlay.IsDisposed)
            {
                _fpsOverlay.ApplyPosition(
                    selectedPos,
                    AppSettings.Current.FpsOverlayCustomX,
                    AppSettings.Current.FpsOverlayCustomY);
            }
        }

        private void btnMoveFpsOverlay_Click(object? sender, EventArgs e)
        {
            if (_fpsOverlay == null || _fpsOverlay.IsDisposed || !fpsOverlayToggle.Checked)
            {
                MessageBox.Show(
                    "Enable the In-Game FPS Overlay toggle first to position it.",
                    "FPS Overlay",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!_fpsOverlay.IsDragMode)
            {
                _fpsOverlay.SetDragMode(true);
                btnMoveFpsOverlay.Text = "Lock";
            }
            else
            {
                AppSettings.Current.FpsOverlayCustomX = _fpsOverlay.Left;
                AppSettings.Current.FpsOverlayCustomY = _fpsOverlay.Top;
                AppSettings.Current.FpsOverlayPosition = "Custom";
                AppSettings.Save();

                _fpsOverlay.SetDragMode(false);
                btnMoveFpsOverlay.Text = "Move";

                _isLoadingUi = true;
                int customIdx = fpsPositionDropdown.Items.IndexOf("Custom");
                if (customIdx >= 0) fpsPositionDropdown.SelectedIndex = customIdx;
                _isLoadingUi = false;
            }
        }

        private void OnFpsOverlayDragModeChanged(bool isDragMode)
        {
            if (IsDisposed || !IsHandleCreated) return;
            BeginInvoke(() => btnMoveFpsOverlay.Text = isDragMode ? "Lock" : "Move");
        }

        private void fpsOpacityDropdown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isLoadingUi) return;

            string raw = (fpsOpacityDropdown.SelectedItem?.ToString() ?? "75%").TrimEnd('%');
            if (int.TryParse(raw, out int percent))
            {
                AppSettings.Current.FpsOverlayOpacity = Math.Clamp(percent, 5, 100);
                AppSettings.Save();

                if (_fpsOverlay != null && !_fpsOverlay.IsDisposed)
                {
                    _fpsOverlay.ApplyOpacity(AppSettings.Current.FpsOverlayOpacity);
                }
            }
        }

        private void LoadHotkeyFieldsFromSettings()
        {
            txtHkFpsOverlay.Text = FormatDisplayHotkey(AppSettings.Current.HotkeyToggleFpsOverlay);
            txtHk1PercentLow.Text = FormatDisplayHotkey(AppSettings.Current.HotkeyToggle1PercentLow);
            txtHkFlushRam.Text = FormatDisplayHotkey(AppSettings.Current.HotkeyFlushRam);
            txtHkToggleWindow.Text = FormatDisplayHotkey(AppSettings.Current.HotkeyToggleWindow);
        }

        private static string FormatDisplayHotkey(string? raw) =>
            string.IsNullOrWhiteSpace(raw) ? HotkeyUnboundLabel : raw.Trim();

        private static string ReadHotkeyFromBox(GUI.ROTextBox box)
        {
            string val = box.Text.Trim();
            return val.Equals(HotkeyUnboundLabel, StringComparison.OrdinalIgnoreCase) ||
                   val.Equals("Press keys...", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : val;
        }

        private void RegisterHotkeys()
        {
            _hotkeyTracker.ClearAll();

            if (!AppSettings.Current.HotkeysEnabled)
            {
                _hotkeyTracker.StopListening();
                return;
            }

            int boundCount = 0;
            boundCount += TryRegisterHotkey("ToggleFpsOverlay", AppSettings.Current.HotkeyToggleFpsOverlay);
            boundCount += TryRegisterHotkey("Toggle1PercentLow", AppSettings.Current.HotkeyToggle1PercentLow);
            boundCount += TryRegisterHotkey("FlushRam", AppSettings.Current.HotkeyFlushRam);
            boundCount += TryRegisterHotkey("ToggleWindow", AppSettings.Current.HotkeyToggleWindow);

            if (boundCount > 0)
                _hotkeyTracker.StartListening();
            else
                _hotkeyTracker.StopListening();
        }

        private int TryRegisterHotkey(string name, string? keyString)
        {
            if (HotkeyFilter.TryParseKeys(keyString, out Keys parsed))
            {
                _hotkeyTracker.Register(name, parsed);
                return 1;
            }
            return 0;
        }

        private void BeginHotkeyCapture(GUI.ROTextBox targetBox, GUI.ROModernButton targetBtn)
        {
            bool sameButton = _capturingButton == targetBtn;
            CancelHotkeyCapture();
            if (sameButton) return;

            _capturingTextBox = targetBox;
            _capturingButton = targetBtn;
            _capturePreviousText = targetBox.Text;

            targetBox.Text = "Press keys...";
            targetBtn.Text = "...";

            _activeCaptureFilter = new HotkeyFilter(OnHotkeyCaptured);
            Application.AddMessageFilter(_activeCaptureFilter);
        }

        private void OnHotkeyCaptured(Keys captured)
        {
            var box = _capturingTextBox;
            var btn = _capturingButton;
            string prev = _capturePreviousText;

            CleanupCaptureFilter();

            if (box == null || btn == null) return;

            if (captured == Keys.Escape)
            {
                box.Text = prev;
                btn.Text = "Bind";
                return;
            }

            string formatted = HotkeyFilter.FormatKeys(captured);
            box.Text = string.IsNullOrWhiteSpace(formatted) ? prev : formatted;
            btn.Text = "Bind";
        }

        private void CancelHotkeyCapture()
        {
            if (_capturingTextBox != null)
                _capturingTextBox.Text = _capturePreviousText;
            if (_capturingButton != null)
                _capturingButton.Text = "Bind";

            CleanupCaptureFilter();
        }

        private void CleanupCaptureFilter()
        {
            if (_activeCaptureFilter != null)
            {
                Application.RemoveMessageFilter(_activeCaptureFilter);
                _activeCaptureFilter = null;
            }

            HotkeyFilter.IsCapturingHotkey = false;
            _capturingTextBox = null;
            _capturingButton = null;
            _capturePreviousText = string.Empty;
        }

        private void OnPassiveHotkeyTriggered(string hotkeyName)
        {
            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke(() =>
            {
                if (HotkeyFilter.IsCapturingHotkey) return;

                switch (hotkeyName)
                {
                    case "ToggleFpsOverlay":
                        if (_fpsOverlay != null && !_fpsOverlay.IsDisposed && _fpsOverlay.IsEtwRunning)
                        {
                            _fpsOverlay.ToggleUserHidden();
                        }
                        else if (Optimizer.IsAdministrator() && !fpsOverlayToggle.Checked)
                        {
                            fpsOverlayToggle.Checked = true;
                        }
                        break;

                    case "Toggle1PercentLow":
                        btnToggle1PercentLow_Click(this, EventArgs.Empty);
                        break;

                    case "FlushRam":
                        if (Optimizer.IsAdministrator())
                        {
                            Task.Run(() => Optimizer.FlushStandbyList(silent: true));
                        }
                        break;

                    case "ToggleWindow":
                        if (Visible && WindowState != FormWindowState.Minimized)
                            TrayHandler.HideFormToTray();
                        else
                            ShowFormAsHotkeyOverlay();
                        break;
                }
            });
        }

        private void btnBindHkFps_Click(object? sender, EventArgs e)
        {
            BeginHotkeyCapture(txtHkFpsOverlay, btnBindHkFps);
        }

        private void btnBindHk1Low_Click(object? sender, EventArgs e)
        {
            BeginHotkeyCapture(txtHk1PercentLow, btnBindHk1Low);
        }

        private void btnBindHkFlush_Click(object? sender, EventArgs e)
        {
            BeginHotkeyCapture(txtHkFlushRam, btnBindHkFlush);
        }

        private void btnBindHkWindow_Click(object? sender, EventArgs e)
        {
            BeginHotkeyCapture(txtHkToggleWindow, btnBindHkWindow);
        }

        private void btnClearHkFps_Click(object? sender, EventArgs e) 
        { 
            CancelHotkeyCapture(); txtHkFpsOverlay.Text = HotkeyUnboundLabel; 
        }

        private void btnClearHk1Low_Click(object? sender, EventArgs e)
        { 
            CancelHotkeyCapture(); txtHk1PercentLow.Text = HotkeyUnboundLabel; 
        }
        private void btnClearHkFlush_Click(object? sender, EventArgs e)
        { 
            CancelHotkeyCapture(); txtHkFlushRam.Text = HotkeyUnboundLabel; 
        }
        private void btnClearHkWindow_Click(object? sender, EventArgs e)
        { 
            CancelHotkeyCapture(); txtHkToggleWindow.Text = HotkeyUnboundLabel;
        }

        private void btnSaveHotkeys_Click(object? sender, EventArgs e)
        {
            CancelHotkeyCapture();

            string hkFps = ReadHotkeyFromBox(txtHkFpsOverlay);
            string hk1Low = ReadHotkeyFromBox(txtHk1PercentLow);
            string hkFlush = ReadHotkeyFromBox(txtHkFlushRam);
            string hkWin = ReadHotkeyFromBox(txtHkToggleWindow);

            var nonEmpty = new[] { hkFps, hk1Low, hkFlush, hkWin }
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (nonEmpty.Count != nonEmpty.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            {
                MessageBox.Show(
                    "Two or more actions are assigned to the same hotkey. Clear or rebind the duplicate first.",
                    "Duplicate Hotkey",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            AppSettings.Current.HotkeysEnabled = hotkeysEnabledToggle.Checked;
            AppSettings.Current.HotkeyToggleFpsOverlay = hkFps;
            AppSettings.Current.HotkeyToggle1PercentLow = hk1Low;
            AppSettings.Current.HotkeyFlushRam = hkFlush;
            AppSettings.Current.HotkeyToggleWindow = hkWin;
            AppSettings.Save();

            RegisterHotkeys();

            MessageBox.Show("Hotkey settings saved and applied!", "Global Hotkeys", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnResetHotkeys_Click(object? sender, EventArgs e)
        {
            CancelHotkeyCapture();

            hotkeysEnabledToggle.Checked = true;
            txtHkFpsOverlay.Text = "Ctrl+Shift+O";
            txtHk1PercentLow.Text = "Ctrl+Shift+L";
            txtHkFlushRam.Text = "Ctrl+Shift+R";
            txtHkToggleWindow.Text = "Ctrl+Shift+M";
        }

        public void ShowFormAsHotkeyOverlay()
        {
            int opacityPercent = Math.Clamp(AppSettings.Current.DashboardOverlayOpacity, 20, 100);

            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            if (sysTrayIcon != null)
                sysTrayIcon.Visible = false;

            TopMost = true;
            Opacity = opacityPercent / 100.0;
            BringToFront();
            Activate();

            if (_dashboardOpacityOverlay == null || _dashboardOpacityOverlay.IsDisposed)
            {
                _dashboardOpacityOverlay = new DashboardOpacityOverlay();
                _dashboardOpacityOverlay.OpacityValueChanged += pct =>
                {
                    if (!IsDisposed)
                        Opacity = Math.Clamp(pct, 20, 100) / 100.0;
                };
                _dashboardOpacityOverlay.OpacityCommitted += pct =>
                {
                    AppSettings.Current.DashboardOverlayOpacity = Math.Clamp(pct, 20, 100);
                    AppSettings.Save();
                };
            }

            _dashboardOpacityOverlay.ShowFor(this, opacityPercent);
        }

        public void ExitHotkeyOverlayMode()
        {
            if (_dashboardOpacityOverlay != null && !_dashboardOpacityOverlay.IsDisposed)
                _dashboardOpacityOverlay.DetachAndHide();

            TopMost = false;
            Opacity = 1.0;
        }
    }
}