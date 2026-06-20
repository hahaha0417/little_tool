using System.Text.Json;

namespace hardware_info;

public sealed class AppSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsPath;

    public AppSettingsStore()
    {
        _settingsPath = Path.Combine(AppContext.BaseDirectory, "app-settings.json");
    }

    public AppSettings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return new AppSettings();
        }

        var json = File.ReadAllText(_settingsPath);
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(_settingsPath, json);
    }
}

public sealed record AppSettings
{
    public string JsonOutputPath { get; init; } = string.Empty;
    public int WriteIntervalMs { get; init; } = 1000;
    public int UiUpdateIntervalMs { get; init; } = 1000;
    public int InternalSamplingIntervalMs { get; init; } = 250;
    public int CpuSmoothingWindowSize { get; init; } = 5;
}
