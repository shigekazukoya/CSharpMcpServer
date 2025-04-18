using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace FileSystem.Tools;
public static partial class FileSystemTools
{
    [McpServerTool, Description("ファイルまたはフォルダを規定のアプリケーションで開く")]
    public static string Launch(
    [Description("ファイルまたはフォルダのパス")] string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            return $"指定されたパスが見つかりません: {path}";
        }

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            };
            
            Process.Start(processStartInfo);
            return $"'{path}' を規定のアプリケーションで開きました。";
        }
        catch (Exception ex)
        {
            return $"エラーが発生しました: {ex.Message}";
        }
    }
}
