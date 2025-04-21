using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;
namespace WebTools;

[McpServerToolType]
public static class WebTools
{
    [McpServerTool, Description("規定のブラウザでURLを開きます")]
    public static void OpenUrlInDefaultBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"URLを開く際にエラーが発生しました: {ex.Message}");
        }
    }
}
