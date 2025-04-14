using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;


namespace FileSystem.Tools;

[McpServerToolType]
public static class FileSystemTools
{
    [McpServerTool, Description("Writes text content to a file on the file system. Creates directories if needed.")]
    public static void WriteFile(
        [Description("The full path to the target file.")] string filePath,
        [Description("The text content to be written to the file.")] string text)
    {
        Security.ValidateIsAllowedDirectory(filePath);

        // 親ディレクトリが無ければ作成
        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, text);
    }

    [McpServerTool, Description("Reads text content from a file on the file system.")]
    public static string ReadFile(
        [Description("The full path to the file to be read.")] string filePath)
    {
        Security.ValidateIsAllowedDirectory(filePath);
        return File.ReadAllText(filePath);
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

    [McpServerTool, Description("Retrieves the hierarchical folder structure in YAML format from a specified directory path.")]
    public static string GetFolderStructure(
        [Description("Absolute path to the root directory whose folder structure should be retrieved.")] string fullPath,
        [Description("Specifies whether to include subdirectories recursively in the folder structure. If set to true, the function will traverse through all nested directories. If false, only the immediate children of the root directory will be included.")] bool recursive = true)
    {
        if (!Directory.Exists(fullPath))
            throw new DirectoryNotFoundException($"Directory not found: {fullPath}");
        
        var ignorePatterns = LoadIgnorePatterns(fullPath);
        var sb = new StringBuilder();
        
        // ルートディレクトリ名をYAMLのルート要素として追加
        string rootName = Path.GetFileName(fullPath);
        sb.AppendLine($"{rootName}:");
        
        // ディレクトリの内容をYAML形式で追加
        TraverseDirectoryYaml(fullPath, sb, "  ", ignorePatterns, fullPath, recursive);
        
        return sb.ToString();
    }

    private static void TraverseDirectoryYaml(
        string path, 
        StringBuilder sb, 
        string indent,
        List<Regex> ignorePatterns, 
        string rootPath, 
        bool recursive)
    {
        string relativePath = Path.GetRelativePath(rootPath, path).Replace("\\", "/");
        // ルートディレクトリはスキップしない
        if (path != rootPath && IsIgnored(relativePath, ignorePatterns)) return;
        
        // ファイルとディレクトリの一覧を取得
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        // 無視パターンでフィルタリング
        var filteredFiles = files
            .Where(file => !IsIgnored(Path.GetRelativePath(rootPath, file).Replace("\\", "/"), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !IsIgnored(Path.GetRelativePath(rootPath, dir).Replace("\\", "/"), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();

        // ファイルの表示（YAMLリスト形式）
        if (filteredFiles.Length > 0)
        {
            // ファイルをリスト形式で追加（"files:"ラベルなし）
            if (path == rootPath)
            {
                foreach (var file in filteredFiles)
                {
                    sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
                }
            }
            else
            {
                // サブディレクトリでは、ファイルをリスト形式で追加
                foreach (var file in filteredFiles)
                {
                    sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
                }
            }
        }

        // サブディレクトリを処理（"directories:"ラベルなし）
        foreach (var dir in filteredDirs)
        {
            var dirName = Path.GetFileName(dir);

            // ディレクトリ名を表示
            if (path == rootPath)
            {
                sb.AppendLine($"{indent}{dirName}:");
            }
            else
            {
                sb.AppendLine($"{indent}{dirName}:");
            }

            // 子ディレクトリの.gitignoreを確認
            string gitignorePath = Path.Combine(dir, ".gitignore");
            List<Regex> childIgnorePatterns = new List<Regex>(ignorePatterns);

            if (File.Exists(gitignorePath))
            {
                // 親の無視パターンに加えて、子ディレクトリの.gitignoreからのパターンを追加
                childIgnorePatterns.AddRange(ParseGitIgnore(gitignorePath, dir, rootPath));
            }

            if (recursive == false) continue;

            // 再帰的にサブディレクトリを処理
            string newIndent = path == rootPath ? indent + "  " : indent + "  ";
            TraverseDirectoryYaml(
                dir,
                sb,
                newIndent,
                childIgnorePatterns,
                rootPath,
                recursive
            );
        }
    }

    // 以下のメソッドは変更なし
    private static void TraverseDirectory(string path, StringBuilder sb, string indent,
        List<Regex> ignorePatterns, string rootPath, bool recursive)
    {
        string relativePath = Path.GetRelativePath(rootPath, path).Replace("\\", "/");
        // ルートディレクトリはスキップしない
        if (path != rootPath && IsIgnored(relativePath, ignorePatterns)) return;
        // ファイルとディレクトリの一覧を取得
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        // 無視パターンでフィルタリング
        var filteredFiles = files
            .Where(file => !IsIgnored(Path.GetRelativePath(rootPath, file).Replace("\\", "/"), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !IsIgnored(Path.GetRelativePath(rootPath, dir).Replace("\\", "/"), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();
        // ファイルの表示
        for (int i = 0; i < filteredFiles.Length; i++)
        {
            var fileName = Path.GetFileName(filteredFiles[i]);
            bool isLast = i == filteredFiles.Length - 1 && filteredDirs.Length == 0;
            sb.AppendLine($"{indent}|-- {fileName}");
        }
        // サブディレクトリを処理
        for (int i = 0; i < filteredDirs.Length; i++)
        {
            var dirName = Path.GetFileName(filteredDirs[i]);
            bool isLast = i == filteredDirs.Length - 1;

            // ディレクトリ名を表示
            sb.AppendLine($"{indent}|-- {dirName}/");

            // 子ディレクトリの.gitignoreを確認
            string gitignorePath = Path.Combine(filteredDirs[i], ".gitignore");
            List<Regex> childIgnorePatterns = new List<Regex>(ignorePatterns);

            if (File.Exists(gitignorePath))
            {
                // 親の無視パターンに加えて、子ディレクトリの.gitignoreからのパターンを追加
                childIgnorePatterns.AddRange(ParseGitIgnore(gitignorePath, filteredDirs[i], rootPath));
            }

            if (recursive == false) continue;

            // 再帰的にサブディレクトリを処理
            TraverseDirectory(
                filteredDirs[i],
                sb,
                indent + (isLast ? "    " : "|   "),
                childIgnorePatterns,
                rootPath,
                recursive
            );
        }
    }

    // ファイルやディレクトリが無視対象かどうかを判定
    private static bool IsIgnored(string relativePath, List<Regex> ignorePatterns)
    {
        // .で始まるディレクトリは無視
        if (relativePath.Split('/').Any(part => part.StartsWith(".") && part != "." && part != ".."))
            return true;

        // 正規表現パターンに一致するかチェック
        foreach (var pattern in ignorePatterns)
        {
            if (pattern.IsMatch(relativePath))
                return true;
        }

        return false;
    }

    // ルートディレクトリの.gitignoreを読み込む
    private static List<Regex> LoadIgnorePatterns(string rootPath)
    {
        var patterns = new List<Regex>();

        // 共通テンプレートの無視パターンを追加
        patterns.AddRange(GetCommonIgnorePatterns());

        // ルートディレクトリの.gitignoreファイルを読み込む
        string gitignorePath = Path.Combine(rootPath, ".gitignore");
        if (File.Exists(gitignorePath))
        {
            patterns.AddRange(ParseGitIgnore(gitignorePath, rootPath, rootPath));
        }

        return patterns;
    }

    // .gitignoreファイルを解析して正規表現パターンのリストを返す
    private static List<Regex> ParseGitIgnore(string gitignorePath, string currentDir, string rootPath)
    {
        var patterns = new List<Regex>();
        var lines = File.ReadAllLines(gitignorePath);

        string relativeDir = Path.GetRelativePath(rootPath, currentDir).Replace("\\", "/");
        if (relativeDir == ".")
            relativeDir = "";

        foreach (var line in lines)
        {
            string trimmedLine = line.Trim();

            // コメント行や空行をスキップ
            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            // 無視しないパターン（!から始まる）は現在サポートしていない
            if (trimmedLine.StartsWith("!"))
                continue;

            // パターンを正規表現に変換
            string regexPattern = ConvertGitWildcardToRegex(trimmedLine, relativeDir);
            patterns.Add(new Regex(regexPattern, RegexOptions.IgnoreCase));
        }

        return patterns;
    }

    // Gitのワイルドカードパターンを正規表現に変換
    private static string ConvertGitWildcardToRegex(string pattern, string relativeDir)
    {
        string result;
        // 末尾のスラッシュを処理（ディレクトリのみにマッチ）
        bool dirOnly = pattern.EndsWith("/");
        if (dirOnly)
            pattern = pattern.TrimEnd('/');

        // パターンが /で始まる場合は、ルートからの絶対パス
        if (pattern.StartsWith("/"))
        {
            pattern = pattern.TrimStart('/');
            // 絶対パスの場合、relativeDirを含めて厳密なパスを構築
            if (!string.IsNullOrEmpty(relativeDir))
            {
                result = relativeDir + "/" + pattern;
            }
            else
            {
                result = pattern;
            }
        }
        else
        {
            // 相対パスの場合
            if (!pattern.Contains("/"))
            {
                // スラッシュを含まないパターン（ファイル名やディレクトリ名のみ）
                if (!string.IsNullOrEmpty(relativeDir))
                {
                    // relativeDirが指定されている場合、そのディレクトリ内のどこかを探す
                    result = relativeDir + "/(?:.*/)?(" + pattern + ")";
                }
                else
                {
                    // relativeDirがない場合は、任意のディレクトリ階層にマッチ
                    result = "(?:.*/)?(" + pattern + ")";
                }
            }
            else
            {
                // スラッシュを含むパターン（パス指定あり）
                if (!string.IsNullOrEmpty(relativeDir))
                {
                    // relativeDirが指定されている場合、そのディレクトリをプレフィックスとして使用
                    result = relativeDir + "/" + pattern;
                }
                else
                {
                    // relativeDirがない場合はパターンをそのまま使用
                    result = pattern;
                }
            }
        }

        // ワイルドカードをRegexに変換
        result = result
            .Replace(".", "\\.")
            .Replace("**/", "(?:.*/)?")
            .Replace("**", ".*")
            .Replace("*", "[^/]*")
            .Replace("?", "[^/]");

        // ディレクトリのみにマッチさせる場合は末尾に/を付ける条件を追加
        if (dirOnly)
        {
            result += "(?:/.*)?";
        }
        else
        {
            result += "(?:$|/.*)";
        }

        // 先頭と末尾のマッチングを調整
        return @"^" + result + @"$";
    }

    // 共通で無視するパターンを取得
    private static List<Regex> GetCommonIgnorePatterns()
    {
        var patterns = new List<Regex>
        {
            // .gitフォルダを無視
            new Regex(@"^(?:.+/)?\.git(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?\.next(?:/.*)?$", RegexOptions.IgnoreCase),
            
            // ビルド生成物を無視
            new Regex(@"^(?:.+/)?bin(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?obj(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?target(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?dist(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?lib(?:/.*)?$", RegexOptions.IgnoreCase),
            
            // Visual Studioのファイルを無視
            new Regex(@"^(?:.+/)?\.vs(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^.*\.user$", RegexOptions.IgnoreCase),
            new Regex(@"^.*\.suo$", RegexOptions.IgnoreCase),
            
            // キャッシュファイルを無視
            new Regex(@"^.*\.cache$", RegexOptions.IgnoreCase),
            
            // バックアップファイルを無視
            new Regex(@"^.*~$", RegexOptions.IgnoreCase),
            new Regex(@"^.*\.bak$", RegexOptions.IgnoreCase),
            
            // ログファイルを無視
            new Regex(@"^.*\.log$", RegexOptions.IgnoreCase),
            
            // NuGetパッケージを無視
            new Regex(@"^(?:.+/)?packages(?:/.*)?$", RegexOptions.IgnoreCase),
            
            // npmモジュールを無視
            new Regex(@"^(?:.+/)?node_modules(?:/.*)?$", RegexOptions.IgnoreCase)
        };

        return patterns;
    }
}
