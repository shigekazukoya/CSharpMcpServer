using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FileSystem.Tools;

/// <summary>
/// ファイルシステム関連の操作を提供するツールクラス
/// </summary>
[McpServerToolType]
public static partial class FileSystemTools
{
    [McpServerTool,
         Description(
         "Edits a file by replacing or inserting content at specific line positions. For full file replacement, use lineNumber=1 and set linesToDelete to the total number of lines in the file.")]
    public static void EditFile(
        [Description("The path to the file to edit")] string filePath,
        [Description("The 1-based line number where the edit should start. Use 1 to start from the beginning of the file.")]
        int lineNumber,
        [Description("The number of lines to delete from the start line. Set to the total line count for complete file replacement, or 0 for pure insertion without deletion.")]
        int linesToDelete,
        [Description("The text content to insert at the specified position. For full file replacement, provide the entire new content.")]
        string content)
    {
        Security.ValidateIsAllowedDirectory(filePath);

        var lines = File.ReadAllLines(filePath);

        var newLinesList = new List<string>();
        newLinesList.AddRange(lines.Take(lineNumber - 1));
        newLinesList.AddRange(content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None));
        newLinesList.AddRange(lines.Skip(lineNumber - 1 + linesToDelete));

        File.WriteAllLines(filePath, newLinesList);
    }

    [McpServerTool, Description("Gets file information including path, line count, and content.")]
    public static string GetFileInfo([Description("The full path to the file to be read.")] string filePath)
    {
        Security.ValidateIsAllowedDirectory(filePath);

        var fileInfo = new
        {
            FilePath = filePath,
            LineCount = File.ReadAllLines(filePath).Length,
            Content = File.ReadAllText(filePath)
        };

        return JsonSerializer.Serialize(fileInfo, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Retrieves the hierarchical folder structure in YAML format from a specified directory path.")]
    public static string GetFolderStructure(
        [Description("Absolute path to the root directory whose folder structure should be retrieved.")] string fullPath,
        [Description("Specifies whether to include subdirectories recursively in the folder structure. If set to true, the function will traverse through all nested directories. If false, only the immediate children of the root directory will be included.")] bool recursive = true)
    {
        Security.ValidateIsAllowedDirectory(fullPath);

        var ignorePatterns = GitIgnoreParser.LoadIgnorePatterns(fullPath);
        var sb = new StringBuilder();

        var rootName = Path.GetFileName(fullPath);
        sb.AppendLine($"{rootName}:");

        TraverseDirectoryYaml(fullPath, sb, "  ", ignorePatterns, fullPath, recursive);

        return sb.ToString();
    }

    [McpServerTool, Description("Deletes a file or directory from the file system.")]
    public static void Delete(
        [Description("The full path of the file or directory to delete.")] string fullPath,
        [Description("Whether to delete all contents inside a directory. Ignored for files. Default is false.")] bool recursive = false)
    {
        Security.ValidateIsAllowedDirectory(fullPath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        else if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive);
        }
        else
        {
            throw new FileNotFoundException($"No file or directory found at path: {fullPath}");
        }
    }

    #region Private Methods

    private static void TraverseDirectoryYaml(
        string path,
        StringBuilder sb,
        string indent,
        List<Regex> ignorePatterns,
        string rootPath,
        bool recursive)
    {
        // Get filtered files and directories
        var (filteredFiles, filteredDirs) = GetFilteredItems(path, ignorePatterns, rootPath);

        // Output files
        foreach (var file in filteredFiles)
        {
            sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
        }

        // Output and process directories
        foreach (var dir in filteredDirs)
        {
            var dirName = Path.GetFileName(dir);
            sb.AppendLine($"{indent}{dirName}:");

            if (!recursive) continue;

            // Handle .gitignore in subdirectory
            var childIgnorePatterns = new List<Regex>(ignorePatterns);
            string gitignorePath = Path.Combine(dir, ".gitignore");
            if (File.Exists(gitignorePath))
            {
                childIgnorePatterns.AddRange(GitIgnoreParser.ParseGitIgnore(gitignorePath, dir, rootPath));
            }

            TraverseDirectoryYaml(
                dir,
                sb,
                indent + "  ",
                childIgnorePatterns,
                rootPath,
                recursive
            );
        }
    }

    private static (string[] files, string[] dirs) GetFilteredItems(string path, List<Regex> ignorePatterns, string rootPath)
    {
        string relativePath = GetNormalizedRelativePath(path, rootPath);
        if (path != rootPath && GitIgnoreParser.IsIgnored(relativePath, ignorePatterns))
        {
            return (Array.Empty<string>(), Array.Empty<string>());
        }

        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        var filteredFiles = files
            .Where(file => !GitIgnoreParser.IsIgnored(GetNormalizedRelativePath(file, rootPath), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !GitIgnoreParser.IsIgnored(GetNormalizedRelativePath(dir, rootPath), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();

        return (filteredFiles, filteredDirs);
    }

    private static string GetNormalizedRelativePath(string path, string rootPath)
    {
        return Path.GetRelativePath(rootPath, path).Replace("\\", "/");
    }

    #endregion
}
