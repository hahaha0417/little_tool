using System.IO.Enumeration;

namespace ftp_sync;

internal sealed class ExcludeMatcher
{
    private readonly List<string> _patterns;
    private readonly Dictionary<(string Path, bool IsDirectory), bool> _cache = new();

    public ExcludeMatcher(string? rawPatterns)
    {
        _patterns = ParsePatterns(rawPatterns);
    }

    public bool IsExcluded(string relativePath, bool isDirectory)
    {
        var normalizedPath = NormalizePath(relativePath, isDirectory);
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            return false;
        }

        var cacheKey = (normalizedPath, isDirectory);
        if (_cache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        foreach (var pattern in _patterns)
        {
            if (MatchesPattern(normalizedPath, pattern, isDirectory))
            {
                _cache[cacheKey] = true;
                return true;
            }
        }

        _cache[cacheKey] = false;
        return false;
    }

    private static bool MatchesPattern(string normalizedPath, string pattern, bool isDirectory)
    {
        var directoryPattern = pattern.EndsWith('/');
        var corePattern = directoryPattern ? pattern.TrimEnd('/') : pattern;

        if (string.IsNullOrWhiteSpace(corePattern))
        {
            return false;
        }

        if (directoryPattern)
        {
            if (corePattern.Contains('/'))
            {
                return normalizedPath.Equals(corePattern, StringComparison.OrdinalIgnoreCase)
                    || normalizedPath.StartsWith(corePattern + "/", StringComparison.OrdinalIgnoreCase);
            }

            foreach (var segment in normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                if (FileSystemName.MatchesSimpleExpression(corePattern, segment, ignoreCase: true))
                {
                    return true;
                }
            }

            return false;
        }

        if (corePattern.Contains('/'))
        {
            return FileSystemName.MatchesSimpleExpression(corePattern, normalizedPath, ignoreCase: true);
        }

        var fileName = normalizedPath.Split('/').LastOrDefault() ?? normalizedPath;
        if (FileSystemName.MatchesSimpleExpression(corePattern, fileName, ignoreCase: true))
        {
            return true;
        }

        if (isDirectory)
        {
            foreach (var segment in normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                if (FileSystemName.MatchesSimpleExpression(corePattern, segment, ignoreCase: true))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static List<string> ParsePatterns(string? rawPatterns)
    {
        if (string.IsNullOrWhiteSpace(rawPatterns))
        {
            return [];
        }

        return rawPatterns
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split(['\n', ';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizePattern)
            .Where(pattern => !string.IsNullOrWhiteSpace(pattern))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string NormalizePattern(string pattern)
    {
        var normalized = pattern.Replace('\\', '/').Trim();
        while (normalized.Contains("//", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("//", "/", StringComparison.Ordinal);
        }

        return normalized.TrimStart('/');
    }

    private static string NormalizePath(string relativePath, bool isDirectory)
    {
        var normalized = relativePath.Replace('\\', '/').Trim().Trim('/');
        while (normalized.Contains("//", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("//", "/", StringComparison.Ordinal);
        }

        if (isDirectory && !string.IsNullOrWhiteSpace(normalized))
        {
            return normalized.TrimEnd('/');
        }

        return normalized;
    }
}
