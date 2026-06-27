namespace ftp_sync;

public partial class Form1 : Form
{
    private readonly string _profilesRootPath = Path.Combine(AppContext.BaseDirectory, "profiles");
    private readonly string _profileStatePath = Path.Combine(AppContext.BaseDirectory, "profiles", "profiles.json");
    private CancellationTokenSource? _syncCancellationTokenSource;
    private string _currentProfileName = "default";

    public Form1()
    {
        InitializeComponent();
        InitializeProfiles();
        UpdateUiState(isRunning: false);
    }

    private string CurrentProfileDirectoryPath => Path.Combine(_profilesRootPath, _currentProfileName);

    private string CurrentSettingsPath => Path.Combine(CurrentProfileDirectoryPath, "syncsettings.json");

    private string CurrentSyncHistoryPath => Path.Combine(CurrentProfileDirectoryPath, "sync-history.json");

    private void InitializeProfiles()
    {
        Directory.CreateDirectory(_profilesRootPath);
        var activeProfile = LoadLastProfileName();

        EnsureProfileDirectory(activeProfile);
        LoadProfileList(activeProfile);
        SwitchProfile(activeProfile, saveCurrent: false);
    }

    private string LoadLastProfileName()
    {
        var state = ProfileState.Load(_profileStatePath);
        return NormalizeProfileName(state.LastProfileName);
    }

    private void SaveLastProfileName(string profileName)
    {
        Directory.CreateDirectory(_profilesRootPath);
        var state = new ProfileState
        {
            LastProfileName = NormalizeProfileName(profileName)
        };
        state.Save(_profileStatePath);
    }

    private void LoadProfileList(string? selectedProfileName = null)
    {
        Directory.CreateDirectory(_profilesRootPath);
        var profileNames = Directory.EnumerateDirectories(_profilesRootPath)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (profileNames.Count == 0)
        {
            profileNames.Add("default");
            EnsureProfileDirectory("default");
        }

        cboProfiles.BeginUpdate();
        cboProfiles.Items.Clear();
        cboProfiles.Items.AddRange(profileNames.Cast<object>().ToArray());
        cboProfiles.EndUpdate();
        cboProfiles.Text = selectedProfileName ?? _currentProfileName;
    }

