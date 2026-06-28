using System.Text;
using System.Text.RegularExpressions;
using CppDocGenerator.Models;

namespace CppDocGenerator.Services;

public sealed class CppDocParser
{
    private static readonly string[] SupportedExtensions =
    [
        ".h", ".hpp", ".hh", ".hxx", ".h++",
        ".c", ".cc", ".cp", ".cpp", ".cxx", ".c++",
        ".ipp", ".ixx", ".inl", ".tpp", ".txx", ".inc", ".def", ".cppm"
    ];
    private static readonly Regex NamespaceRegex = new(@"^\s*namespace\s+([A-Za-z_]\w*(?:::\w+)*)\s*\{?", RegexOptions.Compiled);
    private static readonly Regex ClassRegex = new(@"^\s*(class|struct)\s+([A-Za-z_]\w*)[^;{]*\{?", RegexOptions.Compiled);
    private static readonly Regex EnumRegex = new(@"^\s*enum(?:\s+class)?\s+([A-Za-z_]\w*)[^;{]*\{?", RegexOptions.Compiled);
    private static readonly Regex FunctionRegex = new(@"^\s*([~A-Za-z_][\w:<>\s\*&,:]*)\s+([A-Za-z_~]\w*)\s*\(([^;{}()]*)\)\s*(?:const)?\s*(?:noexcept)?\s*(?:=\s*0)?\s*(?:override)?\s*(?:final)?\s*(?:;|\{)", RegexOptions.Compiled);

    public CppDocProject ParseDirectory(string sourceRoot, CppDocParseOptions? options = null)
    {
        options ??= new CppDocParseOptions();
        var root = Path.GetFullPath(sourceRoot);
        var files = Directory
            .EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
            .Where(path => SupportedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var project = new CppDocProject
        {
            SourceRoot = root,
            GeneratedAtUtc = DateTime.UtcNow,
            FileCount = files.Count,
            Parallelism = Math.Max(1, options.Parallelism),
            GitHubRepositoryUrl = NormalizeGitHubRepoUrl(options.GitHubRepositoryUrl),
            GitReference = string.IsNullOrWhiteSpace(options.GitReference) ? "main" : options.GitReference.Trim(),
            SupportedExtensions = SupportedExtensions.ToList()
        };

        var parsedFiles = new CppDocFile[files.Count];
        Parallel.ForEach(
            Enumerable.Range(0, files.Count),
            new ParallelOptions { MaxDegreeOfParallelism = project.Parallelism },
            index =>
            {
                parsedFiles[index] = ParseFile(root, files[index], project.GitHubRepositoryUrl, project.GitReference);
            });

        project.Files.AddRange(parsedFiles);

        project.SymbolCount = CountSymbols(project.Files.SelectMany(file => file.Symbols));
        project.Tree = BuildTree(root, project.Files);
        return project;
    }

    private static CppDocFile ParseFile(string root, string filePath, string gitHubRepositoryUrl, string gitReference)
    {
        var lines = File.ReadAllLines(filePath);
        var relativePath = NormalizePath(Path.GetRelativePath(root, filePath));
        var gitHubBlobUrl = BuildGitHubBlobUrl(gitHubRepositoryUrl, gitReference, relativePath);
        var symbols = new List<CppDocSymbol>();
        var scopeStack = new Stack<ScopeItem>();
        var pendingComment = new StringBuilder();
        var accessModifier = "private";
        var braceDepth = 0;
        var inBlockComment = false;

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var trimmed = line.Trim();

            if (TryCaptureComment(trimmed, pendingComment, ref inBlockComment))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            if (trimmed is "public:" or "protected:" or "private:")
            {
                accessModifier = trimmed.TrimEnd(':');
                continue;
            }

            var currentComment = ConsumeComment(pendingComment);

            var namespaceMatch = NamespaceRegex.Match(trimmed);
            if (namespaceMatch.Success)
            {
                var symbol = CreateSymbol("namespace", namespaceMatch.Groups[1].Value, trimmed, currentComment, "public", index + 1, scopeStack, gitHubBlobUrl);
                AddSymbol(symbols, scopeStack, symbol);
                PushScopeIfNeeded(scopeStack, symbol, braceDepth, trimmed.Contains('{'));
            }
            else
            {
                var classMatch = ClassRegex.Match(trimmed);
                if (classMatch.Success)
                {
                    accessModifier = classMatch.Groups[1].Value == "struct" ? "public" : "private";
                    var kind = classMatch.Groups[1].Value;
                    var symbol = CreateSymbol(kind, classMatch.Groups[2].Value, trimmed, currentComment, accessModifier, index + 1, scopeStack, gitHubBlobUrl);
                    AddSymbol(symbols, scopeStack, symbol);
                    PushScopeIfNeeded(scopeStack, symbol, braceDepth, trimmed.Contains('{'));
                }
                else
                {
                    var enumMatch = EnumRegex.Match(trimmed);
                    if (enumMatch.Success)
                    {
                        var symbol = CreateSymbol("enum", enumMatch.Groups[1].Value, trimmed, currentComment, accessModifier, index + 1, scopeStack, gitHubBlobUrl);
                        AddSymbol(symbols, scopeStack, symbol);
                        PushScopeIfNeeded(scopeStack, symbol, braceDepth, trimmed.Contains('{'));
                    }
                    else if (LooksLikeFunction(trimmed))
                    {
                        var functionMatch = FunctionRegex.Match(trimmed);
                        if (functionMatch.Success)
                        {
                            var symbol = CreateSymbol("function", functionMatch.Groups[2].Value, trimmed, currentComment, accessModifier, index + 1, scopeStack, gitHubBlobUrl);
                            AddSymbol(symbols, scopeStack, symbol);
                        }
                    }
                }
            }

            braceDepth += CountChar(trimmed, '{');
            braceDepth -= CountChar(trimmed, '}');
            PopClosedScopes(scopeStack, braceDepth);
        }

        return new CppDocFile
        {
            FileName = Path.GetFileName(filePath),
            RelativePath = relativePath,
            DirectoryPath = NormalizePath(Path.GetDirectoryName(relativePath) ?? string.Empty),
            SourceFilePath = NormalizePath(filePath),
            GitHubBlobUrl = gitHubBlobUrl,
            GitHubRawUrl = BuildGitHubRawUrl(gitHubRepositoryUrl, gitReference, relativePath),
            Extension = Path.GetExtension(filePath),
            Symbols = symbols,
            LineCount = lines.Length
        };
    }

