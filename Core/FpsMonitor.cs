using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Overcapped_Optimizer.Helpers
{
    public sealed class FpsTimelinePoint
    {
        public int ElapsedSeconds { get; init; }
        public double AvgFps { get; init; }
        public double OnePercentLowFps { get; init; }
    }

    public sealed class FpsSessionSummary
    {
        public string GameName { get; init; } = string.Empty;
        public string ExecutableName { get; init; } = string.Empty;
        public string PresetName { get; init; } = string.Empty;
        public string EngineName { get; init; } = string.Empty;
        public DateTime StartedAt { get; init; }
        public DateTime EndedAt { get; init; }
        public TimeSpan Duration { get; init; }
        public int TotalFrames { get; init; }
        public double AvgFps { get; init; }
        public double MedianFps { get; init; }
        public double OnePercentLowFps { get; init; }
        public double PointOnePercentLowFps { get; init; }
        public double MinFps { get; init; }
        public double MaxFps { get; init; }
        public double AvgFrameTimeMs { get; init; }
        public double P99FrameTimeMs { get; init; }
        public double MaxFrameTimeMs { get; init; }
        public int StutterFrames33Ms { get; init; }
        public int SevereStutterFrames50Ms { get; init; }
        public double PctUnder8_3Ms { get; init; }
        public double Pct8_3To16_7Ms { get; init; }
        public double Pct16_7To33_3Ms { get; init; }
        public double PctOver33_3Ms { get; init; }
        public List<FpsTimelinePoint> Timeline { get; init; } = new();
    }

    public sealed class EtwFpsMonitor : IDisposable
    {
        private const string SessionName = "OvercappedOptimizer_FpsTrace";
        private static readonly Guid DxgiProviderGuid = new("CA11C036-0102-4A2D-A6AD-F03CFED5D3C9");
        private static readonly Guid D3D9ProviderGuid = new("783ACA0A-790E-4D7F-8451-AA850511C6B9");
        private const uint WNODE_FLAG_TRACED_GUID = 0x00020000;
        private const uint EVENT_TRACE_REAL_TIME_MODE = 0x00000100;
        private const uint EVENT_TRACE_CONTROL_STOP = 1;
        private const uint EVENT_CONTROL_CODE_ENABLE_PROVIDER = 1;
        private const uint PROCESS_TRACE_MODE_REAL_TIME = 0x00000100;
        private const uint PROCESS_TRACE_MODE_EVENT_RECORD = 0x10000000;
        private const byte TRACE_LEVEL_INFORMATION = 4;
        private static readonly ulong InvalidTraceHandle = unchecked((ulong)-1);
        private const int MaxStoredSessionFrames = 2_000_000;
        private ulong _sessionHandle;
        private ulong _traceHandle = InvalidTraceHandle;
        private Thread? _traceThread;
        private EventRecordCallbackDelegate? _callbackPin;
        private volatile bool _running;
        private int _targetPid;
        private readonly object _frameLock = new();
        private readonly Queue<long> _frameEventTicks = new();
        private readonly Queue<(long EventTick, double FrameMs)> _recentFrameTimes = new();
        private long _lastEventTick;
        private long _lastWallTick;
        private readonly object _sessionLock = new();
        private int _sessionPid;
        private string _sessionProcessName = string.Empty;
        private string _sessionGameName = string.Empty;
        private string _sessionExeName = string.Empty;
        private string _sessionPreset = string.Empty;
        private string _sessionEngine = string.Empty;
        private DateTime _sessionStartedAt;
        private long _sessionStartEventTick;
        private long _sessionLastEventTick;
        private int _sessionTotalFrames;
        private readonly List<float> _sessionFrameTimesMs = new();
        private readonly List<double> _bucketFrameTimesMs = new();
        private int _currentBucketSecond;
        private readonly List<FpsTimelinePoint> _sessionTimeline = new();
        public bool IsRunning => _running;
        public bool HasActiveSession => Volatile.Read(ref _sessionPid) > 0;
        public int ActiveSessionPid => Volatile.Read(ref _sessionPid);
        public string ActiveSessionProcessName
        {
            get { lock (_sessionLock) return _sessionProcessName; }
        }

        public void SetTargetProcessId(int pid)
        {
            int previous = Interlocked.Exchange(ref _targetPid, pid);
            if (previous != pid)
            {
                lock (_frameLock)
                {
                    _frameEventTicks.Clear();
                    _recentFrameTimes.Clear();
                    _lastEventTick = 0;
                    _lastWallTick = 0;
                }
            }
        }

        public void BeginSession(string gameName, string executableName, string processName, string presetName, string engineName, int pid)
        {
            if (pid <= 0) return;

            lock (_sessionLock)
            {
                if (_sessionPid == pid) return;

                _sessionGameName = gameName;
                _sessionExeName = executableName;
                _sessionProcessName = processName;
                _sessionPreset = presetName;
                _sessionEngine = engineName;
                _sessionStartedAt = DateTime.Now;
                _sessionStartEventTick = 0;
                _sessionLastEventTick = 0;
                _sessionTotalFrames = 0;
                _sessionFrameTimesMs.Clear();
                _bucketFrameTimesMs.Clear();
                _sessionTimeline.Clear();
                _currentBucketSecond = 0;
                Volatile.Write(ref _sessionPid, pid);
            }

            ExceptionHandler.LogMessage($"[Benchmark] Started session recording for {gameName} (PID {pid}).");
        }

        public FpsSessionSummary? EndSession()
        {
            List<float> samples;
            List<FpsTimelinePoint> timeline;
            string gameName, exeName, presetName, engineName;
            DateTime startedAt;
            DateTime endedAt = DateTime.Now;
            int totalFrames;

            lock (_sessionLock)
            {
                if (_sessionPid <= 0) return null;
                Volatile.Write(ref _sessionPid, 0);

                FlushCurrentTimelineBucketLocked();

                gameName = _sessionGameName;
                exeName = _sessionExeName;
                presetName = _sessionPreset;
                engineName = _sessionEngine;
                startedAt = _sessionStartedAt;
                totalFrames = _sessionTotalFrames;
                samples = new List<float>(_sessionFrameTimesMs);
                timeline = new List<FpsTimelinePoint>(_sessionTimeline);

                _sessionProcessName = string.Empty;
                _sessionFrameTimesMs.Clear();
                _bucketFrameTimesMs.Clear();
                _sessionTimeline.Clear();
                _sessionStartEventTick = 0;
                _sessionLastEventTick = 0;
                _sessionTotalFrames = 0;
            }

            TimeSpan duration = endedAt - startedAt;
            if (samples.Count < 60 || duration.TotalSeconds < 5.0)
            {
                ExceptionHandler.LogMessage($"[Benchmark] Skipped report for {gameName}: {samples.Count} frame samples over {duration.TotalSeconds:F1}s.");
                return null;
            }

            double sumMs = 0;
            int stutter33 = 0;
            int stutter50 = 0;
            int under8_3 = 0, between8_3And16_7 = 0, between16_7And33_3 = 0, over33_3 = 0;

            for (int i = 0; i < samples.Count; i++)
            {
                float ms = samples[i];
                sumMs += ms;

                if (ms < 8.333f) under8_3++;
                else if (ms < 16.667f) between8_3And16_7++;
                else if (ms < 33.333f) between16_7And33_3++;
                else
                {
                    over33_3++;
                    stutter33++;
                    if (ms >= 50.0f) stutter50++;
                }
            }

            samples.Sort();
            int n = samples.Count;

            double avgMs = sumMs / n;
            double medianMs = samples[n / 2];
            double minMs = samples[0];
            double maxMs = samples[n - 1];
            double p99Ms = samples[Math.Min(n - 1, (int)(n * 0.99))];

            double onePercentLowFps = CalculateLowPercentileFps(samples, 0.01);
            double pointOnePercentLowFps = CalculateLowPercentileFps(samples, 0.001);

            return new FpsSessionSummary
            {
                GameName = gameName,
                ExecutableName = exeName,
                PresetName = presetName,
                EngineName = engineName,
                StartedAt = startedAt,
                EndedAt = endedAt,
                Duration = duration,
                TotalFrames = totalFrames,
                AvgFps = avgMs > 0 ? 1000.0 / avgMs : 0,
                MedianFps = medianMs > 0 ? 1000.0 / medianMs : 0,
                OnePercentLowFps = onePercentLowFps,
                PointOnePercentLowFps = pointOnePercentLowFps,
                MinFps = maxMs > 0 ? 1000.0 / maxMs : 0,
                MaxFps = minMs > 0 ? 1000.0 / minMs : 0,
                AvgFrameTimeMs = avgMs,
                P99FrameTimeMs = p99Ms,
                MaxFrameTimeMs = maxMs,
                StutterFrames33Ms = stutter33,
                SevereStutterFrames50Ms = stutter50,
                PctUnder8_3Ms = (under8_3 * 100.0) / n,
                Pct8_3To16_7Ms = (between8_3And16_7 * 100.0) / n,
                Pct16_7To33_3Ms = (between16_7And33_3 * 100.0) / n,
                PctOver33_3Ms = (over33_3 * 100.0) / n,
                Timeline = DownsampleTimeline(timeline, 180)
            };
        }

        public (int Fps, double FrameTimeMs) GetCurrentStats()
        {
            var (fps, frameTimeMs, _) = GetDetailedStats();
            return (fps, frameTimeMs);
        }

        public (int Fps, double FrameTimeMs, int OnePercentLowFps) GetDetailedStats()
        {
            long wallNow = Stopwatch.GetTimestamp();
            long oneSecond = Stopwatch.Frequency;
            long staleThreshold = Stopwatch.Frequency * 2;
            long tenSeconds = Stopwatch.Frequency * 10;

            int count;
            double[]? recentCopy = null;

            lock (_frameLock)
            {
                if (_lastWallTick == 0 || (wallNow - _lastWallTick) > staleThreshold)
                {
                    _frameEventTicks.Clear();
                    _recentFrameTimes.Clear();
                    return (0, 0.0, 0);
                }

                long refTick = _lastEventTick;

                while (_frameEventTicks.Count > 0 && (refTick - _frameEventTicks.Peek()) > oneSecond)
                {
                    _frameEventTicks.Dequeue();
                }

                while (_recentFrameTimes.Count > 0 && (refTick - _recentFrameTimes.Peek().EventTick) > tenSeconds)
                {
                    _recentFrameTimes.Dequeue();
                }

                count = _frameEventTicks.Count;
                if (count == 0)
                {
                    return (0, 0.0, 0);
                }

                if (_recentFrameTimes.Count >= 10)
                {
                    recentCopy = new double[_recentFrameTimes.Count];
                    int idx = 0;
                    foreach (var item in _recentFrameTimes)
                        recentCopy[idx++] = item.FrameMs;
                }
            }

            double frameTimeMs = 1000.0 / count;
            int onePercentLow = 0;

            if (recentCopy != null && recentCopy.Length >= 10)
            {
                Array.Sort(recentCopy);
                int worstCount = Math.Max(1, (int)Math.Ceiling(recentCopy.Length * 0.01));
                double worstSum = 0;
                for (int i = recentCopy.Length - worstCount; i < recentCopy.Length; i++)
                    worstSum += recentCopy[i];

                double avgWorstMs = worstSum / worstCount;
                if (avgWorstMs > 0)
                    onePercentLow = Math.Min(count, (int)Math.Round(1000.0 / avgWorstMs));
            }

            return (count, frameTimeMs, onePercentLow);
        }

        public bool Start()
        {
            if (_running) return true;

            try
            {
                StopExistingSession();

                IntPtr propsPtr = AllocateTraceProperties(out _);
                try
                {
                    uint status = StartTraceW(out _sessionHandle, SessionName, propsPtr);
                    if (status != 0)
                    {
                        ExceptionHandler.LogMessage($"[FPS] StartTraceW failed with error {status} (Admin privileges required).");
                        return false;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(propsPtr);
                }

                Guid dxgi = DxgiProviderGuid;
                Guid d3d9 = D3D9ProviderGuid;

                EnableTraceEx2(_sessionHandle, ref dxgi, EVENT_CONTROL_CODE_ENABLE_PROVIDER, TRACE_LEVEL_INFORMATION, 0xFFFFFFFFFFFFFFFF, 0, 0, IntPtr.Zero);
                EnableTraceEx2(_sessionHandle, ref d3d9, EVENT_CONTROL_CODE_ENABLE_PROVIDER, TRACE_LEVEL_INFORMATION, 0xFFFFFFFFFFFFFFFF, 0, 0, IntPtr.Zero);

                _callbackPin = OnEventRecord;

                var logFile = new EVENT_TRACE_LOGFILEW
                {
                    LoggerName = SessionName,
                    ProcessTraceMode = PROCESS_TRACE_MODE_REAL_TIME | PROCESS_TRACE_MODE_EVENT_RECORD,
                    EventRecordCallback = _callbackPin
                };

                _traceHandle = OpenTraceW(ref logFile);
                if (_traceHandle == InvalidTraceHandle)
                {
                    Stop();
                    return false;
                }

                _running = true;
                _traceThread = new Thread(TraceLoop)
                {
                    IsBackground = true,
                    Name = "Optimizer_FpsMonitor"
                };
                _traceThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                Stop();
                return false;
            }
        }

        private void TraceLoop()
        {
            try
            {
                ulong[] handles = { _traceHandle };
                ProcessTrace(handles, 1, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
            }
            finally
            {
                _running = false;
            }
        }

        private void OnEventRecord(ref EVENT_RECORD record)
        {
            if (!_running) return;

            uint eventPid = record.EventHeader.ProcessId;
            int wantedPid = Volatile.Read(ref _targetPid);
            int sessionPid = Volatile.Read(ref _sessionPid);

            bool matchesHud = wantedPid > 0 && eventPid == (uint)wantedPid;
            bool matchesSession = sessionPid > 0 && eventPid == (uint)sessionPid;
            if (!matchesHud && !matchesSession) return;

            ushort eventId = record.EventHeader.EventDescriptor.Id;
            Guid provider = record.EventHeader.ProviderId;

            bool isPresent =
                (provider == DxgiProviderGuid && (eventId == 42 || eventId == 55)) ||
                (provider == D3D9ProviderGuid && eventId == 1);

            if (!isPresent) return;

            long wallNow = Stopwatch.GetTimestamp();
            long eventTs = record.EventHeader.TimeStamp > 0 ? record.EventHeader.TimeStamp : wallNow;
            double ticksToMs = 1000.0 / Stopwatch.Frequency;

            if (matchesHud)
            {
                lock (_frameLock)
                {
                    if (_lastEventTick > 0 && eventTs > _lastEventTick)
                    {
                        double dtMs = (eventTs - _lastEventTick) * ticksToMs;
                        if (dtMs >= 0.25 && dtMs <= 1000.0)
                        {
                            _recentFrameTimes.Enqueue((eventTs, dtMs));
                        }
                    }

                    _frameEventTicks.Enqueue(eventTs);
                    if (eventTs > _lastEventTick) _lastEventTick = eventTs;
                    _lastWallTick = wallNow;
                }
            }

            if (matchesSession)
            {
                lock (_sessionLock)
                {
                    if (_sessionPid > 0 && eventPid == (uint)_sessionPid)
                    {
                        _sessionTotalFrames++;

                        if (_sessionStartEventTick == 0)
                            _sessionStartEventTick = eventTs;

                        if (_sessionLastEventTick > 0 && eventTs > _sessionLastEventTick)
                        {
                            double dtMs = (eventTs - _sessionLastEventTick) * ticksToMs;
                            if (dtMs >= 0.25 && dtMs <= 1000.0)
                            {
                                if (_sessionFrameTimesMs.Count < MaxStoredSessionFrames)
                                    _sessionFrameTimesMs.Add((float)dtMs);

                                int elapsedSec = (int)Math.Max(0, (eventTs - _sessionStartEventTick) / Stopwatch.Frequency);
                                if (elapsedSec > _currentBucketSecond)
                                {
                                    FlushCurrentTimelineBucketLocked();
                                    _currentBucketSecond = elapsedSec;
                                }
                                _bucketFrameTimesMs.Add(dtMs);
                            }
                        }

                        if (eventTs > _sessionLastEventTick)
                            _sessionLastEventTick = eventTs;
                    }
                }
            }
        }

        private void FlushCurrentTimelineBucketLocked()
        {
            if (_bucketFrameTimesMs.Count < 3)
            {
                _bucketFrameTimesMs.Clear();
                return;
            }

            double sum = 0;
            for (int i = 0; i < _bucketFrameTimesMs.Count; i++)
                sum += _bucketFrameTimesMs[i];

            double avgMs = sum / _bucketFrameTimesMs.Count;
            _bucketFrameTimesMs.Sort();
            int worstCount = Math.Max(1, (int)Math.Ceiling(_bucketFrameTimesMs.Count * 0.01));
            double worstSum = 0;
            for (int i = _bucketFrameTimesMs.Count - worstCount; i < _bucketFrameTimesMs.Count; i++)
                worstSum += _bucketFrameTimesMs[i];

            double worstMs = worstSum / worstCount;

            _sessionTimeline.Add(new FpsTimelinePoint
            {
                ElapsedSeconds = _currentBucketSecond,
                AvgFps = avgMs > 0 ? 1000.0 / avgMs : 0,
                OnePercentLowFps = worstMs > 0 ? 1000.0 / worstMs : 0
            });

            _bucketFrameTimesMs.Clear();
        }

        private static double CalculateLowPercentileFps(List<float> sortedAscendingMs, double fraction)
        {
            if (sortedAscendingMs.Count == 0) return 0;
            int worstCount = Math.Max(1, (int)Math.Ceiling(sortedAscendingMs.Count * fraction));
            double sum = 0;
            for (int i = sortedAscendingMs.Count - worstCount; i < sortedAscendingMs.Count; i++)
                sum += sortedAscendingMs[i];

            double avgWorstMs = sum / worstCount;
            return avgWorstMs > 0 ? 1000.0 / avgWorstMs : 0;
        }

        private static List<FpsTimelinePoint> DownsampleTimeline(List<FpsTimelinePoint> source, int maxPoints)
        {
            if (source.Count <= maxPoints) return source;

            var result = new List<FpsTimelinePoint>(maxPoints);
            double step = (double)source.Count / maxPoints;

            for (int i = 0; i < maxPoints; i++)
            {
                int start = (int)(i * step);
                int end = Math.Min(source.Count, (int)((i + 1) * step));
                if (end <= start) end = Math.Min(source.Count, start + 1);

                double avgSum = 0;
                double minLow = double.MaxValue;
                int sec = source[start].ElapsedSeconds;

                for (int j = start; j < end; j++)
                {
                    avgSum += source[j].AvgFps;
                    if (source[j].OnePercentLowFps < minLow)
                        minLow = source[j].OnePercentLowFps;
                }

                result.Add(new FpsTimelinePoint
                {
                    ElapsedSeconds = sec,
                    AvgFps = avgSum / (end - start),
                    OnePercentLowFps = minLow == double.MaxValue ? 0 : minLow
                });
            }

            return result;
        }

        public void Stop()
        {
            _running = false;
            Volatile.Write(ref _sessionPid, 0);

            if (_traceHandle != InvalidTraceHandle)
            {
                try { CloseTrace(_traceHandle); } catch { }
                _traceHandle = InvalidTraceHandle;
            }

            if (_sessionHandle != 0)
            {
                StopExistingSession();
                _sessionHandle = 0;
            }

            if (_traceThread != null && _traceThread.IsAlive)
            {
                _traceThread.Join(500);
                _traceThread = null;
            }

            lock (_frameLock)
            {
                _frameEventTicks.Clear();
                _recentFrameTimes.Clear();
                _lastEventTick = 0;
                _lastWallTick = 0;
            }
        }

        public void Dispose() => Stop();

        private static void StopExistingSession()
        {
            IntPtr propsPtr = AllocateTraceProperties(out _);
            try
            {
                ControlTraceW(0, SessionName, propsPtr, EVENT_TRACE_CONTROL_STOP);
            }
            catch { }
            finally
            {
                Marshal.FreeHGlobal(propsPtr);
            }
        }

        private static IntPtr AllocateTraceProperties(out uint totalSize)
        {
            int headerSize = Marshal.SizeOf<EVENT_TRACE_PROPERTIES>();
            totalSize = (uint)(headerSize + 1024);
            IntPtr ptr = Marshal.AllocHGlobal((int)totalSize);
            Marshal.Copy(new byte[totalSize], 0, ptr, (int)totalSize);

            var props = new EVENT_TRACE_PROPERTIES
            {
                Wnode = new WNODE_HEADER
                {
                    BufferSize = totalSize,
                    Flags = WNODE_FLAG_TRACED_GUID,
                    ClientContext = 1
                },
                BufferSize = 64,
                MinimumBuffers = 2,
                MaximumBuffers = 16,
                LogFileMode = EVENT_TRACE_REAL_TIME_MODE,
                FlushTimer = 1,
                LoggerNameOffset = (uint)headerSize,
                LogFileNameOffset = 0
            };

            Marshal.StructureToPtr(props, ptr, false);
            return ptr;
        }

        private delegate void EventRecordCallbackDelegate(ref EVENT_RECORD eventRecord);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern uint StartTraceW(out ulong sessionHandle, string sessionName, IntPtr properties);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern uint ControlTraceW(ulong sessionHandle, string sessionName, IntPtr properties, uint controlCode);

        [DllImport("advapi32.dll")]
        private static extern uint EnableTraceEx2(ulong traceHandle, ref Guid providerId, uint controlCode, byte level, ulong matchAnyKeyword, ulong matchAllKeyword, uint timeout, IntPtr enableParameters);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern ulong OpenTraceW(ref EVENT_TRACE_LOGFILEW logFile);

        [DllImport("advapi32.dll")]
        private static extern uint ProcessTrace(ulong[] handleArray, uint handleCount, IntPtr startTime, IntPtr endTime);

        [DllImport("advapi32.dll")]
        private static extern uint CloseTrace(ulong traceHandle);

        [StructLayout(LayoutKind.Sequential)]
        private struct WNODE_HEADER
        {
            public uint BufferSize;
            public uint ProviderId;
            public ulong HistoricalContext;
            public long TimeStamp;
            public Guid Guid;
            public uint ClientContext;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_TRACE_PROPERTIES
        {
            public WNODE_HEADER Wnode;
            public uint BufferSize;
            public uint MinimumBuffers;
            public uint MaximumBuffers;
            public uint MaximumFileSize;
            public uint LogFileMode;
            public uint FlushTimer;
            public uint EnableFlags;
            public int AgeLimit;
            public uint NumberOfBuffers;
            public uint FreeBuffers;
            public uint EventsLost;
            public uint BuffersWritten;
            public uint LogBuffersLost;
            public uint RealTimeBuffersLost;
            public IntPtr LoggerThreadId;
            public uint LogFileNameOffset;
            public uint LoggerNameOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_DESCRIPTOR
        {
            public ushort Id;
            public byte Version;
            public byte Channel;
            public byte Level;
            public byte Opcode;
            public ushort Task;
            public ulong Keyword;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_HEADER
        {
            public ushort Size;
            public ushort HeaderType;
            public ushort Flags;
            public ushort EventProperty;
            public uint ThreadId;
            public uint ProcessId;
            public long TimeStamp;
            public Guid ProviderId;
            public EVENT_DESCRIPTOR EventDescriptor;
            public ulong ProcessorTime;
            public Guid ActivityId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ETW_BUFFER_CONTEXT
        {
            public ushort ProcessorNumber;
            public byte Alignment;
            public byte LoggerId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_RECORD
        {
            public EVENT_HEADER EventHeader;
            public ETW_BUFFER_CONTEXT BufferContext;
            public ushort ExtendedDataCount;
            public ushort UserDataLength;
            public IntPtr ExtendedData;
            public IntPtr UserData;
            public IntPtr UserContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_TRACE_HEADER
        {
            public ushort Size;
            public ushort FieldTypeFlags;
            public uint Version;
            public uint ThreadId;
            public uint ProcessId;
            public long TimeStamp;
            public Guid Guid;
            public uint KernelTime;
            public uint UserTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_TRACE
        {
            public EVENT_TRACE_HEADER Header;
            public uint InstanceId;
            public uint ParentInstanceId;
            public Guid ParentGuid;
            public IntPtr MofData;
            public uint MofLength;
            public uint ClientContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort Year, Month, DayOfWeek, Day, Hour, Minute, Second, Milliseconds;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct TIME_ZONE_INFORMATION
        {
            public int Bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string StandardName;
            public SYSTEMTIME StandardDate;
            public int StandardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DaylightName;
            public SYSTEMTIME DaylightDate;
            public int DaylightBias;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct TRACE_LOGFILE_HEADER
        {
            public uint BufferSize;
            public uint Version;
            public uint ProviderVersion;
            public uint NumberOfProcessors;
            public long EndTime;
            public uint TimerResolution;
            public uint MaximumFileSize;
            public uint LogFileMode;
            public uint BuffersWritten;
            public Guid LogInstanceGuid;
            public IntPtr LoggerName;
            public IntPtr LogFileName;
            public TIME_ZONE_INFORMATION TimeZone;
            public long BootTime;
            public long PerfFreq;
            public long StartTime;
            public uint ReservedFlags;
            public uint BuffersLost;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct EVENT_TRACE_LOGFILEW
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string? LogFileName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string? LoggerName;
            public long CurrentTime;
            public uint BuffersRead;
            public uint ProcessTraceMode;
            public EVENT_TRACE CurrentEvent;
            public TRACE_LOGFILE_HEADER LogfileHeader;
            public IntPtr BufferCallback;
            public uint BufferSize;
            public uint Filled;
            public uint EventsLost;
            public EventRecordCallbackDelegate? EventRecordCallback;
            public uint IsKernelTrace;
            public IntPtr Context;
        }
    }
}