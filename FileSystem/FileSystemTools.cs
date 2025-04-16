using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FileSystem.Tools;

[McpServerToolType]
public static partial class FileSystemTools
{
    private class EditFileResult
    {
        public string FilePath { get; set; } = "";
        public int OriginalLineCount { get; set; }
        public int NewLineCount { get; set; }
        public int LineDifference { get; set; }
        public int EditStartLine { get; set; }
        public int EditEndLine { get; set; }
        public string Message { get; set; } = "";
    }

    [McpServerTool,
        Description("Edits a file by deleting a specified number of lines starting from a specific line position and then inserting new content at that position. For full file replacement, use lineNumber=1 and set linesToDelete to the total number of lines in the file. When executing EditFile multiple times, be aware that line numbers change after each edit. The result object provides information about the new line count and other edit statistics.")]
    public static string EditFile(
        [Description("The path to the file to edit")] string filePath,
        [Description("The 1-based line number where the edit should start. Use 1 to start from the beginning of the file.")] 
        int lineNumber,
        [Description("The number of lines to delete starting from the line specified by lineNumber. For complete file replacement, set lineNumber=1 and set this parameter to the total number of lines in the file. Set to 0 for insertion without deleting any existing lines.")] 
        int linesToDelete,
        [Description("The text content to insert at the specified position after the deletion operation. For full file replacement, provide the entire new content.")] 
        string content)
    {
        Security.ValidateIsAllowedDirectory(filePath);

        var result = new EditFileResult
        {
            FilePath = filePath,
            EditStartLine = lineNumber,
        };

        var lines = File.ReadAllLines(filePath);
        result.OriginalLineCount = lines.Length;

        var newLinesList = new List<string>();
        newLinesList.AddRange(lines.Take(lineNumber - 1));
        
        var contentLines = content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        var addLineLength = contentLines.Length;
        newLinesList.AddRange(contentLines);
        
        newLinesList.AddRange(lines.Skip(lineNumber - 1 + linesToDelete));

        File.WriteAllLines(filePath, newLinesList);
        
        result.NewLineCount = newLinesList.Count;
        result.LineDifference = result.NewLineCount - result.OriginalLineCount;
        result.EditEndLine = lineNumber + addLineLength - 1;
        result.Message = $"File edited. Deleted {linesToDelete} lines and added {addLineLength} lines. Line count change: {result.LineDifference} lines.";

        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, 
        Description("Gets file information including path, line count, content, and line number guides. Useful when planning line-based edits.")]
    public static string GetFileInfo([Description("The full path to the file to be read.")] string filePath)
    {
        Security.ValidateIsAllowedDirectory(filePath);
        var content = File.ReadAllText(filePath);
        var lines = File.ReadAllLines(filePath);

        // Generate line number guide (display up to 10 lines)
        var lineGuide = new List<object>();
        int maxLinesToShow = Math.Min(lines.Length, 10);
        for (int i = 0; i < maxLinesToShow; i++)
        {
            lineGuide.Add(new { 
                LineNumber = i + 1, 
                Content = lines[i].Length > 50 ? lines[i].Substring(0, 47) + "..." : lines[i]
            });
        }

        var fileInfo = new
        {
            FilePath = filePath,
            Content = content,
            NewLine = DetectNewline(content),
            LineCount = lines.Length,
            ContentPreview = content.Length > 200 ? content.Substring(0, 197) + "..." : content,
            LineGuide = lineGuide,
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

        foreach (var file in filteredFiles)
        {
            sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
        }

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

    private static string DetectNewline(string text)
    {
        // Detect newline code: \r\n > \n > \r
        if (text.Contains("\r\n"))
            return "CRLF (\\r\\n)";
        else if (text.Contains("\n"))
            return "LF (\\n)";
        else if (text.Contains("\r"))
            return "CR (\\r)";
        else
            return "No newline detected";
    }

    #endregion
}