    private static CppDocTreeNode BuildTree(string root, List<CppDocFile> files)
    {
        var rootNode = new CppDocTreeNode
        {
            Name = Path.GetFileName(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)),
            RelativePath = ".",
            NodeType = "folder"
        };

        foreach (var file in files.OrderBy(item => item.RelativePath, StringComparer.OrdinalIgnoreCase))
        {
            var currentNode = rootNode;
            var folderParts = file.DirectoryPath
                .Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);
            var currentPath = new List<string>();

            foreach (var part in folderParts)
            {
                currentPath.Add(part);
                var relativePath = NormalizePath(Path.Combine(currentPath.ToArray()));
                var nextNode = currentNode.Children.FirstOrDefault(child =>
                    child.NodeType == "folder" &&
                    string.Equals(child.Name, part, StringComparison.OrdinalIgnoreCase));

                if (nextNode is null)
                {
                    nextNode = new CppDocTreeNode
                    {
                        Name = part,
                        RelativePath = relativePath,
                        NodeType = "folder"
                    };

                    currentNode.Children.Add(nextNode);
                }

                currentNode = nextNode;
            }

            currentNode.Children.Add(new CppDocTreeNode
            {
                Name = file.FileName,
                RelativePath = NormalizePath(file.RelativePath),
                NodeType = "file",
                File = file
            });
        }

        SortTree(rootNode);
        return rootNode;
    }

    private static void SortTree(CppDocTreeNode node)
    {
        node.Children = node.Children
            .OrderByDescending(child => child.NodeType == "folder")
            .ThenBy(child => child.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var child in node.Children.Where(child => child.NodeType == "folder"))
        {
            SortTree(child);
        }
    }

    private static bool TryCaptureComment(string trimmed, StringBuilder pendingComment, ref bool inBlockComment)
    {
        if (inBlockComment)
        {
            pendingComment.AppendLine(CleanBlockCommentLine(trimmed));
            if (trimmed.Contains("*/", StringComparison.Ordinal))
            {
                inBlockComment = false;
            }

            return true;
        }

        if (trimmed.StartsWith("///", StringComparison.Ordinal))
        {
            pendingComment.AppendLine(trimmed[3..].Trim());
            return true;
        }

        if (trimmed.StartsWith("/**", StringComparison.Ordinal) || trimmed.StartsWith("/*", StringComparison.Ordinal))
        {
            inBlockComment = !trimmed.Contains("*/", StringComparison.Ordinal);
            pendingComment.AppendLine(CleanBlockCommentLine(trimmed));
            return true;
        }

        if (trimmed.StartsWith("//", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static string CleanBlockCommentLine(string line)
    {
        return line
            .Replace("/**", string.Empty, StringComparison.Ordinal)
            .Replace("/*", string.Empty, StringComparison.Ordinal)
            .Replace("*/", string.Empty, StringComparison.Ordinal)
            .Trim()
            .TrimStart('*')
            .Trim();
    }

    private static bool LooksLikeFunction(string line)
    {
        if (!line.Contains('(') || !line.Contains(')'))
        {
            return false;
        }

        return !(line.StartsWith("if", StringComparison.Ordinal) ||
                 line.StartsWith("for", StringComparison.Ordinal) ||
                 line.StartsWith("while", StringComparison.Ordinal) ||
                 line.StartsWith("switch", StringComparison.Ordinal) ||
                 line.StartsWith("catch", StringComparison.Ordinal));
    }

    private static CppDocSymbol CreateSymbol(string kind, string name, string signature, string rawComment, string accessModifier, int lineNumber, Stack<ScopeItem> scopeStack, string gitHubBlobUrl)
    {
        var qualifiedParts = scopeStack
            .Reverse()
            .Select(item => item.Symbol.Name)
            .Concat([name]);

        return new CppDocSymbol
        {
            Kind = kind,
            Name = name,
            QualifiedName = string.Join("::", qualifiedParts),
            Signature = signature,
            Summary = BuildSummary(rawComment),
            RawComment = rawComment,
            AccessModifier = accessModifier,
            LineNumber = lineNumber,
            GitHubUrl = BuildGitHubLineUrl(gitHubBlobUrl, lineNumber)
        };
    }

    private static string BuildSummary(string rawComment)
    {
        if (string.IsNullOrWhiteSpace(rawComment))
        {
            return string.Empty;
        }

        return rawComment
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault() ?? string.Empty;
    }

    private static void AddSymbol(List<CppDocSymbol> rootSymbols, Stack<ScopeItem> scopeStack, CppDocSymbol symbol)
    {
        if (scopeStack.Count == 0)
        {
            rootSymbols.Add(symbol);
            return;
        }

        scopeStack.Peek().Symbol.Children.Add(symbol);
    }

    private static void PushScopeIfNeeded(Stack<ScopeItem> scopeStack, CppDocSymbol symbol, int braceDepth, bool hasInlineOpenBrace)
    {
        if (!hasInlineOpenBrace)
        {
            return;
        }

        scopeStack.Push(new ScopeItem(symbol, braceDepth + 1));
    }

    private static void PopClosedScopes(Stack<ScopeItem> scopeStack, int braceDepth)
    {
        while (scopeStack.Count > 0 && braceDepth < scopeStack.Peek().Depth)
        {
            scopeStack.Pop();
        }
    }

    private static string ConsumeComment(StringBuilder builder)
    {
        var text = builder.ToString().Trim();
        builder.Clear();
        return text;
    }

    private static int CountSymbols(IEnumerable<CppDocSymbol> symbols)
    {
        var total = 0;
        foreach (var symbol in symbols)
        {
            total++;
            total += CountSymbols(symbol.Children);
        }

        return total;
    }

    private static int CountChar(string text, char value)
    {
        var count = 0;
        foreach (var ch in text)
        {
            if (ch == value)
            {
                count++;
            }
        }

        return count;
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }

    private static string NormalizeGitHubRepoUrl(string url)
    {
        var normalized = url.Trim().TrimEnd('/');
        if (normalized.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[..^4];
        }

        return normalized;
    }

    private static string BuildGitHubBlobUrl(string repositoryUrl, string gitReference, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(repositoryUrl))
        {
            return string.Empty;
        }

        return $"{repositoryUrl}/blob/{Uri.EscapeDataString(gitReference).Replace("%2F", "/")}/{NormalizePath(relativePath)}";
    }

    private static string BuildGitHubRawUrl(string repositoryUrl, string gitReference, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(repositoryUrl))
        {
            return string.Empty;
        }

        var rawBaseUrl = repositoryUrl
            .Replace("https://github.com/", "https://raw.githubusercontent.com/", StringComparison.OrdinalIgnoreCase);

        return $"{rawBaseUrl}/{Uri.EscapeDataString(gitReference).Replace("%2F", "/")}/{NormalizePath(relativePath)}";
    }

    private static string BuildGitHubLineUrl(string gitHubBlobUrl, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(gitHubBlobUrl))
        {
            return string.Empty;
        }

        return $"{gitHubBlobUrl}#L{lineNumber}";
    }

    private sealed record ScopeItem(CppDocSymbol Symbol, int Depth);
}
