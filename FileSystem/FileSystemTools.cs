using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FileSystem.Tools;

[McpServerToolType]
public static partial class FileSystemTools
{
    // ファイル処理時のデフォルトエンコーディング
    private static readonly Encoding DefaultEncoding = Encoding.UTF8;

    // 大容量ファイル処理時の最大サイズ（10MB）
    private const long MaxFileSize = 10 * 1024 * 1024;

  [McpServerTool,
        Description("Write File")]
    public static void WriteFile(
        [Description("The path to the file to edit")] string filePath,
        [Description("The content to write to the file")] string content,
        [Description("The encoding to use (utf-8, shift-jis, etc.). Default is utf-8.")] string encodingName = "utf-8")
    {
        try
        {
            Security.ValidateIsAllowedDirectory(filePath);
            
            // エンコーディングの解決
            Encoding encoding = ResolveEncoding(encodingName);
            
            File.WriteAllText(filePath, content, encoding);
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to write file '{filePath}': {ex.Message}", ex);
        }
    }

[McpServerTool,
        Description("Gets file information including path, line count and content.")]
    public static string GetFileInfo(
        [Description("The full path to the file to be read.")] string filePath,
        [Description("The encoding to use (utf-8, shift-jis, etc.). Default is utf-8.")] string encodingName = "utf-8",
        [Description("Whether to include file content in the result. For large files, setting this to false is recommended.")] bool includeContent = true)
    {
        try
        {
            Security.ValidateIsAllowedDirectory(filePath);
            
            var fileInfo = new FileInfo(filePath);
            
            bool isLargeFile = fileInfo.Length > MaxFileSize;
            
            Encoding encoding = ResolveEncoding(encodingName);
            
            var result = new
            {
                FilePath = filePath,
                NewLine = Environment.NewLine,
                FileSize = fileInfo.Length,
                LastModified = fileInfo.LastWriteTime,
                IsLargeFile = isLargeFile,
                ContentIncluded = includeContent && (!isLargeFile),
                LineCount = 0, // 後で設定
                Content = string.Empty // 後で設定（必要な場合）
            };
            
            // パフォーマンス改善: 一度だけファイルを読み込み、必要に応じて行数とコンテンツを設定
            var resultObj = JsonDocument.Parse(JsonSerializer.Serialize(result)).RootElement.Clone();
            var resultDict = System.Text.Json.Nodes.JsonNode.Parse(resultObj.GetRawText()).AsObject();
            
            if (result.ContentIncluded)
            {
                // 効率化: 一度の読み込みで行数とコンテンツ両方を取得
                string content = File.ReadAllText(filePath, encoding);
                int lineCount = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;
                
                resultDict["lineCount"] = lineCount;
                resultDict["content"] = content;
            }
            else
            {
                // コンテンツは含めず、行だけカウント（メモリ効率良く）
                int lineCount = 0;
                using (var reader = new StreamReader(filePath, encoding))
                {
                    while (reader.ReadLine() != null)
                    {
                        lineCount++;
                    }
                }
                
                resultDict["lineCount"] = lineCount;
                resultDict["content"] = null;
            }
            
            return JsonSerializer.Serialize(resultDict, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to get file information for '{filePath}': {ex.Message}", ex);
        }
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


    private static Encoding ResolveEncoding(string encodingName)
    {
        if (string.IsNullOrWhiteSpace(encodingName))
        {
            return DefaultEncoding;
        }

        try
        {
            return Encoding.GetEncoding(encodingName);
        }
        catch (ArgumentException)
        {
            // 不明なエンコーディング名の場合はデフォルトを使用
            return DefaultEncoding;
        }
    }

    #endregion
}