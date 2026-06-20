namespace hardware_info;

public partial class Form1 : Form
{
    private const int DefaultWriteIntervalMs = 1000;
    private const int DefaultUiUpdateIntervalMs = 1000;
    private const int DefaultInternalSamplingIntervalMs = 250;
    private const int DefaultCpuSmoothingWindowSize = 5;
    private const int MinimumWriteIntervalMs = 100;
    private const int MinimumUiUpdateIntervalMs = 100;
    private const int MinimumInternalSamplingIntervalMs = 250;
    private const int MinimumCpuSmoothingWindowSize = 3;

    private readonly HardwareInfoService _hardwareInfoService = new();
    private readonly HardwareJsonWriter _hardwareJsonWriter = new();
    private readonly AppSettingsStore _appSettingsStore = new();
    private readonly ManualResetEventSlim _stopSignal = new(false);
    private readonly Lock _jsonPathLock = new();
    private readonly Lock _writeIntervalLock = new();
    private readonly Lock _uiUpdateIntervalLock = new();
    private readonly Lock _internalSamplingIntervalLock = new();
    private readonly Lock _cpuSmoothingWindowLock = new();
    private Thread? _workerThread;
    private volatile bool _isRunning;
    private string _jsonOutputPath = Path.Combine(AppContext.BaseDirectory, "hardware-info.json");
    private int _writeIntervalMs = DefaultWriteIntervalMs;
    private int _uiUpdateIntervalMs = DefaultUiUpdateIntervalMs;
    private int _internalSamplingIntervalMs = DefaultInternalSamplingIntervalMs;
    private int _cpuSmoothingWindowSize = DefaultCpuSmoothingWindowSize;

