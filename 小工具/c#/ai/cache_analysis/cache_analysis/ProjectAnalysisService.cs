using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace cache_analysis
{
    public sealed class ProjectAnalysisService
    {
        private static readonly string[] IgnoreDirectories =
        [
            "bin",
            "obj",
            ".vs",
            ".git",
            ".idea",
            ".vscode",
            "packages"
        ];

        private static readonly string[] IndexedFileExtensions =
        [
            ".sln",
            ".slnx",
            ".csproj",
            ".props",
            ".targets",
            ".config",
            ".json",
            ".cs",
            ".resx"
        ];

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public AnalysisResult Generate(string projectPath, string outputPath, int maxDegreeOfParallelism)
        {
            var root = new DirectoryInfo(projectPath);
            var normalizedParallelism = Math.Max(1, maxDegreeOfParallelism);

            if (!root.Exists)
            {
                throw new DirectoryNotFoundException($"找不到專案資料夾: {projectPath}");
            }

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new InvalidOperationException("輸出檔路徑無效。");
            }

            Directory.CreateDirectory(outputDirectory);

            var cacheRoot = BuildCacheRoot(outputPath);
            var fileCacheRoot = Path.Combine(cacheRoot, "files");
            var treeCacheRoot = Path.Combine(cacheRoot, "tree");
            Directory.CreateDirectory(cacheRoot);
            Directory.CreateDirectory(fileCacheRoot);
            Directory.CreateDirectory(treeCacheRoot);

            var statePath = Path.Combine(cacheRoot, "state.json");
            var existingState = LoadState(statePath);
            var files = EnumerateFiles(root);
            var snapshot = BuildSnapshot(root, files);
            var nextEntries = BuildFileEntries(root, files, fileCacheRoot, existingState.Entries, normalizedParallelism);
            var nextState = new CacheState(snapshot, nextEntries.Items.ToDictionary(x => x.RelativePath, StringComparer.OrdinalIgnoreCase));

            RemoveDeletedCacheFiles(fileCacheRoot, existingState.Entries, nextState.Entries);
            WriteTreeIndexes(root, cacheRoot, treeCacheRoot, nextEntries.Items);
            File.WriteAllText(statePath, JsonSerializer.Serialize(nextState, JsonOptions), new UTF8Encoding(false));

            var markdown = BuildMainMarkdown(root, outputPath, cacheRoot, snapshot, nextEntries, normalizedParallelism);
            File.WriteAllText(outputPath, markdown, new UTF8Encoding(false));

            return new AnalysisResult(outputPath, markdown, nextEntries.CreatedCount, nextEntries.ReusedCount, normalizedParallelism);
        }

        private static List<FileInfo> EnumerateFiles(DirectoryInfo root)
        {
            return root.EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(file => !IsIgnored(file))
                .Where(file => IndexedFileExtensions.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
                .OrderBy(file => file.FullName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static bool IsIgnored(FileInfo file)
        {
            for (var current = file.Directory; current is not null; current = current.Parent)
            {
                if (IgnoreDirectories.Contains(current.Name, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string BuildCacheRoot(string outputPath)
        {
            return Path.Combine(
                Path.GetDirectoryName(outputPath) ?? throw new InvalidOperationException("無法建立快取路徑。"),
                $"{Path.GetFileNameWithoutExtension(outputPath)}.cache");
        }

        private static CacheState LoadState(string statePath)
        {
            if (!File.Exists(statePath))
            {
                return new CacheState("", new Dictionary<string, FileCacheEntry>(StringComparer.OrdinalIgnoreCase));
            }

            try
            {
                var state = JsonSerializer.Deserialize<CacheState>(File.ReadAllText(statePath));
                return state ?? new CacheState("", new Dictionary<string, FileCacheEntry>(StringComparer.OrdinalIgnoreCase));
            }
            catch
            {
                return new CacheState("", new Dictionary<string, FileCacheEntry>(StringComparer.OrdinalIgnoreCase));
            }
        }

        private static string BuildSnapshot(DirectoryInfo root, List<FileInfo> files)
        {
            var summary = string.Join('\n', files.Select(file =>
            {
                var relativePath = GetRelativePath(root, file.FullName);
                return $"{relativePath}|{file.Length}|{file.LastWriteTimeUtc.Ticks}";
            }));

            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(summary)));
        }

        private static BuildEntriesResult BuildFileEntries(
            DirectoryInfo root,
            List<FileInfo> files,
            string fileCacheRoot,
            IReadOnlyDictionary<string, FileCacheEntry> existingEntries,
            int maxDegreeOfParallelism)
        {
            var items = new ConcurrentBag<FileCacheEntry>();
            var createdCount = 0;
            var reusedCount = 0;
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            Parallel.ForEach(files, parallelOptions, file =>
            {
                var relativePath = GetRelativePath(root, file.FullName);
                var createDetailCache = ShouldCreateDetailCache(file);
                var cacheFilePath = createDetailCache ? BuildPerFileCachePath(fileCacheRoot, relativePath) : null;

                if (existingEntries.TryGetValue(relativePath, out var cached)
                    && cached.LastWriteUtcTicks == file.LastWriteTimeUtc.Ticks
                    && cached.Length == file.Length
                    && (!createDetailCache || File.Exists(cacheFilePath)))
                {
                    items.Add(cached with
                    {
                        CacheFileRelativePath = createDetailCache && cacheFilePath is not null
                            ? NormalizeCachePath(cacheFilePath, fileCacheRoot)
                            : string.Empty
                    });
                    Interlocked.Increment(ref reusedCount);
                    return;
                }

                var entry = BuildFileEntry(root, file, cacheFilePath, fileCacheRoot, createDetailCache);
                items.Add(entry);
                Interlocked.Increment(ref createdCount);
            });

            return new BuildEntriesResult(
                items.OrderBy(x => x.RelativePath, StringComparer.OrdinalIgnoreCase).ToList(),
                createdCount,
                reusedCount);
        }

        private static FileCacheEntry BuildFileEntry(
            DirectoryInfo root,
            FileInfo file,
            string? cacheFilePath,
            string fileCacheRoot,
            bool createDetailCache)
        {
            var relativePath = GetRelativePath(root, file.FullName);
            var summary = BuildFileSummary(root, file, createDetailCache);

            if (createDetailCache && cacheFilePath is not null && !string.IsNullOrWhiteSpace(summary.Markdown))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath)!);
                File.WriteAllText(cacheFilePath, summary.Markdown, new UTF8Encoding(false));
            }

            return new FileCacheEntry(
                relativePath,
                file.Length,
                file.LastWriteTimeUtc.Ticks,
                summary.Sha12,
                string.Join("|", summary.Namespaces),
                summary.TypeSummary,
                createDetailCache && cacheFilePath is not null ? NormalizeCachePath(cacheFilePath, fileCacheRoot) : string.Empty,
                summary.Kind,
                summary.MethodSummary);
        }

        private static FileSummary BuildFileSummary(DirectoryInfo root, FileInfo file, bool createDetailCache)
        {
            var relativePath = GetRelativePath(root, file.FullName);
            var kind = DetectKind(file);
            var needsContent = createDetailCache || string.Equals(file.Extension, ".csproj", StringComparison.OrdinalIgnoreCase);

            if (!needsContent)
            {
                return new FileSummary(BuildMetadataSha12(relativePath, file), [], "-", kind, string.Empty, "-");
            }

            var content = File.ReadAllText(file.FullName);
            var sha12 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content))).Substring(0, 12);
            var builder = new StringBuilder();
            builder.AppendLine($"# {relativePath}");
            builder.AppendLine();
            builder.AppendLine("## 快取說明");
            builder.AppendLine("- 這份檔案是單檔分析快取，不是原始碼。");
            builder.AppendLine("- 先看這份快取，再決定是否需要開啟真正的 `.cs` 檔案。");
            builder.AppendLine("- 只有在需要方法內容、精確邏輯或行級修改時，才回頭讀原始碼。");
            builder.AppendLine();
            builder.AppendLine($"- Kind: {kind}");
            builder.AppendLine($"- Sha12: {sha12}");
            builder.AppendLine($"- Size: {file.Length}");
            builder.AppendLine($"- LastWriteUtc: {file.LastWriteTimeUtc:O}");
            builder.AppendLine();

            if (string.Equals(file.Extension, ".csproj", StringComparison.OrdinalIgnoreCase))
            {
                AppendProjectDetail(builder, root, file, content);
                return new FileSummary(sha12, [], "project", kind, builder.ToString().TrimEnd() + Environment.NewLine, "-");
            }

            if (string.Equals(file.Extension, ".cs", StringComparison.OrdinalIgnoreCase))
            {
                var namespaces = Regex.Matches(content, @"\bnamespace\s+([A-Za-z0-9_.]+)")
                    .Select(match => match.Groups[1].Value)
                    .Distinct()
                    .Take(5)
                    .ToList();
                var typeMatches = Regex.Matches(content, @"\b(class|interface|record|struct|enum)\s+([A-Za-z_][A-Za-z0-9_]*)")
                    .Select(match => $"{match.Groups[1].Value} {match.Groups[2].Value}")
                    .Distinct()
                    .Take(16)
                    .ToList();
                var methodMatches = Regex.Matches(content, @"\b(?:public|private|protected|internal)\s+(?:static\s+)?(?:async\s+)?(?:[A-Za-z0-9_<>\[\],?.]+\s+)+([A-Za-z_][A-Za-z0-9_]*)\s*\(")
                    .Select(match => match.Groups[1].Value)
                    .Distinct()
                    .Take(20)
                    .ToList();

                builder.AppendLine("## Namespaces");
                builder.AppendLine(namespaces.Count == 0 ? "- None" : string.Join(Environment.NewLine, namespaces.Select(x => $"- {x}")));
                builder.AppendLine();
                builder.AppendLine("## Types");
                builder.AppendLine(typeMatches.Count == 0 ? "- None" : string.Join(Environment.NewLine, typeMatches.Select(x => $"- {x}")));
                builder.AppendLine();
                builder.AppendLine("## Methods");
                builder.AppendLine(methodMatches.Count == 0 ? "- None" : string.Join(Environment.NewLine, methodMatches.Select(x => $"- {x}")));
                builder.AppendLine();
                builder.AppendLine("## Note");
                builder.AppendLine(BuildHeuristicNote(file.Name, typeMatches));

                return new FileSummary(
                    sha12,
                    namespaces,
                    typeMatches.Count == 0 ? "-" : string.Join(", ", typeMatches.Take(4)),
                    kind,
                    builder.ToString().TrimEnd() + Environment.NewLine,
                    methodMatches.Count == 0 ? "-" : string.Join(", ", methodMatches.Take(6)));
            }

            builder.AppendLine("## Preview");
            builder.AppendLine(BuildShortPreview(content));
            return new FileSummary(sha12, [], kind, kind, builder.ToString().TrimEnd() + Environment.NewLine, "-");
        }

        private static void AppendProjectDetail(StringBuilder builder, DirectoryInfo root, FileInfo projectFile, string content)
        {
            var document = XDocument.Parse(content);
            var targetFramework = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "TargetFramework")?.Value
                ?? document.Descendants().FirstOrDefault(x => x.Name.LocalName == "TargetFrameworks")?.Value
                ?? "?";
            var packageRefs = document.Descendants().Where(x => x.Name.LocalName == "PackageReference")
                .Select(x => $"{x.Attribute("Include")?.Value ?? "(unknown)"}:{x.Attribute("Version")?.Value ?? x.Element(x.Name.Namespace + "Version")?.Value ?? "?"}")
                .Take(20)
                .ToList();
            var projectRefs = document.Descendants().Where(x => x.Name.LocalName == "ProjectReference")
                .Select(x => x.Attribute("Include")?.Value ?? "(unknown)")
                .Take(20)
                .ToList();

            builder.AppendLine("## Project");
            builder.AppendLine($"- TargetFramework: `{targetFramework}`");
            builder.AppendLine($"- PackageRefs: {(packageRefs.Count == 0 ? "none" : string.Join(", ", packageRefs))}");
            builder.AppendLine($"- ProjectRefs: {(projectRefs.Count == 0 ? "none" : string.Join(", ", projectRefs.Select(path => NormalizeProjectReference(root, projectFile, path))))}");
        }

        private static void WriteTreeIndexes(
            DirectoryInfo root,
            string cacheRoot,
            string treeCacheRoot,
            IReadOnlyList<FileCacheEntry> entries)
        {
            foreach (var oldFile in Directory.EnumerateFiles(treeCacheRoot, "*.md", SearchOption.AllDirectories))
            {
                File.Delete(oldFile);
            }

            var groups = entries
                .GroupBy(entry => Path.GetDirectoryName(entry.RelativePath)?.Replace('\\', '/') ?? ".")
                .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                var relativeDir = group.Key;
                var dirFilePath = relativeDir == "."
                    ? Path.Combine(treeCacheRoot, "index.md")
                    : Path.Combine(treeCacheRoot, relativeDir.Replace('/', Path.DirectorySeparatorChar), "index.md");

                Directory.CreateDirectory(Path.GetDirectoryName(dirFilePath)!);

                var childDirectories = entries
                    .Select(entry => Path.GetDirectoryName(entry.RelativePath)?.Replace('\\', '/') ?? ".")
                    .Where(path => IsDirectChildDirectory(relativeDir, path))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var builder = new StringBuilder();
                builder.AppendLine($"# Tree: {relativeDir}");
                builder.AppendLine();
                builder.AppendLine("## 快取說明");
                builder.AppendLine("- 這份樹狀快取是資料夾導覽層，不是單檔詳細內容。");
                builder.AppendLine("- 先用它縮小到相關目錄，再決定要開哪些單檔快取。");
                builder.AppendLine("- 只有路徑與目前任務相關時，才去開啟 `files/...`。");
                builder.AppendLine();
                builder.AppendLine($"- RootRelativeDir: `{relativeDir}`");
                builder.AppendLine($"- CacheRoot: `{GetRelativeDisplayPath(root, cacheRoot)}`");
                builder.AppendLine();
                builder.AppendLine("## ChildDirectories");
                builder.AppendLine(childDirectories.Count == 0
                    ? "- None"
                    : string.Join(Environment.NewLine, childDirectories.Select(path => $"- `{path}` | cache=`{BuildTreeCacheReference(path)}`")));
                builder.AppendLine();
                builder.AppendLine("## Files");
                builder.AppendLine("| Path | Kind | Types | Methods | Cache |");
                builder.AppendLine("| --- | --- | --- | --- | --- |");

                foreach (var entry in group.OrderBy(x => x.RelativePath, StringComparer.OrdinalIgnoreCase))
                {
                    builder.AppendLine($"| `{entry.RelativePath}` | `{entry.Kind}` | {EscapeTableValue(entry.TypeSummary)} | {EscapeTableValue(entry.MethodSummary)} | {BuildCacheCell(entry)} |");
                }

                File.WriteAllText(dirFilePath, builder.ToString().TrimEnd() + Environment.NewLine, new UTF8Encoding(false));
            }
        }

        private static string BuildMainMarkdown(
            DirectoryInfo root,
            string outputPath,
            string cacheRoot,
            string snapshot,
            BuildEntriesResult entries,
            int maxDegreeOfParallelism)
        {
            var solutionEntries = entries.Items.Where(x => x.RelativePath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase) || x.RelativePath.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase)).ToList();
            var projectEntries = entries.Items.Where(x => x.RelativePath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)).ToList();
            var sourceEntries = entries.Items.Where(x => x.RelativePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)).ToList();
            var builder = new StringBuilder();

            builder.AppendLine("# Low Token Analysis Cache");
            builder.AppendLine();
            builder.AppendLine($"- GeneratedAt: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
            builder.AppendLine("- RootPath: `.`");
            builder.AppendLine($"- OutputFile: `{GetRelativeDisplayPath(root, outputPath)}`");
            builder.AppendLine($"- CacheDir: `{GetRelativeDisplayPath(root, cacheRoot)}`");
            builder.AppendLine($"- TreeRoot: `{GetRelativeDisplayPath(root, Path.Combine(cacheRoot, "tree", "index.md"))}`");
            builder.AppendLine($"- Snapshot: `{snapshot[..12]}`");
            builder.AppendLine($"- MaxDegreeOfParallelism: {maxDegreeOfParallelism}");
            builder.AppendLine($"- RebuiltFiles: {entries.CreatedCount}");
            builder.AppendLine($"- ReusedFiles: {entries.ReusedCount}");
            builder.AppendLine($"- Solutions: {solutionEntries.Count}");
            builder.AppendLine($"- Projects: {projectEntries.Count}");
            builder.AppendLine($"- SourceFiles: {sourceEntries.Count}");
            builder.AppendLine();

            builder.AppendLine("## Agent Usage");
            builder.AppendLine("- 先讀這份主索引。");
            builder.AppendLine("- 先用 `analysis-cache.cache/tree/` 縮小到相關資料夾。");
            builder.AppendLine("- 每個 `*.cs` 都有自己的快取檔，放在 `analysis-cache.cache/files/`。");
            builder.AppendLine("- 只有單檔快取資訊不足時，才回頭讀原始碼。");
            builder.AppendLine();

            builder.AppendLine("## 快取說明");
            builder.AppendLine("- `analysis-cache.md` 是整個專案的主索引。");
            builder.AppendLine("- `tree/.../index.md` 是資料夾導覽層。");
            builder.AppendLine("- `files/.../*.md` 是單檔詳細快取層。");
            builder.AppendLine("- 這套快取中的路徑一律使用相對專案目錄表示。");
            builder.AppendLine("- 優先讀快取，只有在需要精確實作細節時才回頭讀原始碼。");
            builder.AppendLine();

            builder.AppendLine("## Solutions");
            builder.AppendLine(solutionEntries.Count == 0
                ? "- None"
                : string.Join(Environment.NewLine, solutionEntries.Select(entry => $"- `{entry.RelativePath}`")));
            builder.AppendLine();

            builder.AppendLine("## Tree Index");
            builder.AppendLine("| Directory | Files | Tree Cache |");
            builder.AppendLine("| --- | ---: | --- |");
            foreach (var item in BuildDirectoryIndex(entries.Items))
            {
                builder.AppendLine(item);
            }

            builder.AppendLine();
            builder.AppendLine("## CSharp File Index");
            builder.AppendLine("| Path | Kind | Namespace | Types | Methods | Cache |");
            builder.AppendLine("| --- | --- | --- | --- | --- | --- |");
            foreach (var entry in sourceEntries)
            {
                builder.AppendLine($"| `{entry.RelativePath}` | `{entry.Kind}` | {EscapeTableValue(entry.Namespaces)} | {EscapeTableValue(entry.TypeSummary)} | {EscapeTableValue(entry.MethodSummary)} | {BuildCacheCell(entry)} |");
            }

            builder.AppendLine();
            builder.AppendLine("## Project Index");
            builder.AppendLine("| Path | Kind | Cache |");
            builder.AppendLine("| --- | --- | --- |");
            foreach (var entry in projectEntries)
            {
                builder.AppendLine($"| `{entry.RelativePath}` | `{entry.Kind}` | {BuildCacheCell(entry)} |");
            }

            return builder.ToString().TrimEnd() + Environment.NewLine;
        }

        private static IEnumerable<string> BuildDirectoryIndex(IEnumerable<FileCacheEntry> entries)
        {
            return entries
                .GroupBy(entry => Path.GetDirectoryName(entry.RelativePath)?.Replace('\\', '/') ?? ".")
                .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
                .Select(group => $"| `{group.Key}` | {group.Count()} | `{BuildTreeCacheReference(group.Key)}` |");
        }

        private static string BuildPerFileCachePath(string fileCacheRoot, string relativePath)
        {
            var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(fileCacheRoot, Path.ChangeExtension(normalized, ".md"));
        }

        private static string NormalizeCachePath(string cacheFilePath, string fileCacheRoot)
        {
            return Path.GetRelativePath(fileCacheRoot, cacheFilePath).Replace('\\', '/');
        }

        private static void RemoveDeletedCacheFiles(
            string fileCacheRoot,
            IReadOnlyDictionary<string, FileCacheEntry> previousEntries,
            IReadOnlyDictionary<string, FileCacheEntry> nextEntries)
        {
            foreach (var item in previousEntries)
            {
                if (nextEntries.ContainsKey(item.Key) || string.IsNullOrWhiteSpace(item.Value.CacheFileRelativePath))
                {
                    continue;
                }

                var fullPath = Path.Combine(fileCacheRoot, item.Value.CacheFileRelativePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        private static string DetectKind(FileInfo file)
        {
            if (string.Equals(file.Extension, ".csproj", StringComparison.OrdinalIgnoreCase))
            {
                return "project";
            }

            if (string.Equals(file.Extension, ".resx", StringComparison.OrdinalIgnoreCase) && IsFormRelated(file))
            {
                return "form-resx";
            }

            if (string.Equals(file.Extension, ".cs", StringComparison.OrdinalIgnoreCase))
            {
                if (file.Name.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase))
                {
                    return "designer";
                }

                if (file.Name.Equals("Program.cs", StringComparison.OrdinalIgnoreCase))
                {
                    return "entry";
                }

                if (IsFormRelated(file))
                {
                    return "form";
                }

                return "source";
            }

            return file.Extension.TrimStart('.').ToLowerInvariant();
        }

        private static bool ShouldCreateDetailCache(FileInfo file)
        {
            if (string.Equals(file.Extension, ".csproj", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return string.Equals(file.Extension, ".cs", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFormRelated(FileInfo file)
        {
            return file.Name.StartsWith("Form", StringComparison.OrdinalIgnoreCase)
                || file.Name.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)
                || file.Name.EndsWith(".resx", StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildMetadataSha12(string relativePath, FileInfo file)
        {
            var raw = $"{relativePath}|{file.Length}|{file.LastWriteTimeUtc.Ticks}";
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw))).Substring(0, 12);
        }

        private static string BuildHeuristicNote(string fileName, IReadOnlyCollection<string> typeMatches)
        {
            if (fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase))
            {
                return "Generated WinForms designer file. Load only when UI control layout or event wiring matters.";
            }

            if (fileName.Equals("Program.cs", StringComparison.OrdinalIgnoreCase))
            {
                return "Application entry point. Usually needed for startup flow only.";
            }

            if (typeMatches.Any(type => type.Contains("Form ", StringComparison.OrdinalIgnoreCase)))
            {
                return "Likely WinForms screen logic. Load when investigating UI behavior.";
            }

            return "General source file. Read this cache first before opening the original file.";
        }

        private static string BuildShortPreview(string content)
        {
            var normalized = Regex.Replace(content, @"\s+", " ").Trim();
            return string.IsNullOrEmpty(normalized)
                ? "- Empty"
                : $"- {normalized[..Math.Min(normalized.Length, 240)]}";
        }

        private static string GetRelativePath(DirectoryInfo root, string fullPath)
        {
            return Path.GetRelativePath(root.FullName, fullPath).Replace('\\', '/');
        }

        private static string GetRelativeDisplayPath(DirectoryInfo root, string fullPath)
        {
            return Path.GetRelativePath(root.FullName, fullPath).Replace('\\', '/');
        }

        private static string NormalizeProjectReference(DirectoryInfo root, FileInfo projectFile, string projectReferencePath)
        {
            if (string.IsNullOrWhiteSpace(projectReferencePath))
            {
                return "(unknown)";
            }

            var baseDirectory = projectFile.Directory?.FullName ?? root.FullName;
            var combined = Path.GetFullPath(Path.Combine(baseDirectory, projectReferencePath));
            return GetRelativePath(root, combined);
        }

        private static bool IsDirectChildDirectory(string parent, string candidate)
        {
            if (string.Equals(parent, candidate, StringComparison.OrdinalIgnoreCase) || candidate == ".")
            {
                return false;
            }

            if (parent == ".")
            {
                return !candidate.Contains('/');
            }

            if (!candidate.StartsWith(parent + "/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return candidate[(parent.Length + 1)..].IndexOf('/') < 0;
        }

        private static string BuildTreeCacheReference(string relativeDirectory)
        {
            return relativeDirectory == "."
                ? "tree/index.md"
                : $"tree/{relativeDirectory}/index.md";
        }

        private static string BuildCacheCell(FileCacheEntry entry)
        {
            return string.IsNullOrWhiteSpace(entry.CacheFileRelativePath)
                ? "-"
                : $"`files/{entry.CacheFileRelativePath}`";
        }

        private static string EscapeTableValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            return $"`{value.Replace("|", "/")}`";
        }
    }

    public sealed record AnalysisResult(string OutputPath, string Markdown, int RebuiltFiles, int ReusedFiles, int MaxDegreeOfParallelism);

    public sealed record CacheState(string Snapshot, Dictionary<string, FileCacheEntry> Entries);

    public sealed record FileCacheEntry(
        string RelativePath,
        long Length,
        long LastWriteUtcTicks,
        string Sha12,
        string Namespaces,
        string TypeSummary,
        string CacheFileRelativePath,
        string Kind,
        string MethodSummary);

    public sealed record BuildEntriesResult(List<FileCacheEntry> Items, int CreatedCount, int ReusedCount);

    public sealed record FileSummary(
        string Sha12,
        IReadOnlyList<string> Namespaces,
        string TypeSummary,
        string Kind,
        string Markdown,
        string MethodSummary);
}
