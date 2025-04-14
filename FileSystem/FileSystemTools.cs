using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace FileSystem.Tools;

[McpServerToolType]
public static class FileSystemTools
{
    [McpServerTool,
         Description(
         "Edits a file by replacing or inserting content at specific line positions. For full file replacement, use lineNumber=1 and set linesToDelete to the total number of lines in the file.")]
    public static async Task EditFile(
        [Description("The path to the file to edit")] string path,
        [Description("The 1-based line number where the edit should start. Use 1 to start from the beginning of the file.")]
        int lineNumber,
        [Description("The number of lines to delete from the start line. Set to the total line count for complete file replacement, or 0 for pure insertion without deletion.")]
        int linesToDelete,
        [Description("The text content to insert at the specified position. For full file replacement, provide the entire new content.")]
        string content)
    {
        try
        {
            Security.ValidateIsAllowedDirectory(path);
            string[] lines = await File.ReadAllLinesAsync(path);
            ValidateEditParameters(lines, lineNumber, linesToDelete);
            string[] contentLines = SplitContentIntoLines(content);
            string[] newLines = CreateNewLines(lines, lineNumber, linesToDelete, contentLines);
            await File.WriteAllLinesAsync(path, newLines);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new IOException($"Failed to edit file {path}", ex);
        }
    }

    private static string[] CreateNewLines(string[] lines, int lineNumber, int linesToDelete, string[] contentLines)
    {
        // 適切な削除行数を計算
        if (lineNumber + linesToDelete - 1 > lines.Length)
        {
            linesToDelete = lines.Length - lineNumber + 1;
        }

        // Listを使用して動的に行を追加する方が効率的
        List<string> newLinesList = new List<string>(lines.Length + contentLines.Length - linesToDelete);

        // 1. 編集位置より前の行を追加
        newLinesList.AddRange(lines.Take(lineNumber - 1));

        // 2. 新しいコンテンツを追加
        newLinesList.AddRange(contentLines);

        // 3. 削除した行の後の残りの行を追加
        if (lineNumber - 1 + linesToDelete < lines.Length)
        {
            newLinesList.AddRange(lines.Skip(lineNumber - 1 + linesToDelete));
        }

        return newLinesList.ToArray();
    }

    private static string[] SplitContentIntoLines(string content)
    {
        return string.IsNullOrEmpty(content)
                   ? Array.Empty<string>()
                   : content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    }

    private static void ValidateEditParameters(string[] lines, int lineNumber, int linesToDelete)
    {
        if (lineNumber > lines.Length + 1)
        {
            throw new ArgumentException($"Line number {lineNumber} is out of range. File has {lines.Length} lines.");
        }
        if (linesToDelete < 0)
        {
            throw new ArgumentException("Lines to delete must be at least 0", nameof(linesToDelete));
        }
    }

    private class FileInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public int LineCount { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    [McpServerTool, Description("Gets file information including path, line count, and content.")]
    public static string GetFileInfo([Description("The full path to the file to be read.")] string filePath)
    {
        Security.ValidateIsAllowedDirectory(filePath);

        try
        {
            var content = File.ReadAllText(filePath);
            var lineCount = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;

            var fileInfo = new FileInfo
            {
                FilePath = filePath,
                LineCount = lineCount,
                Content = content
            };

            return JsonSerializer.Serialize(fileInfo, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to read file information from {filePath}", ex);
        }
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
}