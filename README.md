<div align="center">
  <img src="https://overcapped.us/img/logo.png" alt="Overcapped Logo" width="96" height="96" />
  <h1>Overcapped | PC Gaming Optimizer</h1>
  <p>Overcapped is a free, open-source PC gaming optimizer from <strong>Void Tech Studios</strong></p>

  <p>
    <a href="https://overcapped.us/">Website</a> •
    <a href="https://overcapped.us/downloads">Download</a> •
    <a href="https://overcapped.us/docs">Documentation</a> •
    <a href="https://overcapped.us/invite">Discord</a> •
    <a href="https://buymeacoffee.com/rustforgedev">Donate to Support (Buy Me A Coffee)</a>
  </p>

  <img src="https://overcapped.us/img/AppUI.png" alt="Overcapped Optimizer Dashboard" width="820" />
</div>

---

## What Overcapped Actually Does

Designed to cut down stutters and get more out of your PC without digging through Windows settings and config files by hand. Includes built-in support for Unreal Engine, Unity, and Source games, a built-in JSON Profile Pack Builder for creating and sharing custom game packs, per-game override rules, global hotkeys with a live dashboard opacity slider, hardware health checks, and automatic offline HTML benchmark reports. Everything stays anti-cheat safe and resets cleanly when you close your game.

---

## Requirements & Setup

