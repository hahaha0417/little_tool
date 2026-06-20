using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace hardware_info;

public sealed class HardwareInfoService : IDisposable
{
    private const uint PdhFmtDouble = 0x00000200;
    private const uint PerfDetailWizard = 400;
    private const uint PdhMoreData = 0x800007D2;
    private const uint ErrorSuccess = 0;

    private readonly Queue<float> _totalCpuSamples = [];
    private readonly Dictionary<string, Queue<float>> _coreCpuSamples = [];
    private readonly CpuBackend _backend;

    private readonly nint _queryHandle;
    private readonly nint _pdhTotalCpuCounterHandle;
    private readonly List<PdhCoreCounter> _pdhCoreCounters = [];

    private readonly PerformanceCounter? _perfTotalCpuCounter;
    private readonly List<PerfCoreCounter> _perfCoreCounters = [];

    private int _cpuSmoothingWindowSize = 5;
    private bool _disposed;

    public HardwareInfoService()
    {
        if (TryInitializePdhCounters(out _queryHandle, out _pdhTotalCpuCounterHandle, out _pdhCoreCounters))
        {
            _backend = CpuBackend.Pdh;
            return;
        }

        if (TryInitializePerformanceCounters(out _perfTotalCpuCounter, out _perfCoreCounters))
        {
            _backend = CpuBackend.PerformanceCounter;
            return;
        }

        throw new InvalidOperationException("Unable to initialize CPU counters.");
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        switch (_backend)
        {
            case CpuBackend.Pdh:
                CollectPdhQueryData();
                await Task.Delay(1000, cancellationToken);
                CollectPdhQueryData();
                break;
            case CpuBackend.PerformanceCounter:
                _perfTotalCpuCounter!.NextValue();

                foreach (var coreCounter in _perfCoreCounters)
                {
                    coreCounter.Counter.NextValue();
                }

                await Task.Delay(1000, cancellationToken);
                break;
        }
    }

    public HardwareSnapshot GetSnapshot()
    {
        ThrowIfDisposed();

        float totalCpuUsage;
        IReadOnlyList<CpuCoreUsage> coreUsages;

        switch (_backend)
        {
            case CpuBackend.Pdh:
                CollectPdhQueryData();
                totalCpuUsage = GetSmoothedValue(_totalCpuSamples, ClampCpuUsage(ReadPdhCounterValue(_pdhTotalCpuCounterHandle)));
                coreUsages = _pdhCoreCounters
                    .Select(counter => new CpuCoreUsage(
                        counter.Name,
                        GetSmoothedCoreValue(counter.Name, ClampCpuUsage(ReadPdhCounterValue(counter.Handle)))))
                    .ToList();
                break;
            case CpuBackend.PerformanceCounter:
                totalCpuUsage = GetSmoothedValue(_totalCpuSamples, ClampCpuUsage(_perfTotalCpuCounter!.NextValue()));
                coreUsages = _perfCoreCounters
                    .Select(counter => new CpuCoreUsage(
                        counter.Name,
                        GetSmoothedCoreValue(counter.Name, ClampCpuUsage(counter.Counter.NextValue()))))
                    .ToList();
                break;
            default:
                throw new InvalidOperationException("CPU backend is not initialized.");
        }

        var memoryStatus = MemoryStatus.GetCurrent();
        var totalMemoryBytes = (long)memoryStatus.TotalPhysical;
        var availableMemoryBytes = (long)memoryStatus.AvailablePhysical;
        var usedMemoryBytes = totalMemoryBytes - availableMemoryBytes;

        var fixedDrives = DriveInfo.GetDrives()
            .Where(drive => drive.IsReady && drive.DriveType == DriveType.Fixed)
            .ToList();

        long totalDiskBytes = fixedDrives.Sum(drive => drive.TotalSize);
        long freeDiskBytes = fixedDrives.Sum(drive => drive.AvailableFreeSpace);
        long usedDiskBytes = totalDiskBytes - freeDiskBytes;

        return new HardwareSnapshot(
            totalCpuUsage,
            coreUsages,
            usedMemoryBytes,
            totalMemoryBytes,
            usedDiskBytes,
            totalDiskBytes);
    }

