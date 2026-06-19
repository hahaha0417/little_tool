using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ftp_sync;

internal sealed class SyncHistoryStore
{
    private readonly string _path;
    private readonly Dictionary<string, SyncProfileHistory> _profiles;

    private SyncHistoryStore(string path, Dictionary<string, SyncProfileHistory> profiles)
    {
        _path = path;
        _profiles = profiles;
    }

    public static SyncHistoryStore Load(string path)
    {
        if (!File.Exists(path))
        {
            return new SyncHistoryStore(path, new Dictionary<string, SyncProfileHistory>(StringComparer.OrdinalIgnoreCase));
        }

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<SyncHistoryFile>(json) ?? new SyncHistoryFile();
        return new SyncHistoryStore(
            path,
            data.Profiles ?? new Dictionary<string, SyncProfileHistory>(StringComparer.OrdinalIgnoreCase));
    }

    public bool TryGetEntry(string profileKey, string relativePath, out SyncHistoryEntry entry)
    {
        entry = default!;

        if (!_profiles.TryGetValue(profileKey, out var profile))
        {
            return false;
        }

        return profile.Files.TryGetValue(relativePath, out entry!);
    }

    public void SetEntry(string profileKey, string relativePath, DateTime localWriteTimeUtc, long fileSize)
    {
        if (!_profiles.TryGetValue(profileKey, out var profile))
        {
            profile = new SyncProfileHistory();
            _profiles[profileKey] = profile;
        }

        profile.Files[relativePath] = new SyncHistoryEntry
        {
            LocalWriteTimeUtc = localWriteTimeUtc,
            FileSize = fileSize
        };
    }

    public void RemoveMissingEntries(string profileKey, IReadOnlyCollection<string> existingRelativePaths)
    {
        if (!_profiles.TryGetValue(profileKey, out var profile))
        {
            return;
        }

        var existing = new HashSet<string>(existingRelativePaths, StringComparer.OrdinalIgnoreCase);
        var toRemove = profile.Files.Keys.Where(path => !existing.Contains(path)).ToList();

        foreach (var path in toRemove)
        {
            profile.Files.Remove(path);
        }
    }

    public bool RemoveProfile(string profileKey)
    {
        return _profiles.Remove(profileKey);
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(new SyncHistoryFile
        {
            Profiles = _profiles
        }, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_path, json);
    }

    public static string BuildProfileKey(FtpSyncRequest request, string syncMode)
    {
        var raw = string.Join("|",
            syncMode.Trim().ToLowerInvariant(),
            request.Host.Trim().ToLowerInvariant(),
            request.Port,
            request.Username.Trim().ToLowerInvariant(),
            NormalizeLocal(request.LocalPath),
            NormalizeRemote(request.RemotePath));

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }

    private static string NormalizeLocal(string path)
    {
        return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant();
    }

    private static string NormalizeRemote(string path)
    {
        var normalized = path.Replace('\\', '/').Trim();
        if (!normalized.StartsWith('/'))
        {
            normalized = "/" + normalized;
        }

        return normalized.TrimEnd('/').ToLowerInvariant();
    }
}

internal sealed class SyncHistoryFile
{
    public Dictionary<string, SyncProfileHistory>? Profiles { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

internal sealed class SyncProfileHistory
{
    public Dictionary<string, SyncHistoryEntry> Files { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

internal sealed class SyncHistoryEntry
{
    public DateTime LocalWriteTimeUtc { get; set; }

    public DateTime? RemoteWriteTimeUtc { get; set; }

    public long FileSize { get; set; }
}
