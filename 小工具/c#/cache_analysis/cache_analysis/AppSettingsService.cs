using System.Text;
using System.Text.Json;

namespace cache_analysis
{
    public sealed class AppSettingsService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public string GetSettingsDirectoryPath()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "setting");
            Directory.CreateDirectory(path);
            return path;
        }

        public string GetDefaultSettingsPath()
        {
            return Path.Combine(GetSettingsDirectoryPath(), "default.json");
        }

        public string GetUiStatePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "ui-state.json");
        }

        public IReadOnlyList<string> GetSettingsFiles()
        {
            return Directory.GetFiles(GetSettingsDirectoryPath(), "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public AppSettings Load(string settingsPath)
        {
            if (string.IsNullOrWhiteSpace(settingsPath) || !File.Exists(settingsPath))
            {
                return AppSettings.Default;
            }

            try
            {
                var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settingsPath));
                return settings ?? AppSettings.Default;
            }
            catch
            {
                return AppSettings.Default;
            }
        }

        public void Save(string settingsPath, AppSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settingsPath))
            {
                throw new InvalidOperationException("設定檔路徑不可為空。");
            }

            var directoryPath = Path.GetDirectoryName(settingsPath);
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new InvalidOperationException("設定檔資料夾路徑無效。");
            }

            Directory.CreateDirectory(directoryPath);

            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(settingsPath, json, new UTF8Encoding(false));
        }

        public void Delete(string settingsPath)
        {
            if (!string.IsNullOrWhiteSpace(settingsPath) && File.Exists(settingsPath))
            {
                File.Delete(settingsPath);
            }
        }

        public string Rename(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            {
                throw new FileNotFoundException("找不到要更名的設定檔。", sourcePath);
            }

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new InvalidOperationException("新設定檔路徑不可為空。");
            }

            var directoryPath = Path.GetDirectoryName(destinationPath);
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new InvalidOperationException("新設定檔資料夾路徑無效。");
            }

            Directory.CreateDirectory(directoryPath);
            File.Move(sourcePath, destinationPath, overwrite: false);
            return destinationPath;
        }

        public AppUiState LoadUiState()
        {
            var uiStatePath = GetUiStatePath();
            if (!File.Exists(uiStatePath))
            {
                return AppUiState.Default;
            }

            try
            {
                var state = JsonSerializer.Deserialize<AppUiState>(File.ReadAllText(uiStatePath));
                return state ?? AppUiState.Default;
            }
            catch
            {
                return AppUiState.Default;
            }
        }

        public void SaveUiState(AppUiState state)
        {
            var json = JsonSerializer.Serialize(state, JsonOptions);
            File.WriteAllText(GetUiStatePath(), json, new UTF8Encoding(false));
        }
    }

    public sealed record AppSettings(string ProjectPath, string OutputPath, int MaxDegreeOfParallelism)
    {
        public static AppSettings Default => new("", "", 0);
    }

    public sealed record AppUiState(string SelectedSettingsFileName)
    {
        public static AppUiState Default => new("");
    }
}
