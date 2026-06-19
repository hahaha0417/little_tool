using System.Text.Json;

namespace ftp_sync;

public sealed class ProfileState
{
    public string LastProfileName { get; set; } = "default";

    public static ProfileState Load(string path)
    {
        if (!File.Exists(path))
        {
            return new ProfileState();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ProfileState>(json) ?? new ProfileState();
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
