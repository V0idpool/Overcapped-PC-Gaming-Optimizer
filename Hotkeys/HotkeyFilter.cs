using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Overcapped_Optimizer.Hotkeys
{
    public class HotkeyFilter : IMessageFilter
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        private readonly Action<Keys>? _onKeyCaptured;
        public static volatile bool IsCapturingHotkey = false;
        public HotkeyFilter(Action<Keys> onKeyCaptured)
        {
            _onKeyCaptured = onKeyCaptured;
            IsCapturingHotkey = true;
        }

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;
            const int WM_SYSKEYDOWN = 0x0104;

            if (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
            {
                Keys key = (Keys)m.WParam.ToInt32();

                if (IsModifierKey(key))
                    return true;

                if (key == Keys.Escape)
                {
                    IsCapturingHotkey = false;
                    _onKeyCaptured?.Invoke(Keys.Escape);
                    return true;
                }

                Keys modifiers = Control.ModifierKeys;
                if (key == Keys.Return && modifiers == Keys.None)
                    return true;

                Keys fullHotkey = key | modifiers;
                IsCapturingHotkey = false;
                _onKeyCaptured?.Invoke(fullHotkey);
                return true;
            }

            return false;
        }

        private static bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey ||
                   key == Keys.LControlKey ||
                   key == Keys.RControlKey ||
                   key == Keys.ShiftKey ||
                   key == Keys.LShiftKey ||
                   key == Keys.RShiftKey ||
                   key == Keys.Menu ||
                   key == Keys.LMenu ||
                   key == Keys.RMenu ||
                   key == Keys.LWin ||
                   key == Keys.RWin;
        }

        private sealed class HotkeyData
        {
            public string Name { get; set; } = string.Empty;
            public Keys BaseKey { get; set; }
            public bool ReqCtrl { get; set; }
            public bool ReqShift { get; set; }
            public bool ReqAlt { get; set; }
            public bool WasDown { get; set; }
            public DateTime LastTriggered { get; set; }
        }

        private readonly List<HotkeyData> _activeHotkeys = new();
        private readonly object _lock = new();
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly int _debounceMilliseconds = 500;

        public event Action<string>? HotkeyTriggered;

        public HotkeyFilter()
        {
        }

        public void Register(string name, Keys keyData)
        {
            Keys baseKey = keyData & ~Keys.Modifiers;
            if (baseKey == Keys.None) return;

            lock (_lock)
            {
                _activeHotkeys.RemoveAll(k => k.Name == name);

                _activeHotkeys.Add(new HotkeyData
                {
                    Name = name,
                    BaseKey = baseKey,
                    ReqCtrl = (keyData & Keys.Control) == Keys.Control,
                    ReqShift = (keyData & Keys.Shift) == Keys.Shift,
                    ReqAlt = (keyData & Keys.Alt) == Keys.Alt,
                    WasDown = IsKeyDown(baseKey),
                    LastTriggered = DateTime.MinValue
                });
            }

            Debug.WriteLine($"[Hotkeys] Registered: {name} as {FormatKeys(keyData)}");
        }

        public void Unregister(string name)
        {
            lock (_lock)
            {
                _activeHotkeys.RemoveAll(k => k.Name == name);
            }
        }

        public void ClearAll()
        {
            lock (_lock)
            {
                _activeHotkeys.Clear();
            }
        }

        public void StartListening()
        {
            if (_cancellationTokenSource != null) return;

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Task.Run(() => PollingLoop(token), token);
        }

        public void StopListening()
        {
            if (_cancellationTokenSource == null) return;

            try { _cancellationTokenSource.Cancel(); } catch { }
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task PollingLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!IsCapturingHotkey)
                    {
                        List<HotkeyData> snapshot;
                        lock (_lock)
                        {
                            snapshot = _activeHotkeys.ToList();
                        }

                        if (snapshot.Count > 0)
                        {
                            bool isCtrlDown = IsKeyDown(Keys.ControlKey);
                            bool isShiftDown = IsKeyDown(Keys.ShiftKey);
                            bool isAltDown = IsKeyDown(Keys.Menu);

                            foreach (var hk in snapshot)
                            {
                                bool baseDown = IsKeyDown(hk.BaseKey);

                                if (baseDown)
                                {
                                    if (!hk.WasDown &&
                                        hk.ReqCtrl == isCtrlDown &&
                                        hk.ReqShift == isShiftDown &&
                                        hk.ReqAlt == isAltDown)
                                    {
                                        hk.WasDown = true;

                                        if ((DateTime.UtcNow - hk.LastTriggered).TotalMilliseconds > _debounceMilliseconds)
                                        {
                                            hk.LastTriggered = DateTime.UtcNow;
                                            HotkeyTriggered?.Invoke(hk.Name);
                                        }
                                    }
                                }
                                else
                                {
                                    hk.WasDown = false;
                                }
                            }
                        }
                    }

                    await Task.Delay(50, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Hotkeys Error] {ex.Message}");
                }
            }
        }

        private static bool IsKeyDown(Keys key) => (GetAsyncKeyState(key) & 0x8000) != 0;

        public static string FormatKeys(Keys keyData)
        {
            Keys baseKey = keyData & ~Keys.Modifiers;
            if (baseKey == Keys.None) return string.Empty;

            var parts = new List<string>(4);
            if ((keyData & Keys.Control) == Keys.Control) parts.Add("Ctrl");
            if ((keyData & Keys.Alt) == Keys.Alt) parts.Add("Alt");
            if ((keyData & Keys.Shift) == Keys.Shift) parts.Add("Shift");

            string keyName = baseKey switch
            {
                >= Keys.D0 and <= Keys.D9 => ((char)('0' + (baseKey - Keys.D0))).ToString(),
                Keys.Oemtilde => "~",
                Keys.OemMinus => "-",
                Keys.Oemplus => "=",
                Keys.OemOpenBrackets => "[",
                Keys.OemCloseBrackets => "]",
                _ => baseKey.ToString()
            };

            parts.Add(keyName);
            return string.Join("+", parts);
        }

        public static bool TryParseKeys(string? keyString, out Keys result)
        {
            result = Keys.None;
            if (string.IsNullOrWhiteSpace(keyString)) return false;

            Keys modifiers = Keys.None;
            Keys baseKey = Keys.None;

            foreach (string raw in keyString.Split('+', StringSplitOptions.RemoveEmptyEntries))
            {
                string p = raw.Trim();
                string lower = p.ToLowerInvariant();

                if (lower is "ctrl" or "control") modifiers |= Keys.Control;
                else if (lower == "alt") modifiers |= Keys.Alt;
                else if (lower == "shift") modifiers |= Keys.Shift;
                else if (p.Length == 1 && char.IsDigit(p[0]))
                    baseKey = Keys.D0 + (p[0] - '0');
                else if (Enum.TryParse(p, ignoreCase: true, out Keys parsed))
                    baseKey = parsed;
            }

            if (baseKey == Keys.None) return false;
            result = baseKey | modifiers;
            return true;
        }
    }
}