- **OS:** Windows 10 or Windows 11 (64-bit)
- **Install:** None. Overcapped is completely portable. Grab the latest `Overcapped.zip` from [overcapped.us/downloads](https://overcapped.us/downloads), extract it to a permanent folder (Desktop, Games drive, Tools folder, etc.), and run `Overcapped Optimizer.exe`.
- **Administrator Privileges:** You can run the app normally, but **RAM Auto-Flush**, **High CPU Priority**, **In-Game FPS Overlay**, **NIC Power Optimizations**, **Delivery Optimization Kill-Switch**, **QoS DSCP 46**, and **System Restore Points** require Administrator rights. If you toggle an Admin feature without it, the app will prompt to restart elevated for you. You can also toggle **Start as Admin** in the sidebar if you want it to always launch elevated.
- **Standard-User Game Launches:** Even when Overcapped is running as Administrator, clicking **LAUNCH GAME** de-elevates through the Windows Explorer shell token so your games and anti-cheats launch with standard user privileges instead of inheriting Admin rights. Steam and Epic games also launch through their native launcher protocols (`steam://` / `com.epicgames.launcher://`) so anti-cheats and overlays initialize cleanly.

### Where Your Files Live

| Data | Path |
| :--- | :--- |
| **Settings & Game Library** | `%LocalAppData%\Overcapped Optimizer\settings.json` |
| **Custom JSON Profile Packs** | `%LocalAppData%\Overcapped Optimizer\Profiles\` *(also loads from `Profiles\` next to the `.exe`)* |
| **Session Benchmark Reports** | `%LocalAppData%\Overcapped Optimizer\Benchmarks\` (offline HTML reports) |
| **Config Backups** | `backups\` inside the Overcapped folder (timestamped folder per game) |
| **Logs** | `User\Logs\log_file.txt` inside the Overcapped folder |

Because your settings, game library, benchmarks, and custom packs live in AppData, dropping in a newer build will not wipe your setup.

---

## Features & How to Use Each Tab

### 1. Dashboard

Your main hub for managing installed games, checking hardware specs and health warnings, and running one-click maintenance.

- **Game Library:**
  - **Scan Games:** Scans across all drives for **Steam** (`libraryfolders.vdf` + `appmanifest_*.acf`), **Epic Games Launcher**, **GOG Galaxy**, **Xbox / Game Pass** (`.GamingRoot` + `MicrosoftGame.config`), **Battle.net**, and **EA App** installs (skipping redistributables, Proton, launcher apps, DLC entries, and incomplete installs). Detects the game engine (**Unreal Engine**, **Unity**, **Source**, or **Generic**), resolves the actual game binary (including `*-Win64-Shipping.exe`, paired `*Client.exe` games, and games using Easy Anti-Cheat's `start_protected_game.exe` bootstrapper so optimizations hit the real game while Launch still boots anti-cheat properly), and auto-discovers config folders across Unreal `Saved\Config`, Unity `LocalLow` (via `*_Data\app.info`), Source `cfg`, and common `Documents` / `AppData` paths.
  - **Add .exe...:** Manually add any game from Ubisoft, Riot, itch.io, or standalone installs by pointing it at the game's `.exe`.
  - **Remove:** Removes a title from Overcapped's list without uninstalling anything. Handy for clearing out non-game Steam apps like Wallpaper Engine or Lossless Scaling.
- **Hardware Information & Health Check:** Reads your CPU model and max clock speed, discrete GPU and VRAM (filtering out virtual/remote display adapters), and total RAM with module info. Runs an automatic background **Hardware Health Check** scanning for common PC gaming misconfigurations:
  - Monitors running below their max supported refresh rate (e.g., a 144Hz+ panel stuck at 60Hz).
  - Single-channel RAM setups, RAM running below rated speed (XMP/DOCP/EXPO disabled), or low total RAM.
  - Windows stuck on the **Power Saver** plan.
  - Low free space (`< 15 GB`) on the system drive starving the pagefile and GPU shader cache.
  - Click **Details** / **Rescan** for the full breakdown.
- **Quick Actions:** **Flush Standby RAM**, **Clear GPU Shader Cache**, **Create System Restore Point** (one-click WMI restore point before making system tweaks, requires Admin), **Minimize to System Tray**, and a support link.
- **Launch Game (Sidebar):** Pinned at the bottom left so you can launch whichever game is currently selected from any tab, automatically forwarding any custom or engine launch arguments as a standard user.

---

### 2. Game Profiles

Manages per-game engine configs, custom JSON profile packs, per-game auto-optimization overrides, launch arguments, Windows registry optimizations, and config backups for whichever game is selected on the Dashboard.

#### Engine & Config Profiles

Depending on the selected game, Overcapped automatically switches between five modes:

1. **JSON Profile Pack (Game-Specific):** If a `.json` pack matches the game's executable (like the included `rust.json` starter pack), Overcapped loads that game's custom presets and scales dynamic values to your hardware.
2. **Unreal Engine 4/5 Module (Built-In):** Automatically activates for detected UE4 and UE5 games. Safely merges optimizations into `Engine.ini` and `GameUserSettings.ini` inside `%LocalAppData%\<Game>\Saved\Config\Windows` (or `WindowsNoEditor` / `WinGDK`) without wiping your resolution or keybinds.
   - **Presets:** `Performance (Max FPS)`, `Balanced (Quality & Speed)`, `Recommended (Optimized)`, `Ultra (Maximum Visuals)`, and `Handheld/SteamDeck`.
   - **Hardware-Scaled Optimizations:** Scales `r.Streaming.PoolSize` to your GPU VRAM, enables UE5 PSO precaching (`r.PSOPrecaching`), async shader compilation, and parallel rendering, cuts GPU crash-debug overhead, and disables mouse smoothing/acceleration.
   - **Display & UE Controls:** Includes **FPS Limit**, **Screen Mode** (`Exclusive Fullscreen` / `Borderless`), **UE Lumen** and **UE Fog** toggles, **Auto-Detect** (picks a preset based on your VRAM, RAM, and CPU speed), and **Config Directory...** if a game stores its configs in a non-standard path.
3. **Unity Engine Module (Built-In):** Automatically activates for Unity titles without a dedicated JSON pack. Auto-locates `AppData\LocalLow\<Company>\<Product>` from `*_Data\app.info`, tunes `*_Data\boot.config` (native multi-threaded graphics jobs and incremental GC time-slice), and applies Unity display launch flags (`-window-mode exclusive` or `-popupwindow`). *If Easy Anti-Cheat, BattlEye, or EA AntiCheat is detected in the install folder, `boot.config` edits are skipped automatically for safety.*
4. **Source / Source 2 Module (Built-In):** Automatically activates for Source and Source 2 games. Locates the game's `cfg` folder, writes FPS cap and preset CVars inside a fenced Overcapped block in `autoexec.cfg` (leaving your existing keybinds and crosshair settings untouched), and sets Source launch arguments (`-novid`, `-fullscreen` / `-windowed -noborder`, `+fps_max`, `+exec autoexec.cfg`).
5. **Universal Mode:** For games using custom/proprietary engines without a matching JSON pack, you still get all per-game Windows optimizations, custom launch arguments, per-game auto-tweak overrides, plus all global System, RAM, and Network optimizations (or you can build a pack for it in the Game Pack Builder).

#### Game Optimizations & Backups

- **High CPU Priority (Admin):** Writes a native Windows IFEO entry (`Image File Execution Options\<GameExe>\PerfOptions\CpuPriorityClass = 3`) so Windows starts the game at High Priority before kernel anti-cheats lock the process handle.
- **High-Performance GPU Binding:** Writes `GpuPreference=2;` under `HKCU\Software\Microsoft\DirectX\UserGpuPreferences` so laptops and dual-GPU setups always run the game on your discrete GPU.
- **Backup Configs / Restore Backup:** Snapshots every `.ini`, `.cfg`, `.json`, and `.config` file in the active game's config folder into a timestamped backup folder and lets you restore from any saved backup. Overcapped also writes a `.universal.bak` file automatically before modifying configs.
- **JSON Packs...:** Opens your `%LocalAppData%\Overcapped Optimizer\Profiles\` folder so you can drop in, edit, or share custom game packs.
- **Per-Game Optimizations:** Opens a per-game override window for the selected title without breaking your global System Optimization toggles:
  - **Auto-Optimize When This Game Runs:** Toggle OFF to completely exclude a specific game from automatic runtime tweaks.
  - **Tri-State Overrides (`Use Global`, `Always ON`, `Always OFF`):** Override **Physical Cores Only** (e.g., set to `Always OFF` for a title that prefers SMT/Hyper-Threading while keeping it on globally), **Auto High / Ultimate Power Plan**, and **Throttle Background Apps** per title.
  - **Custom Launch Arguments:** Set per-game command-line flags passed automatically whenever you launch from Overcapped.
- **Open Game Pack Builder...:** Launches the built-in visual JSON Pack Builder / Editor (also available under **File -> Open Game Pack Builder...**).

---

### 3. System Optimization

Global Windows settings and a live background auto-optimizer (polls every 5 seconds when enabled). Whenever any monitored game in your library is running, the title bar updates to `ACTIVE: <GAME NAME>`, your enabled runtime optimizations (respecting any per-game overrides) kick in, and everything reverts cleanly when you close the game or exit Overcapped.

- **Physical Cores Only:** Pins running game processes to physical CPU cores to reduce SMT/hyperthreading contention, and restores full core affinity when toggled off or closed. *(If a kernel anti-cheat blocks external affinity changes, Overcapped handles it cleanly without crashing.)*
- **Auto High / Ultimate Power Plan:** Switches Windows to Ultimate Performance (falling back to High Performance if Ultimate isn't present) while a monitored game runs, then switches back to your previous power plan on exit.
- **Game Mode ON & Disable Game DVR:** Keeps Windows Game Mode turned on (important for modern CPU scheduling and Ryzen X3D CCD parking) while disabling background Xbox Game DVR capture overhead.
- **Disable Fullscreen Optimizations (FSO):** Applies Windows `GameConfigStore` overrides to cut down DWM fullscreen compositing latency.
- **Throttle Background Apps (with Custom Edit List):** Lowers listed background apps (browsers, launchers, RGB suites, Wallpaper Engine, Spotify, Overwolf, etc.) to `BelowNormal` priority while you play, then restores their original priority when the game closes. Click **Edit List** to add processes by name, browse for `.exe`s, quick-add currently running apps, remove entries, or reset to defaults.
- **In-Game FPS Overlay & Session Benchmarks (Admin):** A zero-injection FPS, **1% Low FPS**, and frametime (`ms`) HUD powered by native Windows ETW (`Microsoft-Windows-DXGI` and `Microsoft-Windows-D3D9` Present events).
  - Automatically tracks whichever window is currently in the foreground, pauses telemetry and hides when you tab out of the game, and resumes seamlessly when you tab back in.
  - Color-coded readout: Green (`60+ FPS`), Amber (`30-59 FPS`), Red (`< 30 FPS`).
  - **1% Low Toggle:** Toggle the live `1% Low` readout on or off directly from the button next to the position dropdown.
  - **Position & Opacity:** Pick a corner preset (`Top Left`, `Top Right`, `Top Center`, `Bottom Left`, `Bottom Right`, or `Custom`) and adjust opacity directly from the dropdown.
  - **Drag & Lock:** Click **Move** to unlock the HUD, drag it anywhere across any monitor, then **right-click the overlay** (or click **Lock**) to save the exact coordinates and return to click-through mode.
  - **Automatic HTML Benchmark Reports:** Whenever you exit a game while the FPS overlay is running, Overcapped automatically saves a self-contained HTML session report to `%LocalAppData%\Overcapped Optimizer\Benchmarks\` and prompts you to open it (or pops a clickable tray notification if minimized). Reports work 100% offline with and include Average FPS, 1% Low, 0.1% Low, frametime breakdown, stutter spike counts (`>33 ms` and `>50 ms`), an SVG FPS & 1% Low timeline graph, hardware snapshot, and active tweaks.
  - *Note: This uses zero DLL injection or DirectX hooking.*
- **Cache Cleaning & Utilities:**
  - **Clear DirectX / GPU Shader Cache:** Clears `%LocalAppData%\D3DSCache`, NVIDIA `DXCache` / `GLCache`, and AMD `DxCache` / `GLCache`. Good after driver updates or major game patches. *(Expect brief shader rebuild stutters during your first 5 or so minutes back in-game.)*
  - **Open Active Game Folder:** Opens the currently selected game's install directory in Explorer.
  - **Session Benchmarks...:** Opens your saved HTML benchmark reports folder (you can also jump straight to your latest report via **Help -> Open Latest Benchmark Report...**). You can also do a comparison of two reports by simply dragging another report file, into the currently open one in your browser. This helps fine tune the best settings for your rig, and figure out what settings make the biggest impact, and which don't.

---

### 4. RAM Auto-Flush

Uses the native Windows `NtSetSystemInformation` API (requires Admin) to purge the Windows Standby Memory List without touching active game memory. Helps stop late-session stuttering in memory-heavy games (Rust, Tarkov, DayZ, Star Citizen, UE5 titles, heavily modded games).

- **Enable Auto-Flush:** Master toggle for automatic standby memory clearing. *(Saving other tabs while running un-elevated will no longer overwrite a previously saved Auto-Flush setting.)*
- **Smart Threshold Mode (Recommended):** Checks live free physical RAM and Standby cache every 30 seconds. Only flushes when free RAM drops below your chosen GB limit (default `2 GB`) and standby cache is at least `0.5 GB`, with a 60-second cooldown so it won't spam mid-fight.
- **Fixed Timer Interval:** Turn Smart Mode off if you prefer a set interval in **Minutes** or **Hours**.
- **RAM Flush Sound Effect (SFX):** Optional audio cue whenever a flush runs.
- **Flush RAM Now:** Immediately purges standby memory on demand.

---

### 5. Network & Ping

Ethernet stability optimizations and real game-server diagnostics.

- **Disable NIC Power Saving / Energy-Efficient Ethernet (EEE) (Admin):** Disables EEE, Green Ethernet, GigaLite, PowerSavingMode, ULPMode, and PnP adapter sleep on active Ethernet NICs to stop random ping spikes and micro-disconnects. *(Reconnect your Ethernet adapter or reboot after applying so the driver reloads the settings.)*
- **QoS Expedited Tagging for Active Game (Admin):** Creates a Windows Policy-based QoS rule tagging the active game's outgoing UDP traffic with DSCP 46 (Expedited Forwarding) for QoS-aware routers.
- **Delivery Optimization Kill-Switch (Admin):** Disables Windows Update P2P uploads (`DODownloadMode = 0`, HTTP only) so Windows won't chew up your upload bandwidth mid-match.
- **Flush DNS Cache:** Runs `ipconfig /flushdns` in one click.
- **Server Jitter & Packet Loss Monitor:** Paste any IP, `IP:port`, hostname, website url, or console `connect IP:port` / `client.connect IP:port` command and set your ping amount count (`5` to `50`).
  - Reports **Avg / Min / Max Ping**, **Jitter** (with stability rating: Rock Solid / Good / Spiky), and **Packet Loss %**.
  - Automatically discovers Valve/Steam **UDP A2S** query ports so it works on game servers that block ICMP pings, and falls back to **ICMP** for websites and normal IPs.

---

### 6. Hotkeys

Bind global key combos so you can control Overcapped mid-game without Alt-Tabbing out of your match.

- **Enable Global Hotkeys:** Master toggle to turn global hotkey listening on or off.
- **Show / Hide FPS Overlay** (`Ctrl+Shift+O` default): Hides or shows the FPS HUD on demand without stopping background benchmark recording.
- **Toggle 1% Low FPS Readout** (`Ctrl+Shift+L` default): Switches the HUD between compact FPS/ms and full 1% Low telemetry.
- **Flush Standby RAM Now (Admin)** (`Ctrl+Shift+R` default): Silently clears Windows standby memory mid-match.
- **Show / Minimize Overcapped Window** (`Ctrl+Shift+M` default): Toggles Overcapped open or hidden to the system tray. When opened via this hotkey, Overcapped sets `TopMost` and shows a floating opacity slider bar so you can adjust window opacity on the fly; opening normally or via the tray's **Open** menu keeps it non-topmost at full opacity without the slider.
- **Bind / Clear / Reset Defaults:** Click **Bind**, press your combo (using `Ctrl`, `Shift`, or `Alt` is recommended), or press `Esc` to cancel.

---

### 7. System Tray, Updates & Logs

- **Minimize to System Tray:** Available from the top menu bar (`Minimize to Tray`) or Dashboard Quick Actions. Keeps auto-flush, the game auto-optimizer, global hotkeys, and the FPS overlay running quietly in the background. Right-click the tray icon to launch your active game, reopen the window, or exit cleanly (restoring all temporary tweaks).
- **Start as Admin & Check For Updates Toggles:** Both located in the sidebar. Toggling **Check For Updates** off and back on checks for updates right away.
- **Logs & About:** Quick links to the site, docs, GitHub, and Discord, plus one-click buttons for opening the log file, logs folder, and AppData folder.

---

## Custom JSON Profile Packs & Built-In Pack Builder

You do not need to hand-write JSON in Notepad anymore. Open **File -> Open Game Pack Builder...** (or click **Open Game Pack Builder...** on the **Game Profiles** tab) to build, edit, import, or export packs visually:

- **Auto-Fill from Library:** Pick any scanned game to auto-fill Game Name, Pack ID, Match Executable, and detected config file path.
- **Portable Config Paths:** Browsing to a config file automatically converts paths into portable tokens (`%LOCALAPPDATA%`, `%APPDATA%`, `{DOCUMENTS}`, `{LOCALLOW}`, `%USERPROFILE%`, or relative to the game's install folder) so shared packs work on other PCs out of the box.
- **Pull Keys...:** Reads an existing `.ini`, `.cfg`, or `.json` file and lets you check the exact keys you want to pull into Universal Settings or any Preset (including multi-section INI files tagged as `[SectionName]KeyName`).
- **Edit, Import & Export:** Load any installed pack with **Edit Installed Pack...**, import community `.json` packs to update them, inspect/copy the output with **Preview JSON**, click **Export .json...** to share in Discord, or **Save to Overcapped** to activate it right away.

A starter `rust.json` pack is generated automatically on first launch so you have a working reference.

### Supported Formats & Dynamic Tokens

- **`ConfigFormat`:**
  - `"SpaceDelimited"` - writes `key "value"` (Rust, Source, Frostbite `user.cfg`)
  - `"Ini"` - merges `key=value` under `IniSection` (supports multi-section targeting via `[SectionName]KeyName`)
  - `"KeyValueEquals"` - flat `key=value` files without INI section headers
  - `"Json"` - type-preserving nested JSON patching via dot-notation keys (e.g., `graphics.shadowQuality`)
- **Dynamic Tokens:**
  - `{AUTO_GC_BUFFER}` - scales to `1024`, `2048`, or `4096` based on total system RAM
  - `{AUTO_VRAM_POOL}` - calculates texture streaming pool size in MB from your GPU VRAM
  - `{FPS_LIMIT}` - pulls from your selected FPS cap in the UI
  - `{MENU_FPS_LIMIT}` - pulls from the game's menu FPS cap
  - `{SCREEN_MODE}` - writes `1` (Exclusive Fullscreen) or `0` (Borderless)
  - `{NVIDIA_REFLEX}` - writes `"2"` (On + Boost) on NVIDIA GPUs and skips the key on AMD/Intel

### Example Pack

```json
{
  "PackId": "example_game",
  "GameName": "Example Game",
  "MatchExecutable": "ExampleGame.exe",
  "ConfigFormat": "Ini",
  "RelativeConfigPath": "%LOCALAPPDATA%\\ExampleGame\\Saved\\Config\\Windows\\GameUserSettings.ini",
  "IniSection": "ScalabilityGroups",
  "UniversalSettings": {
    "[/Script/Engine.GameUserSettings]bUseVSync": "False",
    "[/Script/Engine.GameUserSettings]FrameRateLimit": "{FPS_LIMIT}"
  },
  "Presets": {
    "Performance (Max FPS)": {
      "sg.ShadowQuality": "0",
      "sg.FoliageQuality": "0"
    },
    "Recommended (Optimized)": {
      "sg.ShadowQuality": "2",
      "sg.FoliageQuality": "3"
    }
  }
}
```

---

## Anti-Cheat Safety

Overcapped is built to stay completely outside of protected game processes so it stays safe alongside Easy Anti-Cheat (EAC), BattlEye, Riot Vanguard, Ricochet, EA Javelin, ACE, etc:

- **Zero DLL injection** and **zero process memory reading/writing**.
- **No graphics hooking**, the FPS overlay reads native Windows ETW DXGI/D3D9 Present events externally.
- **Anti-cheat aware engine tuning** - skips Unity `*_Data\boot.config` edits automatically if EAC, BattlEye, or EA AntiCheat is detected in the game folder.
- **De-elevated game launching** - launches games with standard user permissions via Explorer's shell token even when Overcapped is running as Admin, and launches Steam / Epic games through their native launcher protocols.
- **Standard Windows APIs & registry policies** - IFEO CPU priority, DXGI GPU preference, Windows Power Plans, Policy QoS, Standby RAM flushing, and throttling *background* apps instead of poking the game.
- **Security features stay untouched** - never disables TPM, Secure Boot, or VBS/HVCI.

---

## Building From Source

Overcapped is built with C# WinForms on **.NET 8 / .NET 10** (Windows desktop target).

1. Clone the repository:
   ```bash
   git clone [https://github.com/V0idpool/Overcapped.git](https://github.com/V0idpool/Overcapped.git)
   ```
2. Open the solution in **Visual Studio 2022** (or newer) with the **.NET desktop development** workload installed.
3. Restore NuGet packages and build in `Release | x64`.

---

## Troubleshooting

- **Back up your configs first:** Hit **Backup Configs** under **Game Profiles** before applying a preset for the first time.
- **The FPS Counter is not displayed:** Make sure your game is in **Windowed Borderless**, **Borderless**, or **Fullscreen-Borderless** and go to the **System Optimization** tab. Toggle the **In-game FPS Overlay (Admin)** on, set the FPS Overlay settings, and show/hide 1% low information to show a minimal fps counter.
- **Brief stutters right after clearing shader cache:** Normal. DirectX, NVIDIA, and AMD drivers need a few minutes in-game to rebuild fresh shaders.
- **Unreal Engine config folder not auto-detected:** Launch the game once so it creates its `%LocalAppData%\<Game>\Saved\Config\Windows` folder, or use **Config Directory...** to point to it manually.
- **No benchmark report saved after exiting a game:** Make sure the **In-Game FPS Overlay** toggle is turned on (requires Admin) before launching your game, and play for at least a few seconds so ETW captures frame data.

---

## License & Links

Released under the **GPL-3.0 License**.

- **Website:** [https://overcapped.us/](https://overcapped.us/)
- **Documentation:** [https://overcapped.us/docs](https://overcapped.us/docs)
- **Void Tech Studios:** [https://voidtechstudios.us/](https://voidtechstudios.us/)
- **Discord Support & Community Packs:** [https://overcapped.us/invite](https://overcapped.us/invite)
- **Support Development:** [https://buymeacoffee.com/rustforgedev](https://buymeacoffee.com/rustforgedev)
