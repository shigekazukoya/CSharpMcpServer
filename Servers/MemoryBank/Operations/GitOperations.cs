using System.Diagnostics;
using System.Text;

namespace MemoryBankTools.Operations;

public static class GitOperations
{
    // Gitリポジトリを初期化する
    public static bool InitializeRepository(string projectPath)
    {
        try
        {
            // Gitコマンドを実行して初期化
            RunGitCommand(projectPath, "init");
            
            // .gitignoreファイルを作成
            string gitIgnorePath = Path.Combine(projectPath, ".gitignore");
            if (!File.Exists(gitIgnorePath))
            {
                File.WriteAllText(
                    gitIgnorePath,
                    "# MemoryBank temporary files\n*.tmp\n*.temp\n");
                
                // .gitignoreをコミット
                RunGitCommand(projectPath, "add", ".gitignore");
                RunGitCommand(projectPath, "commit", "-m", "Add .gitignore file");
            }
            
            // プロジェクトファイルを初期コミット
            RunGitCommand(projectPath, "add", ".");
            RunGitCommand(projectPath, "commit", "-m", "Initial commit - MemoryBank project created");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Git initialization error: {ex.Message}");
            return false;
        }
    }
    
    // 変更をコミットする
    public static bool CommitChanges(string projectPath, string message)
    {
        try
        {
            // 変更があるか確認
            string status = RunGitCommandWithOutput(projectPath, "status", "--porcelain");
            if (string.IsNullOrWhiteSpace(status))
            {
                // 変更がない場合はコミットしない
                return true;
            }
            
            RunGitCommand(projectPath, "add", ".");
            RunGitCommand(projectPath, "commit", "-m", message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Git commit error: {ex.Message}");
            return false;
        }
    }
    
    // 安全なコミット実行（エラーをキャッチして内部処理）
    public static bool SafeCommitChanges(string projectPath, string message)
    {
        try
        {
            return CommitChanges(projectPath, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Git operation failed: {ex.Message}");
            return false;
        }
    }
    
    // Gitリポジトリか確認する
    public static bool IsGitRepository(string projectPath)
    {
        return Directory.Exists(Path.Combine(projectPath, ".git"));
    }
    
    // Gitが利用可能かチェック
    public static bool IsGitAvailable()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(startInfo);
            process?.WaitForExit();
            
            return process?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
    
    // 最新のコミットハッシュを取得する
    public static string? GetLatestCommitHash(string projectPath)
    {
        try
        {
            if (!IsGitRepository(projectPath))
                return null;
                
            string hash = RunGitCommandWithOutput(projectPath, "rev-parse", "HEAD").Trim();
            return string.IsNullOrEmpty(hash) ? null : hash;
        }
        catch
        {
            return null;
        }
    }
    
    // Gitコマンドを実行する（出力なし）
    private static void RunGitCommand(string workingDirectory, params string[] arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        foreach (var arg in arguments)
        {
            startInfo.ArgumentList.Add(arg);
        }
        
        using var process = Process.Start(startInfo);
        process?.WaitForExit();
        
        if (process?.ExitCode != 0)
        {
            var error = process?.StandardError.ReadToEnd();
            throw new Exception($"Git command failed: {error}");
        }
    }
    
    // Gitコマンドを実行して出力を取得する
    private static string RunGitCommandWithOutput(string workingDirectory, params string[] arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        foreach (var arg in arguments)
        {
            startInfo.ArgumentList.Add(arg);
        }
        
        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new Exception("Failed to start git process");
        }
        
        var output = new StringBuilder();
        process.OutputDataReceived += (sender, e) => 
        {
            if (e.Data != null)
                output.AppendLine(e.Data);
        };
        
        process.BeginOutputReadLine();
        process.WaitForExit();
        
        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            throw new Exception($"Git command failed: {error}");
        }
        
        return output.ToString();
    }
}
