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

Designed to cut down stutters and get more out of your PC without digging through Windows settings and config files by hand. You can also import and export custom config profiles to add support for any game you play. Everything stays anti-cheat safe and resets cleanly when you close your game.

---

## Requirements & Setup

- **OS:** Windows 10 or Windows 11 (64-bit)
- **Install:** None. Overcapped is completely portable. Grab the latest `Overcapped.zip` from [overcapped.us/downloads](https://overcapped.us/downloads), extract it to a permanent folder (Desktop, Games drive, Tools folder, etc.), and run `Overcapped Optimizer.exe`.
- **Administrator Privileges:** You can run the app normally, but **RAM Auto-Flush**, **High CPU Priority**, **In-Game FPS Overlay**, **NIC Power Optimizations**, and **QoS DSCP 46** require Administrator rights. If you toggle an Admin feature without it, the app will prompt to restart elevated for you.

### Where Your Files Live

| Data | Path |
| :--- | :--- |
| **Settings & Game Library** | `%LocalAppData%\Overcapped Optimizer\settings.json` |
| **Custom JSON Profile Packs** | `%LocalAppData%\Overcapped Optimizer\Profiles\` *(also loads from `Profiles\` next to the `.exe`)* |
| **Config Backups** | `backups\` inside the Overcapped folder (timestamped folder per game) |
| **Logs** | `User\Logs\log_file.txt` inside the Overcapped folder |

Because your settings, game library, and custom packs live in AppData, dropping in a newer build will not wipe your setup.

---

## Features & How to Use Each Tab

### 1. Dashboard

Your main hub for managing installed games, checking hardware specs, and running one-click maintenance.

- **Game Library:**
  - **Scan Games:** Scans all Steam libraries across every drive (`libraryfolders.vdf` + `appmanifest_*.acf`) and Epic Games Launcher manifests automatically (skipping Steamworks redistributables, Proton, and incomplete installs). Detects the game engine (**Unreal Engine**, **Unity**, **Source**, or **Generic**), resolves the actual game binary (including `*-Win64-Shipping.exe`, paired `*Client.exe` games, and games using Easy Anti-Cheat's `start_protected_game.exe` bootstrapper so optimizations hit the real game while Launch still boots anti-cheat properly), and locates config folders when available.
  - **Add .exe...:** Manually add any game from Battle.net, EA App, Ubisoft, GOG, or standalone installs by pointing it at the game's `.exe`.
  - **Remove:** Removes a title from Overcapped's list without uninstalling anything. Handy for clearing out non-game Steam apps like Wallpaper Engine or Lossless Scaling.
- **Hardware Information:** Reads your CPU model and max clock speed, discrete GPU and VRAM (filtering out virtual/remote display adapters), and total RAM with module info. Used for preset recommendations and hardware-scaled config tokens.
- **Quick Actions:** **Flush Standby RAM**, **Clear GPU Shader Cache**, **Minimize to System Tray**, and a support link.
- **Launch Game (Sidebar):** Pinned at the bottom left so you can launch whichever game is currently selected from any tab.

---

### 2. Game Profiles

Manages per-game engine configs, custom JSON profile packs, per-game Windows registry optimizations, and config backups for whichever game is selected on the Dashboard.

#### Engine & Config Profiles

Depending on the selected game, Overcapped automatically switches between three modes:

1. **JSON Profile Pack (Game-Specific):** If a `.json` pack matches the game's executable (like the included `rust.json` starter pack), Overcapped loads that game's custom presets and scales dynamic values to your hardware.
2. **Unreal Engine 4/5 Module (Built-In):** Automatically activates for detected UE4 and UE5 games. Safely merges optimizations into `Engine.ini` and `GameUserSettings.ini` inside `%LocalAppData%\<Game>\Saved\Config\Windows` (or `WindowsNoEditor` / `WinGDK`) without wiping your resolution or keybinds.
   - **Presets:** `Performance (Max FPS)`, `Balanced`, `Recommended (Optimized)`, `Ultra (Maximum Visuals)`, and `Handheld/SteamDeck`.
   - **Hardware-Scaled Optimizations:** Scales `r.Streaming.PoolSize` to your GPU VRAM, enables UE5 PSO precaching (`r.PSOPrecaching`), async shader compilation, and parallel rendering, cuts GPU crash-debug overhead, and disables mouse smoothing/acceleration.
   - **Display & UE Controls:** Includes **FPS Limit**, **Screen Mode** (`Exclusive Fullscreen` / `Borderless`), **UE Lumen** and **UE Fog** toggles, **Auto-Detect** (picks a preset based on your VRAM, RAM, and CPU speed), and **Config Directory...** if a game stores its configs in a non-standard path.
3. **Universal Mode:** For games without an Unreal config, Unity config, or JSON pack, you still get all per-game Windows optimizations plus all global System, RAM, and Network optimizations.

#### Game Optimizations & Backups

- **High CPU Priority (Admin):** Writes a native Windows IFEO entry (`Image File Execution Options\<GameExe>\PerfOptions\CpuPriorityClass = 3`) so Windows starts the game at High Priority before kernel anti-cheats lock the process handle.
- **High-Performance GPU Binding:** Writes `GpuPreference=2;` under `HKCU\Software\Microsoft\DirectX\UserGpuPreferences` so laptops and dual-GPU setups always run the game on your discrete GPU.
- **Backup Configs / Restore Backup:** Snapshots every `.ini` and `.cfg` file in the active game's config folder into a timestamped backup folder and lets you restore from any saved backup. Overcapped also writes a `.universal.bak` file automatically before modifying configs.
- **JSON Packs...:** Opens your `%LocalAppData%\Overcapped Optimizer\Profiles\` folder so you can drop in, edit, or share custom game packs.

---

### 3. System Optimization

Global Windows settings and a live background watchdog (polls every 5 seconds when enabled). Whenever any game in your library is running, the title bar updates to `ACTIVE: <GAME NAME>`, your enabled runtime optimizations kick in, and everything reverts cleanly when you close the game or exit Overcapped.

- **Physical Cores Only:** Pins running game processes to physical CPU cores to reduce SMT/hyperthreading contention, and restores full core affinity when toggled off or closed. *(If a kernel anti-cheat blocks external affinity changes, Overcapped handles it cleanly without crashing.)*
- **Auto High / Ultimate Power Plan:** Switches Windows to Ultimate Performance (falling back to High Performance if Ultimate isn't present) while a library game runs, then switches back to your previous power plan on exit.
- **Game Mode ON & Disable Game DVR:** Keeps Windows Game Mode turned on (important for modern CPU scheduling and Ryzen X3D CCD parking) while disabling background Xbox Game DVR capture overhead.
- **Disable Fullscreen Optimizations (FSO):** Applies Windows `GameConfigStore` overrides to cut down DWM fullscreen compositing latency.
- **Throttle Background Apps:** Lowers heavy browsers (`chrome`, `msedge`, `firefox`, `brave`, `opera`, `opera_gx`) and launchers (`steamwebhelper`, `epicgameslauncher`) to `BelowNormal` priority while you play, then restores their original priority when the game closes.
- **In-Game FPS Overlay (Admin):** A zero-injection FPS and frametime (`ms`) HUD powered by native Windows ETW (`Microsoft-Windows-DXGI` and `Microsoft-Windows-D3D9` Present events).
  - Automatically tracks whichever window is currently in the foreground.
  - Color-coded readout: Green (`60+ FPS`), Amber (`30-59 FPS`), Red (`< 30 FPS`).
  - **Position & Opacity:** Pick a corner preset (`Top Left`, `Top Right`, `Top Center`, `Bottom Left`, `Bottom Right`, or `Custom`) and adjust opacity directly from the dropdown.
  - **Drag & Lock:** Click **Move** to unlock the HUD, drag it anywhere across any monitor, then **right-click the overlay** (or click **Lock**) to save the exact coordinates and return to click-through mode.
  - *Note: This uses zero DLL injection or DirectX hooking.*
- **Cache Cleaning & Utilities:**
  - **Clear DirectX / GPU Shader Cache:** Clears `%LocalAppData%\D3DSCache`, NVIDIA `DXCache` / `GLCache`, and AMD `DxCache` / `GLCache`. Good after driver updates or major game patches. *(Expect brief shader rebuild stutters during your first 5 or so minutes back in-game.)*
  - **Open Active Game Folder:** Opens the currently selected game's install directory in Explorer.

---

### 4. RAM Auto-Flush

Uses the native Windows `NtSetSystemInformation` API (requires Admin) to purge the Windows Standby Memory List without touching active game memory. Helps stop late-session stuttering in memory-heavy games (Rust, Tarkov, DayZ, Star Citizen, UE5 titles, heavily modded games).

- **Enable Auto-Flush:** Master toggle for automatic standby memory clearing.
- **Smart Threshold Mode (Recommended):** Checks live free physical RAM and Standby cache every 30 seconds. Only flushes when free RAM drops below your chosen GB limit (default `2 GB`) and standby cache is at least `0.5 GB`, with a 60-second cooldown so it won't spam mid-fight.
- **Fixed Timer Interval:** Turn Smart Mode off if you prefer a set interval in **Minutes** or **Hours**.
- **RAM Flush Sound Effect (SFX):** Optional audio cue whenever a flush runs.
- **Flush RAM Now:** Immediately purges standby memory on demand.

---

### 5. Network & Ping

Ethernet stability optimizations and real game-server diagnostics.

- **Disable NIC Power Saving / Energy-Efficient Ethernet (EEE) (Admin):** Disables EEE, Green Ethernet, GigaLite, PowerSavingMode, ULPMode, and PnP adapter sleep on active Ethernet NICs to stop random ping spikes and micro-disconnects. *(Reconnect your Ethernet adapter or reboot after applying so the driver reloads the settings.)*
- **QoS Expedited Tagging for Active Game (Admin):** Creates a Windows Policy-based QoS rule tagging the active game's outgoing UDP traffic with DSCP 46 (Expedited Forwarding) for QoS-aware routers.
- **Flush DNS Cache:** Runs `ipconfig /flushdns` in one click.
- **Server Jitter & Packet Loss Monitor:** Paste any IP, `IP:port`, hostname, website url, or console `connect IP:port` / `client.connect IP:port` command and set your ping amount count (`5` to `50`).
  - Reports **Avg / Min / Max Ping**, **Jitter** (with stability rating: Rock Solid / Good / Spiky), and **Packet Loss %**.
  - Automatically discovers Valve/Steam **UDP A2S** query ports so it works on game servers that block ICMP pings, and falls back to **ICMP** for websites and normal IPs.

---

### 6. System Tray, Updates & Logs

- **Minimize to System Tray:** Available from the top menu bar (`Minimize to Tray`) or Dashboard Quick Actions. Keeps auto-flush, the game watchdog, and the FPS overlay running quietly in the background. Right-click the tray icon to launch your active game, reopen the window, or exit cleanly (restoring all temporary tweaks).
- **Check For Updates Toggle:** Located in the sidebar (enabled by default). Toggling it off and back on checks for updates right away.
- **Logs & About:** Quick links to the site, docs, GitHub, and Discord, plus one-click buttons for opening the log file, logs folder, and AppData folder.

---

## Custom JSON Profile Packs (Import & Export)

You are not stuck waiting on app updates to get config support for a specific game. Overcapped loads external `.json` packs from `%LocalAppData%\Overcapped Optimizer\Profiles\` (or a `Profiles\` folder next to the `.exe`), so you can create, import, and share packs for pretty much any PC game.

A starter `rust.json` pack is generated automatically on first launch so you have a working reference.

### Supported Formats & Dynamic Tokens

- **`ConfigFormat`:** `"SpaceDelimited"` (writes `key "value"`, like Rust/Source `.cfg` files) or `"Ini"` (merges `key=value` under `IniSection`).
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
  "RelativeConfigPath": "Config\\settings.ini",
  "IniSection": "SystemSettings",
  "UniversalSettings": {
    "bUseVSync": "False",
    "r.Streaming.PoolSize": "{AUTO_VRAM_POOL}"
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
- **Standard Windows APIs & registry policies** - IFEO CPU priority, DXGI GPU preference, Windows Power Plans, Policy QoS, Standby RAM flushing, and throttling *background* apps instead of poking the game.
- **Security features stay untouched** - never disables TPM, Secure Boot, or VBS/HVCI.

---

## Building From Source

Overcapped is built with C# WinForms on **.NET 8 / .NET 10** (Windows desktop target).

1. Clone the repository:
   ```bash
   git clone https://github.com/V0idpool/Overcapped.git
   ```
2. Open the solution in **Visual Studio 2022** (or newer) with the **.NET desktop development** workload installed.
3. Restore NuGet packages and build in `Release | x64`.

---

## Troubleshooting

- **Back up your configs first:** Hit **Backup Configs** under **Game Profiles** before applying a preset for the first time.
- **Brief stutters right after clearing shader cache:** Normal. DirectX, NVIDIA, and AMD drivers need a few minutes in-game to rebuild fresh shaders.
- **Unreal Engine config folder not auto-detected:** Launch the game once so it creates its `%LocalAppData%\<Game>\Saved\Config\Windows` folder, or use **Config Directory...** to point to it manually.

---

## License & Links

Released under the **GPL-3.0 License**.

- **Website:** [https://overcapped.us/](https://overcapped.us/)
- **Documentation:** [https://overcapped.us/docs](https://overcapped.us/docs)
- **Void Tech Studios:** [https://voidtechstudios.us/](https://voidtechstudios.us/)
- **Discord Support & Community Packs:** [https://overcapped.us/invite](https://overcapped.us/invite)
- **Support Development:** [https://buymeacoffee.com/rustforgedev](https://buymeacoffee.com/rustforgedev)
