using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace FileSystem.Tools;

[McpServerToolType]
public static partial class FileSystemTools
{
    // �t�@�C���������̃f�t�H���g�G���R�[�f�B���O
    private static readonly Encoding DefaultEncoding = Encoding.UTF8;

    // ��e�ʃt�@�C���������̍ő�T�C�Y�i10MB�j
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
            
            // �G���R�[�f�B���O�̉���
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
                LineCount = 0, // ��Őݒ�
                Content = string.Empty // ��Őݒ�i�K�v�ȏꍇ�j
            };
            
            // �p�t�H�[�}���X���P: ��x�����t�@�C����ǂݍ��݁A�K�v�ɉ����čs���ƃR���e���c��ݒ�
            var resultObj = JsonDocument.Parse(JsonSerializer.Serialize(result)).RootElement.Clone();
            var resultDict = System.Text.Json.Nodes.JsonNode.Parse(resultObj.GetRawText()).AsObject();
            
            if (result.ContentIncluded)
            {
                // ������: ��x�̓ǂݍ��݂ōs���ƃR���e���c�������擾
                string content = File.ReadAllText(filePath, encoding);
                int lineCount = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;
                
                resultDict["lineCount"] = lineCount;
                resultDict["content"] = content;
            }
            else
            {
                // �R���e���c�͊܂߂��A�s�����J�E���g�i�����������ǂ��j
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
            // �s���ȃG���R�[�f�B���O���̏ꍇ�̓f�t�H���g���g�p
            return DefaultEncoding;
        }
    }

    #endregion
}