using System.Drawing;
using System.Windows.Forms;
using Overcapped_Optimizer.GUI;

namespace Overcapped_Optimizer
{
    partial class MainFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            roThemeContainer1 = new ROThemeContainer();
            pnlContent = new Panel();
            pnlAbout = new Panel();
            grpLinks = new ROGroupBox();
            cardPanel = new Panel();
            roLabel2 = new ROLabel();
            lblDescription = new Label();
            linkWebsite = new AboutLinkTile();
            aboutLinkTile1 = new AboutLinkTile();
            linkDocs = new AboutLinkTile();
            linkDiscord = new AboutLinkTile();
            linkGitHub = new AboutLinkTile();
            grpDiagnostics = new ROGroupBox();
            btnOpenLog = new ROModernButton();
            btnOpenLogDir = new ROModernButton();
            btnOpenSettingsDir = new ROModernButton();
            pnlDashboard = new Panel();
            grpGameLibrary = new ROGroupBox();
            gameDropdown = new ROComboBox();
            btnScanGames = new ROModernButton();
            btnAddManualGame = new ROModernButton();
            btnRemoveGame = new ROModernButton();
            lblSelectedGameMeta = new Label();
            grpHardware = new ROGroupBox();
            lblCPUInfo = new ROStatPill();
            lblGPUInfo = new ROStatPill();
            lblRAMInfo = new ROStatPill();
            lblHealthStatus = new ROStatPill();
            btnHealthDetails = new ROModernButton();
            grpQuickActions = new ROGroupBox();
            linkDonate = new AboutLinkTile();
            cardQuickFlush = new ROActionCard();
            cardQuickShaderCache = new ROActionCard();
            btnDashRestorePoint = new ROModernButton();
            btnDashRevertOptimizations = new ROModernButton();
            btnDashTray = new ROModernButton();
            pnlHotkeys = new Panel();
            grpHotkeys = new ROGroupBox();
            lblHotkeysEnableTitle = new Label();
            lblHotkeysEnableDesc = new Label();
            hotkeysEnabledToggle = new ROToggle();
            sepHk1 = new ROSeparator();
            lblHkFpsTitle = new Label();
            lblHkFpsDesc = new Label();
            txtHkFpsOverlay = new ROTextBox();
            btnBindHkFps = new ROModernButton();
            btnClearHkFps = new ROModernButton();
            sepHk2 = new ROSeparator();
            lblHk1LowTitle = new Label();
            lblHk1LowDesc = new Label();
            txtHk1PercentLow = new ROTextBox();
            btnBindHk1Low = new ROModernButton();
            btnClearHk1Low = new ROModernButton();
            sepHk3 = new ROSeparator();
            lblHkFlushTitle = new Label();
            lblHkFlushDesc = new Label();
            txtHkFlushRam = new ROTextBox();
            btnBindHkFlush = new ROModernButton();
            btnClearHkFlush = new ROModernButton();
            sepHk4 = new ROSeparator();
            lblHkWindowTitle = new Label();
            lblHkWindowDesc = new Label();
            txtHkToggleWindow = new ROTextBox();
            btnBindHkWindow = new ROModernButton();
            btnClearHkWindow = new ROModernButton();
            lblHkHint = new Label();
            btnSaveHotkeys = new ROModernButton();
            btnResetHotkeys = new ROModernButton();
            pnlTools = new Panel();
            grpSystemOptimzations = new ROGroupBox();
            lblPCoresTitle = new Label();
            lblPCoresDesc = new Label();
            pCoresToggle = new ROToggle();
            lblPowerPlanTitle = new Label();
            lblPowerPlanDesc = new Label();
            autoPowerPlanToggle = new ROToggle();
            sepTool1 = new ROSeparator();
            lblGameModeTitle = new Label();
            lblGameModeDesc = new Label();
            gameModeDvrToggle = new ROToggle();
            lblFsoTitle = new Label();
            lblFsoDesc = new Label();
            disabledFSO = new ROToggle();
            sepTool2 = new ROSeparator();
            lblHagsTitle = new Label();
            lblHagsDesc = new Label();
            hagsToggle = new ROToggle();
            lblTelemetryTitle = new Label();
            lblTelemetryDesc = new Label();
            telemetryToggle = new ROToggle();
            sepTool3 = new ROSeparator();
            lblPauseSearchTitle = new Label();
            lblPauseSearchDesc = new Label();
            pauseSearchToggle = new ROToggle();
            sepTool4 = new ROSeparator();
            lblThrottleTitle = new Label();
            lblThrottleDesc = new Label();
            btnEditThrottleList = new ROModernButton();
            throttleAppsToggle = new ROToggle();
            sepTool5 = new ROSeparator();
            lblFpsOverlayTitle = new Label();
            lblFpsOverlayDesc = new Label();
            btnToggle1PercentLow = new ROModernButton();
            fpsPositionDropdown = new ROComboBox();
            btnMoveFpsOverlay = new ROModernButton();
            fpsOverlayToggle = new ROToggle();
            fpsOpacityDropdown = new ROComboBox();
            grpCacheCleaners = new ROGroupBox();
            clearShaderCacheBtn = new ROModernButton();
            btnOpenGameFolder = new ROModernButton();
            btnOpenBenchmarks = new ROModernButton();
            saveAdvancedCfgBtn = new ROModernButton();
            pnlProfiles = new Panel();
            grpProfiles = new ROGroupBox();
            cardProfCompetitive = new ROActionCard();
            cardProfBalanced = new ROActionCard();
            cardProfRecommended = new ROActionCard();
            cardProfUltra = new ROActionCard();
            txtConfigDir = new ROTextBox();
            btnBrowseConfigDir = new ROModernButton();
            lblFpsLimit = new Label();
            fpsLimitDropdown = new ROComboBox();
            lblScreenMode = new Label();
            screenModeDropdown = new ROComboBox();
            lblLumen = new Label();
            lumenToggle = new ROToggle();
            lblFog = new Label();
            fogToggle = new ROToggle();
            profileDropdown = new ROComboBox();
            autoDetectBtn = new ROModernButton();
            optimizeBtn = new ROModernButton();
            grpPerGameOptimzations = new ROGroupBox();
            lblGameHighPrio = new Label();
            gameHighPriorityToggle = new ROToggle();
            lblGameGpuBind = new Label();
            gameGpuBindingToggle = new ROToggle();
            btnApplyGame1 = new ROModernButton();
            saveBackupBtn = new ROModernButton();
            restoreBackupBtn = new ROModernButton();
            btnOpenPacksFolder = new ROModernButton();
            btnGameAutoOptimizes = new ROModernButton();
            btnPackBuilder = new ROModernButton();
            pnlNetwork = new Panel();
            grpNetworkOptimzations = new ROGroupBox();
            lblNicPowerTitle = new Label();
            lblNicPowerDesc = new Label();
            disableNicPowerToggle = new ROToggle();
            sepNet1 = new ROSeparator();
            lblIntModTitle = new Label();
            lblIntModDesc = new Label();
            disableIntModToggle = new ROToggle();
            lblRssTitle = new Label();
            lblRssDesc = new Label();
            enableRssToggle = new ROToggle();
            sepNet2 = new ROSeparator();
            lblQosTitle = new Label();
            lblQosDesc = new Label();
            qosDscpToggle = new ROToggle();
            sepNet3 = new ROSeparator();
            lblDeliveryOptTitle = new Label();
            lblDeliveryOptDesc = new Label();
            deliveryOptToggle = new ROToggle();
            btnApplyNetworkOptimzations = new ROModernButton();
            btnFlushDns = new ROModernButton();
            grpPingMonitor = new ROGroupBox();
            txtServerIp = new ROTextBox();
            pingCountNumeric = new RONumericUpDown();
            btnRunPingTest = new ROModernButton();
            lblPingAvg = new ROStatPill();
            lblPingJitter = new ROStatPill();
            lblPacketLoss = new ROStatPill();
            pnlAutoFlush = new Panel();
            grpAutoFlush = new ROGroupBox();
            lblAutoFlushTitle = new Label();
            lblAutoFlushDesc = new Label();
            autoFlushChk = new ROToggle();
            sepFlush1 = new ROSeparator();
            lblSmartFlushTitle = new Label();
            lblSmartFlushDesc = new Label();
            smartFlushToggle = new ROToggle();
            sepFlush2 = new ROSeparator();
            lblSmartThresholdTitle = new Label();
            lblSmartThresholdDesc = new Label();
            smartFlushThreshold = new RONumericUpDown();
            sepFlush3 = new ROSeparator();
            lblIntervalTitle = new Label();
            lblIntervalDesc = new Label();
            autoFlushinterval = new RONumericUpDown();
            autoFlushMinHour = new ROComboBox();
            sepFlush4 = new ROSeparator();
            lblFlushSfxTitle = new Label();
            lblFlushSfxDesc = new Label();
            autoFlushSound = new ROToggle();
            btnSaveFlush = new ROModernButton();
            btnFlushNow = new ROModernButton();
            pnlSidebar = new Panel();
            startAsAdminToggle = new ROToggle();
            lblStartAsAdmin = new Label();
            label1 = new Label();
            updateToggle = new ROToggle();
            _lnkWebsite = new LinkLabel();
            roLabel1 = new ROLabel();
            navDashboard = new ROSidebarItem();
            navProfiles = new ROSidebarItem();
            navTools = new ROSidebarItem();
            navAutoFlush = new ROSidebarItem();
            navNetwork = new ROSidebarItem();
            navAbout = new ROSidebarItem();
            sepSidebar = new ROSeparator();
            launchButton1 = new ROLaunchButton();
            navHotkeys = new ROSidebarItem();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            scanGamesToolStripMenuItem = new ToolStripMenuItem();
            addGameToolStripMenuItem = new ToolStripMenuItem();
            openPacksToolStripMenuItem = new ToolStripMenuItem();
            packBuilderToolStripMenuItem = new ToolStripMenuItem();
            revertAllOptimizationsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem1 = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            openLogFileToolStripMenuItem = new ToolStripMenuItem();
            openLogFilePathToolStripMenuItem = new ToolStripMenuItem();
            openSettingsFilePathToolStripMenuItem = new ToolStripMenuItem();
            openLatestBenchmarkToolStripMenuItem = new ToolStripMenuItem();
            supportToolStripMenuItem = new ToolStripMenuItem();
            flushRamToolStripMenuItem = new ToolStripMenuItem();
            minimizeToTrayToolStripMenuItem = new ToolStripMenuItem();
            roThemeContainer1.SuspendLayout();
            pnlContent.SuspendLayout();
            pnlAbout.SuspendLayout();
            grpLinks.SuspendLayout();
            cardPanel.SuspendLayout();
            grpDiagnostics.SuspendLayout();
            pnlDashboard.SuspendLayout();
            grpGameLibrary.SuspendLayout();
            grpHardware.SuspendLayout();
            grpQuickActions.SuspendLayout();
            pnlHotkeys.SuspendLayout();
            grpHotkeys.SuspendLayout();
            pnlTools.SuspendLayout();
            grpSystemOptimzations.SuspendLayout();
            grpCacheCleaners.SuspendLayout();
            pnlProfiles.SuspendLayout();
            grpProfiles.SuspendLayout();
            grpPerGameOptimzations.SuspendLayout();
            pnlNetwork.SuspendLayout();
            grpNetworkOptimzations.SuspendLayout();
            grpPingMonitor.SuspendLayout();
            pnlAutoFlush.SuspendLayout();
            grpAutoFlush.SuspendLayout();
            pnlSidebar.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // roThemeContainer1
            // 
            roThemeContainer1.BackColor = Color.FromArgb(9, 9, 12);
            roThemeContainer1.BorderStyle = FormBorderStyle.None;
            roThemeContainer1.Controls.Add(pnlContent);
            roThemeContainer1.Controls.Add(pnlSidebar);
            roThemeContainer1.Controls.Add(menuStrip1);
            roThemeContainer1.Dock = DockStyle.Fill;
            roThemeContainer1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roThemeContainer1.Image = Properties.Resources.OptimizerLogo;
            roThemeContainer1.Location = new Point(0, 0);
            roThemeContainer1.Movable = true;
            roThemeContainer1.Name = "roThemeContainer1";
            roThemeContainer1.Sizable = false;
            roThemeContainer1.Size = new Size(920, 600);
            roThemeContainer1.SmartBounds = true;
            roThemeContainer1.StartPosition = FormStartPosition.CenterScreen;
            roThemeContainer1.SubHeader = "v2.2";
            roThemeContainer1.TabIndex = 0;
            roThemeContainer1.Text = "OVERCAPPED OPTIMIZER";
            roThemeContainer1.TransparencyKey = Color.Empty;
            roThemeContainer1.Transparent = false;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.FromArgb(9, 9, 12);
            pnlContent.Controls.Add(pnlDashboard);
            pnlContent.Controls.Add(pnlAbout);
            pnlContent.Controls.Add(pnlHotkeys);
            pnlContent.Controls.Add(pnlTools);
            pnlContent.Controls.Add(pnlProfiles);
            pnlContent.Controls.Add(pnlNetwork);
            pnlContent.Controls.Add(pnlAutoFlush);
            pnlContent.Location = new Point(214, 74);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(705, 525);
            pnlContent.TabIndex = 0;
            // 
            // pnlAbout
            // 
            pnlAbout.BackColor = Color.FromArgb(9, 9, 12);
            pnlAbout.Controls.Add(grpLinks);
            pnlAbout.Controls.Add(grpDiagnostics);
            pnlAbout.Dock = DockStyle.Fill;
            pnlAbout.Location = new Point(0, 0);
            pnlAbout.Name = "pnlAbout";
            pnlAbout.Size = new Size(705, 525);
            pnlAbout.TabIndex = 5;
            pnlAbout.Visible = false;
            // 
            // grpLinks
            // 
            grpLinks.BackColor = Color.Transparent;
            grpLinks.Controls.Add(cardPanel);
            grpLinks.Controls.Add(linkWebsite);
            grpLinks.Controls.Add(aboutLinkTile1);
            grpLinks.Controls.Add(linkDocs);
            grpLinks.Controls.Add(linkDiscord);
            grpLinks.Controls.Add(linkGitHub);
            grpLinks.Font = new Font("Segoe UI", 9.5F);
            grpLinks.Location = new Point(16, 14);
            grpLinks.Name = "grpLinks";
            grpLinks.Padding = new Padding(14, 44, 14, 14);
            grpLinks.Size = new Size(673, 380);
            grpLinks.SubTitle = "About Overcapped Optimizer";
            grpLinks.TabIndex = 0;
            grpLinks.Title = "About";
            // 
            // cardPanel
            // 
            cardPanel.BackColor = Color.FromArgb(22, 22, 29);
            cardPanel.Controls.Add(roLabel2);
            cardPanel.Controls.Add(lblDescription);
            cardPanel.Location = new Point(20, 41);
            cardPanel.Name = "cardPanel";
            cardPanel.Padding = new Padding(16);
            cardPanel.Size = new Size(636, 112);
            cardPanel.TabIndex = 0;
            // 
            // roLabel2
            // 
            roLabel2.BackColor = Color.Transparent;
            roLabel2.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roLabel2.Location = new Point(18, 5);
            roLabel2.Name = "roLabel2";
            roLabel2.Size = new Size(200, 24);
            roLabel2.TabIndex = 0;
            roLabel2.Value1 = "Overcapped";
            roLabel2.Value2 = " Optimizer";
            // 
            // lblDescription
            // 
            lblDescription.Font = new Font("Segoe UI", 9F);
            lblDescription.ForeColor = Color.FromArgb(162, 162, 176);
            lblDescription.Location = new Point(18, 31);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(601, 76);
            lblDescription.TabIndex = 1;
            lblDescription.Text = resources.GetString("lblDescription.Text");
            // 
            // linkWebsite
            // 
            linkWebsite.BackColor = Color.Transparent;
            linkWebsite.Icon = Properties.Resources.docs;
            linkWebsite.Location = new Point(20, 233);
            linkWebsite.Name = "linkWebsite";
            linkWebsite.Size = new Size(315, 62);
            linkWebsite.TabIndex = 1;
            linkWebsite.Title = "Overcapped Documentation";
            linkWebsite.Url = "https://overcapped.us/docs";
            // 
            // aboutLinkTile1
            // 
            aboutLinkTile1.BackColor = Color.Transparent;
            aboutLinkTile1.Icon = Properties.Resources.cup;
            aboutLinkTile1.Location = new Point(342, 301);
            aboutLinkTile1.Name = "aboutLinkTile1";
            aboutLinkTile1.Size = new Size(315, 62);
            aboutLinkTile1.TabIndex = 2;
            aboutLinkTile1.Title = "Donations";
            aboutLinkTile1.Url = "https://buymeacoffee.com/rustforgedev";
            // 
            // linkDocs
            // 
            linkDocs.BackColor = Color.Transparent;
            linkDocs.Icon = Properties.Resources.OptimizerLogo;
            linkDocs.Location = new Point(20, 159);
            linkDocs.Name = "linkDocs";
            linkDocs.Size = new Size(636, 62);
            linkDocs.TabIndex = 3;
            linkDocs.Title = "Overcapped Optimizer Site";
            linkDocs.Url = "https://overcapped.us/";
            // 
            // linkDiscord
            // 
            linkDiscord.BackColor = Color.Transparent;
            linkDiscord.Icon = Properties.Resources.discord;
            linkDiscord.Location = new Point(20, 301);
            linkDiscord.Name = "linkDiscord";
            linkDiscord.Size = new Size(315, 62);
            linkDiscord.TabIndex = 4;
            linkDiscord.Title = "Discord Support";
            linkDiscord.Url = "https://overcapped.us/invite";
            // 
            // linkGitHub
            // 
            linkGitHub.BackColor = Color.Transparent;
            linkGitHub.Icon = Properties.Resources.github;
            linkGitHub.Location = new Point(342, 233);
            linkGitHub.Name = "linkGitHub";
            linkGitHub.Size = new Size(315, 62);
            linkGitHub.TabIndex = 5;
            linkGitHub.Title = "GitHub Source";
            linkGitHub.Url = "https://github.com/V0idpool/Overcapped-PC-Gaming-Optimizer";
            // 
            // grpDiagnostics
            // 
            grpDiagnostics.BackColor = Color.Transparent;
            grpDiagnostics.Controls.Add(btnOpenLog);
            grpDiagnostics.Controls.Add(btnOpenLogDir);
            grpDiagnostics.Controls.Add(btnOpenSettingsDir);
            grpDiagnostics.Font = new Font("Segoe UI", 9.5F);
            grpDiagnostics.Location = new Point(16, 400);
            grpDiagnostics.Name = "grpDiagnostics";
            grpDiagnostics.Padding = new Padding(14, 44, 14, 14);
            grpDiagnostics.Size = new Size(673, 114);
            grpDiagnostics.SubTitle = "Quick access to logs, settings.json & JSON packs";
            grpDiagnostics.TabIndex = 1;
            grpDiagnostics.Title = "Diagnostics & Config Files";
            // 
            // btnOpenLog
            // 
            btnOpenLog.AccentColor = Color.FromArgb(214, 28, 52);
            btnOpenLog.BackColor = Color.Transparent;
            btnOpenLog.CornerRadius = 5;
            btnOpenLog.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOpenLog.ForeColor = Color.FromArgb(242, 242, 246);
            btnOpenLog.IsPrimaryAction = false;
            btnOpenLog.Location = new Point(14, 54);
            btnOpenLog.Name = "btnOpenLog";
            btnOpenLog.Size = new Size(206, 38);
            btnOpenLog.TabIndex = 0;
            btnOpenLog.Text = "Open Log File";
            btnOpenLog.Click += openLogFileToolStripMenuItem_Click;
            // 
            // btnOpenLogDir
            // 
            btnOpenLogDir.AccentColor = Color.FromArgb(214, 28, 52);
            btnOpenLogDir.BackColor = Color.Transparent;
            btnOpenLogDir.CornerRadius = 5;
            btnOpenLogDir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOpenLogDir.ForeColor = Color.FromArgb(242, 242, 246);
            btnOpenLogDir.IsPrimaryAction = false;
            btnOpenLogDir.Location = new Point(232, 54);
            btnOpenLogDir.Name = "btnOpenLogDir";
            btnOpenLogDir.Size = new Size(206, 38);
            btnOpenLogDir.TabIndex = 1;
            btnOpenLogDir.Text = "Open Logs Folder";
            btnOpenLogDir.Click += openLogFilePathToolStripMenuItem_Click;
            // 
            // btnOpenSettingsDir
            // 
            btnOpenSettingsDir.AccentColor = Color.FromArgb(214, 28, 52);
            btnOpenSettingsDir.BackColor = Color.Transparent;
            btnOpenSettingsDir.CornerRadius = 5;
            btnOpenSettingsDir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOpenSettingsDir.ForeColor = Color.FromArgb(242, 242, 246);
            btnOpenSettingsDir.IsPrimaryAction = false;
            btnOpenSettingsDir.Location = new Point(450, 54);
            btnOpenSettingsDir.Name = "btnOpenSettingsDir";
            btnOpenSettingsDir.Size = new Size(209, 38);
            btnOpenSettingsDir.TabIndex = 2;
            btnOpenSettingsDir.Text = "Open Data Folder";
            btnOpenSettingsDir.Click += openSettingsFilePathToolStripMenuItem_Click;
            // 
            // pnlDashboard
            // 
            pnlDashboard.BackColor = Color.FromArgb(9, 9, 12);
            pnlDashboard.Controls.Add(grpGameLibrary);
            pnlDashboard.Controls.Add(grpHardware);
            pnlDashboard.Controls.Add(grpQuickActions);
            pnlDashboard.Dock = DockStyle.Fill;
            pnlDashboard.Location = new Point(0, 0);
            pnlDashboard.Name = "pnlDashboard";
            pnlDashboard.Size = new Size(705, 525);
            pnlDashboard.TabIndex = 0;
            // 
            // grpGameLibrary
            // 
            grpGameLibrary.BackColor = Color.Transparent;
            grpGameLibrary.Controls.Add(gameDropdown);
            grpGameLibrary.Controls.Add(btnScanGames);
            grpGameLibrary.Controls.Add(btnAddManualGame);
            grpGameLibrary.Controls.Add(btnRemoveGame);
            grpGameLibrary.Controls.Add(lblSelectedGameMeta);
            grpGameLibrary.Font = new Font("Segoe UI", 9.5F);
            grpGameLibrary.Location = new Point(16, 12);
            grpGameLibrary.Name = "grpGameLibrary";
            grpGameLibrary.Padding = new Padding(14, 44, 14, 14);
            grpGameLibrary.Size = new Size(673, 114);
            grpGameLibrary.SubTitle = "Steam, Epic, GOG, Xbox, Battle.net, EA & manual .exe detection";
            grpGameLibrary.TabIndex = 0;
            grpGameLibrary.Title = "Game Library";
            // 
            // gameDropdown
            // 
            gameDropdown.BackColor = Color.FromArgb(22, 22, 29);
            gameDropdown.DrawMode = DrawMode.OwnerDrawFixed;
            gameDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            gameDropdown.Font = new Font("Segoe UI", 9.25F);
            gameDropdown.ForeColor = Color.FromArgb(242, 242, 246);
            gameDropdown.ItemHeight = 26;
            gameDropdown.Location = new Point(14, 48);
            gameDropdown.Name = "gameDropdown";
            gameDropdown.Size = new Size(344, 32);
            gameDropdown.TabIndex = 0;
            gameDropdown.SelectedIndexChanged += gameDropdown_SelectedIndexChanged;
            // 
            // btnScanGames
            // 
            btnScanGames.AccentColor = Color.FromArgb(214, 28, 52);
            btnScanGames.BackColor = Color.Transparent;
            btnScanGames.CornerRadius = 5;
            btnScanGames.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnScanGames.ForeColor = Color.FromArgb(242, 242, 246);
            btnScanGames.IsPrimaryAction = true;
            btnScanGames.Location = new Point(366, 48);
            btnScanGames.Name = "btnScanGames";
            btnScanGames.Size = new Size(106, 32);
            btnScanGames.TabIndex = 1;
            btnScanGames.Text = "Scan Games";
            btnScanGames.Click += btnScanGames_Click;
            // 
            // btnAddManualGame
            // 
            btnAddManualGame.AccentColor = Color.FromArgb(214, 28, 52);
            btnAddManualGame.BackColor = Color.Transparent;
            btnAddManualGame.CornerRadius = 5;
            btnAddManualGame.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnAddManualGame.ForeColor = Color.FromArgb(242, 242, 246);
            btnAddManualGame.IsPrimaryAction = false;
            btnAddManualGame.Location = new Point(480, 48);
            btnAddManualGame.Name = "btnAddManualGame";
            btnAddManualGame.Size = new Size(94, 32);
            btnAddManualGame.TabIndex = 2;
            btnAddManualGame.Text = "Add .exe...";
            btnAddManualGame.Click += btnAddManualGame_Click;
            // 
            // btnRemoveGame
            // 
            btnRemoveGame.AccentColor = Color.FromArgb(244, 63, 94);
            btnRemoveGame.BackColor = Color.Transparent;
            btnRemoveGame.CornerRadius = 5;
            btnRemoveGame.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnRemoveGame.ForeColor = Color.FromArgb(242, 242, 246);
            btnRemoveGame.IsPrimaryAction = false;
            btnRemoveGame.Location = new Point(582, 48);
            btnRemoveGame.Name = "btnRemoveGame";
            btnRemoveGame.Size = new Size(77, 32);
            btnRemoveGame.TabIndex = 3;
            btnRemoveGame.Text = "Remove";
            btnRemoveGame.Click += btnRemoveGame_Click;
            // 
            // lblSelectedGameMeta
            // 
            lblSelectedGameMeta.Font = new Font("Segoe UI", 8F);
            lblSelectedGameMeta.ForeColor = Color.FromArgb(162, 162, 176);
            lblSelectedGameMeta.Location = new Point(16, 86);
            lblSelectedGameMeta.Name = "lblSelectedGameMeta";
            lblSelectedGameMeta.Size = new Size(642, 18);
            lblSelectedGameMeta.TabIndex = 4;
            lblSelectedGameMeta.Text = "Click Scan Games to detect installed titles, or Add .exe manually.";
            // 
            // grpHardware
            // 
            grpHardware.BackColor = Color.Transparent;
            grpHardware.Controls.Add(lblCPUInfo);
            grpHardware.Controls.Add(lblGPUInfo);
            grpHardware.Controls.Add(lblRAMInfo);
            grpHardware.Controls.Add(lblHealthStatus);
            grpHardware.Controls.Add(btnHealthDetails);
            grpHardware.Font = new Font("Segoe UI", 9.5F);
            grpHardware.Location = new Point(16, 134);
            grpHardware.Name = "grpHardware";
            grpHardware.Padding = new Padding(14, 44, 14, 14);
            grpHardware.Size = new Size(673, 198);
            grpHardware.SubTitle = "Detected system specs & health diagnostics";
            grpHardware.TabIndex = 1;
            grpHardware.Title = "Hardware Information";
            // 
            // lblCPUInfo
            // 
            lblCPUInfo.BackColor = Color.Transparent;
            lblCPUInfo.DotColor = Color.FromArgb(214, 28, 52);
            lblCPUInfo.Location = new Point(14, 44);
            lblCPUInfo.Name = "lblCPUInfo";
            lblCPUInfo.Prefix = "CPU:";
            lblCPUInfo.Size = new Size(645, 30);
            lblCPUInfo.TabIndex = 0;
            lblCPUInfo.Text = "Detecting...";
            // 
            // lblGPUInfo
            // 
            lblGPUInfo.BackColor = Color.Transparent;
            lblGPUInfo.DotColor = Color.FromArgb(214, 28, 52);
            lblGPUInfo.Location = new Point(14, 80);
            lblGPUInfo.Name = "lblGPUInfo";
            lblGPUInfo.Prefix = "GPU:";
            lblGPUInfo.Size = new Size(645, 30);
            lblGPUInfo.TabIndex = 1;
            lblGPUInfo.Text = "Detecting...";
            // 
            // lblRAMInfo
            // 
            lblRAMInfo.BackColor = Color.Transparent;
            lblRAMInfo.DotColor = Color.FromArgb(214, 28, 52);
            lblRAMInfo.Location = new Point(14, 116);
            lblRAMInfo.Name = "lblRAMInfo";
            lblRAMInfo.Prefix = "RAM:";
            lblRAMInfo.Size = new Size(645, 30);
            lblRAMInfo.TabIndex = 2;
            lblRAMInfo.Text = "Detecting...";
            // 
            // lblHealthStatus
            // 
            lblHealthStatus.BackColor = Color.Transparent;
            lblHealthStatus.DotColor = Color.FromArgb(162, 162, 176);
            lblHealthStatus.Location = new Point(14, 152);
            lblHealthStatus.Name = "lblHealthStatus";
            lblHealthStatus.Prefix = "HEALTH:";
            lblHealthStatus.Size = new Size(559, 32);
            lblHealthStatus.TabIndex = 3;
            lblHealthStatus.Text = "Scanning monitor Hz, RAM, Page File, ReBAR, HAGS & drive...";
            // 
            // btnHealthDetails
            // 
            btnHealthDetails.AccentColor = Color.FromArgb(214, 28, 52);
            btnHealthDetails.BackColor = Color.Transparent;
            btnHealthDetails.CornerRadius = 5;
            btnHealthDetails.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnHealthDetails.ForeColor = Color.FromArgb(242, 242, 246);
            btnHealthDetails.IsPrimaryAction = false;
            btnHealthDetails.Location = new Point(581, 152);
            btnHealthDetails.Name = "btnHealthDetails";
            btnHealthDetails.Size = new Size(78, 32);
            btnHealthDetails.TabIndex = 4;
            btnHealthDetails.Text = "Details";
            btnHealthDetails.Click += btnHealthDetails_Click;
            // 
            // grpQuickActions
            // 
            grpQuickActions.BackColor = Color.Transparent;
            grpQuickActions.Controls.Add(linkDonate);
            grpQuickActions.Controls.Add(cardQuickFlush);
            grpQuickActions.Controls.Add(cardQuickShaderCache);
            grpQuickActions.Controls.Add(btnDashRestorePoint);
            grpQuickActions.Controls.Add(btnDashRevertOptimizations);
            grpQuickActions.Controls.Add(btnDashTray);
            grpQuickActions.Font = new Font("Segoe UI", 9.5F);
            grpQuickActions.Location = new Point(16, 340);
            grpQuickActions.Name = "grpQuickActions";
            grpQuickActions.Padding = new Padding(14, 44, 14, 14);
            grpQuickActions.Size = new Size(673, 174);
            grpQuickActions.SubTitle = "One-click maintenance & safety utilities";
            grpQuickActions.TabIndex = 2;
            grpQuickActions.Title = "Quick Actions";
            // 
            // linkDonate
            // 
            linkDonate.BackColor = Color.Transparent;
            linkDonate.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkDonate.Icon = Properties.Resources.cup;
            linkDonate.Location = new Point(14, 124);
            linkDonate.Name = "linkDonate";
            linkDonate.Size = new Size(155, 36);
            linkDonate.TabIndex = 2;
            linkDonate.Title = "Donations";
            linkDonate.Url = "https://buymeacoffee.com/rustforgedev";
            // 
            // cardQuickFlush
            // 
            cardQuickFlush.BackColor = Color.Transparent;
            cardQuickFlush.BadgeCharacter = "↻";
            cardQuickFlush.Location = new Point(14, 46);
            cardQuickFlush.Name = "cardQuickFlush";
            cardQuickFlush.Selected = false;
            cardQuickFlush.Size = new Size(316, 68);
            cardQuickFlush.Subtitle = "Purge Windows standby memory list now";
            cardQuickFlush.TabIndex = 0;
            cardQuickFlush.Title = "Flush Standby RAM";
            cardQuickFlush.Click += flushRamToolStripMenuItem_Click;
            // 
            // cardQuickShaderCache
            // 
            cardQuickShaderCache.BackColor = Color.Transparent;
            cardQuickShaderCache.BadgeCharacter = "✕";
            cardQuickShaderCache.Location = new Point(342, 46);
            cardQuickShaderCache.Name = "cardQuickShaderCache";
            cardQuickShaderCache.Selected = false;
            cardQuickShaderCache.Size = new Size(317, 68);
            cardQuickShaderCache.Subtitle = "Clear DirectX, NVIDIA & AMD shader caches";
            cardQuickShaderCache.TabIndex = 1;
            cardQuickShaderCache.Title = "Clear GPU Shader Cache";
            cardQuickShaderCache.Click += clearShaderCacheBtn_Click;
            // 
            // btnDashRestorePoint
            // 
            btnDashRestorePoint.AccentColor = Color.FromArgb(214, 28, 52);
            btnDashRestorePoint.BackColor = Color.Transparent;
            btnDashRestorePoint.CornerRadius = 5;
            btnDashRestorePoint.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnDashRestorePoint.ForeColor = Color.FromArgb(242, 242, 246);
            btnDashRestorePoint.IsPrimaryAction = false;
            btnDashRestorePoint.Location = new Point(176, 124);
            btnDashRestorePoint.Name = "btnDashRestorePoint";
            btnDashRestorePoint.Size = new Size(160, 36);
            btnDashRestorePoint.TabIndex = 3;
            btnDashRestorePoint.Text = "System Restore Point";
            btnDashRestorePoint.Click += btnDashRestorePoint_Click;
            // 
            // btnDashRevertOptimizations
            // 
            btnDashRevertOptimizations.AccentColor = Color.FromArgb(244, 63, 94);
            btnDashRevertOptimizations.BackColor = Color.Transparent;
            btnDashRevertOptimizations.CornerRadius = 5;
            btnDashRevertOptimizations.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnDashRevertOptimizations.ForeColor = Color.FromArgb(242, 242, 246);
            btnDashRevertOptimizations.IsPrimaryAction = false;
            btnDashRevertOptimizations.Location = new Point(342, 124);
            btnDashRevertOptimizations.Name = "btnDashRevertOptimizations";
            btnDashRevertOptimizations.Size = new Size(172, 36);
            btnDashRevertOptimizations.TabIndex = 4;
            btnDashRevertOptimizations.Text = "Revert All Optimizations";
            btnDashRevertOptimizations.Click += btnDashRevertOptimizations_Click;
            // 
            // btnDashTray
            // 
            btnDashTray.AccentColor = Color.FromArgb(214, 28, 52);
            btnDashTray.BackColor = Color.Transparent;
            btnDashTray.CornerRadius = 5;
            btnDashTray.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnDashTray.ForeColor = Color.FromArgb(242, 242, 246);
            btnDashTray.IsPrimaryAction = false;
            btnDashTray.Location = new Point(520, 124);
            btnDashTray.Name = "btnDashTray";
            btnDashTray.Size = new Size(139, 36);
            btnDashTray.TabIndex = 5;
            btnDashTray.Text = "Minimize to Tray";
            btnDashTray.Click += minimizeToTrayToolStripMenuItem_Click;
            // 
            // pnlHotkeys
            // 
            pnlHotkeys.BackColor = Color.FromArgb(9, 9, 12);
            pnlHotkeys.Controls.Add(grpHotkeys);
            pnlHotkeys.Controls.Add(btnSaveHotkeys);
            pnlHotkeys.Controls.Add(btnResetHotkeys);
            pnlHotkeys.Dock = DockStyle.Fill;
            pnlHotkeys.Location = new Point(0, 0);
            pnlHotkeys.Name = "pnlHotkeys";
            pnlHotkeys.Size = new Size(705, 525);
            pnlHotkeys.TabIndex = 6;
            pnlHotkeys.Visible = false;
            // 
            // grpHotkeys
            // 
            grpHotkeys.BackColor = Color.Transparent;
            grpHotkeys.Controls.Add(lblHotkeysEnableTitle);
            grpHotkeys.Controls.Add(lblHotkeysEnableDesc);
            grpHotkeys.Controls.Add(hotkeysEnabledToggle);
            grpHotkeys.Controls.Add(sepHk1);
            grpHotkeys.Controls.Add(lblHkFpsTitle);
            grpHotkeys.Controls.Add(lblHkFpsDesc);
            grpHotkeys.Controls.Add(txtHkFpsOverlay);
            grpHotkeys.Controls.Add(btnBindHkFps);
            grpHotkeys.Controls.Add(btnClearHkFps);
            grpHotkeys.Controls.Add(sepHk2);
            grpHotkeys.Controls.Add(lblHk1LowTitle);
            grpHotkeys.Controls.Add(lblHk1LowDesc);
            grpHotkeys.Controls.Add(txtHk1PercentLow);
            grpHotkeys.Controls.Add(btnBindHk1Low);
            grpHotkeys.Controls.Add(btnClearHk1Low);
            grpHotkeys.Controls.Add(sepHk3);
            grpHotkeys.Controls.Add(lblHkFlushTitle);
            grpHotkeys.Controls.Add(lblHkFlushDesc);
            grpHotkeys.Controls.Add(txtHkFlushRam);
            grpHotkeys.Controls.Add(btnBindHkFlush);
            grpHotkeys.Controls.Add(btnClearHkFlush);
            grpHotkeys.Controls.Add(sepHk4);
            grpHotkeys.Controls.Add(lblHkWindowTitle);
            grpHotkeys.Controls.Add(lblHkWindowDesc);
            grpHotkeys.Controls.Add(txtHkToggleWindow);
            grpHotkeys.Controls.Add(btnBindHkWindow);
            grpHotkeys.Controls.Add(btnClearHkWindow);
            grpHotkeys.Controls.Add(lblHkHint);
            grpHotkeys.Font = new Font("Segoe UI", 9.5F);
            grpHotkeys.Location = new Point(16, 14);
            grpHotkeys.Name = "grpHotkeys";
            grpHotkeys.Padding = new Padding(14, 44, 14, 14);
            grpHotkeys.Size = new Size(673, 388);
            grpHotkeys.SubTitle = "Control Overcapped mid-game without Alt-Tabbing";
            grpHotkeys.TabIndex = 0;
            grpHotkeys.Title = "Global Hotkeys";
            // 
            // lblHotkeysEnableTitle
            // 
            lblHotkeysEnableTitle.AutoSize = true;
            lblHotkeysEnableTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblHotkeysEnableTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblHotkeysEnableTitle.Location = new Point(16, 48);
            lblHotkeysEnableTitle.Name = "lblHotkeysEnableTitle";
            lblHotkeysEnableTitle.Size = new Size(147, 17);
            lblHotkeysEnableTitle.TabIndex = 0;
            lblHotkeysEnableTitle.Text = "Enable Global Hotkeys";
            // 
            // lblHotkeysEnableDesc
            // 
            lblHotkeysEnableDesc.AutoSize = true;
            lblHotkeysEnableDesc.Font = new Font("Segoe UI", 8.25F);
            lblHotkeysEnableDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblHotkeysEnableDesc.Location = new Point(16, 68);
            lblHotkeysEnableDesc.Name = "lblHotkeysEnableDesc";
            lblHotkeysEnableDesc.Size = new Size(427, 13);
            lblHotkeysEnableDesc.TabIndex = 1;
            lblHotkeysEnableDesc.Text = "Listen for your bound key combos while Overcapped is open or minimized to tray.";
            // 
            // hotkeysEnabledToggle
            // 
            hotkeysEnabledToggle.BackColor = Color.Transparent;
            hotkeysEnabledToggle.Checked = true;
            hotkeysEnabledToggle.Location = new Point(610, 55);
            hotkeysEnabledToggle.Name = "hotkeysEnabledToggle";
            hotkeysEnabledToggle.Size = new Size(46, 22);
            hotkeysEnabledToggle.TabIndex = 2;
            // 
            // sepHk1
            // 
            sepHk1.BackColor = Color.Transparent;
            sepHk1.Location = new Point(16, 94);
            sepHk1.Name = "sepHk1";
            sepHk1.Size = new Size(641, 8);
            sepHk1.TabIndex = 3;
            // 
            // lblHkFpsTitle
            // 
            lblHkFpsTitle.AutoSize = true;
            lblHkFpsTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblHkFpsTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblHkFpsTitle.Location = new Point(16, 108);
            lblHkFpsTitle.Name = "lblHkFpsTitle";
            lblHkFpsTitle.Size = new Size(161, 17);
            lblHkFpsTitle.TabIndex = 4;
            lblHkFpsTitle.Text = "Show / Hide FPS Overlay";
            // 
            // lblHkFpsDesc
            // 
            lblHkFpsDesc.AutoSize = true;
            lblHkFpsDesc.Font = new Font("Segoe UI", 8.25F);
            lblHkFpsDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblHkFpsDesc.Location = new Point(16, 128);
            lblHkFpsDesc.Name = "lblHkFpsDesc";
            lblHkFpsDesc.Size = new Size(316, 13);
            lblHkFpsDesc.TabIndex = 5;
            lblHkFpsDesc.Text = "Hides or shows the FPS HUD without stopping benchmarks.";
            // 
            // txtHkFpsOverlay
            // 
            txtHkFpsOverlay.BackColor = Color.Transparent;
            txtHkFpsOverlay.Location = new Point(420, 110);
            txtHkFpsOverlay.MaxLength = 64;
            txtHkFpsOverlay.Multiline = false;
            txtHkFpsOverlay.Name = "txtHkFpsOverlay";
            txtHkFpsOverlay.ReadOnly = true;
            txtHkFpsOverlay.Size = new Size(106, 32);
            txtHkFpsOverlay.TabIndex = 6;
            txtHkFpsOverlay.Text = "Ctrl+Shift+O";
            txtHkFpsOverlay.TextAlign = HorizontalAlignment.Center;
            txtHkFpsOverlay.UseSystemPasswordChar = false;
            // 
            // btnBindHkFps
            // 
            btnBindHkFps.AccentColor = Color.FromArgb(214, 28, 52);
            btnBindHkFps.BackColor = Color.Transparent;
            btnBindHkFps.CornerRadius = 5;
            btnBindHkFps.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnBindHkFps.ForeColor = Color.FromArgb(242, 242, 246);
            btnBindHkFps.IsPrimaryAction = false;
            btnBindHkFps.Location = new Point(532, 110);
            btnBindHkFps.Name = "btnBindHkFps";
            btnBindHkFps.Size = new Size(54, 32);
            btnBindHkFps.TabIndex = 7;
            btnBindHkFps.Text = "Bind";
            btnBindHkFps.Click += btnBindHkFps_Click;
            // 
            // btnClearHkFps
            // 
            btnClearHkFps.AccentColor = Color.FromArgb(244, 63, 94);
            btnClearHkFps.BackColor = Color.Transparent;
            btnClearHkFps.CornerRadius = 5;
            btnClearHkFps.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnClearHkFps.ForeColor = Color.FromArgb(242, 242, 246);
            btnClearHkFps.IsPrimaryAction = false;
            btnClearHkFps.Location = new Point(592, 110);
            btnClearHkFps.Name = "btnClearHkFps";
            btnClearHkFps.Size = new Size(64, 32);
            btnClearHkFps.TabIndex = 8;
            btnClearHkFps.Text = "Clear";
            btnClearHkFps.Click += btnClearHkFps_Click;
            // 
            // sepHk2
            // 
            sepHk2.BackColor = Color.Transparent;
            sepHk2.Location = new Point(16, 154);
            sepHk2.Name = "sepHk2";
            sepHk2.Size = new Size(641, 8);
            sepHk2.TabIndex = 9;
            // 
            // lblHk1LowTitle
            // 
            lblHk1LowTitle.AutoSize = true;
            lblHk1LowTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblHk1LowTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblHk1LowTitle.Location = new Point(16, 168);
            lblHk1LowTitle.Name = "lblHk1LowTitle";
            lblHk1LowTitle.Size = new Size(182, 17);
            lblHk1LowTitle.TabIndex = 10;
            lblHk1LowTitle.Text = "Toggle 1% Low FPS Readout";
            // 
            // lblHk1LowDesc
            // 
            lblHk1LowDesc.AutoSize = true;
            lblHk1LowDesc.Font = new Font("Segoe UI", 8.25F);
            lblHk1LowDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblHk1LowDesc.Location = new Point(16, 188);
            lblHk1LowDesc.Name = "lblHk1LowDesc";
            lblHk1LowDesc.Size = new Size(300, 13);
            lblHk1LowDesc.TabIndex = 11;
            lblHk1LowDesc.Text = "Switches HUD between compact FPS/ms and full 1% Low.";
            // 
            // txtHk1PercentLow
            // 
            txtHk1PercentLow.BackColor = Color.Transparent;
            txtHk1PercentLow.Location = new Point(420, 170);
            txtHk1PercentLow.MaxLength = 64;
            txtHk1PercentLow.Multiline = false;
            txtHk1PercentLow.Name = "txtHk1PercentLow";
            txtHk1PercentLow.ReadOnly = true;
            txtHk1PercentLow.Size = new Size(106, 32);
            txtHk1PercentLow.TabIndex = 12;
            txtHk1PercentLow.Text = "Ctrl+Shift+L";
            txtHk1PercentLow.TextAlign = HorizontalAlignment.Center;
            txtHk1PercentLow.UseSystemPasswordChar = false;
            // 
            // btnBindHk1Low
            // 
            btnBindHk1Low.AccentColor = Color.FromArgb(214, 28, 52);
            btnBindHk1Low.BackColor = Color.Transparent;
            btnBindHk1Low.CornerRadius = 5;
            btnBindHk1Low.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnBindHk1Low.ForeColor = Color.FromArgb(242, 242, 246);
            btnBindHk1Low.IsPrimaryAction = false;
            btnBindHk1Low.Location = new Point(532, 170);
            btnBindHk1Low.Name = "btnBindHk1Low";
            btnBindHk1Low.Size = new Size(54, 32);
            btnBindHk1Low.TabIndex = 13;
            btnBindHk1Low.Text = "Bind";
            btnBindHk1Low.Click += btnBindHk1Low_Click;
            // 
            // btnClearHk1Low
            // 
            btnClearHk1Low.AccentColor = Color.FromArgb(244, 63, 94);
            btnClearHk1Low.BackColor = Color.Transparent;
            btnClearHk1Low.CornerRadius = 5;
            btnClearHk1Low.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnClearHk1Low.ForeColor = Color.FromArgb(242, 242, 246);
            btnClearHk1Low.IsPrimaryAction = false;
            btnClearHk1Low.Location = new Point(592, 170);
            btnClearHk1Low.Name = "btnClearHk1Low";
            btnClearHk1Low.Size = new Size(64, 32);
            btnClearHk1Low.TabIndex = 14;
            btnClearHk1Low.Text = "Clear";
            btnClearHk1Low.Click += btnClearHk1Low_Click;
            // 
            // sepHk3
            // 
            sepHk3.BackColor = Color.Transparent;
            sepHk3.Location = new Point(16, 214);
            sepHk3.Name = "sepHk3";
            sepHk3.Size = new Size(641, 8);
            sepHk3.TabIndex = 15;
            // 
            // lblHkFlushTitle
            // 
            lblHkFlushTitle.AutoSize = true;
            lblHkFlushTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblHkFlushTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblHkFlushTitle.Location = new Point(16, 228);
            lblHkFlushTitle.Name = "lblHkFlushTitle";
            lblHkFlushTitle.Size = new Size(215, 17);
            lblHkFlushTitle.TabIndex = 16;
            lblHkFlushTitle.Text = "Flush Standby RAM Now (Admin)";
            // 
            // lblHkFlushDesc
            // 
            lblHkFlushDesc.AutoSize = true;
            lblHkFlushDesc.Font = new Font("Segoe UI", 8.25F);
            lblHkFlushDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblHkFlushDesc.Location = new Point(16, 248);
            lblHkFlushDesc.Name = "lblHkFlushDesc";
            lblHkFlushDesc.Size = new Size(275, 13);
            lblHkFlushDesc.TabIndex = 17;
            lblHkFlushDesc.Text = "Silently clears Windows standby memory mid-match.";
            // 
            // txtHkFlushRam
            // 
            txtHkFlushRam.BackColor = Color.Transparent;
            txtHkFlushRam.Location = new Point(420, 230);
            txtHkFlushRam.MaxLength = 64;
            txtHkFlushRam.Multiline = false;
            txtHkFlushRam.Name = "txtHkFlushRam";
            txtHkFlushRam.ReadOnly = true;
            txtHkFlushRam.Size = new Size(106, 32);
            txtHkFlushRam.TabIndex = 18;
            txtHkFlushRam.Text = "Ctrl+Shift+R";
            txtHkFlushRam.TextAlign = HorizontalAlignment.Center;
            txtHkFlushRam.UseSystemPasswordChar = false;
            // 
            // btnBindHkFlush
            // 
            btnBindHkFlush.AccentColor = Color.FromArgb(214, 28, 52);
            btnBindHkFlush.BackColor = Color.Transparent;
            btnBindHkFlush.CornerRadius = 5;
            btnBindHkFlush.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnBindHkFlush.ForeColor = Color.FromArgb(242, 242, 246);
            btnBindHkFlush.IsPrimaryAction = false;
            btnBindHkFlush.Location = new Point(532, 229);
            btnBindHkFlush.Name = "btnBindHkFlush";
            btnBindHkFlush.Size = new Size(54, 32);
            btnBindHkFlush.TabIndex = 19;
            btnBindHkFlush.Text = "Bind";
            btnBindHkFlush.Click += btnBindHkFlush_Click;
            // 
            // btnClearHkFlush
            // 
            btnClearHkFlush.AccentColor = Color.FromArgb(244, 63, 94);
            btnClearHkFlush.BackColor = Color.Transparent;
            btnClearHkFlush.CornerRadius = 5;
            btnClearHkFlush.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnClearHkFlush.ForeColor = Color.FromArgb(242, 242, 246);
            btnClearHkFlush.IsPrimaryAction = false;
            btnClearHkFlush.Location = new Point(592, 230);
            btnClearHkFlush.Name = "btnClearHkFlush";
            btnClearHkFlush.Size = new Size(64, 32);
            btnClearHkFlush.TabIndex = 20;
            btnClearHkFlush.Text = "Clear";
            btnClearHkFlush.Click += btnClearHkFlush_Click;
            // 
            // sepHk4
            // 
            sepHk4.BackColor = Color.Transparent;
            sepHk4.Location = new Point(16, 274);
            sepHk4.Name = "sepHk4";
            sepHk4.Size = new Size(641, 8);
            sepHk4.TabIndex = 21;
            // 
            // lblHkWindowTitle
            // 
            lblHkWindowTitle.AutoSize = true;
            lblHkWindowTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblHkWindowTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblHkWindowTitle.Location = new Point(16, 288);
            lblHkWindowTitle.Name = "lblHkWindowTitle";
            lblHkWindowTitle.Size = new Size(244, 17);
            lblHkWindowTitle.TabIndex = 22;
            lblHkWindowTitle.Text = "Show / Minimize Overcapped Window";
            // 
            // lblHkWindowDesc
            // 
            lblHkWindowDesc.AutoSize = true;
            lblHkWindowDesc.Font = new Font("Segoe UI", 8.25F);
            lblHkWindowDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblHkWindowDesc.Location = new Point(16, 308);
            lblHkWindowDesc.Name = "lblHkWindowDesc";
            lblHkWindowDesc.Size = new Size(269, 13);
            lblHkWindowDesc.TabIndex = 23;
            lblHkWindowDesc.Text = "Open Overcapped or minimize it to the system tray.";
            // 
            // txtHkToggleWindow
            // 
            txtHkToggleWindow.BackColor = Color.Transparent;
            txtHkToggleWindow.Location = new Point(420, 290);
            txtHkToggleWindow.MaxLength = 64;
            txtHkToggleWindow.Multiline = false;
            txtHkToggleWindow.Name = "txtHkToggleWindow";
            txtHkToggleWindow.ReadOnly = true;
            txtHkToggleWindow.Size = new Size(106, 32);
            txtHkToggleWindow.TabIndex = 24;
            txtHkToggleWindow.Text = "Ctrl+Shift+M";
            txtHkToggleWindow.TextAlign = HorizontalAlignment.Center;
            txtHkToggleWindow.UseSystemPasswordChar = false;
            // 
            // btnBindHkWindow
            // 
            btnBindHkWindow.AccentColor = Color.FromArgb(214, 28, 52);
            btnBindHkWindow.BackColor = Color.Transparent;
            btnBindHkWindow.CornerRadius = 5;
            btnBindHkWindow.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnBindHkWindow.ForeColor = Color.FromArgb(242, 242, 246);
            btnBindHkWindow.IsPrimaryAction = false;
            btnBindHkWindow.Location = new Point(532, 289);
            btnBindHkWindow.Name = "btnBindHkWindow";
            btnBindHkWindow.Size = new Size(54, 32);
            btnBindHkWindow.TabIndex = 25;
            btnBindHkWindow.Text = "Bind";
            btnBindHkWindow.Click += btnBindHkWindow_Click;
            // 
            // btnClearHkWindow
            // 
            btnClearHkWindow.AccentColor = Color.FromArgb(244, 63, 94);
            btnClearHkWindow.BackColor = Color.Transparent;
            btnClearHkWindow.CornerRadius = 5;
            btnClearHkWindow.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnClearHkWindow.ForeColor = Color.FromArgb(242, 242, 246);
            btnClearHkWindow.IsPrimaryAction = false;
            btnClearHkWindow.Location = new Point(592, 290);
            btnClearHkWindow.Name = "btnClearHkWindow";
            btnClearHkWindow.Size = new Size(64, 32);
            btnClearHkWindow.TabIndex = 26;
            btnClearHkWindow.Text = "Clear";
            btnClearHkWindow.Click += btnClearHkWindow_Click;
            // 
            // lblHkHint
            // 
            lblHkHint.AutoSize = true;
            lblHkHint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblHkHint.ForeColor = Color.FromArgb(162, 162, 176);
            lblHkHint.Location = new Point(16, 350);
            lblHkHint.Name = "lblHkHint";
            lblHkHint.Size = new Size(441, 13);
            lblHkHint.TabIndex = 27;
            lblHkHint.Text = "Tip: Click Bind, press your key combo (recommend using Ctrl/Shift/Alt), or press Esc to cancel.";
            // 
            // btnSaveHotkeys
            // 
            btnSaveHotkeys.AccentColor = Color.FromArgb(214, 28, 52);
            btnSaveHotkeys.BackColor = Color.Transparent;
            btnSaveHotkeys.CornerRadius = 5;
            btnSaveHotkeys.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSaveHotkeys.ForeColor = Color.FromArgb(242, 242, 246);
            btnSaveHotkeys.IsPrimaryAction = true;
            btnSaveHotkeys.Location = new Point(16, 414);
            btnSaveHotkeys.Name = "btnSaveHotkeys";
            btnSaveHotkeys.Size = new Size(444, 40);
            btnSaveHotkeys.TabIndex = 1;
            btnSaveHotkeys.Text = "Save & Apply Hotkeys";
            btnSaveHotkeys.Click += btnSaveHotkeys_Click;
            // 
            // btnResetHotkeys
            // 
            btnResetHotkeys.AccentColor = Color.FromArgb(214, 28, 52);
            btnResetHotkeys.BackColor = Color.Transparent;
            btnResetHotkeys.CornerRadius = 5;
            btnResetHotkeys.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnResetHotkeys.ForeColor = Color.FromArgb(242, 242, 246);
            btnResetHotkeys.IsPrimaryAction = false;
            btnResetHotkeys.Location = new Point(472, 414);
            btnResetHotkeys.Name = "btnResetHotkeys";
            btnResetHotkeys.Size = new Size(217, 40);
            btnResetHotkeys.TabIndex = 2;
            btnResetHotkeys.Text = "Reset Defaults";
            btnResetHotkeys.Click += btnResetHotkeys_Click;
            // 
            // pnlTools
            // 
            pnlTools.BackColor = Color.FromArgb(9, 9, 12);
            pnlTools.Controls.Add(grpSystemOptimzations);
            pnlTools.Controls.Add(grpCacheCleaners);
            pnlTools.Controls.Add(saveAdvancedCfgBtn);
            pnlTools.Dock = DockStyle.Fill;
            pnlTools.Location = new Point(0, 0);
            pnlTools.Name = "pnlTools";
            pnlTools.Size = new Size(705, 525);
            pnlTools.TabIndex = 2;
            pnlTools.Visible = false;
            // 
            // grpSystemOptimzations
            // 
            grpSystemOptimzations.BackColor = Color.Transparent;
            grpSystemOptimzations.Controls.Add(lblPCoresTitle);
            grpSystemOptimzations.Controls.Add(lblPCoresDesc);
            grpSystemOptimzations.Controls.Add(pCoresToggle);
            grpSystemOptimzations.Controls.Add(lblPowerPlanTitle);
            grpSystemOptimzations.Controls.Add(lblPowerPlanDesc);
            grpSystemOptimzations.Controls.Add(autoPowerPlanToggle);
            grpSystemOptimzations.Controls.Add(sepTool1);
            grpSystemOptimzations.Controls.Add(lblGameModeTitle);
            grpSystemOptimzations.Controls.Add(lblGameModeDesc);
            grpSystemOptimzations.Controls.Add(gameModeDvrToggle);
            grpSystemOptimzations.Controls.Add(lblFsoTitle);
            grpSystemOptimzations.Controls.Add(lblFsoDesc);
            grpSystemOptimzations.Controls.Add(disabledFSO);
            grpSystemOptimzations.Controls.Add(sepTool2);
            grpSystemOptimzations.Controls.Add(lblHagsTitle);
            grpSystemOptimzations.Controls.Add(lblHagsDesc);
            grpSystemOptimzations.Controls.Add(hagsToggle);
            grpSystemOptimzations.Controls.Add(lblTelemetryTitle);
            grpSystemOptimzations.Controls.Add(lblTelemetryDesc);
            grpSystemOptimzations.Controls.Add(telemetryToggle);
            grpSystemOptimzations.Controls.Add(sepTool3);
            grpSystemOptimzations.Controls.Add(lblPauseSearchTitle);
            grpSystemOptimzations.Controls.Add(lblPauseSearchDesc);
            grpSystemOptimzations.Controls.Add(pauseSearchToggle);
            grpSystemOptimzations.Controls.Add(sepTool4);
            grpSystemOptimzations.Controls.Add(lblThrottleTitle);
            grpSystemOptimzations.Controls.Add(lblThrottleDesc);
            grpSystemOptimzations.Controls.Add(btnEditThrottleList);
            grpSystemOptimzations.Controls.Add(throttleAppsToggle);
            grpSystemOptimzations.Controls.Add(sepTool5);
            grpSystemOptimzations.Controls.Add(lblFpsOverlayTitle);
            grpSystemOptimzations.Controls.Add(lblFpsOverlayDesc);
            grpSystemOptimzations.Controls.Add(btnToggle1PercentLow);
            grpSystemOptimzations.Controls.Add(fpsPositionDropdown);
            grpSystemOptimzations.Controls.Add(btnMoveFpsOverlay);
            grpSystemOptimzations.Controls.Add(fpsOverlayToggle);
            grpSystemOptimzations.Controls.Add(fpsOpacityDropdown);
            grpSystemOptimzations.Font = new Font("Segoe UI", 9.5F);
            grpSystemOptimzations.Location = new Point(16, 12);
            grpSystemOptimzations.Name = "grpSystemOptimzations";
            grpSystemOptimzations.Padding = new Padding(14, 44, 14, 14);
            grpSystemOptimzations.Size = new Size(673, 346);
            grpSystemOptimzations.SubTitle = "Applies automatically to all monitored games";
            grpSystemOptimzations.TabIndex = 0;
            grpSystemOptimzations.Title = "System Optimization";
            // 
            // lblPCoresTitle
            // 
            lblPCoresTitle.AutoSize = true;
            lblPCoresTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPCoresTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblPCoresTitle.Location = new Point(16, 42);
            lblPCoresTitle.Name = "lblPCoresTitle";
            lblPCoresTitle.Size = new Size(112, 15);
            lblPCoresTitle.TabIndex = 0;
            lblPCoresTitle.Text = "Physical Cores Only";
            // 
            // lblPCoresDesc
            // 
            lblPCoresDesc.AutoSize = true;
            lblPCoresDesc.Font = new Font("Segoe UI", 7.75F);
            lblPCoresDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblPCoresDesc.Location = new Point(16, 59);
            lblPCoresDesc.Name = "lblPCoresDesc";
            lblPCoresDesc.Size = new Size(203, 13);
            lblPCoresDesc.TabIndex = 1;
            lblPCoresDesc.Text = "Pin game to physical cores (avoid SMT)";
            // 
            // pCoresToggle
            // 
            pCoresToggle.BackColor = Color.Transparent;
            pCoresToggle.Checked = false;
            pCoresToggle.Location = new Point(276, 48);
            pCoresToggle.Name = "pCoresToggle";
            pCoresToggle.Size = new Size(46, 22);
            pCoresToggle.TabIndex = 2;
            // 
            // lblPowerPlanTitle
            // 
            lblPowerPlanTitle.AutoSize = true;
            lblPowerPlanTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPowerPlanTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblPowerPlanTitle.Location = new Point(344, 42);
            lblPowerPlanTitle.Name = "lblPowerPlanTitle";
            lblPowerPlanTitle.Size = new Size(162, 15);
            lblPowerPlanTitle.TabIndex = 3;
            lblPowerPlanTitle.Text = "Auto High / Ultimate Power";
            // 
            // lblPowerPlanDesc
            // 
            lblPowerPlanDesc.AutoSize = true;
            lblPowerPlanDesc.Font = new Font("Segoe UI", 7.75F);
            lblPowerPlanDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblPowerPlanDesc.Location = new Point(344, 59);
            lblPowerPlanDesc.Name = "lblPowerPlanDesc";
            lblPowerPlanDesc.Size = new Size(215, 13);
            lblPowerPlanDesc.TabIndex = 4;
            lblPowerPlanDesc.Text = "Gaming power plan, auto-reverts on exit";
            // 
            // autoPowerPlanToggle
            // 
            autoPowerPlanToggle.BackColor = Color.Transparent;
            autoPowerPlanToggle.Checked = false;
            autoPowerPlanToggle.Location = new Point(610, 48);
            autoPowerPlanToggle.Name = "autoPowerPlanToggle";
            autoPowerPlanToggle.Size = new Size(46, 22);
            autoPowerPlanToggle.TabIndex = 5;
            // 
            // sepTool1
            // 
            sepTool1.BackColor = Color.Transparent;
            sepTool1.Location = new Point(16, 82);
            sepTool1.Name = "sepTool1";
            sepTool1.Size = new Size(641, 6);
            sepTool1.TabIndex = 6;
            // 
            // lblGameModeTitle
            // 
            lblGameModeTitle.AutoSize = true;
            lblGameModeTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGameModeTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblGameModeTitle.Location = new Point(16, 92);
            lblGameModeTitle.Name = "lblGameModeTitle";
            lblGameModeTitle.Size = new Size(159, 15);
            lblGameModeTitle.TabIndex = 7;
            lblGameModeTitle.Text = "Game Mode ON & DVR Off";
            lblGameModeTitle.UseMnemonic = false;
            // 
            // lblGameModeDesc
            // 
            lblGameModeDesc.AutoSize = true;
            lblGameModeDesc.Font = new Font("Segoe UI", 7.75F);
            lblGameModeDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblGameModeDesc.Location = new Point(16, 109);
            lblGameModeDesc.Name = "lblGameModeDesc";
            lblGameModeDesc.Size = new Size(236, 13);
            lblGameModeDesc.TabIndex = 8;
            lblGameModeDesc.Text = "Enables Game Mode & disables DVR capture";
            lblGameModeDesc.UseMnemonic = false;
            // 
            // gameModeDvrToggle
            // 
            gameModeDvrToggle.BackColor = Color.Transparent;
            gameModeDvrToggle.Checked = false;
            gameModeDvrToggle.Location = new Point(276, 98);
            gameModeDvrToggle.Name = "gameModeDvrToggle";
            gameModeDvrToggle.Size = new Size(46, 22);
            gameModeDvrToggle.TabIndex = 9;
            // 
            // lblFsoTitle
            // 
            lblFsoTitle.AutoSize = true;
            lblFsoTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFsoTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblFsoTitle.Location = new Point(344, 92);
            lblFsoTitle.Name = "lblFsoTitle";
            lblFsoTitle.Size = new Size(163, 15);
            lblFsoTitle.TabIndex = 10;
            lblFsoTitle.Text = "Disable Fullscreen Opt (FSO)";
            // 
            // lblFsoDesc
            // 
            lblFsoDesc.AutoSize = true;
            lblFsoDesc.Font = new Font("Segoe UI", 7.75F);
            lblFsoDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblFsoDesc.Location = new Point(344, 109);
            lblFsoDesc.Name = "lblFsoDesc";
            lblFsoDesc.Size = new Size(225, 13);
            lblFsoDesc.TabIndex = 11;
            lblFsoDesc.Text = "Prevents DWM fullscreen latency overhead";
            // 
            // disabledFSO
            // 
            disabledFSO.BackColor = Color.Transparent;
            disabledFSO.Checked = false;
            disabledFSO.Location = new Point(610, 98);
            disabledFSO.Name = "disabledFSO";
            disabledFSO.Size = new Size(46, 22);
            disabledFSO.TabIndex = 12;
            // 
            // sepTool2
            // 
            sepTool2.BackColor = Color.Transparent;
            sepTool2.Location = new Point(16, 132);
            sepTool2.Name = "sepTool2";
            sepTool2.Size = new Size(641, 6);
            sepTool2.TabIndex = 13;
            // 
            // lblHagsTitle
            // 
            lblHagsTitle.AutoSize = true;
            lblHagsTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHagsTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblHagsTitle.Location = new Point(16, 142);
            lblHagsTitle.Name = "lblHagsTitle";
            lblHagsTitle.Size = new Size(184, 15);
            lblHagsTitle.TabIndex = 14;
            lblHagsTitle.Text = "HAGS GPU Scheduling (Reboot)";
            // 
            // lblHagsDesc
            // 
            lblHagsDesc.AutoSize = true;
            lblHagsDesc.Font = new Font("Segoe UI", 7.75F);
            lblHagsDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblHagsDesc.Location = new Point(16, 159);
            lblHagsDesc.Name = "lblHagsDesc";
            lblHagsDesc.Size = new Size(217, 13);
            lblHagsDesc.TabIndex = 15;
            lblHagsDesc.Text = "Hardware GPU scheduling policy (Admin)";
            // 
            // hagsToggle
            // 
            hagsToggle.BackColor = Color.Transparent;
            hagsToggle.Checked = false;
            hagsToggle.Location = new Point(276, 148);
            hagsToggle.Name = "hagsToggle";
            hagsToggle.Size = new Size(46, 22);
            hagsToggle.TabIndex = 16;
            // 
            // lblTelemetryTitle
            // 
            lblTelemetryTitle.AutoSize = true;
            lblTelemetryTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTelemetryTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblTelemetryTitle.Location = new Point(344, 142);
            lblTelemetryTitle.Name = "lblTelemetryTitle";
            lblTelemetryTitle.Size = new Size(181, 15);
            lblTelemetryTitle.TabIndex = 17;
            lblTelemetryTitle.Text = "Reduce Win Telemetry (Admin)";
            // 
            // lblTelemetryDesc
            // 
            lblTelemetryDesc.AutoSize = true;
            lblTelemetryDesc.Font = new Font("Segoe UI", 7.75F);
            lblTelemetryDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblTelemetryDesc.Location = new Point(344, 159);
            lblTelemetryDesc.Name = "lblTelemetryDesc";
            lblTelemetryDesc.Size = new Size(216, 13);
            lblTelemetryDesc.TabIndex = 18;
            lblTelemetryDesc.Text = "Reversible AllowTelemetry policy override";
            // 
            // telemetryToggle
            // 
            telemetryToggle.BackColor = Color.Transparent;
            telemetryToggle.Checked = false;
            telemetryToggle.Location = new Point(610, 148);
            telemetryToggle.Name = "telemetryToggle";
            telemetryToggle.Size = new Size(46, 22);
            telemetryToggle.TabIndex = 19;
            // 
            // sepTool3
            // 
            sepTool3.BackColor = Color.Transparent;
            sepTool3.Location = new Point(16, 182);
            sepTool3.Name = "sepTool3";
            sepTool3.Size = new Size(641, 6);
            sepTool3.TabIndex = 20;
            // 
            // lblPauseSearchTitle
            // 
            lblPauseSearchTitle.AutoSize = true;
            lblPauseSearchTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPauseSearchTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblPauseSearchTitle.Location = new Point(16, 192);
            lblPauseSearchTitle.Name = "lblPauseSearchTitle";
            lblPauseSearchTitle.Size = new Size(314, 15);
            lblPauseSearchTitle.TabIndex = 21;
            lblPauseSearchTitle.Text = "Pause Windows Search Indexing While Gaming (Admin)";
            // 
            // lblPauseSearchDesc
            // 
            lblPauseSearchDesc.AutoSize = true;
            lblPauseSearchDesc.Font = new Font("Segoe UI", 8F);
            lblPauseSearchDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblPauseSearchDesc.Location = new Point(16, 209);
            lblPauseSearchDesc.Name = "lblPauseSearchDesc";
            lblPauseSearchDesc.Size = new Size(440, 13);
            lblPauseSearchDesc.TabIndex = 22;
            lblPauseSearchDesc.Text = "Temporarily pauses WSearch disk indexing while a game runs and resumes it on exit.";
            // 
            // pauseSearchToggle
            // 
            pauseSearchToggle.BackColor = Color.Transparent;
            pauseSearchToggle.Checked = false;
            pauseSearchToggle.Location = new Point(610, 198);
            pauseSearchToggle.Name = "pauseSearchToggle";
            pauseSearchToggle.Size = new Size(46, 22);
            pauseSearchToggle.TabIndex = 23;
            // 
            // sepTool4
            // 
            sepTool4.BackColor = Color.Transparent;
            sepTool4.Location = new Point(16, 232);
            sepTool4.Name = "sepTool4";
            sepTool4.Size = new Size(641, 6);
            sepTool4.TabIndex = 24;
            // 
            // lblThrottleTitle
            // 
            lblThrottleTitle.AutoSize = true;
            lblThrottleTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblThrottleTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblThrottleTitle.Location = new Point(16, 242);
            lblThrottleTitle.Name = "lblThrottleTitle";
            lblThrottleTitle.Size = new Size(153, 15);
            lblThrottleTitle.TabIndex = 25;
            lblThrottleTitle.Text = "Throttle Background Apps";
            // 
            // lblThrottleDesc
            // 
            lblThrottleDesc.AutoSize = true;
            lblThrottleDesc.Font = new Font("Segoe UI", 8F);
            lblThrottleDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblThrottleDesc.Location = new Point(16, 259);
            lblThrottleDesc.Name = "lblThrottleDesc";
            lblThrottleDesc.Size = new Size(482, 13);
            lblThrottleDesc.TabIndex = 26;
            lblThrottleDesc.Text = "Lowers browsers and launchers to Below Normal priority while gaming, and restores on exit.";
            // 
            // btnEditThrottleList
            // 
            btnEditThrottleList.AccentColor = Color.FromArgb(214, 28, 52);
            btnEditThrottleList.BackColor = Color.Transparent;
            btnEditThrottleList.CornerRadius = 5;
            btnEditThrottleList.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnEditThrottleList.ForeColor = Color.FromArgb(242, 242, 246);
            btnEditThrottleList.IsPrimaryAction = false;
            btnEditThrottleList.Location = new Point(526, 244);
            btnEditThrottleList.Name = "btnEditThrottleList";
            btnEditThrottleList.Size = new Size(76, 30);
            btnEditThrottleList.TabIndex = 27;
            btnEditThrottleList.Text = "Edit List";
            btnEditThrottleList.Click += btnEditThrottleList_Click;
            // 
            // throttleAppsToggle
            // 
            throttleAppsToggle.BackColor = Color.Transparent;
            throttleAppsToggle.Checked = false;
            throttleAppsToggle.Location = new Point(610, 248);
            throttleAppsToggle.Name = "throttleAppsToggle";
            throttleAppsToggle.Size = new Size(46, 22);
            throttleAppsToggle.TabIndex = 28;
            // 
            // sepTool5
            // 
            sepTool5.BackColor = Color.Transparent;
            sepTool5.Location = new Point(16, 282);
            sepTool5.Name = "sepTool5";
            sepTool5.Size = new Size(641, 6);
            sepTool5.TabIndex = 29;
            // 
            // lblFpsOverlayTitle
            // 
            lblFpsOverlayTitle.AutoSize = true;
            lblFpsOverlayTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFpsOverlayTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblFpsOverlayTitle.Location = new Point(16, 294);
            lblFpsOverlayTitle.Name = "lblFpsOverlayTitle";
            lblFpsOverlayTitle.Size = new Size(172, 15);
            lblFpsOverlayTitle.TabIndex = 30;
            lblFpsOverlayTitle.Text = "In-Game FPS Overlay (Admin)";
            // 
            // lblFpsOverlayDesc
            // 
            lblFpsOverlayDesc.AutoSize = true;
            lblFpsOverlayDesc.Font = new Font("Segoe UI", 8F);
            lblFpsOverlayDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblFpsOverlayDesc.Location = new Point(16, 311);
            lblFpsOverlayDesc.Name = "lblFpsOverlayDesc";
            lblFpsOverlayDesc.Size = new Size(146, 13);
            lblFpsOverlayDesc.TabIndex = 31;
            lblFpsOverlayDesc.Text = "Zero-injection FPS counter.";
            // 
            // btnToggle1PercentLow
            // 
            btnToggle1PercentLow.AccentColor = Color.FromArgb(214, 28, 52);
            btnToggle1PercentLow.BackColor = Color.Transparent;
            btnToggle1PercentLow.CornerRadius = 5;
            btnToggle1PercentLow.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnToggle1PercentLow.ForeColor = Color.FromArgb(242, 242, 246);
            btnToggle1PercentLow.IsPrimaryAction = false;
            btnToggle1PercentLow.Location = new Point(226, 296);
            btnToggle1PercentLow.Name = "btnToggle1PercentLow";
            btnToggle1PercentLow.Size = new Size(92, 30);
            btnToggle1PercentLow.TabIndex = 32;
            btnToggle1PercentLow.Text = "1% Low: ON";
            btnToggle1PercentLow.Click += btnToggle1PercentLow_Click;
            // 
            // fpsPositionDropdown
            // 
            fpsPositionDropdown.BackColor = Color.FromArgb(22, 22, 29);
            fpsPositionDropdown.DrawMode = DrawMode.OwnerDrawFixed;
            fpsPositionDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            fpsPositionDropdown.Font = new Font("Segoe UI", 8.75F);
            fpsPositionDropdown.ForeColor = Color.FromArgb(242, 242, 246);
            fpsPositionDropdown.ItemHeight = 24;
            fpsPositionDropdown.Items.AddRange(new object[] { "Top Left", "Top Right", "Top Center", "Bottom Left", "Bottom Right", "Custom" });
            fpsPositionDropdown.Location = new Point(324, 296);
            fpsPositionDropdown.Name = "fpsPositionDropdown";
            fpsPositionDropdown.Size = new Size(118, 30);
            fpsPositionDropdown.TabIndex = 33;
            fpsPositionDropdown.SelectedIndexChanged += fpsPositionDropdown_SelectedIndexChanged;
            // 
            // btnMoveFpsOverlay
            // 
            btnMoveFpsOverlay.AccentColor = Color.FromArgb(214, 28, 52);
            btnMoveFpsOverlay.BackColor = Color.Transparent;
            btnMoveFpsOverlay.CornerRadius = 5;
            btnMoveFpsOverlay.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            btnMoveFpsOverlay.ForeColor = Color.FromArgb(242, 242, 246);
            btnMoveFpsOverlay.IsPrimaryAction = false;
            btnMoveFpsOverlay.Location = new Point(528, 296);
            btnMoveFpsOverlay.Name = "btnMoveFpsOverlay";
            btnMoveFpsOverlay.Size = new Size(74, 30);
            btnMoveFpsOverlay.TabIndex = 35;
            btnMoveFpsOverlay.Text = "Move";
            btnMoveFpsOverlay.Click += btnMoveFpsOverlay_Click;
            // 
            // fpsOverlayToggle
            // 
            fpsOverlayToggle.BackColor = Color.Transparent;
            fpsOverlayToggle.Checked = false;
            fpsOverlayToggle.Location = new Point(610, 300);
            fpsOverlayToggle.Name = "fpsOverlayToggle";
            fpsOverlayToggle.Size = new Size(46, 22);
            fpsOverlayToggle.TabIndex = 36;
            fpsOverlayToggle.CheckedChanged += fpsOverlayToggle_CheckedChanged;
            // 
            // fpsOpacityDropdown
            // 
            fpsOpacityDropdown.BackColor = Color.FromArgb(22, 22, 29);
            fpsOpacityDropdown.DrawMode = DrawMode.OwnerDrawFixed;
            fpsOpacityDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            fpsOpacityDropdown.Font = new Font("Segoe UI", 8.75F);
            fpsOpacityDropdown.ForeColor = Color.FromArgb(242, 242, 246);
            fpsOpacityDropdown.ItemHeight = 24;
            fpsOpacityDropdown.Items.AddRange(new object[] { "100%", "90%", "80%", "70%", "60%", "50%", "40%", "30%", "20%", "10%", "5%" });
            fpsOpacityDropdown.Location = new Point(448, 296);
            fpsOpacityDropdown.Name = "fpsOpacityDropdown";
            fpsOpacityDropdown.Size = new Size(74, 30);
            fpsOpacityDropdown.TabIndex = 34;
            fpsOpacityDropdown.SelectedIndexChanged += fpsOpacityDropdown_SelectedIndexChanged;
            // 
            // grpCacheCleaners
            // 
            grpCacheCleaners.BackColor = Color.Transparent;
            grpCacheCleaners.Controls.Add(clearShaderCacheBtn);
            grpCacheCleaners.Controls.Add(btnOpenGameFolder);
            grpCacheCleaners.Controls.Add(btnOpenBenchmarks);
            grpCacheCleaners.Font = new Font("Segoe UI", 9.5F);
            grpCacheCleaners.Location = new Point(16, 366);
            grpCacheCleaners.Name = "grpCacheCleaners";
            grpCacheCleaners.Padding = new Padding(14, 44, 14, 14);
            grpCacheCleaners.Size = new Size(673, 98);
            grpCacheCleaners.SubTitle = "Note: Shaders rebuild during first 5 or so minutes in-game";
            grpCacheCleaners.TabIndex = 1;
            grpCacheCleaners.Title = "Cache Cleaning & Utilities";
            // 
            // clearShaderCacheBtn
            // 
            clearShaderCacheBtn.AccentColor = Color.FromArgb(214, 28, 52);
            clearShaderCacheBtn.BackColor = Color.Transparent;
            clearShaderCacheBtn.CornerRadius = 5;
            clearShaderCacheBtn.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            clearShaderCacheBtn.ForeColor = Color.FromArgb(242, 242, 246);
            clearShaderCacheBtn.IsPrimaryAction = false;
            clearShaderCacheBtn.Location = new Point(14, 48);
            clearShaderCacheBtn.Name = "clearShaderCacheBtn";
            clearShaderCacheBtn.Size = new Size(228, 36);
            clearShaderCacheBtn.TabIndex = 0;
            clearShaderCacheBtn.Text = "Clear DirectX / GPU Shader Cache";
            clearShaderCacheBtn.Click += clearShaderCacheBtn_Click;
            // 
            // btnOpenGameFolder
            // 
            btnOpenGameFolder.AccentColor = Color.FromArgb(214, 28, 52);
            btnOpenGameFolder.BackColor = Color.Transparent;
            btnOpenGameFolder.CornerRadius = 5;
            btnOpenGameFolder.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnOpenGameFolder.ForeColor = Color.FromArgb(242, 242, 246);
            btnOpenGameFolder.IsPrimaryAction = false;
            btnOpenGameFolder.Location = new Point(250, 48);
            btnOpenGameFolder.Name = "btnOpenGameFolder";
            btnOpenGameFolder.Size = new Size(200, 36);
            btnOpenGameFolder.TabIndex = 1;
            btnOpenGameFolder.Text = "Open Active Game Folder";
            btnOpenGameFolder.Click += btnOpenGameFolder_Click;
            // 
            // btnOpenBenchmarks
            // 
            btnOpenBenchmarks.AccentColor = Color.FromArgb(214, 28, 52);
            btnOpenBenchmarks.BackColor = Color.Transparent;
            btnOpenBenchmarks.CornerRadius = 5;
            btnOpenBenchmarks.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnOpenBenchmarks.ForeColor = Color.FromArgb(242, 242, 246);
            btnOpenBenchmarks.IsPrimaryAction = false;
            btnOpenBenchmarks.Location = new Point(458, 48);
            btnOpenBenchmarks.Name = "btnOpenBenchmarks";
            btnOpenBenchmarks.Size = new Size(201, 36);
            btnOpenBenchmarks.TabIndex = 2;
            btnOpenBenchmarks.Text = "Session Benchmarks...";
            btnOpenBenchmarks.Click += btnOpenBenchmarks_Click;
            // 
            // saveAdvancedCfgBtn
            // 
            saveAdvancedCfgBtn.AccentColor = Color.FromArgb(214, 28, 52);
            saveAdvancedCfgBtn.BackColor = Color.Transparent;
            saveAdvancedCfgBtn.CornerRadius = 5;
            saveAdvancedCfgBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            saveAdvancedCfgBtn.ForeColor = Color.FromArgb(242, 242, 246);
            saveAdvancedCfgBtn.IsPrimaryAction = true;
            saveAdvancedCfgBtn.Location = new Point(16, 472);
            saveAdvancedCfgBtn.Name = "saveAdvancedCfgBtn";
            saveAdvancedCfgBtn.Size = new Size(673, 40);
            saveAdvancedCfgBtn.TabIndex = 2;
            saveAdvancedCfgBtn.Text = "Save & Apply System Optimizations";
            saveAdvancedCfgBtn.Click += saveAdvancedCfgBtn_Click;
            // 
            // pnlProfiles
            // 
            pnlProfiles.BackColor = Color.FromArgb(9, 9, 12);
            pnlProfiles.Controls.Add(grpProfiles);
            pnlProfiles.Controls.Add(grpPerGameOptimzations);
            pnlProfiles.Dock = DockStyle.Fill;
            pnlProfiles.Location = new Point(0, 0);
            pnlProfiles.Name = "pnlProfiles";
            pnlProfiles.Size = new Size(705, 525);
            pnlProfiles.TabIndex = 1;
            pnlProfiles.Visible = false;
            // 
            // grpProfiles
            // 
            grpProfiles.BackColor = Color.Transparent;
            grpProfiles.Controls.Add(cardProfCompetitive);
            grpProfiles.Controls.Add(cardProfBalanced);
            grpProfiles.Controls.Add(cardProfRecommended);
            grpProfiles.Controls.Add(cardProfUltra);
            grpProfiles.Controls.Add(txtConfigDir);
            grpProfiles.Controls.Add(btnBrowseConfigDir);
            grpProfiles.Controls.Add(lblFpsLimit);
            grpProfiles.Controls.Add(fpsLimitDropdown);
            grpProfiles.Controls.Add(lblScreenMode);
            grpProfiles.Controls.Add(screenModeDropdown);
            grpProfiles.Controls.Add(lblLumen);
            grpProfiles.Controls.Add(lumenToggle);
            grpProfiles.Controls.Add(lblFog);
            grpProfiles.Controls.Add(fogToggle);
            grpProfiles.Controls.Add(profileDropdown);
            grpProfiles.Controls.Add(autoDetectBtn);
            grpProfiles.Controls.Add(optimizeBtn);
            grpProfiles.Font = new Font("Segoe UI", 9.5F);
            grpProfiles.Location = new Point(16, 12);
            grpProfiles.Name = "grpProfiles";
            grpProfiles.Padding = new Padding(14, 44, 14, 14);
            grpProfiles.Size = new Size(673, 304);
            grpProfiles.SubTitle = "Select a game on the Dashboard to tune its configs";
            grpProfiles.TabIndex = 0;
            grpProfiles.Title = "Engine & Config Profiles";
            // 
            // cardProfCompetitive
            // 
            cardProfCompetitive.BackColor = Color.Transparent;
            cardProfCompetitive.BadgeCharacter = "⚡";
            cardProfCompetitive.Location = new Point(14, 44);
            cardProfCompetitive.Name = "cardProfCompetitive";
            cardProfCompetitive.Selected = false;
            cardProfCompetitive.Size = new Size(316, 56);
            cardProfCompetitive.Subtitle = "Lowest visuals for maximum FPS & lowest latency";
            cardProfCompetitive.TabIndex = 0;
            cardProfCompetitive.Title = "Performance (Max FPS)";
            // 
            // cardProfBalanced
            // 
            cardProfBalanced.BackColor = Color.Transparent;
            cardProfBalanced.BadgeCharacter = "◈";
            cardProfBalanced.Location = new Point(342, 44);
            cardProfBalanced.Name = "cardProfBalanced";
            cardProfBalanced.Selected = false;
            cardProfBalanced.Size = new Size(317, 56);
            cardProfBalanced.Subtitle = "Clean visuals balanced with smooth frame pacing";
            cardProfBalanced.TabIndex = 1;
            cardProfBalanced.Title = "Balanced";
            // 
            // cardProfRecommended
            // 
            cardProfRecommended.BackColor = Color.Transparent;
            cardProfRecommended.BadgeCharacter = "★";
            cardProfRecommended.Location = new Point(14, 106);
            cardProfRecommended.Name = "cardProfRecommended";
            cardProfRecommended.Selected = false;
            cardProfRecommended.Size = new Size(316, 56);
            cardProfRecommended.Subtitle = "Best all-around hardware-scaled preset";
            cardProfRecommended.TabIndex = 2;
            cardProfRecommended.Title = "Recommended (Optimized)";
            // 
            // cardProfUltra
            // 
            cardProfUltra.BackColor = Color.Transparent;
            cardProfUltra.BadgeCharacter = "✦";
            cardProfUltra.Location = new Point(342, 106);
            cardProfUltra.Name = "cardProfUltra";
            cardProfUltra.Selected = false;
            cardProfUltra.Size = new Size(317, 56);
            cardProfUltra.Subtitle = "Maximum visual fidelity for high-end rigs";
            cardProfUltra.TabIndex = 3;
            cardProfUltra.Title = "Ultra (Maximum Visuals)";
            // 
            // txtConfigDir
            // 
            txtConfigDir.BackColor = Color.Transparent;
            txtConfigDir.Location = new Point(14, 170);
            txtConfigDir.MaxLength = 32767;
            txtConfigDir.Multiline = false;
            txtConfigDir.Name = "txtConfigDir";
            txtConfigDir.ReadOnly = false;
            txtConfigDir.Size = new Size(490, 32);
            txtConfigDir.TabIndex = 4;
            txtConfigDir.TextAlign = HorizontalAlignment.Left;
            txtConfigDir.UseSystemPasswordChar = false;
            // 
            // btnBrowseConfigDir
            // 
            btnBrowseConfigDir.AccentColor = Color.FromArgb(214, 28, 52);
            btnBrowseConfigDir.BackColor = Color.Transparent;
            btnBrowseConfigDir.CornerRadius = 5;
            btnBrowseConfigDir.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnBrowseConfigDir.ForeColor = Color.FromArgb(242, 242, 246);
            btnBrowseConfigDir.IsPrimaryAction = false;
            btnBrowseConfigDir.Location = new Point(510, 170);
            btnBrowseConfigDir.Name = "btnBrowseConfigDir";
            btnBrowseConfigDir.Size = new Size(149, 32);
            btnBrowseConfigDir.TabIndex = 5;
            btnBrowseConfigDir.Text = "Config Directory...";
            btnBrowseConfigDir.Click += btnBrowseConfigDir_Click;
            // 
            // lblFpsLimit
            // 
            lblFpsLimit.AutoSize = true;
            lblFpsLimit.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblFpsLimit.ForeColor = Color.FromArgb(162, 162, 176);
            lblFpsLimit.Location = new Point(14, 208);
            lblFpsLimit.Name = "lblFpsLimit";
            lblFpsLimit.Size = new Size(58, 13);
            lblFpsLimit.TabIndex = 6;
            lblFpsLimit.Text = "FPS Limit:";
            // 
            // fpsLimitDropdown
            // 
            fpsLimitDropdown.BackColor = Color.FromArgb(22, 22, 29);
            fpsLimitDropdown.DrawMode = DrawMode.OwnerDrawFixed;
            fpsLimitDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            fpsLimitDropdown.Font = new Font("Segoe UI", 9F);
            fpsLimitDropdown.ForeColor = Color.FromArgb(242, 242, 246);
            fpsLimitDropdown.ItemHeight = 24;
            fpsLimitDropdown.Items.AddRange(new object[] { "Unlimited (0)", "60", "90", "120", "144", "165", "180", "200", "240", "300" });
            fpsLimitDropdown.Location = new Point(14, 224);
            fpsLimitDropdown.Name = "fpsLimitDropdown";
            fpsLimitDropdown.Size = new Size(166, 30);
            fpsLimitDropdown.TabIndex = 7;
            // 
            // lblScreenMode
            // 
            lblScreenMode.AutoSize = true;
            lblScreenMode.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblScreenMode.ForeColor = Color.FromArgb(162, 162, 176);
            lblScreenMode.Location = new Point(190, 208);
            lblScreenMode.Name = "lblScreenMode";
            lblScreenMode.Size = new Size(78, 13);
            lblScreenMode.TabIndex = 8;
            lblScreenMode.Text = "Screen Mode:";
            // 
            // screenModeDropdown
            // 
            screenModeDropdown.BackColor = Color.FromArgb(22, 22, 29);
            screenModeDropdown.DrawMode = DrawMode.OwnerDrawFixed;
            screenModeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            screenModeDropdown.Font = new Font("Segoe UI", 9F);
            screenModeDropdown.ForeColor = Color.FromArgb(242, 242, 246);
            screenModeDropdown.ItemHeight = 24;
            screenModeDropdown.Items.AddRange(new object[] { "Exclusive Fullscreen", "Borderless" });
            screenModeDropdown.Location = new Point(190, 224);
            screenModeDropdown.Name = "screenModeDropdown";
            screenModeDropdown.Size = new Size(182, 30);
            screenModeDropdown.TabIndex = 9;
            // 
            // lblLumen
            // 
            lblLumen.AutoSize = true;
            lblLumen.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblLumen.ForeColor = Color.FromArgb(162, 162, 176);
            lblLumen.Location = new Point(386, 230);
            lblLumen.Name = "lblLumen";
            lblLumen.Size = new Size(63, 13);
            lblLumen.TabIndex = 10;
            lblLumen.Text = "UE Lumen:";
            // 
            // lumenToggle
            // 
            lumenToggle.BackColor = Color.Transparent;
            lumenToggle.Checked = false;
            lumenToggle.Location = new Point(460, 226);
            lumenToggle.Name = "lumenToggle";
            lumenToggle.Size = new Size(46, 22);
            lumenToggle.TabIndex = 11;
            // 
            // lblFog
            // 
            lblFog.AutoSize = true;
            lblFog.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblFog.ForeColor = Color.FromArgb(162, 162, 176);
            lblFog.Location = new Point(526, 230);
            lblFog.Name = "lblFog";
            lblFog.Size = new Size(47, 13);
            lblFog.TabIndex = 12;
            lblFog.Text = "UE Fog:";
            // 
            // fogToggle
            // 
            fogToggle.BackColor = Color.Transparent;
            fogToggle.Checked = false;
            fogToggle.Location = new Point(584, 226);
            fogToggle.Name = "fogToggle";
            fogToggle.Size = new Size(46, 22);
            fogToggle.TabIndex = 13;
            // 
            // profileDropdown
            // 
            profileDropdown.BackColor = Color.FromArgb(22, 22, 29);
            profileDropdown.DrawMode = DrawMode.OwnerDrawFixed;
            profileDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            profileDropdown.Font = new Font("Segoe UI", 9.25F);
            profileDropdown.ForeColor = Color.FromArgb(242, 242, 246);
            profileDropdown.ItemHeight = 26;
            profileDropdown.Location = new Point(14, 262);
            profileDropdown.Name = "profileDropdown";
            profileDropdown.Size = new Size(358, 32);
            profileDropdown.TabIndex = 14;
            // 
            // autoDetectBtn
            // 
            autoDetectBtn.AccentColor = Color.FromArgb(214, 28, 52);
            autoDetectBtn.BackColor = Color.Transparent;
            autoDetectBtn.CornerRadius = 5;
            autoDetectBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            autoDetectBtn.ForeColor = Color.FromArgb(242, 242, 246);
            autoDetectBtn.IsPrimaryAction = false;
            autoDetectBtn.Location = new Point(382, 262);
            autoDetectBtn.Name = "autoDetectBtn";
            autoDetectBtn.Size = new Size(126, 32);
            autoDetectBtn.TabIndex = 15;
            autoDetectBtn.Text = "Auto-Detect";
            autoDetectBtn.Click += autoDetectBtn_Click;
            // 
            // optimizeBtn
            // 
            optimizeBtn.AccentColor = Color.FromArgb(214, 28, 52);
            optimizeBtn.BackColor = Color.Transparent;
            optimizeBtn.CornerRadius = 5;
            optimizeBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            optimizeBtn.ForeColor = Color.FromArgb(242, 242, 246);
            optimizeBtn.IsPrimaryAction = true;
            optimizeBtn.Location = new Point(516, 262);
            optimizeBtn.Name = "optimizeBtn";
            optimizeBtn.Size = new Size(143, 32);
            optimizeBtn.TabIndex = 16;
            optimizeBtn.Text = "Apply Config";
            optimizeBtn.Click += optimizeBtn_Click;
            // 
            // grpPerGameOptimzations
            // 
            grpPerGameOptimzations.BackColor = Color.Transparent;
            grpPerGameOptimzations.Controls.Add(lblGameHighPrio);
            grpPerGameOptimzations.Controls.Add(gameHighPriorityToggle);
            grpPerGameOptimzations.Controls.Add(lblGameGpuBind);
            grpPerGameOptimzations.Controls.Add(gameGpuBindingToggle);
            grpPerGameOptimzations.Controls.Add(btnApplyGame1);
            grpPerGameOptimzations.Controls.Add(saveBackupBtn);
            grpPerGameOptimzations.Controls.Add(restoreBackupBtn);
            grpPerGameOptimzations.Controls.Add(btnOpenPacksFolder);
            grpPerGameOptimzations.Controls.Add(btnGameAutoOptimizes);
            grpPerGameOptimzations.Controls.Add(btnPackBuilder);
            grpPerGameOptimzations.Font = new Font("Segoe UI", 9.5F);
            grpPerGameOptimzations.Location = new Point(16, 322);
            grpPerGameOptimzations.Name = "grpPerGameOptimzations";
            grpPerGameOptimzations.Padding = new Padding(14, 44, 14, 14);
            grpPerGameOptimzations.Size = new Size(673, 188);
            grpPerGameOptimzations.SubTitle = "Anti-cheat safe Windows App Optimizations";
            grpPerGameOptimzations.TabIndex = 1;
            grpPerGameOptimzations.Title = "Game Optimizations & Backups";
            // 
            // lblGameHighPrio
            // 
            lblGameHighPrio.AutoSize = true;
            lblGameHighPrio.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGameHighPrio.ForeColor = Color.FromArgb(242, 242, 246);
            lblGameHighPrio.Location = new Point(16, 54);
            lblGameHighPrio.Name = "lblGameHighPrio";
            lblGameHighPrio.Size = new Size(150, 15);
            lblGameHighPrio.TabIndex = 0;
            lblGameHighPrio.Text = "High CPU Priority (Admin)";
            // 
            // gameHighPriorityToggle
            // 
            gameHighPriorityToggle.BackColor = Color.Transparent;
            gameHighPriorityToggle.Checked = false;
            gameHighPriorityToggle.Location = new Point(274, 52);
            gameHighPriorityToggle.Name = "gameHighPriorityToggle";
            gameHighPriorityToggle.Size = new Size(46, 22);
            gameHighPriorityToggle.TabIndex = 1;
            // 
            // lblGameGpuBind
            // 
            lblGameGpuBind.AutoSize = true;
            lblGameGpuBind.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGameGpuBind.ForeColor = Color.FromArgb(242, 242, 246);
            lblGameGpuBind.Location = new Point(356, 54);
            lblGameGpuBind.Name = "lblGameGpuBind";
            lblGameGpuBind.Size = new Size(184, 15);
            lblGameGpuBind.TabIndex = 2;
            lblGameGpuBind.Text = "High-Performance GPU Binding";
            // 
            // gameGpuBindingToggle
            // 
            gameGpuBindingToggle.BackColor = Color.Transparent;
            gameGpuBindingToggle.Checked = false;
            gameGpuBindingToggle.Location = new Point(612, 52);
            gameGpuBindingToggle.Name = "gameGpuBindingToggle";
            gameGpuBindingToggle.Size = new Size(46, 22);
            gameGpuBindingToggle.TabIndex = 3;
            // 
            // btnApplyGame1
            // 
            btnApplyGame1.AccentColor = Color.FromArgb(214, 28, 52);
            btnApplyGame1.BackColor = Color.Transparent;
            btnApplyGame1.CornerRadius = 5;
            btnApplyGame1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnApplyGame1.ForeColor = Color.FromArgb(242, 242, 246);
            btnApplyGame1.IsPrimaryAction = true;
            btnApplyGame1.Location = new Point(449, 96);
            btnApplyGame1.Name = "btnApplyGame1";
            btnApplyGame1.Size = new Size(210, 36);
            btnApplyGame1.TabIndex = 8;
            btnApplyGame1.Text = "Save & Apply";
            btnApplyGame1.Click += btnApplyGame_Click;
            // 
            // saveBackupBtn
            // 
            saveBackupBtn.AccentColor = Color.FromArgb(214, 28, 52);
            saveBackupBtn.BackColor = Color.Transparent;
            saveBackupBtn.CornerRadius = 5;
            saveBackupBtn.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            saveBackupBtn.ForeColor = Color.FromArgb(242, 242, 246);
            saveBackupBtn.IsPrimaryAction = false;
            saveBackupBtn.Location = new Point(14, 96);
            saveBackupBtn.Name = "saveBackupBtn";
            saveBackupBtn.Size = new Size(134, 36);
            saveBackupBtn.TabIndex = 9;
            saveBackupBtn.Text = "Backup Configs";
            saveBackupBtn.Click += saveBackupBtn_Click;
            // 
            // restoreBackupBtn
            // 
            restoreBackupBtn.AccentColor = Color.FromArgb(214, 28, 52);
            restoreBackupBtn.BackColor = Color.Transparent;
            restoreBackupBtn.CornerRadius = 5;
            restoreBackupBtn.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            restoreBackupBtn.ForeColor = Color.FromArgb(242, 242, 246);
            restoreBackupBtn.IsPrimaryAction = false;
            restoreBackupBtn.Location = new Point(158, 96);
            restoreBackupBtn.Name = "restoreBackupBtn";
            restoreBackupBtn.Size = new Size(134, 36);
            restoreBackupBtn.TabIndex = 10;
            restoreBackupBtn.Text = "Restore Backup";
            restoreBackupBtn.Click += restoreBackupBtn_Click;
            // 
            // btnOpenPacksFolder
            // 
            btnOpenPacksFolder.AccentColor = Color.FromArgb(214, 28, 52);
            btnOpenPacksFolder.BackColor = Color.Transparent;
            btnOpenPacksFolder.CornerRadius = 5;
            btnOpenPacksFolder.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnOpenPacksFolder.ForeColor = Color.FromArgb(242, 242, 246);
            btnOpenPacksFolder.IsPrimaryAction = false;
            btnOpenPacksFolder.Location = new Point(302, 96);
            btnOpenPacksFolder.Name = "btnOpenPacksFolder";
            btnOpenPacksFolder.Size = new Size(137, 36);
            btnOpenPacksFolder.TabIndex = 11;
            btnOpenPacksFolder.Text = "JSON Packs...";
            btnOpenPacksFolder.Click += btnOpenPacksFolder_Click;
            // 
            // btnGameAutoOptimizes
            // 
            btnGameAutoOptimizes.AccentColor = Color.FromArgb(214, 28, 52);
            btnGameAutoOptimizes.BackColor = Color.Transparent;
            btnGameAutoOptimizes.CornerRadius = 5;
            btnGameAutoOptimizes.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnGameAutoOptimizes.ForeColor = Color.FromArgb(242, 242, 246);
            btnGameAutoOptimizes.IsPrimaryAction = false;
            btnGameAutoOptimizes.Location = new Point(14, 140);
            btnGameAutoOptimizes.Name = "btnGameAutoOptimizes";
            btnGameAutoOptimizes.Size = new Size(316, 36);
            btnGameAutoOptimizes.TabIndex = 12;
            btnGameAutoOptimizes.Text = "Per-Game Optimizations";
            btnGameAutoOptimizes.Click += btnGameAutoOptimizes_Click;
            // 
            // btnPackBuilder
            // 
            btnPackBuilder.AccentColor = Color.FromArgb(214, 28, 52);
            btnPackBuilder.BackColor = Color.Transparent;
            btnPackBuilder.CornerRadius = 5;
            btnPackBuilder.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            btnPackBuilder.ForeColor = Color.FromArgb(242, 242, 246);
            btnPackBuilder.IsPrimaryAction = false;
            btnPackBuilder.Location = new Point(342, 140);
            btnPackBuilder.Name = "btnPackBuilder";
            btnPackBuilder.Size = new Size(317, 36);
            btnPackBuilder.TabIndex = 13;
            btnPackBuilder.Text = "Open Game Pack Builder...";
            btnPackBuilder.Click += btnPackBuilder_Click;
            // 
            // pnlNetwork
            // 
            pnlNetwork.BackColor = Color.FromArgb(9, 9, 12);
            pnlNetwork.Controls.Add(grpNetworkOptimzations);
            pnlNetwork.Controls.Add(btnApplyNetworkOptimzations);
            pnlNetwork.Controls.Add(btnFlushDns);
            pnlNetwork.Controls.Add(grpPingMonitor);
            pnlNetwork.Dock = DockStyle.Fill;
            pnlNetwork.Location = new Point(0, 0);
            pnlNetwork.Name = "pnlNetwork";
            pnlNetwork.Size = new Size(705, 525);
            pnlNetwork.TabIndex = 4;
            pnlNetwork.Visible = false;
            // 
            // grpNetworkOptimzations
            // 
            grpNetworkOptimzations.BackColor = Color.Transparent;
            grpNetworkOptimzations.Controls.Add(lblNicPowerTitle);
            grpNetworkOptimzations.Controls.Add(lblNicPowerDesc);
            grpNetworkOptimzations.Controls.Add(disableNicPowerToggle);
            grpNetworkOptimzations.Controls.Add(sepNet1);
            grpNetworkOptimzations.Controls.Add(lblIntModTitle);
            grpNetworkOptimzations.Controls.Add(lblIntModDesc);
            grpNetworkOptimzations.Controls.Add(disableIntModToggle);
            grpNetworkOptimzations.Controls.Add(lblRssTitle);
            grpNetworkOptimzations.Controls.Add(lblRssDesc);
            grpNetworkOptimzations.Controls.Add(enableRssToggle);
            grpNetworkOptimzations.Controls.Add(sepNet2);
            grpNetworkOptimzations.Controls.Add(lblQosTitle);
            grpNetworkOptimzations.Controls.Add(lblQosDesc);
            grpNetworkOptimzations.Controls.Add(qosDscpToggle);
            grpNetworkOptimzations.Controls.Add(sepNet3);
            grpNetworkOptimzations.Controls.Add(lblDeliveryOptTitle);
            grpNetworkOptimzations.Controls.Add(lblDeliveryOptDesc);
            grpNetworkOptimzations.Controls.Add(deliveryOptToggle);
            grpNetworkOptimzations.Font = new Font("Segoe UI", 9.5F);
            grpNetworkOptimzations.Location = new Point(16, 12);
            grpNetworkOptimzations.Name = "grpNetworkOptimzations";
            grpNetworkOptimzations.Padding = new Padding(14, 44, 14, 14);
            grpNetworkOptimzations.Size = new Size(673, 244);
            grpNetworkOptimzations.SubTitle = "Requires Administrator privileges";
            grpNetworkOptimzations.TabIndex = 0;
            grpNetworkOptimzations.Title = "Network Optimizations";
            // 
            // lblNicPowerTitle
            // 
            lblNicPowerTitle.AutoSize = true;
            lblNicPowerTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNicPowerTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblNicPowerTitle.Location = new Point(16, 40);
            lblNicPowerTitle.Name = "lblNicPowerTitle";
            lblNicPowerTitle.Size = new Size(331, 15);
            lblNicPowerTitle.TabIndex = 0;
            lblNicPowerTitle.Text = "Disable NIC Power Saving / Energy-Efficient Ethernet (EEE)";
            // 
            // lblNicPowerDesc
            // 
            lblNicPowerDesc.AutoSize = true;
            lblNicPowerDesc.Font = new Font("Segoe UI", 8F);
            lblNicPowerDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblNicPowerDesc.Location = new Point(16, 57);
            lblNicPowerDesc.Name = "lblNicPowerDesc";
            lblNicPowerDesc.Size = new Size(506, 13);
            lblNicPowerDesc.TabIndex = 1;
            lblNicPowerDesc.Text = "Disables EEE, Green Ethernet and power sleep on active Ethernet NICs to stop random lag spikes.";
            // 
            // disableNicPowerToggle
            // 
            disableNicPowerToggle.BackColor = Color.Transparent;
            disableNicPowerToggle.Checked = false;
            disableNicPowerToggle.Location = new Point(610, 46);
            disableNicPowerToggle.Name = "disableNicPowerToggle";
            disableNicPowerToggle.Size = new Size(46, 22);
            disableNicPowerToggle.TabIndex = 2;
            // 
            // sepNet1
            // 
            sepNet1.BackColor = Color.Transparent;
            sepNet1.Location = new Point(16, 80);
            sepNet1.Name = "sepNet1";
            sepNet1.Size = new Size(641, 6);
            sepNet1.TabIndex = 3;
            // 
            // lblIntModTitle
            // 
            lblIntModTitle.AutoSize = true;
            lblIntModTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblIntModTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblIntModTitle.Location = new Point(16, 90);
            lblIntModTitle.Name = "lblIntModTitle";
            lblIntModTitle.Size = new Size(170, 15);
            lblIntModTitle.TabIndex = 4;
            lblIntModTitle.Text = "Disable Interrupt Moderation";
            // 
            // lblIntModDesc
            // 
            lblIntModDesc.AutoSize = true;
            lblIntModDesc.Font = new Font("Segoe UI", 7.75F);
            lblIntModDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblIntModDesc.Location = new Point(16, 107);
            lblIntModDesc.Name = "lblIntModDesc";
            lblIntModDesc.Size = new Size(176, 13);
            lblIntModDesc.TabIndex = 5;
            lblIntModDesc.Text = "Cuts NIC packet batching latency";
            // 
            // disableIntModToggle
            // 
            disableIntModToggle.BackColor = Color.Transparent;
            disableIntModToggle.Checked = false;
            disableIntModToggle.Location = new Point(276, 96);
            disableIntModToggle.Name = "disableIntModToggle";
            disableIntModToggle.Size = new Size(46, 22);
            disableIntModToggle.TabIndex = 6;
            // 
            // lblRssTitle
            // 
            lblRssTitle.AutoSize = true;
            lblRssTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRssTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblRssTitle.Location = new Point(344, 90);
            lblRssTitle.Name = "lblRssTitle";
            lblRssTitle.Size = new Size(154, 15);
            lblRssTitle.TabIndex = 7;
            lblRssTitle.Text = "Receive Side Scaling (RSS)";
            // 
            // lblRssDesc
            // 
            lblRssDesc.AutoSize = true;
            lblRssDesc.Font = new Font("Segoe UI", 7.75F);
            lblRssDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblRssDesc.Location = new Point(344, 107);
            lblRssDesc.Name = "lblRssDesc";
            lblRssDesc.Size = new Size(184, 13);
            lblRssDesc.TabIndex = 8;
            lblRssDesc.Text = "Spreads NIC load across CPU cores";
            // 
            // enableRssToggle
            // 
            enableRssToggle.BackColor = Color.Transparent;
            enableRssToggle.Checked = true;
            enableRssToggle.Location = new Point(610, 96);
            enableRssToggle.Name = "enableRssToggle";
            enableRssToggle.Size = new Size(46, 22);
            enableRssToggle.TabIndex = 9;
            // 
            // sepNet2
            // 
            sepNet2.BackColor = Color.Transparent;
            sepNet2.Location = new Point(16, 130);
            sepNet2.Name = "sepNet2";
            sepNet2.Size = new Size(641, 6);
            sepNet2.TabIndex = 10;
            // 
            // lblQosTitle
            // 
            lblQosTitle.AutoSize = true;
            lblQosTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblQosTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblQosTitle.Location = new Point(16, 140);
            lblQosTitle.Name = "lblQosTitle";
            lblQosTitle.Size = new Size(230, 15);
            lblQosTitle.TabIndex = 11;
            lblQosTitle.Text = "QoS Expedited Tagging for Active Game";
            // 
            // lblQosDesc
            // 
            lblQosDesc.AutoSize = true;
            lblQosDesc.Font = new Font("Segoe UI", 8F);
            lblQosDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblQosDesc.Location = new Point(16, 157);
            lblQosDesc.Name = "lblQosDesc";
            lblQosDesc.Size = new Size(514, 13);
            lblQosDesc.TabIndex = 12;
            lblQosDesc.Text = "Tags the selected game's UDP packets with DSCP 46 (Expedited Forwarding) for QoS-aware routers.";
            // 
            // qosDscpToggle
            // 
            qosDscpToggle.BackColor = Color.Transparent;
            qosDscpToggle.Checked = false;
            qosDscpToggle.Location = new Point(610, 146);
            qosDscpToggle.Name = "qosDscpToggle";
            qosDscpToggle.Size = new Size(46, 22);
            qosDscpToggle.TabIndex = 13;
            // 
            // sepNet3
            // 
            sepNet3.BackColor = Color.Transparent;
            sepNet3.Location = new Point(16, 180);
            sepNet3.Name = "sepNet3";
            sepNet3.Size = new Size(641, 6);
            sepNet3.TabIndex = 14;
            // 
            // lblDeliveryOptTitle
            // 
            lblDeliveryOptTitle.AutoSize = true;
            lblDeliveryOptTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDeliveryOptTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblDeliveryOptTitle.Location = new Point(16, 190);
            lblDeliveryOptTitle.Name = "lblDeliveryOptTitle";
            lblDeliveryOptTitle.Size = new Size(311, 15);
            lblDeliveryOptTitle.TabIndex = 15;
            lblDeliveryOptTitle.Text = "Delivery Optimization Kill-Switch (Disable Update P2P)";
            // 
            // lblDeliveryOptDesc
            // 
            lblDeliveryOptDesc.AutoSize = true;
            lblDeliveryOptDesc.Font = new Font("Segoe UI", 8F);
            lblDeliveryOptDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblDeliveryOptDesc.Location = new Point(16, 207);
            lblDeliveryOptDesc.Name = "lblDeliveryOptDesc";
            lblDeliveryOptDesc.Size = new Size(516, 13);
            lblDeliveryOptDesc.TabIndex = 16;
            lblDeliveryOptDesc.Text = "Disables Windows Update P2P uploads so Windows won't chew up upload bandwidth mid-match.";
            // 
            // deliveryOptToggle
            // 
            deliveryOptToggle.BackColor = Color.Transparent;
            deliveryOptToggle.Checked = false;
            deliveryOptToggle.Location = new Point(610, 196);
            deliveryOptToggle.Name = "deliveryOptToggle";
            deliveryOptToggle.Size = new Size(46, 22);
            deliveryOptToggle.TabIndex = 17;
            // 
            // btnApplyNetworkOptimzations
            // 
            btnApplyNetworkOptimzations.AccentColor = Color.FromArgb(214, 28, 52);
            btnApplyNetworkOptimzations.BackColor = Color.Transparent;
            btnApplyNetworkOptimzations.CornerRadius = 5;
            btnApplyNetworkOptimzations.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnApplyNetworkOptimzations.ForeColor = Color.FromArgb(242, 242, 246);
            btnApplyNetworkOptimzations.IsPrimaryAction = true;
            btnApplyNetworkOptimzations.Location = new Point(16, 262);
            btnApplyNetworkOptimzations.Name = "btnApplyNetworkOptimzations";
            btnApplyNetworkOptimzations.Size = new Size(444, 34);
            btnApplyNetworkOptimzations.TabIndex = 1;
            btnApplyNetworkOptimzations.Text = "Apply Network Optimizations";
            btnApplyNetworkOptimzations.Click += btnApplyNetworkOptimzations_Click;
            // 
            // btnFlushDns
            // 
            btnFlushDns.AccentColor = Color.FromArgb(214, 28, 52);
            btnFlushDns.BackColor = Color.Transparent;
            btnFlushDns.CornerRadius = 5;
            btnFlushDns.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnFlushDns.ForeColor = Color.FromArgb(242, 242, 246);
            btnFlushDns.IsPrimaryAction = false;
            btnFlushDns.Location = new Point(472, 262);
            btnFlushDns.Name = "btnFlushDns";
            btnFlushDns.Size = new Size(217, 34);
            btnFlushDns.TabIndex = 2;
            btnFlushDns.Text = "Flush DNS Cache";
            btnFlushDns.Click += btnFlushDns_Click;
            // 
            // grpPingMonitor
            // 
            grpPingMonitor.BackColor = Color.Transparent;
            grpPingMonitor.Controls.Add(txtServerIp);
            grpPingMonitor.Controls.Add(pingCountNumeric);
            grpPingMonitor.Controls.Add(btnRunPingTest);
            grpPingMonitor.Controls.Add(lblPingAvg);
            grpPingMonitor.Controls.Add(lblPingJitter);
            grpPingMonitor.Controls.Add(lblPacketLoss);
            grpPingMonitor.Font = new Font("Segoe UI", 9.5F);
            grpPingMonitor.Location = new Point(16, 304);
            grpPingMonitor.Name = "grpPingMonitor";
            grpPingMonitor.Padding = new Padding(14, 44, 14, 14);
            grpPingMonitor.Size = new Size(673, 212);
            grpPingMonitor.SubTitle = "Enter server IP and port, or hostname and port and ping attempts";
            grpPingMonitor.TabIndex = 3;
            grpPingMonitor.Title = "Server Jitter & Packet Loss Monitor";
            // 
            // txtServerIp
            // 
            txtServerIp.BackColor = Color.Transparent;
            txtServerIp.Location = new Point(14, 44);
            txtServerIp.MaxLength = 256;
            txtServerIp.Multiline = false;
            txtServerIp.Name = "txtServerIp";
            txtServerIp.ReadOnly = false;
            txtServerIp.Size = new Size(406, 32);
            txtServerIp.TabIndex = 0;
            txtServerIp.Text = "1.1.1.1";
            txtServerIp.TextAlign = HorizontalAlignment.Left;
            txtServerIp.UseSystemPasswordChar = false;
            // 
            // pingCountNumeric
            // 
            pingCountNumeric.BackColor = Color.Transparent;
            pingCountNumeric.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            pingCountNumeric.Location = new Point(430, 44);
            pingCountNumeric.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            pingCountNumeric.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            pingCountNumeric.Name = "pingCountNumeric";
            pingCountNumeric.Size = new Size(96, 32);
            pingCountNumeric.TabIndex = 1;
            pingCountNumeric.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // btnRunPingTest
            // 
            btnRunPingTest.AccentColor = Color.FromArgb(214, 28, 52);
            btnRunPingTest.BackColor = Color.Transparent;
            btnRunPingTest.CornerRadius = 5;
            btnRunPingTest.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRunPingTest.ForeColor = Color.FromArgb(242, 242, 246);
            btnRunPingTest.IsPrimaryAction = true;
            btnRunPingTest.Location = new Point(536, 44);
            btnRunPingTest.Name = "btnRunPingTest";
            btnRunPingTest.Size = new Size(123, 32);
            btnRunPingTest.TabIndex = 2;
            btnRunPingTest.Text = "Run Test";
            btnRunPingTest.Click += btnRunPingTest_Click;
            // 
            // lblPingAvg
            // 
            lblPingAvg.BackColor = Color.Transparent;
            lblPingAvg.DotColor = Color.FromArgb(248, 54, 78);
            lblPingAvg.Location = new Point(14, 86);
            lblPingAvg.Name = "lblPingAvg";
            lblPingAvg.Prefix = "AVG PING:";
            lblPingAvg.Size = new Size(645, 32);
            lblPingAvg.TabIndex = 3;
            lblPingAvg.Text = "Idle | enter a server IP:port or hostname and click Run Test";
            // 
            // lblPingJitter
            // 
            lblPingJitter.BackColor = Color.Transparent;
            lblPingJitter.DotColor = Color.FromArgb(214, 28, 52);
            lblPingJitter.Location = new Point(14, 126);
            lblPingJitter.Name = "lblPingJitter";
            lblPingJitter.Prefix = "JITTER:";
            lblPingJitter.Size = new Size(645, 32);
            lblPingJitter.TabIndex = 4;
            lblPingJitter.Text = "--";
            // 
            // lblPacketLoss
            // 
            lblPacketLoss.BackColor = Color.Transparent;
            lblPacketLoss.DotColor = Color.FromArgb(16, 185, 129);
            lblPacketLoss.Location = new Point(14, 166);
            lblPacketLoss.Name = "lblPacketLoss";
            lblPacketLoss.Prefix = "PACKET LOSS:";
            lblPacketLoss.Size = new Size(645, 32);
            lblPacketLoss.TabIndex = 5;
            lblPacketLoss.Text = "--";
            // 
            // pnlAutoFlush
            // 
            pnlAutoFlush.BackColor = Color.FromArgb(9, 9, 12);
            pnlAutoFlush.Controls.Add(grpAutoFlush);
            pnlAutoFlush.Controls.Add(btnSaveFlush);
            pnlAutoFlush.Controls.Add(btnFlushNow);
            pnlAutoFlush.Dock = DockStyle.Fill;
            pnlAutoFlush.Location = new Point(0, 0);
            pnlAutoFlush.Name = "pnlAutoFlush";
            pnlAutoFlush.Size = new Size(705, 525);
            pnlAutoFlush.TabIndex = 3;
            pnlAutoFlush.Visible = false;
            // 
            // grpAutoFlush
            // 
            grpAutoFlush.BackColor = Color.Transparent;
            grpAutoFlush.Controls.Add(lblAutoFlushTitle);
            grpAutoFlush.Controls.Add(lblAutoFlushDesc);
            grpAutoFlush.Controls.Add(autoFlushChk);
            grpAutoFlush.Controls.Add(sepFlush1);
            grpAutoFlush.Controls.Add(lblSmartFlushTitle);
            grpAutoFlush.Controls.Add(lblSmartFlushDesc);
            grpAutoFlush.Controls.Add(smartFlushToggle);
            grpAutoFlush.Controls.Add(sepFlush2);
            grpAutoFlush.Controls.Add(lblSmartThresholdTitle);
            grpAutoFlush.Controls.Add(lblSmartThresholdDesc);
            grpAutoFlush.Controls.Add(smartFlushThreshold);
            grpAutoFlush.Controls.Add(sepFlush3);
            grpAutoFlush.Controls.Add(lblIntervalTitle);
            grpAutoFlush.Controls.Add(lblIntervalDesc);
            grpAutoFlush.Controls.Add(autoFlushinterval);
            grpAutoFlush.Controls.Add(autoFlushMinHour);
            grpAutoFlush.Controls.Add(sepFlush4);
            grpAutoFlush.Controls.Add(lblFlushSfxTitle);
            grpAutoFlush.Controls.Add(lblFlushSfxDesc);
            grpAutoFlush.Controls.Add(autoFlushSound);
            grpAutoFlush.Font = new Font("Segoe UI", 9.5F);
            grpAutoFlush.Location = new Point(16, 14);
            grpAutoFlush.Name = "grpAutoFlush";
            grpAutoFlush.Padding = new Padding(14, 44, 14, 14);
            grpAutoFlush.Size = new Size(673, 388);
            grpAutoFlush.SubTitle = "Requires Administrator privileges";
            grpAutoFlush.TabIndex = 0;
            grpAutoFlush.Title = "Standby Memory Auto-Flush";
            // 
            // lblAutoFlushTitle
            // 
            lblAutoFlushTitle.AutoSize = true;
            lblAutoFlushTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblAutoFlushTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblAutoFlushTitle.Location = new Point(16, 48);
            lblAutoFlushTitle.Name = "lblAutoFlushTitle";
            lblAutoFlushTitle.Size = new Size(121, 17);
            lblAutoFlushTitle.TabIndex = 0;
            lblAutoFlushTitle.Text = "Enable Auto-Flush";
            // 
            // lblAutoFlushDesc
            // 
            lblAutoFlushDesc.AutoSize = true;
            lblAutoFlushDesc.Font = new Font("Segoe UI", 8.25F);
            lblAutoFlushDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblAutoFlushDesc.Location = new Point(16, 68);
            lblAutoFlushDesc.Name = "lblAutoFlushDesc";
            lblAutoFlushDesc.Size = new Size(444, 13);
            lblAutoFlushDesc.TabIndex = 1;
            lblAutoFlushDesc.Text = "Clears Windows standby RAM automatically to prevent long game session stuttering.";
            // 
            // autoFlushChk
            // 
            autoFlushChk.BackColor = Color.Transparent;
            autoFlushChk.Checked = false;
            autoFlushChk.Location = new Point(610, 55);
            autoFlushChk.Name = "autoFlushChk";
            autoFlushChk.Size = new Size(46, 22);
            autoFlushChk.TabIndex = 2;
            // 
            // sepFlush1
            // 
            sepFlush1.BackColor = Color.Transparent;
            sepFlush1.Location = new Point(16, 94);
            sepFlush1.Name = "sepFlush1";
            sepFlush1.Size = new Size(641, 8);
            sepFlush1.TabIndex = 3;
            // 
            // lblSmartFlushTitle
            // 
            lblSmartFlushTitle.AutoSize = true;
            lblSmartFlushTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblSmartFlushTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblSmartFlushTitle.Location = new Point(16, 108);
            lblSmartFlushTitle.Name = "lblSmartFlushTitle";
            lblSmartFlushTitle.Size = new Size(254, 17);
            lblSmartFlushTitle.TabIndex = 4;
            lblSmartFlushTitle.Text = "Smart Threshold Mode (Recommended)";
            // 
            // lblSmartFlushDesc
            // 
            lblSmartFlushDesc.AutoSize = true;
            lblSmartFlushDesc.Font = new Font("Segoe UI", 8.25F);
            lblSmartFlushDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblSmartFlushDesc.Location = new Point(16, 128);
            lblSmartFlushDesc.Name = "lblSmartFlushDesc";
            lblSmartFlushDesc.Size = new Size(440, 13);
            lblSmartFlushDesc.TabIndex = 5;
            lblSmartFlushDesc.Text = "Only flushes when free RAM drops below your threshold and standby cache is high.";
            // 
            // smartFlushToggle
            // 
            smartFlushToggle.BackColor = Color.Transparent;
            smartFlushToggle.Checked = false;
            smartFlushToggle.Location = new Point(610, 115);
            smartFlushToggle.Name = "smartFlushToggle";
            smartFlushToggle.Size = new Size(46, 22);
            smartFlushToggle.TabIndex = 6;
            // 
            // sepFlush2
            // 
            sepFlush2.BackColor = Color.Transparent;
            sepFlush2.Location = new Point(16, 154);
            sepFlush2.Name = "sepFlush2";
            sepFlush2.Size = new Size(641, 8);
            sepFlush2.TabIndex = 7;
            // 
            // lblSmartThresholdTitle
            // 
            lblSmartThresholdTitle.AutoSize = true;
            lblSmartThresholdTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblSmartThresholdTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblSmartThresholdTitle.Location = new Point(16, 168);
            lblSmartThresholdTitle.Name = "lblSmartThresholdTitle";
            lblSmartThresholdTitle.Size = new Size(212, 17);
            lblSmartThresholdTitle.TabIndex = 8;
            lblSmartThresholdTitle.Text = "Free RAM Trigger Threshold (GB)";
            // 
            // lblSmartThresholdDesc
            // 
            lblSmartThresholdDesc.AutoSize = true;
            lblSmartThresholdDesc.Font = new Font("Segoe UI", 8.25F);
            lblSmartThresholdDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblSmartThresholdDesc.Location = new Point(16, 188);
            lblSmartThresholdDesc.Name = "lblSmartThresholdDesc";
            lblSmartThresholdDesc.Size = new Size(448, 13);
            lblSmartThresholdDesc.TabIndex = 9;
            lblSmartThresholdDesc.Text = "Triggers Smart Flush when free physical RAM drops below this amount (default: 2 GB).";
            // 
            // smartFlushThreshold
            // 
            smartFlushThreshold.BackColor = Color.Transparent;
            smartFlushThreshold.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            smartFlushThreshold.Location = new Point(548, 172);
            smartFlushThreshold.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            smartFlushThreshold.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            smartFlushThreshold.Name = "smartFlushThreshold";
            smartFlushThreshold.Size = new Size(108, 32);
            smartFlushThreshold.TabIndex = 10;
            smartFlushThreshold.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // sepFlush3
            // 
            sepFlush3.BackColor = Color.Transparent;
            sepFlush3.Location = new Point(16, 218);
            sepFlush3.Name = "sepFlush3";
            sepFlush3.Size = new Size(641, 8);
            sepFlush3.TabIndex = 11;
            // 
            // lblIntervalTitle
            // 
            lblIntervalTitle.AutoSize = true;
            lblIntervalTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblIntervalTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblIntervalTitle.Location = new Point(16, 232);
            lblIntervalTitle.Name = "lblIntervalTitle";
            lblIntervalTitle.Size = new Size(304, 17);
            lblIntervalTitle.TabIndex = 12;
            lblIntervalTitle.Text = "Fixed Timer Interval (When Smart Mode is OFF)";
            // 
            // lblIntervalDesc
            // 
            lblIntervalDesc.AutoSize = true;
            lblIntervalDesc.Font = new Font("Segoe UI", 8.25F);
            lblIntervalDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblIntervalDesc.Location = new Point(16, 252);
            lblIntervalDesc.Name = "lblIntervalDesc";
            lblIntervalDesc.Size = new Size(379, 13);
            lblIntervalDesc.TabIndex = 13;
            lblIntervalDesc.Text = "Fixed RAM flush interval, if you prefer timed over Smart Threshold Mode.";
            // 
            // autoFlushinterval
            // 
            autoFlushinterval.BackColor = Color.Transparent;
            autoFlushinterval.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            autoFlushinterval.Location = new Point(443, 236);
            autoFlushinterval.Maximum = new decimal(new int[] { 240, 0, 0, 0 });
            autoFlushinterval.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            autoFlushinterval.Name = "autoFlushinterval";
            autoFlushinterval.Size = new Size(99, 32);
            autoFlushinterval.TabIndex = 14;
            autoFlushinterval.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // autoFlushMinHour
            // 
            autoFlushMinHour.BackColor = Color.FromArgb(22, 22, 29);
            autoFlushMinHour.DrawMode = DrawMode.OwnerDrawFixed;
            autoFlushMinHour.DropDownStyle = ComboBoxStyle.DropDownList;
            autoFlushMinHour.Font = new Font("Segoe UI", 9.25F);
            autoFlushMinHour.ForeColor = Color.FromArgb(242, 242, 246);
            autoFlushMinHour.ItemHeight = 26;
            autoFlushMinHour.Items.AddRange(new object[] { "Minutes", "Hours" });
            autoFlushMinHour.Location = new Point(548, 236);
            autoFlushMinHour.Name = "autoFlushMinHour";
            autoFlushMinHour.Size = new Size(108, 32);
            autoFlushMinHour.TabIndex = 15;
            // 
            // sepFlush4
            // 
            sepFlush4.BackColor = Color.Transparent;
            sepFlush4.Location = new Point(16, 282);
            sepFlush4.Name = "sepFlush4";
            sepFlush4.Size = new Size(641, 8);
            sepFlush4.TabIndex = 16;
            // 
            // lblFlushSfxTitle
            // 
            lblFlushSfxTitle.AutoSize = true;
            lblFlushSfxTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblFlushSfxTitle.ForeColor = Color.FromArgb(242, 242, 246);
            lblFlushSfxTitle.Location = new Point(16, 296);
            lblFlushSfxTitle.Name = "lblFlushSfxTitle";
            lblFlushSfxTitle.Size = new Size(193, 17);
            lblFlushSfxTitle.TabIndex = 17;
            lblFlushSfxTitle.Text = "RAM Flush Sound Effect (SFX)";
            // 
            // lblFlushSfxDesc
            // 
            lblFlushSfxDesc.AutoSize = true;
            lblFlushSfxDesc.Font = new Font("Segoe UI", 8.25F);
            lblFlushSfxDesc.ForeColor = Color.FromArgb(162, 162, 176);
            lblFlushSfxDesc.Location = new Point(16, 316);
            lblFlushSfxDesc.Name = "lblFlushSfxDesc";
            lblFlushSfxDesc.Size = new Size(314, 13);
            lblFlushSfxDesc.TabIndex = 18;
            lblFlushSfxDesc.Text = "Play a toilet flush sound each time an auto-flush completes.";
            // 
            // autoFlushSound
            // 
            autoFlushSound.BackColor = Color.Transparent;
            autoFlushSound.Checked = false;
            autoFlushSound.Location = new Point(610, 308);
            autoFlushSound.Name = "autoFlushSound";
            autoFlushSound.Size = new Size(46, 22);
            autoFlushSound.TabIndex = 19;
            // 
            // btnSaveFlush
            // 
            btnSaveFlush.AccentColor = Color.FromArgb(214, 28, 52);
            btnSaveFlush.BackColor = Color.Transparent;
            btnSaveFlush.CornerRadius = 5;
            btnSaveFlush.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSaveFlush.ForeColor = Color.FromArgb(242, 242, 246);
            btnSaveFlush.IsPrimaryAction = true;
            btnSaveFlush.Location = new Point(16, 414);
            btnSaveFlush.Name = "btnSaveFlush";
            btnSaveFlush.Size = new Size(444, 40);
            btnSaveFlush.TabIndex = 1;
            btnSaveFlush.Text = "Save Auto-Flush Settings";
            btnSaveFlush.Click += saveAdvancedCfgBtn_Click;
            // 
            // btnFlushNow
            // 
            btnFlushNow.AccentColor = Color.FromArgb(214, 28, 52);
            btnFlushNow.BackColor = Color.Transparent;
            btnFlushNow.CornerRadius = 5;
            btnFlushNow.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnFlushNow.ForeColor = Color.FromArgb(242, 242, 246);
            btnFlushNow.IsPrimaryAction = false;
            btnFlushNow.Location = new Point(472, 414);
            btnFlushNow.Name = "btnFlushNow";
            btnFlushNow.Size = new Size(217, 40);
            btnFlushNow.TabIndex = 2;
            btnFlushNow.Text = "Flush RAM Now";
            btnFlushNow.Click += flushRamToolStripMenuItem_Click;
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(15, 15, 20);
            pnlSidebar.Controls.Add(startAsAdminToggle);
            pnlSidebar.Controls.Add(lblStartAsAdmin);
            pnlSidebar.Controls.Add(label1);
            pnlSidebar.Controls.Add(updateToggle);
            pnlSidebar.Controls.Add(_lnkWebsite);
            pnlSidebar.Controls.Add(roLabel1);
            pnlSidebar.Controls.Add(navDashboard);
            pnlSidebar.Controls.Add(navProfiles);
            pnlSidebar.Controls.Add(navTools);
            pnlSidebar.Controls.Add(navAutoFlush);
            pnlSidebar.Controls.Add(navNetwork);
            pnlSidebar.Controls.Add(navAbout);
            pnlSidebar.Controls.Add(sepSidebar);
            pnlSidebar.Controls.Add(launchButton1);
            pnlSidebar.Controls.Add(navHotkeys);
            pnlSidebar.Location = new Point(1, 74);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(212, 525);
            pnlSidebar.TabIndex = 1;
            // 
            // startAsAdminToggle
            // 
            startAsAdminToggle.BackColor = Color.Transparent;
            startAsAdminToggle.Checked = false;
            startAsAdminToggle.Location = new Point(154, 339);
            startAsAdminToggle.Name = "startAsAdminToggle";
            startAsAdminToggle.Size = new Size(46, 22);
            startAsAdminToggle.TabIndex = 3;
            startAsAdminToggle.Text = "roToggle1";
            startAsAdminToggle.CheckedChanged += startAsAdminToggle_CheckedChanged;
            // 
            // lblStartAsAdmin
            // 
            lblStartAsAdmin.AutoSize = true;
            lblStartAsAdmin.Font = new Font("Segoe UI", 8.25F);
            lblStartAsAdmin.ForeColor = Color.FromArgb(162, 162, 176);
            lblStartAsAdmin.Location = new Point(14, 344);
            lblStartAsAdmin.Name = "lblStartAsAdmin";
            lblStartAsAdmin.Size = new Size(84, 13);
            lblStartAsAdmin.TabIndex = 12;
            lblStartAsAdmin.Text = "Start as Admin:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F);
            label1.ForeColor = Color.FromArgb(162, 162, 176);
            label1.Location = new Point(14, 378);
            label1.Name = "label1";
            label1.Size = new Size(107, 13);
            label1.TabIndex = 0;
            label1.Text = "Check For Updates:";
            // 
            // updateToggle
            // 
            updateToggle.BackColor = Color.Transparent;
            updateToggle.Checked = true;
            updateToggle.Location = new Point(154, 373);
            updateToggle.Name = "updateToggle";
            updateToggle.Size = new Size(46, 22);
            updateToggle.TabIndex = 1;
            updateToggle.CheckedChanged += updateToggle_CheckedChanged;
            // 
            // _lnkWebsite
            // 
            _lnkWebsite.ActiveLinkColor = Color.White;
            _lnkWebsite.AutoSize = true;
            _lnkWebsite.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lnkWebsite.LinkBehavior = LinkBehavior.HoverUnderline;
            _lnkWebsite.LinkColor = Color.FromArgb(248, 54, 78);
            _lnkWebsite.Location = new Point(45, 428);
            _lnkWebsite.Name = "_lnkWebsite";
            _lnkWebsite.Size = new Size(123, 15);
            _lnkWebsite.TabIndex = 2;
            _lnkWebsite.TabStop = true;
            _lnkWebsite.Text = "voidtechstudios.us ↗";
            _lnkWebsite.VisitedLinkColor = Color.FromArgb(248, 54, 78);
            _lnkWebsite.LinkClicked += _lnkWebsite_LinkClicked;
            // 
            // roLabel1
            // 
            roLabel1.BackColor = Color.Transparent;
            roLabel1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            roLabel1.Location = new Point(29, 402);
            roLabel1.Name = "roLabel1";
            roLabel1.Size = new Size(154, 24);
            roLabel1.TabIndex = 3;
            roLabel1.Value1 = "VOID TECH";
            roLabel1.Value2 = " STUDIOS";
            // 
            // navDashboard
            // 
            navDashboard.BackColor = Color.Transparent;
            navDashboard.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navDashboard.IsHorizontalTab = false;
            navDashboard.LinkedPanel = pnlDashboard;
            navDashboard.Location = new Point(9, 16);
            navDashboard.Name = "navDashboard";
            navDashboard.Selected = true;
            navDashboard.Size = new Size(194, 38);
            navDashboard.TabIndex = 4;
            navDashboard.Text = "Dashboard";
            navDashboard.TextCharacter = "⌂";
            // 
            // navProfiles
            // 
            navProfiles.BackColor = Color.Transparent;
            navProfiles.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navProfiles.IsHorizontalTab = false;
            navProfiles.LinkedPanel = pnlProfiles;
            navProfiles.Location = new Point(9, 60);
            navProfiles.Name = "navProfiles";
            navProfiles.Selected = false;
            navProfiles.Size = new Size(194, 38);
            navProfiles.TabIndex = 5;
            navProfiles.Text = "Game Profiles";
            navProfiles.TextCharacter = "⚡";
            // 
            // navTools
            // 
            navTools.BackColor = Color.Transparent;
            navTools.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navTools.IsHorizontalTab = false;
            navTools.LinkedPanel = pnlTools;
            navTools.Location = new Point(9, 104);
            navTools.Name = "navTools";
            navTools.Selected = false;
            navTools.Size = new Size(194, 38);
            navTools.TabIndex = 6;
            navTools.Text = "System Optimization";
            navTools.TextCharacter = "⚙";
            // 
            // navAutoFlush
            // 
            navAutoFlush.BackColor = Color.Transparent;
            navAutoFlush.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navAutoFlush.IsHorizontalTab = false;
            navAutoFlush.LinkedPanel = pnlAutoFlush;
            navAutoFlush.Location = new Point(9, 148);
            navAutoFlush.Name = "navAutoFlush";
            navAutoFlush.Selected = false;
            navAutoFlush.Size = new Size(194, 38);
            navAutoFlush.TabIndex = 7;
            navAutoFlush.Text = "RAM Auto-Flush";
            navAutoFlush.TextCharacter = "↻";
            // 
            // navNetwork
            // 
            navNetwork.BackColor = Color.Transparent;
            navNetwork.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navNetwork.IsHorizontalTab = false;
            navNetwork.LinkedPanel = pnlNetwork;
            navNetwork.Location = new Point(9, 192);
            navNetwork.Name = "navNetwork";
            navNetwork.Selected = false;
            navNetwork.Size = new Size(194, 38);
            navNetwork.TabIndex = 8;
            navNetwork.Text = "Network & Ping";
            navNetwork.TextCharacter = "⇄";
            // 
            // navAbout
            // 
            navAbout.BackColor = Color.Transparent;
            navAbout.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navAbout.IsHorizontalTab = false;
            navAbout.LinkedPanel = pnlAbout;
            navAbout.Location = new Point(9, 280);
            navAbout.Name = "navAbout";
            navAbout.Selected = false;
            navAbout.Size = new Size(194, 38);
            navAbout.TabIndex = 10;
            navAbout.Text = "Logs & About";
            navAbout.TextCharacter = "★";
            // 
            // sepSidebar
            // 
            sepSidebar.BackColor = Color.Transparent;
            sepSidebar.Location = new Point(12, 446);
            sepSidebar.Name = "sepSidebar";
            sepSidebar.Size = new Size(188, 8);
            sepSidebar.TabIndex = 11;
            // 
            // launchButton1
            // 
            launchButton1.BackColor = Color.Transparent;
            launchButton1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            launchButton1.ForeColor = Color.White;
            launchButton1.Location = new Point(12, 460);
            launchButton1.Name = "launchButton1";
            launchButton1.Size = new Size(188, 54);
            launchButton1.SubText = "SELECT A GAME";
            launchButton1.TabIndex = 13;
            launchButton1.Text = "LAUNCH GAME";
            launchButton1.Click += launchButton1_Click;
            // 
            // navHotkeys
            // 
            navHotkeys.BackColor = Color.Transparent;
            navHotkeys.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navHotkeys.IsHorizontalTab = false;
            navHotkeys.LinkedPanel = pnlHotkeys;
            navHotkeys.Location = new Point(9, 236);
            navHotkeys.Name = "navHotkeys";
            navHotkeys.Selected = false;
            navHotkeys.Size = new Size(194, 38);
            navHotkeys.TabIndex = 9;
            navHotkeys.Text = "Hotkeys";
            navHotkeys.TextCharacter = "⌨";
            // 
            // menuStrip1
            // 
            menuStrip1.AutoSize = false;
            menuStrip1.BackColor = Color.FromArgb(9, 9, 12);
            menuStrip1.Dock = DockStyle.None;
            menuStrip1.Font = new Font("Segoe UI", 9F);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, aboutToolStripMenuItem, supportToolStripMenuItem, flushRamToolStripMenuItem, minimizeToTrayToolStripMenuItem });
            menuStrip1.Location = new Point(1, 48);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(8, 2, 0, 2);
            menuStrip1.Size = new Size(918, 26);
            menuStrip1.TabIndex = 2;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { scanGamesToolStripMenuItem, addGameToolStripMenuItem, openPacksToolStripMenuItem, packBuilderToolStripMenuItem, revertAllOptimizationsToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem1 });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 22);
            fileToolStripMenuItem.Text = "File";
            // 
            // scanGamesToolStripMenuItem
            // 
            scanGamesToolStripMenuItem.Name = "scanGamesToolStripMenuItem";
            scanGamesToolStripMenuItem.Size = new Size(235, 22);
            scanGamesToolStripMenuItem.Text = "Scan Installed Games...";
            scanGamesToolStripMenuItem.Click += btnScanGames_Click;
            // 
            // addGameToolStripMenuItem
            // 
            addGameToolStripMenuItem.Name = "addGameToolStripMenuItem";
            addGameToolStripMenuItem.Size = new Size(235, 22);
            addGameToolStripMenuItem.Text = "Add Game .exe...";
            addGameToolStripMenuItem.Click += btnAddManualGame_Click;
            // 
            // openPacksToolStripMenuItem
            // 
            openPacksToolStripMenuItem.Name = "openPacksToolStripMenuItem";
            openPacksToolStripMenuItem.Size = new Size(235, 22);
            openPacksToolStripMenuItem.Text = "Open JSON Profile Packs...";
            openPacksToolStripMenuItem.Click += btnOpenPacksFolder_Click;
            // 
            // packBuilderToolStripMenuItem
            // 
            packBuilderToolStripMenuItem.Name = "packBuilderToolStripMenuItem";
            packBuilderToolStripMenuItem.Size = new Size(235, 22);
            packBuilderToolStripMenuItem.Text = "Open Game Pack Builder...";
            packBuilderToolStripMenuItem.Click += btnPackBuilder_Click;
            // 
            // revertAllOptimizationsToolStripMenuItem
            // 
            revertAllOptimizationsToolStripMenuItem.Name = "revertAllOptimizationsToolStripMenuItem";
            revertAllOptimizationsToolStripMenuItem.Size = new Size(235, 22);
            revertAllOptimizationsToolStripMenuItem.Text = "Revert All Permanent Optimizations...";
            revertAllOptimizationsToolStripMenuItem.Click += btnDashRevertOptimizations_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(232, 6);
            // 
            // exitToolStripMenuItem1
            // 
            exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            exitToolStripMenuItem1.Size = new Size(235, 22);
            exitToolStripMenuItem1.Text = "Exit";
            exitToolStripMenuItem1.Click += exitToolStripMenuItem1_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openLogFileToolStripMenuItem, openLogFilePathToolStripMenuItem, openSettingsFilePathToolStripMenuItem, openLatestBenchmarkToolStripMenuItem });
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(44, 22);
            aboutToolStripMenuItem.Text = "Help";
            // 
            // openLogFileToolStripMenuItem
            // 
            openLogFileToolStripMenuItem.Name = "openLogFileToolStripMenuItem";
            openLogFileToolStripMenuItem.Size = new Size(247, 22);
            openLogFileToolStripMenuItem.Text = "Open Log File...";
            openLogFileToolStripMenuItem.Click += openLogFileToolStripMenuItem_Click;
            // 
            // openLogFilePathToolStripMenuItem
            // 
            openLogFilePathToolStripMenuItem.Name = "openLogFilePathToolStripMenuItem";
            openLogFilePathToolStripMenuItem.Size = new Size(247, 22);
            openLogFilePathToolStripMenuItem.Text = "Open Logs Folder...";
            openLogFilePathToolStripMenuItem.Click += openLogFilePathToolStripMenuItem_Click;
            // 
            // openSettingsFilePathToolStripMenuItem
            // 
            openSettingsFilePathToolStripMenuItem.Name = "openSettingsFilePathToolStripMenuItem";
            openSettingsFilePathToolStripMenuItem.Size = new Size(247, 22);
            openSettingsFilePathToolStripMenuItem.Text = "Open Data Folder...";
            openSettingsFilePathToolStripMenuItem.Click += openSettingsFilePathToolStripMenuItem_Click;
            // 
            // openLatestBenchmarkToolStripMenuItem
            // 
            openLatestBenchmarkToolStripMenuItem.Name = "openLatestBenchmarkToolStripMenuItem";
            openLatestBenchmarkToolStripMenuItem.Size = new Size(247, 22);
            openLatestBenchmarkToolStripMenuItem.Text = "Open Latest Benchmark Report...";
            openLatestBenchmarkToolStripMenuItem.Click += openLatestBenchmarkToolStripMenuItem_Click;
            // 
            // supportToolStripMenuItem
            // 
            supportToolStripMenuItem.Name = "supportToolStripMenuItem";
            supportToolStripMenuItem.Size = new Size(61, 22);
            supportToolStripMenuItem.Text = "Support";
            supportToolStripMenuItem.Click += supportToolStripMenuItem_Click;
            // 
            // flushRamToolStripMenuItem
            // 
            flushRamToolStripMenuItem.Name = "flushRamToolStripMenuItem";
            flushRamToolStripMenuItem.Size = new Size(76, 22);
            flushRamToolStripMenuItem.Text = "Flush RAM";
            flushRamToolStripMenuItem.Click += flushRamToolStripMenuItem_Click;
            // 
            // minimizeToTrayToolStripMenuItem
            // 
            minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            minimizeToTrayToolStripMenuItem.Size = new Size(107, 22);
            minimizeToTrayToolStripMenuItem.Text = "Minimize to Tray";
            minimizeToTrayToolStripMenuItem.Click += minimizeToTrayToolStripMenuItem_Click;
            // 
            // MainFrm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(920, 600);
            Controls.Add(roThemeContainer1);
            ForeColor = Color.FromArgb(242, 242, 246);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "MainFrm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Overcapped Optimizer";
            FormClosing += MainFrm_FormClosing;
            Load += MainFrm_Load;
            roThemeContainer1.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            pnlAbout.ResumeLayout(false);
            grpLinks.ResumeLayout(false);
            cardPanel.ResumeLayout(false);
            grpDiagnostics.ResumeLayout(false);
            pnlDashboard.ResumeLayout(false);
            grpGameLibrary.ResumeLayout(false);
            grpHardware.ResumeLayout(false);
            grpQuickActions.ResumeLayout(false);
            pnlHotkeys.ResumeLayout(false);
            grpHotkeys.ResumeLayout(false);
            grpHotkeys.PerformLayout();
            pnlTools.ResumeLayout(false);
            grpSystemOptimzations.ResumeLayout(false);
            grpSystemOptimzations.PerformLayout();
            grpCacheCleaners.ResumeLayout(false);
            pnlProfiles.ResumeLayout(false);
            grpProfiles.ResumeLayout(false);
            grpProfiles.PerformLayout();
            grpPerGameOptimzations.ResumeLayout(false);
            grpPerGameOptimzations.PerformLayout();
            pnlNetwork.ResumeLayout(false);
            grpNetworkOptimzations.ResumeLayout(false);
            grpNetworkOptimzations.PerformLayout();
            grpPingMonitor.ResumeLayout(false);
            pnlAutoFlush.ResumeLayout(false);
            grpAutoFlush.ResumeLayout(false);
            grpAutoFlush.PerformLayout();
            pnlSidebar.ResumeLayout(false);
            pnlSidebar.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ROThemeContainer roThemeContainer1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem scanGamesToolStripMenuItem;
        private ToolStripMenuItem addGameToolStripMenuItem;
        private ToolStripMenuItem openPacksToolStripMenuItem;
        private ToolStripMenuItem packBuilderToolStripMenuItem;
        private ToolStripMenuItem revertAllOptimizationsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem openLogFileToolStripMenuItem;
        private ToolStripMenuItem openLogFilePathToolStripMenuItem;
        private ToolStripMenuItem openSettingsFilePathToolStripMenuItem;
        private ToolStripMenuItem openLatestBenchmarkToolStripMenuItem;
        private ToolStripMenuItem supportToolStripMenuItem;
        private ToolStripMenuItem flushRamToolStripMenuItem;
        private ToolStripMenuItem minimizeToTrayToolStripMenuItem;
        private Panel pnlSidebar;
        private ROSidebarItem navDashboard;
        private ROSidebarItem navProfiles;
        private ROSidebarItem navTools;
        private ROSidebarItem navAutoFlush;
        private ROSidebarItem navNetwork;
        private ROSidebarItem navAbout;
        private ROSeparator sepSidebar;
        private ROLaunchButton launchButton1;
        private ROLabel roLabel1;
        private LinkLabel _lnkWebsite;
        private Label label1;
        private ROToggle updateToggle;
        private Panel pnlContent;
        private Panel pnlDashboard;
        private ROGroupBox grpGameLibrary;
        private ROComboBox gameDropdown;
        private ROModernButton btnScanGames;
        private ROModernButton btnAddManualGame;
        private ROModernButton btnRemoveGame;
        private Label lblSelectedGameMeta;
        private ROGroupBox grpHardware;
        private ROStatPill lblCPUInfo;
        private ROStatPill lblGPUInfo;
        private ROStatPill lblRAMInfo;
        private ROStatPill lblHealthStatus;
        private ROModernButton btnHealthDetails;
        private ROGroupBox grpQuickActions;
        private ROActionCard cardQuickFlush;
        private ROActionCard cardQuickShaderCache;
        private AboutLinkTile linkDonate;
        private ROModernButton btnDashRestorePoint;
        private ROModernButton btnDashRevertOptimizations;
        private ROModernButton btnDashTray;
        private Panel pnlProfiles;
        private ROGroupBox grpProfiles;
        private ROActionCard cardProfCompetitive;
        private ROActionCard cardProfBalanced;
        private ROActionCard cardProfRecommended;
        private ROActionCard cardProfUltra;
        private ROTextBox txtConfigDir;
        private ROModernButton btnBrowseConfigDir;
        private Label lblFpsLimit;
        private ROComboBox fpsLimitDropdown;
        private Label lblScreenMode;
        private ROComboBox screenModeDropdown;
        private Label lblLumen;
        private ROToggle lumenToggle;
        private Label lblFog;
        private ROToggle fogToggle;
        private ROComboBox profileDropdown;
        private ROModernButton autoDetectBtn;
        private ROModernButton optimizeBtn;
        private ROGroupBox grpPerGameOptimzations;
        private Label lblGameHighPrio;
        private ROToggle gameHighPriorityToggle;
        private Label lblGameGpuBind;
        private ROToggle gameGpuBindingToggle;
        private ROModernButton btnApplyGame1;
        private ROModernButton saveBackupBtn;
        private ROModernButton restoreBackupBtn;
        private ROModernButton btnOpenPacksFolder;
        private Panel pnlTools;
        private ROGroupBox grpSystemOptimzations;
        private Label lblPCoresTitle;
        private Label lblPCoresDesc;
        public ROToggle pCoresToggle;
        private Label lblPowerPlanTitle;
        private Label lblPowerPlanDesc;
        public ROToggle autoPowerPlanToggle;
        private ROSeparator sepTool1;
        private Label lblGameModeTitle;
        private Label lblGameModeDesc;
        public ROToggle gameModeDvrToggle;
        private Label lblFsoTitle;
        private Label lblFsoDesc;
        public ROToggle disabledFSO;
        private ROSeparator sepTool2;
        private Label lblHagsTitle;
        private Label lblHagsDesc;
        public ROToggle hagsToggle;
        private Label lblTelemetryTitle;
        private Label lblTelemetryDesc;
        public ROToggle telemetryToggle;
        private ROSeparator sepTool3;
        private Label lblPauseSearchTitle;
        private Label lblPauseSearchDesc;
        public ROToggle pauseSearchToggle;
        private ROSeparator sepTool4;
        private Label lblThrottleTitle;
        private Label lblThrottleDesc;
        private ROModernButton btnEditThrottleList;
        public ROToggle throttleAppsToggle;
        private ROSeparator sepTool5;
        private Label lblFpsOverlayTitle;
        private Label lblFpsOverlayDesc;
        private ROModernButton btnToggle1PercentLow;
        private ROComboBox fpsPositionDropdown;
        private ROComboBox fpsOpacityDropdown;
        private ROModernButton btnMoveFpsOverlay;
        public ROToggle fpsOverlayToggle;
        private ROGroupBox grpCacheCleaners;
        private ROModernButton clearShaderCacheBtn;
        private ROModernButton btnOpenGameFolder;
        private ROModernButton btnOpenBenchmarks;
        private ROModernButton saveAdvancedCfgBtn;
        private Panel pnlAutoFlush;
        private ROGroupBox grpAutoFlush;
        private Label lblAutoFlushTitle;
        private Label lblAutoFlushDesc;
        public ROToggle autoFlushChk;
        private ROSeparator sepFlush1;
        private Label lblSmartFlushTitle;
        private Label lblSmartFlushDesc;
        public ROToggle smartFlushToggle;
        private ROSeparator sepFlush2;
        private Label lblSmartThresholdTitle;
        private Label lblSmartThresholdDesc;
        private RONumericUpDown smartFlushThreshold;
        private ROSeparator sepFlush3;
        private Label lblIntervalTitle;
        private Label lblIntervalDesc;
        private RONumericUpDown autoFlushinterval;
        private ROComboBox autoFlushMinHour;
        private ROSeparator sepFlush4;
        private Label lblFlushSfxTitle;
        private Label lblFlushSfxDesc;
        private ROToggle autoFlushSound;
        private ROModernButton btnSaveFlush;
        private ROModernButton btnFlushNow;
        private Panel pnlNetwork;
        private ROGroupBox grpNetworkOptimzations;
        private Label lblNicPowerTitle;
        private Label lblNicPowerDesc;
        public ROToggle disableNicPowerToggle;
        private ROSeparator sepNet1;
        private Label lblIntModTitle;
        private Label lblIntModDesc;
        public ROToggle disableIntModToggle;
        private Label lblRssTitle;
        private Label lblRssDesc;
        public ROToggle enableRssToggle;
        private ROSeparator sepNet2;
        private Label lblQosTitle;
        private Label lblQosDesc;
        public ROToggle qosDscpToggle;
        private ROSeparator sepNet3;
        private Label lblDeliveryOptTitle;
        private Label lblDeliveryOptDesc;
        public ROToggle deliveryOptToggle;
        private ROModernButton btnApplyNetworkOptimzations;
        private ROModernButton btnFlushDns;
        private ROGroupBox grpPingMonitor;
        private ROTextBox txtServerIp;
        private RONumericUpDown pingCountNumeric;
        private ROModernButton btnRunPingTest;
        private ROStatPill lblPingAvg;
        private ROStatPill lblPingJitter;
        private ROStatPill lblPacketLoss;
        private Panel pnlAbout;
        private ROGroupBox grpLinks;
        private Panel cardPanel;
        private ROLabel roLabel2;
        private Label lblDescription;
        private AboutLinkTile linkWebsite;
        private AboutLinkTile aboutLinkTile1;
        private AboutLinkTile linkDocs;
        private AboutLinkTile linkDiscord;
        private AboutLinkTile linkGitHub;
        private ROGroupBox grpDiagnostics;
        private ROModernButton btnOpenLog;
        private ROModernButton btnOpenLogDir;
        private ROModernButton btnOpenSettingsDir;
        private ROToggle startAsAdminToggle;
        private Label lblStartAsAdmin;
        private ROModernButton btnGameAutoOptimizes;
        private ROModernButton btnPackBuilder;
        private ROSidebarItem navHotkeys;
        private Panel pnlHotkeys;
        private ROGroupBox grpHotkeys;
        private Label lblHotkeysEnableTitle;
        private Label lblHotkeysEnableDesc;
        private ROToggle hotkeysEnabledToggle;
        private ROSeparator sepHk1;
        private Label lblHkFpsTitle;
        private Label lblHkFpsDesc;
        private ROTextBox txtHkFpsOverlay;
        private ROModernButton btnBindHkFps;
        private ROModernButton btnClearHkFps;
        private ROSeparator sepHk2;
        private Label lblHk1LowTitle;
        private Label lblHk1LowDesc;
        private ROTextBox txtHk1PercentLow;
        private ROModernButton btnBindHk1Low;
        private ROModernButton btnClearHk1Low;
        private ROSeparator sepHk3;
        private Label lblHkFlushTitle;
        private Label lblHkFlushDesc;
        private ROTextBox txtHkFlushRam;
        private ROModernButton btnBindHkFlush;
        private ROModernButton btnClearHkFlush;
        private ROSeparator sepHk4;
        private Label lblHkWindowTitle;
        private Label lblHkWindowDesc;
        private ROTextBox txtHkToggleWindow;
        private ROModernButton btnBindHkWindow;
        private ROModernButton btnClearHkWindow;
        private Label lblHkHint;
        private ROModernButton btnSaveHotkeys;
        private ROModernButton btnResetHotkeys;
    }
}