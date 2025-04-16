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
        Description("Edits a file by finding and replacing text patterns using regular expressions or writing new content. Allows for flexible file editing including complete file replacement.")]
    public static void EditFile(
        [Description("The path to the file to edit or create")] string filePath,
        [Description("The regular expression pattern to search for in the file. Use '.*' to match all content for full file replacement.")]
        string pattern,
        [Description("The replacement text. Can include regex capture group references like $1, $2, etc. For full file replacement, simply provide the new content.")]
        string replacement,
        [Description("Optional: Set to true to replace all occurrences of the pattern. Set to false to replace only the first occurrence. Default is true.")]
        bool replaceAll = true)
    {
        Security.ValidateIsAllowedDirectory(filePath);

        // Check if file exists
        bool fileExists = File.Exists(filePath);

        // For full file replacement pattern, create the file if it doesn't exist
        if (pattern == ".*" || pattern == "^.*$" || pattern == "^[\\s\\S]*$")
        {
            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the content directly to the file
            File.WriteAllText(filePath, replacement);
            return;
        }

        // If we're using a specific pattern and the file doesn't exist, throw an error
        if (!fileExists)
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        // Read the existing file content
        string content = File.ReadAllText(filePath);
        string[] originalLines = content.Split(new[] { "\\r\\n", "\\r", "\\n" }, StringSplitOptions.None);

        // Create regex with options for multiline matching
        var regex = new Regex(pattern, RegexOptions.Multiline);
        var matches = regex.Matches(content);

        if (matches.Count > 0)
        {
            // Perform replacement based on the replaceAll parameter
            string newContent;
            if (replaceAll)
            {
                newContent = regex.Replace(content, replacement);
            }
            else
            {
                newContent = regex.Replace(content, replacement, 1);
            }

            // Calculate line information after modification
            string[] newLines = newContent.Split(new[] { "\\r\\n", "\\r", "\\n" }, StringSplitOptions.None);

            // Rough approximation of edit start/end lines
            // Find the first match position and determine line
            if (matches.Count > 0)
            {
                int firstMatchPos = matches[0].Index;
                int lastMatchPos = replaceAll && matches.Count > 1 ?
                    matches[matches.Count - 1].Index + matches[matches.Count - 1].Length :
                    firstMatchPos + matches[0].Length;
            }

            // Write the modified content to the file
            File.WriteAllText(filePath, newContent);
        }
        else
        {
            // If no matching pattern was found, exit without changes
            // Logs or error messages can be added here if needed
        }
    }

    [McpServerTool,
        Description("Gets file information including path, line count, content, and line number guides. Useful when planning line-based edits.")]
    public static string GetFileInfo([Description("The full path to the file to be read.")] string filePath)
    {
        Security.ValidateIsAllowedDirectory(filePath);
        var content = File.ReadAllText(filePath);
        var lines = File.ReadAllLines(filePath);

        var fileInfo = new
        {
            FilePath = filePath,
            Content = content,
            NewLine = DetectNewline(content),
            LineCount = lines.Length,
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