    public void SetCpuSmoothingWindowSize(int windowSize)
    {
        _cpuSmoothingWindowSize = Math.Max(1, windowSize);
        TrimSamples(_totalCpuSamples);

        foreach (var samples in _coreCpuSamples.Values)
        {
            TrimSamples(samples);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_backend == CpuBackend.Pdh)
        {
            foreach (var coreCounter in _pdhCoreCounters)
            {
                PdhRemoveCounter(coreCounter.Handle);
            }

            if (_pdhTotalCpuCounterHandle != nint.Zero)
            {
                PdhRemoveCounter(_pdhTotalCpuCounterHandle);
            }

            if (_queryHandle != nint.Zero)
            {
                PdhCloseQuery(_queryHandle);
            }
        }

        if (_backend == CpuBackend.PerformanceCounter)
        {
            _perfTotalCpuCounter?.Dispose();

            foreach (var coreCounter in _perfCoreCounters)
            {
                coreCounter.Counter.Dispose();
            }
        }

        _disposed = true;
    }

    private float GetSmoothedCoreValue(string coreName, float rawValue)
    {
        if (!_coreCpuSamples.TryGetValue(coreName, out var samples))
        {
            samples = [];
            _coreCpuSamples[coreName] = samples;
        }

        return GetSmoothedValue(samples, rawValue);
    }

    private float GetSmoothedValue(Queue<float> samples, float rawValue)
    {
        samples.Enqueue(rawValue);

        while (samples.Count > _cpuSmoothingWindowSize)
        {
            samples.Dequeue();
        }

        return samples.Average();
    }

    private void TrimSamples(Queue<float> samples)
    {
        while (samples.Count > _cpuSmoothingWindowSize)
        {
            samples.Dequeue();
        }
    }

    private void CollectPdhQueryData()
    {
        var status = PdhCollectQueryData(_queryHandle);

        if (status != ErrorSuccess)
        {
            throw new InvalidOperationException($"PDH collect query data failed: 0x{status:X8}");
        }
    }

    private static float ReadPdhCounterValue(nint counterHandle)
    {
        var status = PdhGetFormattedCounterValue(counterHandle, PdhFmtDouble, out _, out var value);

        if (status != ErrorSuccess)
        {
            throw new InvalidOperationException($"PDH get formatted counter value failed: 0x{status:X8}");
        }

        return (float)value.DoubleValue;
    }

    private static bool TryInitializePdhCounters(
        out nint queryHandle,
        out nint totalCpuCounterHandle,
        out List<PdhCoreCounter> coreCounters)
    {
        if (TryInitializePdhCounters("Processor Information", "% Processor Utility", out queryHandle, out totalCpuCounterHandle, out coreCounters))
        {
            return true;
        }

        return TryInitializePdhCounters("Processor", "% Processor Time", out queryHandle, out totalCpuCounterHandle, out coreCounters);
    }

    private static bool TryInitializePdhCounters(
        string categoryName,
        string counterName,
        out nint queryHandle,
        out nint totalCpuCounterHandle,
        out List<PdhCoreCounter> coreCounters)
    {
        queryHandle = nint.Zero;
        totalCpuCounterHandle = nint.Zero;
        coreCounters = [];

        try
        {
            var openStatus = PdhOpenQuery(null, nint.Zero, out queryHandle);

            if (openStatus != ErrorSuccess)
            {
                return false;
            }

            var allInstances = EnumerateObjectInstances(categoryName).ToList();
            var totalInstanceName = allInstances.FirstOrDefault(IsPreferredTotalInstanceName) ??
                allInstances.FirstOrDefault(IsTotalInstanceName);

            if (string.IsNullOrWhiteSpace(totalInstanceName))
            {
                CleanupFailedPdhInitialization(queryHandle, totalCpuCounterHandle, coreCounters);
                return false;
            }

            var instances = allInstances
                .Where(name => !IsTotalInstanceName(name))
                .Select(name => new { RawName = name, SortKey = GetCpuSortKey(name) })
                .Where(item => item.SortKey is not null)
                .OrderBy(item => item.SortKey)
                .ToList();

            if (instances.Count == 0)
            {
                CleanupFailedPdhInitialization(queryHandle, totalCpuCounterHandle, coreCounters);
                return false;
            }

            var totalCounterPath = $@"\\{categoryName}({totalInstanceName})\\{counterName}";
            var addTotalStatus = PdhAddEnglishCounter(queryHandle, totalCounterPath, nint.Zero, out totalCpuCounterHandle);

            if (addTotalStatus != ErrorSuccess)
            {
                CleanupFailedPdhInitialization(queryHandle, totalCpuCounterHandle, coreCounters);
                return false;
            }

            foreach (var instance in instances)
            {
                var counterPath = $@"\\{categoryName}({instance.RawName})\\{counterName}";
                var addCounterStatus = PdhAddEnglishCounter(queryHandle, counterPath, nint.Zero, out var counterHandle);

                if (addCounterStatus != ErrorSuccess)
                {
                    CleanupFailedPdhInitialization(queryHandle, totalCpuCounterHandle, coreCounters);
                    return false;
                }

                coreCounters.Add(new PdhCoreCounter(FormatCpuCoreName(instance.RawName), counterHandle));
            }

            return true;
        }
        catch
        {
            CleanupFailedPdhInitialization(queryHandle, totalCpuCounterHandle, coreCounters);
            return false;
        }
    }

