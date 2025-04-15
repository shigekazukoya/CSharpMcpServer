namespace FileSystem.Tools;

public static partial class FileSystemTools
{
    public class FileInfo
    {
        public FileInfo(string filePath)
        {
            this.FilePath = filePath;
            this.Content = File.ReadAllText(filePath);
            this.LineCount = this.Content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None).Length;
        }

        public string FilePath { get; }
        public int LineCount { get;  }
        public string Content { get; }
    }
}