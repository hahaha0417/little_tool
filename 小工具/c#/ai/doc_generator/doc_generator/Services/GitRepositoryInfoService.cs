using System.Text.RegularExpressions;

namespace CppDocGenerator.Services;

public sealed class GitRepositoryInfoService
{
    private static readonly Regex SectionRegex = new(@"^\s*\[(.+?)\]\s*$", RegexOptions.Compiled);

    public GitRepositoryInfo? TryGetInfo(string sourceFolder)
    {
        var repositoryRoot = FindRepositoryRoot(sourceFolder);
        if (repositoryRoot is null)
        {
            return null;
        }

        var gitDirectory = Path.Combine(repositoryRoot, ".git");
        var headPath = Path.Combine(gitDirectory, "HEAD");
        var configPath = Path.Combine(gitDirectory, "config");

        var branch = File.Exists(headPath)
            ? ParseBranchName(File.ReadAllText(headPath))
            : string.Empty;

        var originUrl = File.Exists(configPath)
            ? ParseOriginUrl(File.ReadAllLines(configPath))
            : string.Empty;

        return new GitRepositoryInfo(
            repositoryRoot,
            NormalizeGitHubRemote(originUrl),
            branch);
    }

    private static string? FindRepositoryRoot(string sourceFolder)
    {
        var directory = new DirectoryInfo(Path.GetFullPath(sourceFolder));
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")) ||
                File.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string ParseBranchName(string headContent)
    {
        var text = headContent.Trim();
        const string prefix = "ref:";
        if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        var reference = text[prefix.Length..].Trim();
        var lastSlash = reference.LastIndexOf('/');
        return lastSlash >= 0 ? reference[(lastSlash + 1)..] : reference;
    }

    private static string ParseOriginUrl(IEnumerable<string> lines)
    {
        var currentSection = string.Empty;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || line.StartsWith('#'))
            {
                continue;
            }

            var sectionMatch = SectionRegex.Match(line);
            if (sectionMatch.Success)
            {
                currentSection = sectionMatch.Groups[1].Value.Trim();
                continue;
            }

            if (!string.Equals(currentSection, "remote \"origin\"", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();
            if (string.Equals(key, "url", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }

        return string.Empty;
    }

    private static string NormalizeGitHubRemote(string remoteUrl)
    {
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            return string.Empty;
        }

        var normalized = remoteUrl.Trim();

        if (normalized.StartsWith("git@github.com:", StringComparison.OrdinalIgnoreCase))
        {
            normalized = "https://github.com/" + normalized["git@github.com:".Length..];
        }

        normalized = normalized.Replace('\\', '/');
        if (normalized.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[..^4];
        }

        return normalized.TrimEnd('/');
    }
}

public sealed record GitRepositoryInfo(
    string RepositoryRoot,
    string GitHubRepositoryUrl,
    string CurrentBranch);
