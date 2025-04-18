using FileSystem.Common;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace FileSystem.Tools;

[McpServerToolType]
public static partial class FileSystemTools
{
    /// <summary>
    /// ファイルの内容を書き込みます
    /// </summary>
    /// <param name="filePath">書き込み先ファイルパス</param>
    /// <param name="content">書き込む内容</param>
    /// <param name="encodingName">エンコーディング名（デフォルト: utf-8）</param>
    /// <returns>処理結果</returns>
    [McpServerTool, Description("Write File")]
    public static async Task<string> WriteFileAsync(
        [Description("The path to the file to edit")] string filePath,
        [Description("The content to write to the file")] string content,
        [Description("The encoding to use (utf-8, shift-jis, etc.). Default is utf-8.")] string encodingName = "utf-8")
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(filePath);
            
            if (!Security.HasWritePermission(filePath))
            {
                return $"書き込み権限がありません: {filePath}";
            }
            
            // エンコーディングの解決
            Encoding encoding = ResolveEncoding(encodingName);
            
            // 親ディレクトリが存在しない場合は作成
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 非同期で書き込み
            await File.WriteAllTextAsync(filePath, content, encoding);
            
            return JsonSerializer.Serialize(new
            {
                Status = "Success",
                FilePath = filePath,
                Message = $"ファイルを正常に書き込みました: {filePath}",
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "ファイル書き込み");
        }
    }

    /// <summary>
    /// ファイル情報を取得します
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    /// <param name="encodingName">エンコーディング名（デフォルト: utf-8）</param>
    /// <param name="includeContent">ファイル内容を含めるかどうか</param>
    /// <returns>ファイル情報（JSON形式）</returns>
    [McpServerTool, Description("Gets file information including path, line count and content.")]
    public static async Task<string> GetFileInfoAsync(
        [Description("The full path to the file to be read.")] string filePath,
        [Description("The encoding to use (utf-8, shift-jis, etc.). Default is utf-8.")] string encodingName = "utf-8",
        [Description("Whether to include file content in the result. For large files, setting this to false is recommended.")] bool includeContent = true)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(filePath);
            
            if (!File.Exists(filePath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"ファイルが見つかりません: {filePath}"
                });
            }
            
            if (!Security.HasReadPermission(filePath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"読み取り権限がありません: {filePath}"
                });
            }

            // ファイル情報の取得
            var fileInfo = new FileInfo(filePath);
            bool isLargeFile = fileInfo.Length > Constants.MaxFileSize;
            Encoding encoding = ResolveEncoding(encodingName);
            
            // 基本情報を構築
            var resultDict = new Dictionary<string, object>
            {
                ["status"] = "Success",
                ["filePath"] = filePath,
                ["fileName"] = Path.GetFileName(filePath),
                ["extension"] = Path.GetExtension(filePath),
                ["directoryName"] = Path.GetDirectoryName(filePath),
                ["newLine"] = Environment.NewLine,
                ["fileSize"] = fileInfo.Length,
                ["fileSizeFormatted"] = FormatFileSize(fileInfo.Length),
                ["lastModified"] = fileInfo.LastWriteTime,
                ["created"] = fileInfo.CreationTime,
                ["isLargeFile"] = isLargeFile,
                ["contentIncluded"] = includeContent && (!isLargeFile),
                ["isReadOnly"] = fileInfo.IsReadOnly,
                ["encoding"] = encodingName
            };

            // 内容と行数の取得
            if (resultDict["contentIncluded"].Equals(true))
            {
                // 小さいファイルの場合：一括読み込み
                string content = await File.ReadAllTextAsync(filePath, encoding);
                int lineCount = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;
                
                resultDict["lineCount"] = lineCount;
                resultDict["content"] = content;
            }
            else
            {
                // 大きいファイルまたは内容が不要な場合：行数のみカウント
                int lineCount = 0;
                using (var reader = new StreamReader(filePath, encoding))
                {
                    while (await reader.ReadLineAsync() != null)
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
            return ExceptionHandling.FormatExceptionAsJson(ex, "ファイル情報取得");
        }
    }

    /// <summary>
    /// ファイルやディレクトリを削除します
    /// </summary>
    /// <param name="fullPath">削除するファイルまたはディレクトリのパス</param>
    /// <param name="recursive">ディレクトリ内の内容も削除するかどうか</param>
    /// <returns>処理結果</returns>
    [McpServerTool, Description("Deletes a file or directory from the file system.")]
    public static string Delete(
        [Description("The full path of the file or directory to delete.")] string fullPath,
        [Description("Whether to delete all contents inside a directory. Ignored for files. Default is false.")] bool recursive = false)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(fullPath);

            // パスの存在確認
            bool isFile = File.Exists(fullPath);
            bool isDirectory = Directory.Exists(fullPath);
            
            if (!isFile && !isDirectory)
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"指定されたパスが見つかりません: {fullPath}"
                });
            }

            // 書き込み権限の確認
            if (!Security.HasWritePermission(fullPath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"削除権限がありません: {fullPath}"
                });
            }

            // 削除実行
            if (isFile)
            {
                File.Delete(fullPath);
                return JsonSerializer.Serialize(new
                {
                    Status = "Success",
                    Message = $"ファイルを削除しました: {fullPath}"
                });
            }
            else
            {
                Directory.Delete(fullPath, recursive);
                return JsonSerializer.Serialize(new
                {
                    Status = "Success",
                    Message = $"ディレクトリを削除しました: {fullPath}",
                    Recursive = recursive
                });
            }
        }
        catch (IOException ex) when (ex.Message.Contains("directory is not empty"))
        {
            // ディレクトリが空でない場合の特別なハンドリング
            return JsonSerializer.Serialize(new
            {
                Status = "Error",
                Message = $"ディレクトリが空ではありません。recursive=true を指定してください: {fullPath}"
            });
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "削除");
        }
    }

    /// <summary>
    /// ファイルをコピーします
    /// </summary>
    /// <param name="sourcePath">コピー元ファイルパス</param>
    /// <param name="destinationPath">コピー先ファイルパス</param>
    /// <param name="overwrite">既存ファイルを上書きするかどうか</param>
    /// <returns>処理結果</returns>
    [McpServerTool, Description("Copies a file to a new location.")]
    public static async Task<string> CopyFileAsync(
        [Description("The path of the file to copy.")] string sourcePath,
        [Description("The path to copy the file to.")] string destinationPath,
        [Description("Whether to overwrite an existing file. Default is false.")] bool overwrite = false)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(sourcePath);
            Security.ValidateIsAllowedDirectory(destinationPath);
            
            if (!File.Exists(sourcePath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"コピー元ファイルが見つかりません: {sourcePath}"
                });
            }
            
            if (File.Exists(destinationPath) && !overwrite)
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"コピー先ファイルは既に存在します。上書きする場合は overwrite=true を指定してください: {destinationPath}"
                });
            }
            
            // 読み取り/書き込み権限の確認
            if (!Security.HasReadPermission(sourcePath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"コピー元ファイルの読み取り権限がありません: {sourcePath}"
                });
            }
            
            if (!Security.HasWritePermission(destinationPath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"コピー先への書き込み権限がありません: {destinationPath}"
                });
            }

            // コピー先ディレクトリの作成（存在しない場合）
            string destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // 非同期コピー実装
            using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (var destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await sourceStream.CopyToAsync(destStream);
            }
            
            return JsonSerializer.Serialize(new
            {
                Status = "Success",
                Message = $"ファイルをコピーしました: {sourcePath} -> {destinationPath}"
            });
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "ファイルコピー");
        }
    }

    /// <summary>
    /// ファイルまたはディレクトリを移動します
    /// </summary>
    /// <param name="sourcePath">移動元パス</param>
    /// <param name="destinationPath">移動先パス</param>
    /// <param name="overwrite">既存ファイルを上書きするかどうか</param>
    /// <returns>処理結果</returns>
    [McpServerTool, Description("Moves a file or directory to a new location.")]
    public static string Move(
        [Description("The path of the file or directory to move.")] string sourcePath,
        [Description("The path to move the file or directory to.")] string destinationPath,
        [Description("Whether to overwrite an existing file. Ignored for directories. Default is false.")] bool overwrite = false)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(sourcePath);
            Security.ValidateIsAllowedDirectory(destinationPath);
            
            bool isFile = File.Exists(sourcePath);
            bool isDirectory = Directory.Exists(sourcePath);
            
            if (!isFile && !isDirectory)
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"移動元が見つかりません: {sourcePath}"
                });
            }
            
            // 権限チェック
            if (!Security.HasReadPermission(sourcePath) || !Security.HasWritePermission(sourcePath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"移動元のアクセス権限がありません: {sourcePath}"
                });
            }
            
            if (!Security.HasWritePermission(destinationPath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"移動先への書き込み権限がありません: {destinationPath}"
                });
            }

            // 移動先ディレクトリの作成（必要な場合）
            if (isFile)
            {
                string destDir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                
                // ファイルの移動
                File.Move(sourcePath, destinationPath, overwrite);
                return JsonSerializer.Serialize(new
                {
                    Status = "Success",
                    Message = $"ファイルを移動しました: {sourcePath} -> {destinationPath}"
                });
            }
            else
            {
                // ディレクトリの移動
                Directory.Move(sourcePath, destinationPath);
                return JsonSerializer.Serialize(new
                {
                    Status = "Success",
                    Message = $"ディレクトリを移動しました: {sourcePath} -> {destinationPath}"
                });
            }
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "移動");
        }
    }
    
    /// <summary>
    /// 指定したディレクトリ内のファイル一覧を取得します
    /// </summary>
    /// <param name="directoryPath">ディレクトリパス</param>
    /// <param name="searchPattern">検索パターン（デフォルト: *.*）</param>
    /// <param name="recursive">サブディレクトリも検索するかどうか</param>
    /// <returns>ファイル一覧（JSON形式）</returns>
    [McpServerTool, Description("Lists files in a directory.")]
    public static string ListFiles(
        [Description("The directory path to list files from.")] string directoryPath,
        [Description("The search pattern to use (e.g., '*.txt'). Default is '*.*'.")] string searchPattern = "*.*",
        [Description("Whether to search subdirectories. Default is false.")] bool recursive = false)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(directoryPath);
            
            if (!Directory.Exists(directoryPath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"ディレクトリが見つかりません: {directoryPath}"
                });
            }
            
            if (!Security.HasReadPermission(directoryPath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"読み取り権限がありません: {directoryPath}"
                });
            }

            // ファイル一覧の取得
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(directoryPath, searchPattern, searchOption)
                                .Select(file => new
                                {
                                    FullPath = file,
                                    FileName = Path.GetFileName(file),
                                    Extension = Path.GetExtension(file),
                                    Size = new FileInfo(file).Length,
                                    SizeFormatted = FormatFileSize(new FileInfo(file).Length),
                                    LastModified = new FileInfo(file).LastWriteTime,
                                    IsReadOnly = new FileInfo(file).IsReadOnly
                                })
                                .OrderBy(f => f.FileName)
                                .ToList();

            return JsonSerializer.Serialize(new
            {
                Status = "Success",
                DirectoryPath = directoryPath,
                SearchPattern = searchPattern,
                Recursive = recursive,
                FileCount = files.Count,
                Files = files
            }, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "ファイル一覧取得");
        }
    }

    /// <summary>
    /// ファイルが存在するか確認します
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    /// <returns>存在確認結果（JSON形式）</returns>
    [McpServerTool, Description("Checks if a file exists.")]
    public static string FileExists(
        [Description("The path of the file to check.")] string filePath)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(filePath);
            
            bool exists = File.Exists(filePath);
            
            return JsonSerializer.Serialize(new
            {
                Status = "Success",
                FilePath = filePath,
                Exists = exists,
                Message = exists ? $"ファイルが存在します: {filePath}" : $"ファイルが存在しません: {filePath}"
            });
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "ファイル存在確認");
        }
    }

    /// <summary>
    /// ディレクトリが存在するか確認します
    /// </summary>
    /// <param name="directoryPath">ディレクトリパス</param>
    /// <returns>存在確認結果（JSON形式）</returns>
    [McpServerTool, Description("Checks if a directory exists.")]
    public static string DirectoryExists(
        [Description("The path of the directory to check.")] string directoryPath)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(directoryPath);
            
            bool exists = Directory.Exists(directoryPath);
            
            return JsonSerializer.Serialize(new
            {
                Status = "Success",
                DirectoryPath = directoryPath,
                Exists = exists,
                Message = exists ? $"ディレクトリが存在します: {directoryPath}" : $"ディレクトリが存在しません: {directoryPath}"
            });
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "ディレクトリ存在確認");
        }
    }

    /// <summary>
    /// ディレクトリを作成します
    /// </summary>
    /// <param name="directoryPath">作成するディレクトリパス</param>
    /// <returns>処理結果（JSON形式）</returns>
    [McpServerTool, Description("Creates a directory.")]
    public static string CreateDirectory(
        [Description("The path of the directory to create.")] string directoryPath)
    {
        try
        {
            // セキュリティチェック
            Security.ValidateIsAllowedDirectory(directoryPath);
            
            if (Directory.Exists(directoryPath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Success",
                    DirectoryPath = directoryPath,
                    Message = $"ディレクトリは既に存在します: {directoryPath}"
                });
            }
            
            if (!Security.HasWritePermission(directoryPath))
            {
                return JsonSerializer.Serialize(new
                {
                    Status = "Error",
                    Message = $"書き込み権限がありません: {directoryPath}"
                });
            }

            // ディレクトリ作成
            Directory.CreateDirectory(directoryPath);
            
            return JsonSerializer.Serialize(new
            {
                Status = "Success",
                DirectoryPath = directoryPath,
                Message = $"ディレクトリを作成しました: {directoryPath}"
            });
        }
        catch (Exception ex)
        {
            return ExceptionHandling.FormatExceptionAsJson(ex, "ディレクトリ作成");
        }
    }

    #region Private Methods

    /// <summary>
    /// エンコーディング名からEncodingオブジェクトを取得します
    /// </summary>
    private static Encoding ResolveEncoding(string encodingName)
    {
        if (string.IsNullOrWhiteSpace(encodingName))
        {
            return Constants.DefaultEncoding;
        }

        try
        {
            return Encoding.GetEncoding(encodingName);
        }
        catch (ArgumentException)
        {
            // 不明なエンコーディング名の場合はデフォルトを使用
            return Constants.DefaultEncoding;
        }
    }

    /// <summary>
    /// ファイルサイズを人間が読みやすい形式にフォーマットします
    /// </summary>
    private static string FormatFileSize(long bytes)
    {
        string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB" };
        
        if (bytes == 0)
            return "0 B";
        
        double formattedSize = bytes;
        int suffixIndex = 0;
        
        while (formattedSize >= 1024 && suffixIndex < sizeSuffixes.Length - 1)
        {
            formattedSize /= 1024;
            suffixIndex++;
        }
        
        return $"{formattedSize:0.##} {sizeSuffixes[suffixIndex]}";
    }

    #endregion
}