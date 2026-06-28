using System.Text;
using CppDocGenerator.Models;

namespace CppDocGenerator.Services;

public sealed class IniSettingsService
{
    public string SettingsFolderPath { get; }
    public string AppSettingsPath => Path.Combine(SettingsFolderPath, "app.ini");
    public string ProfilesFolderPath => Path.Combine(SettingsFolderPath, "profiles");
    private string LegacySettingsPath => Path.Combine(SettingsFolderPath, "settings.ini");

    public IniSettingsService(string settingsFolderPath)
    {
        SettingsFolderPath = settingsFolderPath;
    }

    public AppSettings Load()
    {
        Directory.CreateDirectory(SettingsFolderPath);
        Directory.CreateDirectory(ProfilesFolderPath);

        if (!File.Exists(AppSettingsPath) && !Directory.EnumerateFiles(ProfilesFolderPath, "*.ini").Any())
        {
            if (File.Exists(LegacySettingsPath))
            {
                var legacySettings = LoadLegacySingleFile(LegacySettingsPath);
                Save(legacySettings);
                return legacySettings;
            }

            return CreateDefaultSettings();
        }

        var settings = new AppSettings
        {
            ActiveProfileName = LoadActiveProfileName()
        };

        foreach (var profilePath in Directory.EnumerateFiles(ProfilesFolderPath, "*.ini").OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var profile = LoadProfile(profilePath);
            if (profile is not null)
            {
                settings.Profiles.Add(profile);
            }
        }

        if (settings.Profiles.Count == 0)
        {
            return CreateDefaultSettings();
        }

        if (!settings.Profiles.Any(profile => string.Equals(profile.Name, settings.ActiveProfileName, StringComparison.OrdinalIgnoreCase)))
        {
            settings.ActiveProfileName = settings.Profiles[0].Name;
        }

        return settings;
    }

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(SettingsFolderPath);
        Directory.CreateDirectory(ProfilesFolderPath);

        foreach (var oldProfilePath in Directory.EnumerateFiles(ProfilesFolderPath, "*.ini"))
        {
            File.Delete(oldProfilePath);
        }

        foreach (var profile in settings.Profiles.OrderBy(profile => profile.Name, StringComparer.OrdinalIgnoreCase))
        {
            var profilePath = Path.Combine(ProfilesFolderPath, $"{BuildSafeFileName(profile.Name)}.ini");
            SaveProfile(profile, profilePath);
        }

