using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System;
using CommandTools.Models;

namespace CommandTools;

/// <summary>
/// コマンド実行ユーティリティクラス
/// </summary>
[McpServerToolType]
public static class CommandTools
{
    /// <summary>
    /// PowerShellでコマンドを実行します
    /// </summary>
    /// <param name="options">シェルコマンドオプション</param>
    /// <returns>シェル実行結果</returns>
    [McpServerTool]
    [Description("PowerShellでコマンドを実行します")]
    public static ShellResult PowerShell(ShellOptions options)
    {
        return ExecuteShell("powershell.exe", "-Command", options);
    }

    /// <summary>
    /// WSL Bash環境でコマンドを実行します
    /// </summary>
    /// <param name="options">シェルコマンドオプション</param>
    /// <returns>シェル実行結果</returns>
    [McpServerTool]
    [Description("WSL Bash環境でコマンドを実行します")]
    public static ShellResult Bash(ShellOptions options)
    {
        return ExecuteShell("wsl.exe", "", options);
    }

    /// <summary>
    /// 指定されたシェルでコマンドを実行する内部メソッド
    /// </summary>
    /// <param name="shellPath">シェル実行ファイルのパス</param>
    /// <param name="argumentPrefix">コマンド引数のプレフィックス</param>
    /// <param name="options">シェルコマンドオプション</param>
    /// <returns>シェル実行結果</returns>
    private static ShellResult ExecuteShell(string shellPath, string argumentPrefix, ShellOptions options)
    {
        var result = new ShellResult
        {
            Stdout = "",
            Stderr = "",
            Interrupted = false,
            IsImage = false,
            Sandbox = false
        };

        var startInfo = new ProcessStartInfo
        {
            FileName = shellPath,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = new UTF8Encoding(false),
            StandardErrorEncoding = new UTF8Encoding(false)
        };

        // コマンドプロンプトとPowerShellの場合は引数プレフィックスを使用
        if (!string.IsNullOrEmpty(argumentPrefix) && shellPath != "wsl.exe")
        {
            startInfo.Arguments = $"{argumentPrefix} {options.Command}";
        }
        else if (shellPath == "wsl.exe")
        {
            // WSLの場合は引数なしでプロセスを起動し、標準入力にコマンドを送る
            startInfo.Arguments = "";
        }

        try
        {
            using var process = new Process { StartInfo = startInfo };
            
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // WSLの場合は標準入力にコマンドを送る
            if (shellPath == "wsl.exe")
            {
                process.StandardInput.WriteLine(options.Command);
                process.StandardInput.Close();
            }

            // タイムアウト処理
            bool hasExited;
            if (options.Timeout.HasValue)
            {
                // タイムアウト値は最大10分（600000ミリ秒）に制限
                int timeout = Math.Min(options.Timeout.Value, 600000);
                hasExited = process.WaitForExit(timeout);
                if (!hasExited)
                {
                    process.Kill();
                    result.Interrupted = true;
                    result.Stderr = "Process timed out and was terminated.";
                }
            }
            else
            {
                // デフォルトのタイムアウトは30分
                hasExited = process.WaitForExit(1800000);
                if (!hasExited)
                {
                    process.Kill();
                    result.Interrupted = true;
                    result.Stderr = "Process exceeded the default timeout (30 minutes) and was terminated.";
                }
            }

            // プロセスが正常に終了した場合
            if (!result.Interrupted)
            {
                process.WaitForExit(); // 残りのイベントが処理されるのを待つ
                result.Stdout = outputBuilder.ToString();
                result.Stderr = errorBuilder.ToString();
            }

            // 出力が30000文字を超える場合は切り捨てる
            const int maxOutputLength = 30000;
            if (result.Stdout.Length > maxOutputLength)
            {
                result.Stdout = result.Stdout.Substring(0, maxOutputLength) + "\n...[出力が長すぎるため切り捨てられました]";
            }
        }
        catch (Exception ex)
        {
            result.Stderr = $"Error executing command: {ex.Message}";
        }

        return result;
    }
}
