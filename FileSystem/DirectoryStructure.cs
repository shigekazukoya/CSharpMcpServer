using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace FileSystem.Tools;

[McpServerToolType]
public static class DirectoryStructure
{
    [McpServerTool, Description("Retrieves the hierarchical folder structure in YAML format from a specified directory path.")]
    public static string GetFolderStructure(
        [Description("Absolute path to the root directory whose folder structure should be retrieved.")] string fullPath,
        [Description("Specifies whether to include subdirectories recursively in the folder structure. If set to true, the function will traverse through all nested directories. If false, only the immediate children of the root directory will be included.")] bool recursive = true)
    {
        Security.ValidateIsAllowedDirectory(fullPath);

        if (!Directory.Exists(fullPath))
            throw new DirectoryNotFoundException($"Directory not found: {fullPath}");
        
        var ignorePatterns = GitIgnoreParser.LoadIgnorePatterns(fullPath);
        var sb = new StringBuilder();
        
        string rootName = Path.GetFileName(fullPath);
        sb.AppendLine($"{rootName}:");
        
        TraverseDirectoryYaml(fullPath, sb, "  ", ignorePatterns, fullPath, recursive);
        
        return sb.ToString();
    }

    /// <summary>
    /// ディレクトリをYAML形式で走査します
    /// </summary>
    private static void TraverseDirectoryYaml(
        string path, 
        StringBuilder sb, 
        string indent,
        List<Regex> ignorePatterns, 
        string rootPath, 
        bool recursive)
    {
        string relativePath = Path.GetRelativePath(rootPath, path).Replace("\\", "/");
        if (path != rootPath && GitIgnoreParser.IsIgnored(relativePath, ignorePatterns)) return;
        
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        var filteredFiles = files
            .Where(file => !GitIgnoreParser.IsIgnored(Path.GetRelativePath(rootPath, file).Replace("\\", "/"), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !GitIgnoreParser.IsIgnored(Path.GetRelativePath(rootPath, dir).Replace("\\", "/"), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();

        if (filteredFiles.Length > 0)
        {
            if (path == rootPath)
            {
                foreach (var file in filteredFiles)
                {
                    sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
                }
            }
            else
            {
                foreach (var file in filteredFiles)
                {
                    sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
                }
            }
        }

        foreach (var dir in filteredDirs)
        {
            var dirName = Path.GetFileName(dir);

            if (path == rootPath)
            {
                sb.AppendLine($"{indent}{dirName}:");
            }
            else
            {
                sb.AppendLine($"{indent}{dirName}:");
            }

            string gitignorePath = Path.Combine(dir, ".gitignore");
            List<Regex> childIgnorePatterns = new List<Regex>(ignorePatterns);

            if (File.Exists(gitignorePath))
            {
                childIgnorePatterns.AddRange(GitIgnoreParser.ParseGitIgnore(gitignorePath, dir, rootPath));
            }

            if (recursive == false) continue;

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

    /// <summary>
    /// ディレクトリをツリー形式で走査します
    /// </summary>
    public static void TraverseDirectory(
        string path, 
        StringBuilder sb, 
        string indent,
        List<Regex> ignorePatterns, 
        string rootPath, 
        bool recursive)
    {
        string relativePath = Path.GetRelativePath(rootPath, path).Replace("\\", "/");
        if (path != rootPath && GitIgnoreParser.IsIgnored(relativePath, ignorePatterns)) return;
        
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        var filteredFiles = files
            .Where(file => !GitIgnoreParser.IsIgnored(Path.GetRelativePath(rootPath, file).Replace("\\", "/"), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !GitIgnoreParser.IsIgnored(Path.GetRelativePath(rootPath, dir).Replace("\\", "/"), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();
            
        for (int i = 0; i < filteredFiles.Length; i++)
        {
            var fileName = Path.GetFileName(filteredFiles[i]);
            bool isLast = i == filteredFiles.Length - 1 && filteredDirs.Length == 0;
            sb.AppendLine($"{indent}|-- {fileName}");
        }
        
        for (int i = 0; i < filteredDirs.Length; i++)
        {
            var dirName = Path.GetFileName(filteredDirs[i]);
            bool isLast = i == filteredDirs.Length - 1;

            sb.AppendLine($"{indent}|-- {dirName}/");

            string gitignorePath = Path.Combine(filteredDirs[i], ".gitignore");
            List<Regex> childIgnorePatterns = new List<Regex>(ignorePatterns);

            if (File.Exists(gitignorePath))
            {
                childIgnorePatterns.AddRange(GitIgnoreParser.ParseGitIgnore(gitignorePath, filteredDirs[i], rootPath));
            }

            if (recursive == false) continue;

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
}