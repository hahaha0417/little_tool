using System.Text.Json;

namespace ftp_sync;

public sealed class SyncSettings
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 21;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string LocalPath { get; set; } = string.Empty;

    public string RemotePath { get; set; } = "/";

    public bool UsePassiveMode { get; set; } = true;

    public bool UseSsl { get; set; }

    public bool MirrorMode { get; set; }

    public bool DryRun { get; set; }

    public bool SyncEnvFiles { get; set; }

    public bool SyncVendorFiles { get; set; }

    public bool HighSpeedMode { get; set; }

    public int ParallelUploads { get; set; } = 4;

    public string ExcludePatterns { get; set; } = "bin/\r\nobj/\r\n.git/\r\n*.log";

    public static SyncSettings Load(string path)
    {
        if (!File.Exists(path))
        {
            return new SyncSettings();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SyncSettings>(json) ?? new SyncSettings();
    }

    public void Save(string path)
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, json);
    }
}