    public Form1()
    {
        InitializeComponent();

        Load += Form1_Load;
        FormClosed += Form1_FormClosed;
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        try
        {
            LoadSettings();
            txtJsonPath.Text = _jsonOutputPath;
            numWriteIntervalMs.Value = _writeIntervalMs;
            numUiUpdateIntervalMs.Value = _uiUpdateIntervalMs;
            numInternalSamplingIntervalMs.Value = _internalSamplingIntervalMs;
            numCpuSmoothingWindowSize.Value = _cpuSmoothingWindowSize;
            StartWorkerThread();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"初始化硬體資訊失敗:{Environment.NewLine}{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void btnBrowseJsonPath_Click(object? sender, EventArgs e)
    {
        using var saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = Path.GetFileName(GetJsonOutputPath()),
            InitialDirectory = GetInitialDirectory(),
            Title = "選擇硬體資訊 JSON 路徑"
        };

        if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
        {
            txtJsonPath.Text = saveFileDialog.FileName;
        }
    }

    private void txtJsonPath_TextChanged(object? sender, EventArgs e)
    {
        UpdateJsonPath(txtJsonPath.Text);
    }

    private void numWriteIntervalMs_ValueChanged(object? sender, EventArgs e)
    {
        UpdateWriteInterval((int)numWriteIntervalMs.Value);
    }

    private void numUiUpdateIntervalMs_ValueChanged(object? sender, EventArgs e)
    {
        UpdateUiUpdateInterval((int)numUiUpdateIntervalMs.Value);
    }

    private void numInternalSamplingIntervalMs_ValueChanged(object? sender, EventArgs e)
    {
        UpdateInternalSamplingInterval((int)numInternalSamplingIntervalMs.Value);
    }

    private void numCpuSmoothingWindowSize_ValueChanged(object? sender, EventArgs e)
    {
        UpdateCpuSmoothingWindowSize((int)numCpuSmoothingWindowSize.Value);
    }

    private void StartWorkerThread()
    {
        if (_workerThread is not null)
        {
            return;
        }

        _isRunning = true;
        _stopSignal.Reset();
        _workerThread = new Thread(WorkerLoop)
        {
            IsBackground = true,
            Name = "HardwareInfoWorker"
        };
        _workerThread.Start();
    }

    private void WorkerLoop()
    {
        try
        {
            _hardwareInfoService.InitializeAsync().GetAwaiter().GetResult();
            HardwareSnapshot? latestSnapshot = null;
            DateTime lastSampleAt = DateTime.MinValue;
            DateTime lastWriteAt = DateTime.MinValue;
            DateTime lastUiUpdateAt = DateTime.MinValue;

            while (_isRunning)
            {
                var now = DateTime.UtcNow;
                var sampleDue = lastSampleAt == DateTime.MinValue ||
                    (now - lastSampleAt).TotalMilliseconds >= GetInternalSamplingIntervalMs();
                var writeDue = lastWriteAt == DateTime.MinValue ||
                    (now - lastWriteAt).TotalMilliseconds >= GetWriteIntervalMs();
                var uiDue = lastUiUpdateAt == DateTime.MinValue ||
                    (now - lastUiUpdateAt).TotalMilliseconds >= GetUiUpdateIntervalMs();

                if (sampleDue)
                {
                    latestSnapshot = _hardwareInfoService.GetSnapshot();
                    lastSampleAt = now;
                }

                if (latestSnapshot is not null)
                {
                    if (writeDue)
                    {
                        _hardwareJsonWriter.WriteSnapshot(GetJsonOutputPath(), latestSnapshot);
                        lastWriteAt = now;
                    }

                    if (uiDue && !IsDisposed && IsHandleCreated)
                    {
                        var snapshotForUi = latestSnapshot;
                        BeginInvoke(() => UpdateHardwareInfo(snapshotForUi));
                        lastUiUpdateAt = now;
                    }
                }

                var waitMilliseconds = GetNextWaitMilliseconds();

                if (_stopSignal.Wait(waitMilliseconds))
                {
                    break;
                }
            }
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (Exception ex)
        {
            if (!IsDisposed && IsHandleCreated)
            {
                BeginInvoke(() =>
                    MessageBox.Show(
                        $"讀取硬體資訊失敗:{Environment.NewLine}{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error));
            }
        }
    }

    private void UpdateHardwareInfo(HardwareSnapshot snapshot)
    {
        lblCpuTotalValue.Text = $"{snapshot.TotalCpuUsagePercent:F1}%";
        txtCpuCores.Text = string.Join(
            Environment.NewLine,
            snapshot.CoreUsages.Select(core => $"CPU {core.CoreName}: {core.UsagePercent:F1}%"));

        lblMemoryValue.Text = $"{FormatBytes(snapshot.UsedMemoryBytes)} / {FormatBytes(snapshot.TotalMemoryBytes)}";
        lblDiskValue.Text = $"{FormatBytes(snapshot.UsedDiskBytes)} / {FormatBytes(snapshot.TotalDiskBytes)}";
    }

    private void UpdateJsonPath(string path)
    {
        var trimmedPath = path.Trim();

        if (string.IsNullOrWhiteSpace(trimmedPath))
        {
            return;
        }

        lock (_jsonPathLock)
        {
            _jsonOutputPath = trimmedPath;
        }
    }

    private string GetJsonOutputPath()
    {
        lock (_jsonPathLock)
        {
            return _jsonOutputPath;
        }
    }

    private void UpdateWriteInterval(int writeIntervalMs)
    {
        lock (_writeIntervalLock)
        {
            _writeIntervalMs = Math.Max(MinimumWriteIntervalMs, writeIntervalMs);
        }
    }

    private int GetWriteIntervalMs()
    {
        lock (_writeIntervalLock)
        {
            return _writeIntervalMs;
        }
    }

    private void UpdateUiUpdateInterval(int uiUpdateIntervalMs)
    {
        lock (_uiUpdateIntervalLock)
        {
            _uiUpdateIntervalMs = Math.Max(MinimumUiUpdateIntervalMs, uiUpdateIntervalMs);
        }
    }

    private int GetUiUpdateIntervalMs()
    {
        lock (_uiUpdateIntervalLock)
        {
            return _uiUpdateIntervalMs;
        }
    }

    private void UpdateInternalSamplingInterval(int internalSamplingIntervalMs)
    {
        lock (_internalSamplingIntervalLock)
        {
            _internalSamplingIntervalMs = Math.Max(MinimumInternalSamplingIntervalMs, internalSamplingIntervalMs);
        }
    }

    private int GetInternalSamplingIntervalMs()
    {
        lock (_internalSamplingIntervalLock)
        {
            return _internalSamplingIntervalMs;
        }
    }

    private void UpdateCpuSmoothingWindowSize(int cpuSmoothingWindowSize)
    {
        int sanitizedValue;

        lock (_cpuSmoothingWindowLock)
        {
            _cpuSmoothingWindowSize = Math.Max(MinimumCpuSmoothingWindowSize, cpuSmoothingWindowSize);
            sanitizedValue = _cpuSmoothingWindowSize;
        }

        _hardwareInfoService.SetCpuSmoothingWindowSize(sanitizedValue);
    }

    private int GetCpuSmoothingWindowSize()
    {
        lock (_cpuSmoothingWindowLock)
        {
            return _cpuSmoothingWindowSize;
        }
    }

    private int GetNextWaitMilliseconds()
    {
        var sampleWait = GetInternalSamplingIntervalMs();
        var writeWait = GetWriteIntervalMs();
        var uiWait = GetUiUpdateIntervalMs();
        return Math.Max(1, Math.Min(sampleWait, Math.Min(writeWait, uiWait)));
    }

    private string GetInitialDirectory()
    {
        var jsonPath = GetJsonOutputPath();
        var directory = Path.GetDirectoryName(Path.GetFullPath(jsonPath));
        return string.IsNullOrWhiteSpace(directory) ? AppContext.BaseDirectory : directory;
    }

    private void LoadSettings()
    {
        var settings = _appSettingsStore.Load();

        if (!string.IsNullOrWhiteSpace(settings.JsonOutputPath))
        {
            _jsonOutputPath = settings.JsonOutputPath;
        }

        UpdateWriteInterval(settings.WriteIntervalMs);
        UpdateUiUpdateInterval(settings.UiUpdateIntervalMs);
        UpdateInternalSamplingInterval(settings.InternalSamplingIntervalMs);
        UpdateCpuSmoothingWindowSize(settings.CpuSmoothingWindowSize);
    }

    private void SaveSettings()
    {
        _appSettingsStore.Save(new AppSettings
        {
            JsonOutputPath = GetJsonOutputPath(),
            WriteIntervalMs = GetWriteIntervalMs(),
            UiUpdateIntervalMs = GetUiUpdateIntervalMs(),
            InternalSamplingIntervalMs = GetInternalSamplingIntervalMs(),
            CpuSmoothingWindowSize = GetCpuSmoothingWindowSize()
        });
    }

    private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _isRunning = false;
        _stopSignal.Set();
        _workerThread?.Join(1500);
        _workerThread = null;
        SaveSettings();
        _stopSignal.Dispose();
        _hardwareInfoService.Dispose();
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double value = bytes;
        var unitIndex = 0;

        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        return $"{value:F2} {units[unitIndex]}";
    }
}
