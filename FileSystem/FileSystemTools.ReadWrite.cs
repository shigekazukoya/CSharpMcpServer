using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

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
    public static string WriteFile(
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
            return "Success";
        }
        catch (Exception ex)
        {
            return "Failed";
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