    private static string NormalizeProfileName(string? profileName)
    {
        var trimmed = string.IsNullOrWhiteSpace(profileName) ? "default" : profileName.Trim();
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(trimmed.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(sanitized) ? "default" : sanitized;
    }

    private void EnsureProfileDirectory(string profileName)
    {
        Directory.CreateDirectory(Path.Combine(_profilesRootPath, profileName));
    }

    private void SwitchProfile(string requestedProfileName, bool saveCurrent)
    {
        var nextProfileName = NormalizeProfileName(requestedProfileName);

        if (saveCurrent)
        {
            SaveSettingsFromForm();
        }

        EnsureProfileDirectory(nextProfileName);
        _currentProfileName = nextProfileName;
        SaveLastProfileName(_currentProfileName);
        LoadProfileList(_currentProfileName);
        LoadSettingsIntoForm();
        SetLogLines(new[] { $"已切換到設定檔: {_currentProfileName}" });
    }

    private void SaveProfile(string requestedProfileName)
    {
        var nextProfileName = NormalizeProfileName(requestedProfileName);
        EnsureProfileDirectory(nextProfileName);

        var previousProfileName = _currentProfileName;
        _currentProfileName = nextProfileName;
        SaveSettingsFromForm();
        SaveLastProfileName(_currentProfileName);
        LoadProfileList(_currentProfileName);

        if (!string.Equals(previousProfileName, _currentProfileName, StringComparison.OrdinalIgnoreCase))
        {
            SetLogLines(new[] { $"設定已儲存並切換到設定檔: {_currentProfileName}" });
        }
        else
        {
            SetLogLines(new[] { $"設定檔已儲存: {_currentProfileName}" });
        }
    }

    private void LoadSettingsIntoForm()
    {
        try
        {
            var settings = SyncSettings.Load(CurrentSettingsPath);

            txtHost.Text = settings.Host;
            numPort.Value = Math.Clamp(settings.Port, (int)numPort.Minimum, (int)numPort.Maximum);
            txtUsername.Text = settings.Username;
            txtPassword.Text = settings.Password;
            txtLocalPath.Text = settings.LocalPath;
            txtRemotePath.Text = settings.RemotePath;
            chkPassiveMode.Checked = settings.UsePassiveMode;
            chkUseSsl.Checked = settings.UseSsl;
            chkMirrorMode.Checked = settings.MirrorMode;
            chkDryRun.Checked = settings.DryRun;
            chkSyncEnvFiles.Checked = settings.SyncEnvFiles;
            chkSyncVendorFiles.Checked = settings.SyncVendorFiles;
            chkHighSpeedMode.Checked = settings.HighSpeedMode;
            numParallelUploads.Value = Math.Clamp(settings.ParallelUploads, (int)numParallelUploads.Minimum, (int)numParallelUploads.Maximum);
            txtExcludePatterns.Text = settings.ExcludePatterns;
        }
        catch (Exception ex)
        {
            AppendLog($"載入設定失敗: {ex.Message}");
        }
    }

    private void SaveSettingsFromForm()
    {
        var settings = ReadSettingsFromForm();
        EnsureProfileDirectory(_currentProfileName);
        settings.Save(CurrentSettingsPath);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        try
        {
            SaveSettingsFromForm();
        }
        catch (Exception ex)
        {
            AppendLog($"儲存設定失敗: {ex.Message}");
        }

        base.OnFormClosing(e);
    }

    private SyncSettings ReadSettingsFromForm()
    {
        return new SyncSettings
        {
            Host = txtHost.Text.Trim(),
            Port = Decimal.ToInt32(numPort.Value),
            Username = txtUsername.Text.Trim(),
            Password = txtPassword.Text,
            LocalPath = txtLocalPath.Text.Trim(),
            RemotePath = txtRemotePath.Text.Trim(),
            UsePassiveMode = chkPassiveMode.Checked,
            UseSsl = chkUseSsl.Checked,
            MirrorMode = chkMirrorMode.Checked,
            DryRun = chkDryRun.Checked,
            SyncEnvFiles = chkSyncEnvFiles.Checked,
            SyncVendorFiles = chkSyncVendorFiles.Checked,
            HighSpeedMode = chkHighSpeedMode.Checked,
            ParallelUploads = Decimal.ToInt32(numParallelUploads.Value),
            ExcludePatterns = txtExcludePatterns.Text
        };
    }

    private bool TryBuildRequest(out FtpSyncRequest? request)
    {
        request = null;

        var settings = ReadSettingsFromForm();
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(settings.Host))
        {
            validationErrors.Add("FTP 主機不能為空白。");
        }

        if (string.IsNullOrWhiteSpace(settings.Username))
        {
            validationErrors.Add("帳號不能為空白。");
        }

        if (string.IsNullOrWhiteSpace(settings.LocalPath))
        {
            validationErrors.Add("本機資料夾不能為空白。");
        }
        else if (!Directory.Exists(settings.LocalPath))
        {
            validationErrors.Add("本機資料夾不存在。");
        }

        if (validationErrors.Count > 0)
        {
            MessageBox.Show(string.Join(Environment.NewLine, validationErrors), "設定錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        request = new FtpSyncRequest(
            settings.Host,
            settings.Port,
            settings.Username,
            settings.Password,
            settings.LocalPath,
            settings.RemotePath,
            CurrentSyncHistoryPath,
            settings.UsePassiveMode,
            settings.UseSsl,
            settings.MirrorMode,
            settings.DryRun,
            settings.SyncEnvFiles,
            settings.SyncVendorFiles,
            settings.HighSpeedMode,
            settings.ParallelUploads,
            settings.ExcludePatterns);

        return true;
    }

    private void UpdateUiState(bool isRunning)
    {
        btnStart.Enabled = !isRunning;
        btnReverseSync.Enabled = !isRunning;
        btnClearSyncHistory.Enabled = !isRunning;
        btnClearReverseSyncHistory.Enabled = !isRunning;
        btnCancel.Enabled = isRunning;
        btnBrowseLocal.Enabled = !isRunning;
        btnTestConnection.Enabled = !isRunning;
        btnSaveProfile.Enabled = !isRunning;
        btnDeleteProfile.Enabled = !isRunning;
        btnSwitchProfile.Enabled = !isRunning;

        pnlSettings.Enabled = !isRunning;
        if (!isRunning)
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = 0;
        }
    }

    private void AppendLog(string message)
    {
        var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        txtLog.AppendText(line + Environment.NewLine);
    }

    private void SetLogLines(IEnumerable<string> lines)
    {
        txtLog.Clear();
        foreach (var line in lines)
        {
            AppendLog(line);
        }
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        await RunSyncAsync(isReverse: false);
    }

    private async void btnReverseSync_Click(object sender, EventArgs e)
    {
        await RunSyncAsync(isReverse: true);
    }

