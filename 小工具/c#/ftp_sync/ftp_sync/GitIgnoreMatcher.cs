using System.Text.RegularExpressions;

namespace ftp_sync;

internal sealed class GitIgnoreMatcher
{
    private readonly List<GitIgnoreRule> _rules;
    private readonly Dictionary<(string Path, bool IsDirectory), bool> _cache = new();

    private GitIgnoreMatcher(List<GitIgnoreRule> rules)
    {
        _rules = rules;
    }

    public bool HasNegatedRules => _rules.Any(rule => rule.Negated);

    public static GitIgnoreMatcher Load(string rootPath)
    {
        var normalizedRoot = Path.GetFullPath(rootPath);
        var rules = new List<GitIgnoreRule>();

        foreach (var gitIgnorePath in Directory.EnumerateFiles(normalizedRoot, ".gitignore", SearchOption.AllDirectories))
        {
            var gitIgnoreDirectory = Path.GetDirectoryName(gitIgnorePath) ?? normalizedRoot;
            var baseRelativePath = NormalizeRelative(Path.GetRelativePath(normalizedRoot, gitIgnoreDirectory));
            var lines = File.ReadAllLines(gitIgnorePath);

            foreach (var line in lines)
            {
                var rule = GitIgnoreRule.TryCreate(baseRelativePath, line);
                if (rule is not null)
                {
                    rules.Add(rule);
                }
            }
        }

        return new GitIgnoreMatcher(rules);
    }

    public bool IsIgnored(string relativePath, bool isDirectory)
    {
        var normalizedPath = NormalizeRelative(relativePath);
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            return false;
        }

        var cacheKey = (normalizedPath, isDirectory);
        if (_cache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        var ignored = false;
        foreach (var rule in _rules)
        {
            if (rule.IsMatch(normalizedPath, isDirectory))
            {
                ignored = !rule.Negated;
            }
        }

        _cache[cacheKey] = ignored;
        return ignored;
    }

    private static string NormalizeRelative(string path)
    {
        var normalized = path.Replace('\\', '/').Trim().Trim('/');
        while (normalized.Contains("//", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("//", "/", StringComparison.Ordinal);
        }

        return normalized;
    }
}

internal sealed class GitIgnoreRule
{
    private readonly Regex _regex;
    private readonly bool _directoryOnly;

    private GitIgnoreRule(bool negated, bool directoryOnly, Regex regex)
    {
        Negated = negated;
        _directoryOnly = directoryOnly;
        _regex = regex;
    }

    public bool Negated { get; }

    public static GitIgnoreRule? TryCreate(string baseRelativePath, string rawLine)
    {
        var line = rawLine.Trim();
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
        {
            return null;
        }

        var negated = line.StartsWith('!');
        if (negated)
        {
            line = line[1..];
        }

        line = line.Replace('\\', '/').Trim();
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        var directoryOnly = line.EndsWith('/');
        var anchored = line.StartsWith('/');
        var hasSlash = line.Contains('/');

        var core = line.Trim('/');
        if (string.IsNullOrWhiteSpace(core))
        {
            return null;
        }

        var effectivePrefix = baseRelativePath;
        string fullPattern;

        if (!string.IsNullOrWhiteSpace(effectivePrefix))
        {
            if (!anchored && !hasSlash)
            {
                fullPattern = string.IsNullOrWhiteSpace(effectivePrefix)
                    ? $"**/{core}"
                    : $"{effectivePrefix}/**/{core}";
            }
            else
            {
                fullPattern = $"{effectivePrefix}/{core}";
            }
        }
        else if (!anchored && !hasSlash)
        {
            fullPattern = $"**/{core}";
        }
        else
        {
            fullPattern = core;
        }

        var regex = new Regex(BuildRegex(fullPattern, directoryOnly), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return new GitIgnoreRule(negated, directoryOnly, regex);
    }

    public bool IsMatch(string relativePath, bool isDirectory)
    {
        if (_directoryOnly)
        {
            return _regex.IsMatch(relativePath) || (!isDirectory && _regex.IsMatch(GetDirectoryPath(relativePath)));
        }

        return _regex.IsMatch(relativePath);
    }

    private static string GetDirectoryPath(string relativePath)
    {
        var index = relativePath.LastIndexOf('/');
        return index <= 0 ? string.Empty : relativePath[..index];
    }

    private static string BuildRegex(string pattern, bool directoryOnly)
    {
        var normalized = pattern.Replace("//", "/", StringComparison.Ordinal).Trim('/');
        var regex = Regex.Escape(normalized)
            .Replace(@"\*\*", "<<<TWOSTAR>>>", StringComparison.Ordinal)
            .Replace(@"\*", "[^/]*", StringComparison.Ordinal)
            .Replace(@"\?", "[^/]", StringComparison.Ordinal)
            .Replace("<<<TWOSTAR>>>", ".*", StringComparison.Ordinal);

        if (directoryOnly)
        {
            return $"^(?:{regex})(?:/.*)?$";
        }

        return $"^{regex}$";
    }
}
