namespace CppDocGenerator.Models;

public sealed class AppProfile
{
    public string Name { get; set; } = "Default";
    public string SourceFolder { get; set; } = string.Empty;
    public string OutputFolder { get; set; } = string.Empty;
    public string GitHubRepo { get; set; } = string.Empty;
    public string GitReference { get; set; } = "main";
    public int Parallelism { get; set; } = Environment.ProcessorCount;
}

public sealed class AppSettings
{
    public string ActiveProfileName { get; set; } = "Default";
    public List<AppProfile> Profiles { get; set; } = [];
}
