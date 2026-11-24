using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Overcapped_Optimizer.Helpers
{
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

        private ulong _sessionHandle;
        private ulong _traceHandle = InvalidTraceHandle;
        private Thread? _traceThread;
        private EventRecordCallbackDelegate? _callbackPin;
        private volatile bool _running;
        private int _targetPid;

        private readonly object _frameLock = new();
        private readonly Queue<long> _frameTicks = new();
        private long _lastFrameTick;

        public bool IsRunning => _running;

        public void SetTargetProcessId(int pid)
        {
            int previous = Interlocked.Exchange(ref _targetPid, pid);
            if (previous != pid)
            {
                lock (_frameLock)
                {
                    _frameTicks.Clear();
                    _lastFrameTick = 0;
                }
            }
        }

        public (int Fps, double FrameTimeMs) GetCurrentStats()
        {
            long now = Stopwatch.GetTimestamp();
            long oneSecond = Stopwatch.Frequency;

            lock (_frameLock)
            {
                while (_frameTicks.Count > 0 && (now - _frameTicks.Peek()) > oneSecond)
                {
                    _frameTicks.Dequeue();
                }

                int count = _frameTicks.Count;
                if (count == 0 || (now - _lastFrameTick) > oneSecond)
                {
                    return (0, 0.0);
                }

                double frameTimeMs = 1000.0 / count;
                return (count, frameTimeMs);
            }
        }

        public bool Start()
        {
            if (_running) return true;

            try
            {
                StopExistingSession();

                IntPtr propsPtr = AllocateTraceProperties(out uint propsSize);
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

            int wantedPid = Volatile.Read(ref _targetPid);
            if (wantedPid <= 0 || record.EventHeader.ProcessId != (uint)wantedPid)
                return;

            ushort eventId = record.EventHeader.EventDescriptor.Id;
            Guid provider = record.EventHeader.ProviderId;

            bool isPresent =
                (provider == DxgiProviderGuid && (eventId == 42 || eventId == 55)) ||
                (provider == D3D9ProviderGuid && eventId == 1);

            if (!isPresent) return;

            long now = Stopwatch.GetTimestamp();
            lock (_frameLock)
            {
                _frameTicks.Enqueue(now);
                _lastFrameTick = now;
            }
        }

        public void Stop()
        {
            _running = false;

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
                _frameTicks.Clear();
                _lastFrameTick = 0;
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