        var appBuilder = new StringBuilder();
        appBuilder.AppendLine("[app]");
        appBuilder.Append("activeProfile=").AppendLine(Escape(settings.ActiveProfileName));
        appBuilder.AppendLine();
        File.WriteAllText(AppSettingsPath, appBuilder.ToString(), Encoding.UTF8);
    }

    private static AppSettings CreateDefaultSettings()
    {
        return new AppSettings
        {
            Profiles =
            [
                new AppProfile
                {
                    Name = "Default",
                    SourceFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    OutputFolder = Path.Combine(AppContext.BaseDirectory, "cpp-doc-json"),
                    GitHubRepo = "https://github.com/owner/repo",
                    GitReference = "main",
                    Parallelism = Environment.ProcessorCount
                }
            ]
        };
    }

    private string LoadActiveProfileName()
    {
        if (!File.Exists(AppSettingsPath))
        {
            return "Default";
        }

        foreach (var rawLine in File.ReadAllLines(AppSettingsPath, Encoding.UTF8))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || line.StartsWith('#') || line.StartsWith('['))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = Unescape(line[(separatorIndex + 1)..].Trim());
            if (string.Equals(key, "activeProfile", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }

        return "Default";
    }

    private static AppProfile? LoadProfile(string profilePath)
    {
        var profile = new AppProfile
        {
            Name = Path.GetFileNameWithoutExtension(profilePath)
        };

        foreach (var rawLine in File.ReadAllLines(profilePath, Encoding.UTF8))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || line.StartsWith('#') || line.StartsWith('['))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = Unescape(line[(separatorIndex + 1)..].Trim());

            switch (key)
            {
                case "name":
                    profile.Name = value;
                    break;
                case "sourceFolder":
                    profile.SourceFolder = value;
                    break;
                case "outputFolder":
                    profile.OutputFolder = value;
                    break;
                case "gitHubRepo":
                    profile.GitHubRepo = value;
                    break;
                case "gitReference":
                    profile.GitReference = value;
                    break;
                case "parallelism":
                    if (int.TryParse(value, out var parallelism))
                    {
                        profile.Parallelism = Math.Max(1, parallelism);
                    }
                    break;
            }
        }

        return string.IsNullOrWhiteSpace(profile.Name) ? null : profile;
    }

    private static void SaveProfile(AppProfile profile, string profilePath)
    {
        var builder = new StringBuilder();
        builder.AppendLine("[profile]");
        builder.Append("name=").AppendLine(Escape(profile.Name));
        builder.Append("sourceFolder=").AppendLine(Escape(profile.SourceFolder));
        builder.Append("outputFolder=").AppendLine(Escape(profile.OutputFolder));
        builder.Append("gitHubRepo=").AppendLine(Escape(profile.GitHubRepo));
        builder.Append("gitReference=").AppendLine(Escape(profile.GitReference));
        builder.Append("parallelism=").AppendLine(Escape(profile.Parallelism.ToString()));
        builder.AppendLine();
        File.WriteAllText(profilePath, builder.ToString(), Encoding.UTF8);
    }

    private static string BuildSafeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new StringBuilder(fileName.Length);
        foreach (var ch in fileName)
        {
            sanitized.Append(invalidChars.Contains(ch) ? '_' : ch);
        }

        var value = sanitized.ToString().Trim();
        return string.IsNullOrWhiteSpace(value) ? "Profile" : value;
    }

    private static AppSettings LoadLegacySingleFile(string settingsPath)
    {
        var settings = new AppSettings();
        AppProfile? currentProfile = null;

        foreach (var rawLine in File.ReadAllLines(settingsPath, Encoding.UTF8))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                var section = line[1..^1];
                currentProfile = null;

                if (section.StartsWith("profile:", StringComparison.OrdinalIgnoreCase))
                {
                    var profileName = section["profile:".Length..].Trim();
                    currentProfile = new AppProfile { Name = profileName };
                    settings.Profiles.Add(currentProfile);
                }

                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = Unescape(line[(separatorIndex + 1)..].Trim());

            if (currentProfile is null)
            {
                if (string.Equals(key, "activeProfile", StringComparison.OrdinalIgnoreCase))
                {
                    settings.ActiveProfileName = value;
                }

                continue;
            }

            switch (key)
            {
                case "sourceFolder":
                    currentProfile.SourceFolder = value;
                    break;
                case "outputFolder":
                    currentProfile.OutputFolder = value;
                    break;
                case "gitHubRepo":
                    currentProfile.GitHubRepo = value;
                    break;
                case "gitReference":
                    currentProfile.GitReference = value;
                    break;
                case "parallelism":
                    if (int.TryParse(value, out var parallelism))
                    {
                        currentProfile.Parallelism = Math.Max(1, parallelism);
                    }
                    break;
            }
        }

        if (settings.Profiles.Count == 0)
        {
            return CreateDefaultSettings();
        }

        if (!settings.Profiles.Any(profile => string.Equals(profile.Name, settings.ActiveProfileName, StringComparison.OrdinalIgnoreCase)))
        {
            settings.ActiveProfileName = settings.Profiles[0].Name;
        }

        return settings;
    }

    private static string Escape(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

    private static string Unescape(string value)
    {
        return value
            .Replace("\\n", "\n", StringComparison.Ordinal)
            .Replace("\\r", "\r", StringComparison.Ordinal)
            .Replace("\\\\", "\\", StringComparison.Ordinal);
    }
}
