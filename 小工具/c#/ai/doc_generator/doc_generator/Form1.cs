using CppDocGenerator.Models;
using CppDocGenerator.Services;

namespace CppDocGenerator;

public partial class Form1 : Form
{
    private readonly CppDocParser _parser = new();
    private readonly JsonExportService _jsonExportService = new();
    private readonly HtmlExportService _htmlExportService = new();
    private readonly IniSettingsService _settingsService = new(Path.Combine(AppContext.BaseDirectory, "settings"));
    private readonly GitRepositoryInfoService _gitRepositoryInfoService = new();
    private AppSettings _settings = new();
    private bool _isLoadingProfile;

    public Form1()
    {
        InitializeComponent();
        LoadSettings();
        FormClosing += Form1_FormClosing;
    }

    private void btnBrowseSource_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "選擇 C++ 專案資料夾"
        };

        if (Directory.Exists(txtSourceFolder.Text))
        {
            dialog.InitialDirectory = txtSourceFolder.Text;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            txtSourceFolder.Text = dialog.SelectedPath;
        }
    }

    private void btnBrowseOutput_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "選擇輸出 JSON 資料夾"
        };

        if (Directory.Exists(txtOutputFile.Text))
        {
            dialog.InitialDirectory = txtOutputFile.Text;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            txtOutputFile.Text = dialog.SelectedPath;
        }
    }

    private async void btnGenerate_Click(object sender, EventArgs e)
    {
        SaveCurrentProfileFromForm();
        PersistSettings();

        if (!TryGetGenerationRequest(out var request))
        {
            return;
        }

        ToggleControls(false);
        WriteLog($"開始掃描：{request.SourceFolder}");

        try
        {
            var (project, previewJson) = await GenerateJsonOutputAsync(request);

            txtJsonPreview.Text = previewJson;
            WriteLog($"完成，共 {project.FileCount} 個檔案，{project.SymbolCount} 個符號。");
            WriteLog($"平行執行緒：{project.Parallelism}");
            WriteLog($"JSON 資料夾輸出：{request.OutputFolder}");
            WriteLog($"GitHub Repo：{project.GitHubRepositoryUrl}");
            WriteLog($"Git Ref：{project.GitReference}");
            WriteLog($"根索引：{Path.Combine(request.OutputFolder, JsonExportService.FolderIndexFileName)}");
            WriteLog("每個檔案 JSON 會帶 GitHub blob/raw 連結與符號行號連結，供 C++ Doc HTML 嵌入使用。");
        }
        catch (Exception ex)
        {
            WriteLog($"失敗：{ex.Message}");
            MessageBox.Show(this, ex.ToString(), "執行失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            ToggleControls(true);
        }
    }

    private async void btnGenerateHtml_Click(object sender, EventArgs e)
    {
        SaveCurrentProfileFromForm();
        PersistSettings();

        if (!TryGetGenerationRequest(out var request))
        {
            return;
        }

        ToggleControls(false);
        try
        {
            var (project, previewJson) = await GenerateJsonOutputAsync(request);
            txtJsonPreview.Text = previewJson;

            var htmlPath = _htmlExportService.Save(request.OutputFolder);
            WriteLog($"完成，共 {project.FileCount} 個檔案，{project.SymbolCount} 個符號。");
            WriteLog($"平行執行緒：{project.Parallelism}");
            WriteLog($"JSON 資料夾輸出：{request.OutputFolder}");
            WriteLog($"HTML 輸出：{htmlPath}");
            WriteLog("HTML 會使用最新 JSON 重新產生，避免混入舊文件。");
        }
        catch (Exception ex)
        {
            WriteLog($"HTML 產生失敗：{ex.Message}");
            MessageBox.Show(this, ex.ToString(), "執行失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            ToggleControls(true);
        }
    }

    private void ToggleControls(bool enabled)
    {
        cmbProfiles.Enabled = enabled;
        btnAddProfile.Enabled = enabled;
        btnDeleteProfile.Enabled = enabled;
        btnRenameProfile.Enabled = enabled;
        btnBrowseSource.Enabled = enabled;
        btnBrowseOutput.Enabled = enabled;
        txtGitHubRepo.Enabled = enabled;
        txtGitReference.Enabled = enabled;
        numParallelism.Enabled = enabled;
        btnGenerate.Enabled = enabled;
        btnGenerateHtml.Enabled = enabled;
    }

    private void WriteLog(string message)
    {
        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void LoadSettings()
    {
        _settings = _settingsService.Load();
        RebindProfiles();
        SelectProfile(_settings.ActiveProfileName);
    }

    private void RebindProfiles()
    {
        _isLoadingProfile = true;
        cmbProfiles.DataSource = null;
        cmbProfiles.DataSource = _settings.Profiles;
        cmbProfiles.DisplayMember = nameof(AppProfile.Name);
        cmbProfiles.ValueMember = nameof(AppProfile.Name);
        _isLoadingProfile = false;
    }

    private void SelectProfile(string profileName)
    {
        var index = _settings.Profiles.FindIndex(profile =>
            string.Equals(profile.Name, profileName, StringComparison.OrdinalIgnoreCase));

        if (index < 0)
        {
            index = 0;
        }

        if (index >= 0 && index < cmbProfiles.Items.Count)
        {
            _isLoadingProfile = true;
            cmbProfiles.SelectedIndex = index;
            _isLoadingProfile = false;
            LoadProfileToForm(_settings.Profiles[index]);
            _settings.ActiveProfileName = _settings.Profiles[index].Name;
        }
    }

    private void LoadProfileToForm(AppProfile profile)
    {
        _isLoadingProfile = true;
        txtSourceFolder.Text = profile.SourceFolder;
        txtOutputFile.Text = profile.OutputFolder;
        txtGitHubRepo.Text = profile.GitHubRepo;
        txtGitReference.Text = string.IsNullOrWhiteSpace(profile.GitReference) ? "main" : profile.GitReference;
        numParallelism.Value = Math.Max(numParallelism.Minimum, Math.Min(numParallelism.Maximum, profile.Parallelism <= 0 ? Environment.ProcessorCount : profile.Parallelism));
        _isLoadingProfile = false;
    }

    private void SaveCurrentProfileFromForm()
    {
        if (_isLoadingProfile)
        {
            return;
        }

        var profile = GetCurrentProfile();
        if (profile is null)
        {
            return;
        }

        SaveProfileFromForm(profile);
        _settings.ActiveProfileName = profile.Name;
    }

    private void SaveActiveProfileFromForm()
    {
        if (_isLoadingProfile)
        {
            return;
        }

        var profile = _settings.Profiles.FirstOrDefault(item =>
            string.Equals(item.Name, _settings.ActiveProfileName, StringComparison.OrdinalIgnoreCase));

        if (profile is null)
        {
            return;
        }

        SaveProfileFromForm(profile);
    }

    private void SaveProfileFromForm(AppProfile profile)
    {
        profile.SourceFolder = txtSourceFolder.Text.Trim();
        profile.OutputFolder = txtOutputFile.Text.Trim();
        profile.GitHubRepo = txtGitHubRepo.Text.Trim();
        profile.GitReference = txtGitReference.Text.Trim();
        profile.Parallelism = Decimal.ToInt32(numParallelism.Value);
    }

    private AppProfile? GetCurrentProfile()
    {
        return cmbProfiles.SelectedItem as AppProfile;
    }

    private void PersistSettings()
    {
        if (_settings.Profiles.Count == 0)
        {
            return;
        }

        _settingsService.Save(_settings);
    }

    private void cmbProfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isLoadingProfile)
        {
            return;
        }

        SaveActiveProfileFromForm();
        var profile = GetCurrentProfile();
        if (profile is null)
        {
            return;
        }

        _settings.ActiveProfileName = profile.Name;
        LoadProfileToForm(profile);
        PersistSettings();
        WriteLog($"已載入設定檔：{profile.Name}");
    }

    private void btnAddProfile_Click(object sender, EventArgs e)
    {
        SaveCurrentProfileFromForm();

        var name = TextPromptDialog.Show(this, "新增設定檔", "請輸入新的設定檔名稱：", $"Profile{_settings.Profiles.Count + 1}");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        if (_settings.Profiles.Any(profile => string.Equals(profile.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show(this, "設定檔名稱已存在。", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var current = GetCurrentProfile();
        var profile = new AppProfile
        {
            Name = name,
            SourceFolder = current?.SourceFolder ?? txtSourceFolder.Text.Trim(),
            OutputFolder = current?.OutputFolder ?? txtOutputFile.Text.Trim(),
            GitHubRepo = current?.GitHubRepo ?? txtGitHubRepo.Text.Trim(),
            GitReference = current?.GitReference ?? txtGitReference.Text.Trim(),
            Parallelism = current?.Parallelism ?? Decimal.ToInt32(numParallelism.Value)
        };

        _settings.Profiles.Add(profile);
        RebindProfiles();
        SelectProfile(profile.Name);
        PersistSettings();
        WriteLog($"已新增設定檔：{profile.Name}");
    }

    private void btnDeleteProfile_Click(object sender, EventArgs e)
    {
        var profile = GetCurrentProfile();
        if (profile is null)
        {
            return;
        }

        if (_settings.Profiles.Count <= 1)
        {
            MessageBox.Show(this, "至少要保留一個設定檔。", "無法刪除", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show(this, $"確定要刪除設定檔「{profile.Name}」嗎？", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            return;
        }

        _settings.Profiles.Remove(profile);
        RebindProfiles();
        SelectProfile(_settings.Profiles[0].Name);
        PersistSettings();
        WriteLog($"已刪除設定檔：{profile.Name}");
    }

    private void btnRenameProfile_Click(object sender, EventArgs e)
    {
        var profile = GetCurrentProfile();
        if (profile is null)
        {
            return;
        }

        var newName = TextPromptDialog.Show(this, "重新命名設定檔", "請輸入新的設定檔名稱：", profile.Name);
        if (string.IsNullOrWhiteSpace(newName) || string.Equals(newName, profile.Name, StringComparison.Ordinal))
        {
            return;
        }

        if (_settings.Profiles.Any(item => !ReferenceEquals(item, profile) && string.Equals(item.Name, newName, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show(this, "設定檔名稱已存在。", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        profile.Name = newName;
        _settings.ActiveProfileName = newName;
        RebindProfiles();
        SelectProfile(newName);
        PersistSettings();
        WriteLog($"已重新命名設定檔：{newName}");
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        SaveCurrentProfileFromForm();
        PersistSettings();
    }

    private async Task<(CppDocProject Project, string PreviewJson)> GenerateJsonOutputAsync(GenerationRequest request)
    {
        request = ResolveGitSettings(request);

        var options = new CppDocParseOptions
        {
            GitHubRepositoryUrl = request.GitHubRepo,
            GitReference = request.GitReference,
            Parallelism = request.Parallelism
        };

        var project = await Task.Run(() => _parser.ParseDirectory(request.SourceFolder, options));
        var previewJson = await Task.Run(() => _jsonExportService.SaveAsFolder(project, request.OutputFolder));
        return (project, previewJson);
    }

    private GenerationRequest ResolveGitSettings(GenerationRequest request)
    {
        var gitInfo = _gitRepositoryInfoService.TryGetInfo(request.SourceFolder);
        if (gitInfo is null)
        {
            return request with
            {
                GitHubRepo = NormalizeGitHubRepoUrl(request.GitHubRepo)
            };
        }

        var resolvedRepo = request.GitHubRepo;
        if (string.IsNullOrWhiteSpace(resolvedRepo) ||
            resolvedRepo.Contains("owner/repo", StringComparison.OrdinalIgnoreCase))
        {
            resolvedRepo = gitInfo.GitHubRepositoryUrl;
            if (!string.IsNullOrWhiteSpace(resolvedRepo))
            {
                txtGitHubRepo.Text = resolvedRepo;
                WriteLog($"已自動套用 GitHub Repo：{resolvedRepo}");
            }
        }

        var resolvedRef = request.GitReference;
        if (string.IsNullOrWhiteSpace(resolvedRef) ||
            (string.Equals(resolvedRef, "main", StringComparison.OrdinalIgnoreCase) &&
             !string.Equals(gitInfo.CurrentBranch, "main", StringComparison.OrdinalIgnoreCase) &&
             !string.IsNullOrWhiteSpace(gitInfo.CurrentBranch)))
        {
            resolvedRef = gitInfo.CurrentBranch;
            if (!string.IsNullOrWhiteSpace(resolvedRef))
            {
                txtGitReference.Text = resolvedRef;
                WriteLog($"已自動套用 Git Branch：{resolvedRef}");
            }
        }

        return request with
        {
            GitHubRepo = NormalizeGitHubRepoUrl(resolvedRepo),
            GitReference = resolvedRef
        };
    }

    private bool TryGetGenerationRequest(out GenerationRequest request)
    {
        request = new GenerationRequest(
            txtSourceFolder.Text.Trim(),
            txtOutputFile.Text.Trim(),
            txtGitHubRepo.Text.Trim(),
            txtGitReference.Text.Trim(),
            Decimal.ToInt32(numParallelism.Value));

        if (!Directory.Exists(request.SourceFolder))
        {
            MessageBox.Show(this, "C++ Source 資料夾不存在。", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.OutputFolder))
        {
            MessageBox.Show(this, "請指定輸出 JSON 資料夾。", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (!Uri.TryCreate(request.GitHubRepo, UriKind.Absolute, out var gitHubUri) ||
            !string.Equals(gitHubUri.Host, "github.com", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(this, "請輸入有效的 GitHub Repo URL。", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.GitReference))
        {
            MessageBox.Show(this, "請輸入 Git branch 或 tag。", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private static string NormalizeGitHubRepoUrl(string repoUrl)
    {
        var value = repoUrl.Trim().TrimEnd('/');
        if (value.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
        {
            value = value[..^4];
        }

        return value;
    }

    private sealed record GenerationRequest(
        string SourceFolder,
        string OutputFolder,
        string GitHubRepo,
        string GitReference,
        int Parallelism);
}