    private static bool TryInitializePerformanceCounters(
        out PerformanceCounter? totalCpuCounter,
        out List<PerfCoreCounter> coreCounters)
    {
        totalCpuCounter = null;
        coreCounters = [];

        try
        {
            if (TryInitializePerformanceCounters("Processor Information", "% Processor Utility", out totalCpuCounter, out coreCounters))
            {
                return true;
            }

            return TryInitializePerformanceCounters("Processor", "% Processor Time", out totalCpuCounter, out coreCounters);
        }
        catch
        {
            totalCpuCounter?.Dispose();

            foreach (var counter in coreCounters)
            {
                counter.Counter.Dispose();
            }

            totalCpuCounter = null;
            coreCounters = [];
            return false;
        }
    }

    private static bool TryInitializePerformanceCounters(
        string categoryName,
        string counterName,
        out PerformanceCounter? totalCpuCounter,
        out List<PerfCoreCounter> coreCounters)
    {
        totalCpuCounter = null;
        coreCounters = [];

        try
        {
            var category = new PerformanceCounterCategory(categoryName);
            var instanceNames = category.GetInstanceNames();
            var totalInstanceName = instanceNames.FirstOrDefault(IsPreferredTotalInstanceName) ??
                instanceNames.FirstOrDefault(IsTotalInstanceName);

            if (string.IsNullOrWhiteSpace(totalInstanceName))
            {
                return false;
            }

            totalCpuCounter = new PerformanceCounter(categoryName, counterName, totalInstanceName);
            coreCounters = instanceNames
                .Where(name => !IsTotalInstanceName(name))
                .Select(name => new { RawName = name, SortKey = GetCpuSortKey(name) })
                .Where(item => item.SortKey is not null)
                .OrderBy(item => item.SortKey)
                .Select(item => new PerfCoreCounter(
                    FormatCpuCoreName(item.RawName),
                    new PerformanceCounter(categoryName, counterName, item.RawName)))
                .ToList();

            if (coreCounters.Count == 0)
            {
                totalCpuCounter.Dispose();
                totalCpuCounter = null;
                return false;
            }

            return true;
        }
        catch
        {
            totalCpuCounter?.Dispose();

            foreach (var counter in coreCounters)
            {
                counter.Counter.Dispose();
            }

            totalCpuCounter = null;
            coreCounters = [];
            return false;
        }
    }

    private static IEnumerable<string> EnumerateObjectInstances(string categoryName)
    {
        uint counterListLength = 0;
        uint instanceListLength = 0;

        var status = PdhEnumObjectItems(
            null,
            null,
            categoryName,
            null,
            ref counterListLength,
            null,
            ref instanceListLength,
            PerfDetailWizard,
            0);

        if (status != PdhMoreData || instanceListLength == 0)
        {
            return [];
        }

        var instanceBuffer = new char[instanceListLength];
        status = PdhEnumObjectItems(
            null,
            null,
            categoryName,
            null,
            ref counterListLength,
            instanceBuffer,
            ref instanceListLength,
            PerfDetailWizard,
            0);

        if (status != ErrorSuccess)
        {
            return [];
        }

        return ParseMultiString(instanceBuffer);
    }

