using System.Text.Json;
using CppDocGenerator.Models;

namespace CppDocGenerator.Services;

public sealed class JsonExportService
{
    public const string FolderIndexFileName = "index.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string Serialize(CppDocProject project)
    {
        return JsonSerializer.Serialize(project, SerializerOptions);
    }

    public string SaveAsFolder(CppDocProject project, string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);
        CleanupGeneratedFiles(outputFolder);
        SaveFolderRecursive(project.Tree, outputFolder);

        var rootIndex = BuildFolderIndex(project.Tree);
        var rootIndexPath = Path.Combine(outputFolder, FolderIndexFileName);
        File.WriteAllText(rootIndexPath, JsonSerializer.Serialize(rootIndex, SerializerOptions));
        return JsonSerializer.Serialize(rootIndex, SerializerOptions);
    }

    private static void CleanupGeneratedFiles(string outputFolder)
    {
        foreach (var jsonFile in Directory.EnumerateFiles(outputFolder, "*.json", SearchOption.AllDirectories))
        {
            File.Delete(jsonFile);
        }

        foreach (var htmlFile in Directory.EnumerateFiles(outputFolder, HtmlExportService.HtmlFileName, SearchOption.AllDirectories))
        {
            File.Delete(htmlFile);
        }

        DeleteEmptyDirectories(outputFolder);
    }

    private static void DeleteEmptyDirectories(string rootFolder)
    {
        foreach (var directory in Directory.EnumerateDirectories(rootFolder, "*", SearchOption.AllDirectories)
                     .OrderByDescending(path => path.Length))
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory, false);
            }
        }
    }

    private static void SaveFolderRecursive(CppDocTreeNode node, string currentOutputFolder)
    {
        var folderIndex = BuildFolderIndex(node);
        File.WriteAllText(
            Path.Combine(currentOutputFolder, FolderIndexFileName),
            JsonSerializer.Serialize(folderIndex, SerializerOptions));

        foreach (var child in node.Children)
        {
            if (child.NodeType == "folder")
            {
                var childFolder = Path.Combine(currentOutputFolder, child.Name);
                Directory.CreateDirectory(childFolder);
                SaveFolderRecursive(child, childFolder);
                continue;
            }

            if (child.File is null)
            {
                continue;
            }

            var fileJsonPath = Path.Combine(currentOutputFolder, $"{child.Name}.json");
            File.WriteAllText(fileJsonPath, JsonSerializer.Serialize(child.File, SerializerOptions));
        }
    }

    private static CppDocFolderIndex BuildFolderIndex(CppDocTreeNode node)
    {
        return new CppDocFolderIndex
        {
            Name = node.Name,
            RelativePath = node.RelativePath,
            Children = node.Children.Select(BuildFolderIndexItem).ToList()
        };
    }

    private static CppDocFolderIndexItem BuildFolderIndexItem(CppDocTreeNode node)
    {
        return new CppDocFolderIndexItem
        {
            Name = node.Name,
            NodeType = node.NodeType,
            RelativePath = node.RelativePath,
            JsonPath = node.NodeType == "folder"
                ? NormalizeJsonPath(Path.Combine(node.RelativePath == "." ? string.Empty : node.RelativePath, FolderIndexFileName))
                : NormalizeJsonPath($"{node.RelativePath}.json")
        };
    }

    private static string NormalizeJsonPath(string path)
    {
        return path.Replace('\\', '/');
    }
}
