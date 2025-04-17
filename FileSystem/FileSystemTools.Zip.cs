using ModelContextProtocol.Server;
using System.ComponentModel;
using System.IO.Compression;

namespace FileSystem.Tools;
public static partial class FileSystemTools
{
    [McpServerTool, Description("圧縮ファイルを作成します")]
    public static string Zip(
    [Description("圧縮するディレクトリまたはファイルのパス")] string path)
    {
        Security.ValidateIsAllowedDirectory(path);
        try
        {
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                throw new FileNotFoundException($"指定されたパスが見つかりません: {path}");
            }

            string zipFileName = Path.GetFileName(path) + ".zip";
            string zipFilePath = Path.Combine(Path.GetDirectoryName(path), zipFileName);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            if (Directory.Exists(path))
            {
                ZipFile.CreateFromDirectory(path, zipFilePath, CompressionLevel.Optimal, false);
            }
            else
            {
                using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                    {
                        string fileName = Path.GetFileName(path);
                        ZipArchiveEntry entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

                        using (Stream entryStream = entry.Open())
                        using (FileStream fileToCompress = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            fileToCompress.CopyTo(entryStream);
                        }
                    }
                }
            }

            return $"圧縮が完了しました: {zipFilePath}";
        }
        catch (Exception ex)
        {
            return $"圧縮中にエラーが発生しました: {ex.Message}";
        }
    }

    [McpServerTool, Description("圧縮ファイルを展開します")]
    public static string Unzip(
    [Description("展開するZIPファイルのパス")] string filePath)
    {
        Security.ValidateIsAllowedDirectory(filePath);
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"指定されたZIPファイルが見つかりません: {filePath}");
            }

            if (!Path.GetExtension(filePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("ZIPファイルではありません。");
            }

            string extractDir = Path.Combine(
                Path.GetDirectoryName(filePath),
                Path.GetFileNameWithoutExtension(filePath));

            if (Directory.Exists(extractDir))
            {
                Directory.Delete(extractDir, true);
            }

            Directory.CreateDirectory(extractDir);

            ZipFile.ExtractToDirectory(filePath, extractDir);

            return $"展開が完了しました: {extractDir}";
        }
        catch (Exception ex)
        {
            return $"展開中にエラーが発生しました: {ex.Message}";
        }
    }
}