    private async Task RunSyncAsync(bool isReverse)
    {
        if (!TryBuildRequest(out var request) || request is null)
        {
            return;
        }

        if (request.MirrorMode && !request.DryRun)
        {
            var confirmResult = MessageBox.Show(
                "鏡像同步會刪除遠端多餘檔案，是否繼續？",
                "確認鏡像同步",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }
        }

        try
        {
            SaveSettingsFromForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"儲存設定失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _syncCancellationTokenSource = new CancellationTokenSource();
        UpdateUiState(isRunning: true);
        txtLog.Clear();
        lblStatusValue.Text = request.DryRun ? "預演中" : isReverse ? "反向同步中" : "同步中";
        long lastUiTick = 0;

        var progress = new Progress<SyncProgress>(progressInfo =>
        {
            var isTerminalStatus = progressInfo.Status is "完成" or "失敗" or "部分失敗" or "已取消";
            var now = Environment.TickCount64;
            if (request.HighSpeedMode && !isTerminalStatus && now - lastUiTick < 500)
            {
                return;
            }

            lastUiTick = now;
            lblStatusValue.Text = progressInfo.Status;
            lblSummaryValue.Text = $"掃描 {progressInfo.ScannedFiles} / {(isReverse ? "下載" : "上傳")} {progressInfo.UploadedFiles} / 失敗 {progressInfo.FailedEntries} / 刪除 {progressInfo.DeletedEntries} / 略過 {progressInfo.SkippedFiles}";
            progressBar.Style = ProgressBarStyle.Blocks;
            var percent = progressInfo.TotalFiles <= 0
                ? 0
                : Math.Clamp((int)Math.Round(progressInfo.CompletedFiles * 100d / progressInfo.TotalFiles), 0, 100);
            progressBar.Value = percent;
        });

        try
        {
            var result = await Task.Run(
                () => isReverse
                    ? FtpSyncService.ReverseSyncAsync(request, progress, _syncCancellationTokenSource.Token)
                    : FtpSyncService.SyncAsync(request, progress, _syncCancellationTokenSource.Token),
                _syncCancellationTokenSource.Token);
            lblStatusValue.Text = result.FailedEntries > 0 ? "部分失敗" : request.DryRun ? "預演完成" : "完成";
            lblSummaryValue.Text = $"共掃描 {result.ScannedFiles}，{(isReverse ? "下載" : "上傳")} {result.UploadedFiles}，失敗 {result.FailedEntries}，刪除 {result.DeletedEntries}，略過 {result.SkippedFiles}";
            var logLines = new List<string>
            {
                request.DryRun ? $"預演完成 ({(isReverse ? "FTP -> 本機" : "本機 -> FTP")})。" : $"{(isReverse ? "反向同步" : "同步")}完成。",
                $"掃描 {result.ScannedFiles}，{(isReverse ? "下載" : "上傳")} {result.UploadedFiles}，失敗 {result.FailedEntries}，刪除 {result.DeletedEntries}，略過 {result.SkippedFiles}"
            };

            if (result.FailedEntries > 0)
            {
                logLines.Add("失敗檔案/資料夾:");
                logLines.AddRange(result.Failures.Select(failure => $"{failure.RelativePath} -> {failure.ErrorMessage}"));

                MessageBox.Show(
                    $"同步完成，但有 {result.FailedEntries} 筆失敗。請查看下方日誌。",
                    "部分失敗",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            SetLogLines(logLines);
        }
        catch (OperationCanceledException)
        {
            lblStatusValue.Text = "已取消";
            SetLogLines(new[] { "同步已取消。" });
        }
        catch (Exception ex)
        {
            lblStatusValue.Text = "失敗";
            SetLogLines(new[] { $"同步失敗: {ex.Message}" });
            MessageBox.Show(ex.Message, "同步失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _syncCancellationTokenSource.Dispose();
            _syncCancellationTokenSource = null;
            UpdateUiState(isRunning: false);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _syncCancellationTokenSource?.Cancel();
    }

    private void btnClearSyncHistory_Click(object sender, EventArgs e)
    {
        ClearSyncHistory("upload", "正向增量同步");
    }

    private void btnClearReverseSyncHistory_Click(object sender, EventArgs e)
    {
        ClearSyncHistory("download", "反向增量同步");
    }

    private void btnSwitchProfile_Click(object sender, EventArgs e)
    {
        try
        {
            SwitchProfile(cboProfiles.Text, saveCurrent: true);
        }
        catch (Exception ex)
        {
            SetLogLines(new[] { $"切換設定檔失敗: {ex.Message}" });
            MessageBox.Show(ex.Message, "切換失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnSaveProfile_Click(object sender, EventArgs e)
    {
        try
        {
            SaveProfile(cboProfiles.Text);
            MessageBox.Show($"設定檔已儲存: {_currentProfileName}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            SetLogLines(new[] { $"儲存設定檔失敗: {ex.Message}" });
            MessageBox.Show(ex.Message, "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnDeleteProfile_Click(object sender, EventArgs e)
    {
        try
        {
            DeleteProfile(cboProfiles.Text);
        }
        catch (Exception ex)
        {
            SetLogLines(new[] { $"刪除設定檔失敗: {ex.Message}" });
            MessageBox.Show(ex.Message, "刪除失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteProfile(string requestedProfileName)
    {
        var targetProfileName = NormalizeProfileName(requestedProfileName);
        var existingProfiles = Directory.EnumerateDirectories(_profilesRootPath)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!existingProfiles.Contains(targetProfileName, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("找不到要刪除的設定檔。");
        }

        if (existingProfiles.Count <= 1)
        {
            throw new InvalidOperationException("至少要保留一個設定檔，不能刪除最後一個。");
        }

        var confirmResult = MessageBox.Show(
            $"確定要刪除設定檔 '{targetProfileName}' 嗎？\r\n會一併刪除該設定檔的同步紀錄。",
            "確認刪除設定檔",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmResult != DialogResult.Yes)
        {
            return;
        }

        var targetDirectory = Path.Combine(_profilesRootPath, targetProfileName);
        Directory.Delete(targetDirectory, recursive: true);

        var nextProfileName = existingProfiles
            .First(name => !string.Equals(name, targetProfileName, StringComparison.OrdinalIgnoreCase));

        if (string.Equals(_currentProfileName, targetProfileName, StringComparison.OrdinalIgnoreCase))
        {
            SwitchProfile(nextProfileName, saveCurrent: false);
        }
        else
        {
            LoadProfileList(_currentProfileName);
            SetLogLines(new[] { $"設定檔已刪除: {targetProfileName}" });
        }

        MessageBox.Show($"設定檔已刪除: {targetProfileName}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ClearSyncHistory(string syncMode, string displayName)
    {
        if (!TryBuildRequest(out var request) || request is null)
        {
            return;
        }

        var confirmResult = MessageBox.Show(
            $"確定要清空目前設定對應的{displayName}紀錄嗎？",
            "確認清空同步紀錄",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmResult != DialogResult.Yes)
        {
            return;
        }

        try
        {
            var history = SyncHistoryStore.Load(CurrentSyncHistoryPath);
            var profileKey = SyncHistoryStore.BuildProfileKey(request, syncMode);
            var removed = history.RemoveProfile(profileKey);
            history.Save();

            var message = removed
                ? $"{displayName}紀錄已清空。"
                : $"{displayName}沒有可清除的紀錄。";

            SetLogLines(new[] { message });
            MessageBox.Show(message, "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            SetLogLines(new[] { $"清空{displayName}紀錄失敗: {ex.Message}" });
            MessageBox.Show(ex.Message, "清空失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnBrowseLocal_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "選擇要同步的本機資料夾",
            ShowNewFolderButton = true
        };

        if (Directory.Exists(txtLocalPath.Text))
        {
            dialog.InitialDirectory = txtLocalPath.Text;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            txtLocalPath.Text = dialog.SelectedPath;
        }
    }

    private async void btnTestConnection_Click(object sender, EventArgs e)
    {
        if (!TryBuildRequest(out var request) || request is null)
        {
            return;
        }

        UpdateUiState(isRunning: true);
        progressBar.Style = ProgressBarStyle.Marquee;
        lblStatusValue.Text = "測試連線";
        AppendLog("開始測試 FTP 連線...");

        try
        {
            await Task.Run(
                () => FtpSyncService.TestConnectionAsync(request, CancellationToken.None),
                CancellationToken.None);
            lblStatusValue.Text = "連線成功";
            AppendLog("FTP 連線成功。");
            MessageBox.Show("FTP 連線成功。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            lblStatusValue.Text = "連線失敗";
            AppendLog($"FTP 連線失敗: {ex.Message}");
            MessageBox.Show(ex.Message, "連線失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UpdateUiState(isRunning: false);
        }
    }

    private void btnExcludeHelp_Click(object sender, EventArgs e)
    {
        const string message = """
排除規則每行一筆，支援簡單萬用字元。

範例:
bin/
obj/
.git/
*.log
*.tmp
images/*.bak

說明:
- 結尾是 / 代表資料夾規則
- *.log 會排除所有副檔名為 .log 的檔案
- images/*.bak 會排除 images 底下符合的檔案
""";

        MessageBox.Show(message, "排除規則說明", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
