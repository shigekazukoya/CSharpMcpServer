using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FileSystem.Tools;

[McpServerToolType]
public static partial class FileSystemTools
{
    [McpServerTool,
         Description(
         "Edits a file by replacing or inserting content at specific line positions. For full file replacement, use lineNumber=1 and set linesToDelete to the total number of lines in the file.")]
    public static void EditFile(
        [Description("The path to the file to edit")] string path,
        [Description("The 1-based line number where the edit should start. Use 1 to start from the beginning of the file.")]
        int lineNumber,
        [Description("The number of lines to delete from the start line. Set to the total line count for complete file replacement, or 0 for pure insertion without deletion.")]
        int linesToDelete,
        [Description("The text content to insert at the specified position. For full file replacement, provide the entire new content.")]
        string content)
    {
        Security.ValidateIsAllowedDirectory(path);

        var lines = File.ReadAllLines(path);
        
        var newLinesList = new List<string>();
        newLinesList.AddRange(lines.Take(lineNumber - 1));
        newLinesList.AddRange(content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None));
        newLinesList.AddRange(lines.Skip(lineNumber - 1 + linesToDelete));
        
        File.WriteAllLines(path, newLinesList.ToArray());
    }

    [McpServerTool, Description("Gets file information including path, line count, and content.")]
    public static string GetFileInfo([Description("The full path to the file to be read.")] string filePath)
    {
        Security.ValidateIsAllowedDirectory(filePath);

        return JsonSerializer.Serialize(new FileInfo(filePath), new JsonSerializerOptions { WriteIndented = true });
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
        [Description("The full path of the file or directory to delete.")] string path,
        [Description("Whether to delete all contents inside a directory. Ignored for files. Default is false.")] bool recursive = false)
    {
        Security.ValidateIsAllowedDirectory(path);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive);
        }
        else
        {
            throw new FileNotFoundException($"No file or directory found at path: {path}");
        }
    }

    #region private methods

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

    // Helper method to extract common functionality
    private static (string[] files, string[] dirs) GetFilteredItems(string path, List<Regex> ignorePatterns, string rootPath)
    {
        // Check if the current directory should be ignored
        string relativePath = Path.GetRelativePath(rootPath, path).Replace("\\", "/");
        if (path != rootPath && GitIgnoreParser.IsIgnored(relativePath, ignorePatterns))
            return (Array.Empty<string>(), Array.Empty<string>());

        // Get and filter files and directories
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        var filteredFiles = files
            .Where(file => !GitIgnoreParser.IsIgnored(Path.GetRelativePath(rootPath, file).Replace("\\", "/"), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !GitIgnoreParser.IsIgnored(Path.GetRelativePath(rootPath, dir).Replace("\\", "/"), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();

        return (filteredFiles, filteredDirs);
    }

    #endregion
}
