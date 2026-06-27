namespace cache_analysis
{
    public partial class Form1 : Form
    {
        private readonly ProjectAnalysisService _analysisService = new();
        private readonly AppSettingsService _appSettingsService = new();

        public Form1()
        {
            InitializeComponent();
            FormClosing += Form1_FormClosing;
            InitializeDefaults();
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveUiState();
        }

        private void btnNewSettings_Click(object? sender, EventArgs e)
        {
            try
            {
                using var dialog = new SaveFileDialog
                {
                    Filter = "JSON (*.json)|*.json",
                    Title = "新增設定檔到 setting 資料夾",
                    InitialDirectory = _appSettingsService.GetSettingsDirectoryPath(),
                    FileName = Path.GetFileName(_appSettingsService.GetDefaultSettingsPath())
                };

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var settingsPath = dialog.FileName;
                _appSettingsService.Save(settingsPath, CollectSettings());
                RefreshSettingsList(settingsPath);
                LoadSelectedSettings();
                lblStatus.Text = $"已新增設定: {Path.GetFileName(settingsPath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "新增失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoadSettings_Click(object? sender, EventArgs e)
        {
            try
            {
                LoadSelectedSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "載入失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboSettingsFiles_DropDown(object? sender, EventArgs e)
        {
            var selectedPath = cboSettingsFiles.SelectedItem is SettingsFileItem item ? item.FullPath : null;
            RefreshSettingsList(selectedPath);
        }

        private void btnSaveSettings_Click(object? sender, EventArgs e)
        {
            try
            {
                var settingsPath = EnsureSettingsPathForSave();
                _appSettingsService.Save(settingsPath, CollectSettings());
                RefreshSettingsList(settingsPath);
                lblStatus.Text = $"已儲存設定: {Path.GetFileName(settingsPath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRenameSettings_Click(object? sender, EventArgs e)
        {
            try
            {
                var sourcePath = GetSelectedSettingsPath();
                if (!File.Exists(sourcePath))
                {
                    MessageBox.Show(this, "目前設定檔不存在，無法更名。", "更名失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var dialog = new SaveFileDialog
                {
                    Filter = "JSON (*.json)|*.json",
                    Title = "設定檔更名",
                    InitialDirectory = _appSettingsService.GetSettingsDirectoryPath(),
                    FileName = Path.GetFileName(sourcePath)
                };

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var newPath = _appSettingsService.Rename(sourcePath, dialog.FileName);
                RefreshSettingsList(newPath);
                lblStatus.Text = $"已更名設定檔: {Path.GetFileName(newPath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "更名失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteSettings_Click(object? sender, EventArgs e)
        {
            try
            {
                var settingsPath = GetSelectedSettingsPath();
                if (!File.Exists(settingsPath))
                {
                    MessageBox.Show(this, "目前設定檔不存在。", "刪除失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    this,
                    $"確定刪除設定檔?\n{settingsPath}",
                    "確認刪除",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                _appSettingsService.Delete(settingsPath);
                RefreshSettingsList();
                lblStatus.Text = $"已刪除設定檔: {Path.GetFileName(settingsPath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "刪除失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowseProject_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "選擇要分析的 C# / WinForms 專案資料夾",
                UseDescriptionForTitle = true,
                InitialDirectory = Directory.Exists(txtProjectPath.Text)
                    ? txtProjectPath.Text
                    : AppContext.BaseDirectory
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            txtProjectPath.Text = dialog.SelectedPath;

            if (string.IsNullOrWhiteSpace(txtOutputFolder.Text) || !Path.IsPathRooted(txtOutputFolder.Text))
            {
                txtOutputFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnBrowseOutputFolder_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "指定分析快取輸出資料夾",
                UseDescriptionForTitle = true,
                InitialDirectory = Directory.Exists(txtOutputFolder.Text)
                    ? txtOutputFolder.Text
                    : (Directory.Exists(txtProjectPath.Text) ? txtProjectPath.Text : AppContext.BaseDirectory)
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtOutputFolder.Text = dialog.SelectedPath;
            }
        }

        private async void btnGenerate_Click(object? sender, EventArgs e)
        {
            var projectPath = txtProjectPath.Text.Trim();
            var outputFolder = txtOutputFolder.Text.Trim();
            var maxDegreeOfParallelism = decimal.ToInt32(numParallelism.Value);

            if (!Directory.Exists(projectPath))
            {
                MessageBox.Show(this, "專案資料夾不存在。", "路徑錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                outputFolder = projectPath;
                txtOutputFolder.Text = outputFolder;
            }

            Directory.CreateDirectory(outputFolder);
            var outputPath = Path.Combine(outputFolder, "analysis-cache.md");

            SetBusyState(true, "分析中...");

            try
            {
                var result = await Task.Run(() => _analysisService.Generate(projectPath, outputPath, maxDegreeOfParallelism));
                txtPreview.Text = result.Markdown;
                SetBusyState(false, $"完成: cores={result.MaxDegreeOfParallelism}, rebuilt={result.RebuiltFiles}, reused={result.ReusedFiles}");
                MessageBox.Show(
                    this,
                    $"分析完成，已輸出到:\n{result.OutputPath}\n\n核心數: {result.MaxDegreeOfParallelism}\n重建檔案: {result.RebuiltFiles}\n沿用快取: {result.ReusedFiles}",
                    "完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SetBusyState(false, "分析失敗");
                MessageBox.Show(this, ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetBusyState(bool isBusy, string status)
        {
            cboSettingsFiles.Enabled = !isBusy;
            btnNewSettings.Enabled = !isBusy;
            btnLoadSettings.Enabled = !isBusy;
            btnSaveSettings.Enabled = !isBusy;
            btnRenameSettings.Enabled = !isBusy;
            btnDeleteSettings.Enabled = !isBusy;
            btnGenerate.Enabled = !isBusy;
            btnBrowseProject.Enabled = !isBusy;
            btnBrowseOutputFolder.Enabled = !isBusy;
            numParallelism.Enabled = !isBusy;
            lblStatus.Text = status;
        }

        private void InitializeDefaults()
        {
            txtProjectPath.Text = AppContext.BaseDirectory;
            txtOutputFolder.Text = AppContext.BaseDirectory;
            numParallelism.Value = Math.Max(numParallelism.Minimum, Math.Min(numParallelism.Maximum, Environment.ProcessorCount));

            var defaultSettingsPath = _appSettingsService.GetDefaultSettingsPath();
            EnsureDefaultSettingsFile(defaultSettingsPath);
            var startupSettingsPath = ResolveStartupSettingsPath(defaultSettingsPath);
            RefreshSettingsList(startupSettingsPath);
            ApplySettings(_appSettingsService.Load(startupSettingsPath));
            lblStatus.Text = $"已自動載入設定: {Path.GetFileName(startupSettingsPath)}";
        }

        private void RefreshSettingsList(string? selectedPath = null)
        {
            var files = _appSettingsService.GetSettingsFiles();
            cboSettingsFiles.BeginUpdate();
            cboSettingsFiles.Items.Clear();

            foreach (var file in files)
            {
                cboSettingsFiles.Items.Add(new SettingsFileItem(file));
            }

            cboSettingsFiles.EndUpdate();

            if (cboSettingsFiles.Items.Count == 0)
            {
                cboSettingsFiles.Text = string.Empty;
                return;
            }

            var matchPath = selectedPath ?? _appSettingsService.GetDefaultSettingsPath();
            for (var i = 0; i < cboSettingsFiles.Items.Count; i++)
            {
                if (cboSettingsFiles.Items[i] is SettingsFileItem item
                    && string.Equals(item.FullPath, matchPath, StringComparison.OrdinalIgnoreCase))
                {
                    cboSettingsFiles.SelectedIndex = i;
                    return;
                }
            }

            cboSettingsFiles.SelectedIndex = 0;
        }

        private void ApplySettings(AppSettings settings)
        {
            var projectPath = !string.IsNullOrWhiteSpace(settings.ProjectPath) ? settings.ProjectPath : AppContext.BaseDirectory;
            var outputFolder = !string.IsNullOrWhiteSpace(settings.OutputPath) ? settings.OutputPath : projectPath;
            var parallelism = settings.MaxDegreeOfParallelism > 0
                ? settings.MaxDegreeOfParallelism
                : Environment.ProcessorCount;

            txtProjectPath.Text = projectPath;
            txtOutputFolder.Text = outputFolder;
            numParallelism.Value = Math.Max(numParallelism.Minimum, Math.Min(numParallelism.Maximum, parallelism));
        }

        private AppSettings CollectSettings()
        {
            return new AppSettings(
                txtProjectPath.Text.Trim(),
                txtOutputFolder.Text.Trim(),
                decimal.ToInt32(numParallelism.Value));
        }

        private string GetSelectedSettingsPath()
        {
            if (cboSettingsFiles.SelectedItem is SettingsFileItem item)
            {
                return item.FullPath;
            }

            throw new InvalidOperationException("請先從下拉選單選擇設定檔。");
        }

        private string EnsureSettingsPathForSave()
        {
            if (cboSettingsFiles.SelectedItem is SettingsFileItem item)
            {
                return item.FullPath;
            }

            return _appSettingsService.GetDefaultSettingsPath();
        }

        private void LoadSelectedSettings()
        {
            var settingsPath = GetSelectedSettingsPath();
            ApplySettings(_appSettingsService.Load(settingsPath));
            lblStatus.Text = $"已載入設定: {Path.GetFileName(settingsPath)}";
        }

        private void EnsureDefaultSettingsFile(string defaultSettingsPath)
        {
            if (File.Exists(defaultSettingsPath))
            {
                return;
            }

            _appSettingsService.Save(defaultSettingsPath, CollectSettings());
        }

        private string ResolveStartupSettingsPath(string defaultSettingsPath)
        {
            var uiState = _appSettingsService.LoadUiState();
            if (!string.IsNullOrWhiteSpace(uiState.SelectedSettingsFileName))
            {
                var selectedPath = Path.Combine(_appSettingsService.GetSettingsDirectoryPath(), uiState.SelectedSettingsFileName);
                if (File.Exists(selectedPath))
                {
                    return selectedPath;
                }
            }

            return defaultSettingsPath;
        }

        private void SaveUiState()
        {
            var fileName = cboSettingsFiles.SelectedItem is SettingsFileItem item
                ? Path.GetFileName(item.FullPath)
                : Path.GetFileName(_appSettingsService.GetDefaultSettingsPath());

            _appSettingsService.SaveUiState(new AppUiState(fileName));
        }

        private sealed record SettingsFileItem(string FullPath)
        {
            public override string ToString()
            {
                return Path.GetFileName(FullPath);
            }
        }
    }
}