    private static IEnumerable<string> ParseMultiString(char[] buffer)
    {
        var items = new List<string>();
        var current = new StringBuilder();

        foreach (var ch in buffer)
        {
            if (ch == '\0')
            {
                if (current.Length == 0)
                {
                    break;
                }

                items.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        return items;
    }

    private static bool IsPreferredTotalInstanceName(string instanceName)
    {
        return string.Equals(instanceName, "_Total", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTotalInstanceName(string instanceName)
    {
        return string.Equals(instanceName, "_Total", StringComparison.OrdinalIgnoreCase) ||
            instanceName.EndsWith("_Total", true, null);
    }

    private static int? GetCpuSortKey(string instanceName)
    {
        foreach (var part in instanceName.Split(','))
        {
            if (int.TryParse(part, out var value))
            {
                return value;
            }
        }

        return int.TryParse(instanceName, out var directValue) ? directValue : null;
    }

    private static string FormatCpuCoreName(string instanceName)
    {
        var parts = instanceName.Split(',');
        return parts.Length > 1 ? parts[^1] : instanceName;
    }

    private static float ClampCpuUsage(float value)
    {
        return Math.Clamp(value, 0f, 100f);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private static void CleanupFailedPdhInitialization(nint queryHandle, nint totalCpuCounterHandle, List<PdhCoreCounter> coreCounters)
    {
        foreach (var coreCounter in coreCounters)
        {
            PdhRemoveCounter(coreCounter.Handle);
        }

        if (totalCpuCounterHandle != nint.Zero)
        {
            PdhRemoveCounter(totalCpuCounterHandle);
        }

        if (queryHandle != nint.Zero)
        {
            PdhCloseQuery(queryHandle);
        }
    }

    private sealed record PdhCoreCounter(string Name, nint Handle);

    private sealed record PerfCoreCounter(string Name, PerformanceCounter Counter);

    private enum CpuBackend
    {
        Pdh,
        PerformanceCounter
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MemoryStatus
    {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhysical;
        public ulong AvailablePhysical;
        public ulong TotalPageFile;
        public ulong AvailablePageFile;
        public ulong TotalVirtual;
        public ulong AvailableVirtual;
        public ulong AvailableExtendedVirtual;

        public static MemoryStatus GetCurrent()
        {
            var status = new MemoryStatus
            {
                Length = (uint)Marshal.SizeOf<MemoryStatus>()
            };

            if (!GlobalMemoryStatusEx(ref status))
            {
                throw new InvalidOperationException("Unable to read memory status.");
            }

            return status;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PdhFmtCounterValueDouble
    {
        public uint CStatus;
        public double DoubleValue;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatus lpBuffer);

    [DllImport("pdh.dll", CharSet = CharSet.Unicode)]
    private static extern uint PdhOpenQuery(string? dataSource, nint userData, out nint query);

    [DllImport("pdh.dll", CharSet = CharSet.Unicode)]
    private static extern uint PdhAddEnglishCounter(nint query, string fullCounterPath, nint userData, out nint counter);

    [DllImport("pdh.dll")]
    private static extern uint PdhCollectQueryData(nint query);

    [DllImport("pdh.dll")]
    private static extern uint PdhGetFormattedCounterValue(
        nint counter,
        uint format,
        out uint type,
        out PdhFmtCounterValueDouble value);

    [DllImport("pdh.dll", CharSet = CharSet.Unicode)]
    private static extern uint PdhEnumObjectItems(
        string? dataSource,
        string? machineName,
        string objectName,
        char[]? counterList,
        ref uint counterListLength,
        char[]? instanceList,
        ref uint instanceListLength,
        uint detailLevel,
        uint flags);

    [DllImport("pdh.dll")]
    private static extern uint PdhRemoveCounter(nint counter);

    [DllImport("pdh.dll")]
    private static extern uint PdhCloseQuery(nint query);
}

public sealed record CpuCoreUsage(string CoreName, float UsagePercent);

public sealed record HardwareSnapshot(
    float TotalCpuUsagePercent,
    IReadOnlyList<CpuCoreUsage> CoreUsages,
    long UsedMemoryBytes,
    long TotalMemoryBytes,
    long UsedDiskBytes,
    long TotalDiskBytes);
