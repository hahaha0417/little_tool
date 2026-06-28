namespace CppDocGenerator.Models;

public sealed class CppDocProject
{
    public string SourceRoot { get; set; } = string.Empty;
    public DateTime GeneratedAtUtc { get; set; }
    public int FileCount { get; set; }
    public int SymbolCount { get; set; }
    public int Parallelism { get; set; }
    public string GitHubRepositoryUrl { get; set; } = string.Empty;
    public string GitReference { get; set; } = string.Empty;
    public List<string> SupportedExtensions { get; set; } = [];
    public CppDocTreeNode Tree { get; set; } = new();
    public List<CppDocFile> Files { get; set; } = [];
}

public sealed class CppDocFile
{
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string DirectoryPath { get; set; } = string.Empty;
    public string SourceFilePath { get; set; } = string.Empty;
    public string GitHubBlobUrl { get; set; } = string.Empty;
    public string GitHubRawUrl { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Language { get; set; } = "C++";
    public int LineCount { get; set; }
    public List<CppDocSymbol> Symbols { get; set; } = [];
}

public sealed class CppDocTreeNode
{
    public string Name { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty;
    public List<CppDocTreeNode> Children { get; set; } = [];
    public CppDocFile? File { get; set; }
}

public sealed class CppDocFolderIndex
{
    public string Name { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string NodeType { get; set; } = "folder";
    public List<CppDocFolderIndexItem> Children { get; set; } = [];
}

public sealed class CppDocFolderIndexItem
{
    public string Name { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string JsonPath { get; set; } = string.Empty;
}

public sealed class CppDocSymbol
{
    public string Kind { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string QualifiedName { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string RawComment { get; set; } = string.Empty;
    public string AccessModifier { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string GitHubUrl { get; set; } = string.Empty;
    public List<CppDocSymbol> Children { get; set; } = [];
}

public sealed class CppDocParseOptions
{
    public string GitHubRepositoryUrl { get; set; } = string.Empty;
    public string GitReference { get; set; } = "main";
    public int Parallelism { get; set; } = Environment.ProcessorCount;
}
