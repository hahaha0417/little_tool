using System.Text.Json;

namespace hardware_info;

public sealed class HardwareJsonWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public void WriteSnapshot(string filePath, HardwareSnapshot snapshot)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("JSON path is required.", nameof(filePath));
        }

        var fullPath = Path.GetFullPath(filePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var payload = new HardwareSnapshotFile(
            DateTime.Now,
            snapshot.TotalCpuUsagePercent,
            snapshot.CoreUsages.Select(core => new CpuCoreUsageFile(core.CoreName, core.UsagePercent)).ToList(),
            snapshot.UsedMemoryBytes,
            snapshot.TotalMemoryBytes,
            snapshot.UsedDiskBytes,
            snapshot.TotalDiskBytes);

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        File.WriteAllText(fullPath, json);
    }
}

public sealed record CpuCoreUsageFile(string CoreName, float UsagePercent);

public sealed record HardwareSnapshotFile(
    DateTime UpdatedAt,
    float TotalCpuUsagePercent,
    IReadOnlyList<CpuCoreUsageFile> CoreUsages,
    long UsedMemoryBytes,
    long TotalMemoryBytes,
    long UsedDiskBytes,
    long TotalDiskBytes);
