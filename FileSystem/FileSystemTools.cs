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

        // �e�f�B���N�g����������΍쐬
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
        
        // ���[�g�f�B���N�g������YAML�̃��[�g�v�f�Ƃ��Ēǉ�
        string rootName = Path.GetFileName(fullPath);
        sb.AppendLine($"{rootName}:");
        
        // �f�B���N�g���̓��e��YAML�`���Œǉ�
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
        // ���[�g�f�B���N�g���̓X�L�b�v���Ȃ�
        if (path != rootPath && IsIgnored(relativePath, ignorePatterns)) return;
        
        // �t�@�C���ƃf�B���N�g���̈ꗗ���擾
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        // �����p�^�[���Ńt�B���^�����O
        var filteredFiles = files
            .Where(file => !IsIgnored(Path.GetRelativePath(rootPath, file).Replace("\\", "/"), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !IsIgnored(Path.GetRelativePath(rootPath, dir).Replace("\\", "/"), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();

        // �t�@�C���̕\���iYAML���X�g�`���j
        if (filteredFiles.Length > 0)
        {
            // �t�@�C�������X�g�`���Œǉ��i"files:"���x���Ȃ��j
            if (path == rootPath)
            {
                foreach (var file in filteredFiles)
                {
                    sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
                }
            }
            else
            {
                // �T�u�f�B���N�g���ł́A�t�@�C�������X�g�`���Œǉ�
                foreach (var file in filteredFiles)
                {
                    sb.AppendLine($"{indent}- {Path.GetFileName(file)}");
                }
            }
        }

        // �T�u�f�B���N�g���������i"directories:"���x���Ȃ��j
        foreach (var dir in filteredDirs)
        {
            var dirName = Path.GetFileName(dir);

            // �f�B���N�g������\��
            if (path == rootPath)
            {
                sb.AppendLine($"{indent}{dirName}:");
            }
            else
            {
                sb.AppendLine($"{indent}{dirName}:");
            }

            // �q�f�B���N�g����.gitignore���m�F
            string gitignorePath = Path.Combine(dir, ".gitignore");
            List<Regex> childIgnorePatterns = new List<Regex>(ignorePatterns);

            if (File.Exists(gitignorePath))
            {
                // �e�̖����p�^�[���ɉ����āA�q�f�B���N�g����.gitignore����̃p�^�[����ǉ�
                childIgnorePatterns.AddRange(ParseGitIgnore(gitignorePath, dir, rootPath));
            }

            if (recursive == false) continue;

            // �ċA�I�ɃT�u�f�B���N�g��������
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

    // �ȉ��̃��\�b�h�͕ύX�Ȃ�
    private static void TraverseDirectory(string path, StringBuilder sb, string indent,
        List<Regex> ignorePatterns, string rootPath, bool recursive)
    {
        string relativePath = Path.GetRelativePath(rootPath, path).Replace("\\", "/");
        // ���[�g�f�B���N�g���̓X�L�b�v���Ȃ�
        if (path != rootPath && IsIgnored(relativePath, ignorePatterns)) return;
        // �t�@�C���ƃf�B���N�g���̈ꗗ���擾
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        // �����p�^�[���Ńt�B���^�����O
        var filteredFiles = files
            .Where(file => !IsIgnored(Path.GetRelativePath(rootPath, file).Replace("\\", "/"), ignorePatterns))
            .OrderBy(file => Path.GetFileName(file))
            .ToArray();

        var filteredDirs = directories
            .Where(dir => !IsIgnored(Path.GetRelativePath(rootPath, dir).Replace("\\", "/"), ignorePatterns))
            .OrderBy(dir => Path.GetFileName(dir))
            .ToArray();
        // �t�@�C���̕\��
        for (int i = 0; i < filteredFiles.Length; i++)
        {
            var fileName = Path.GetFileName(filteredFiles[i]);
            bool isLast = i == filteredFiles.Length - 1 && filteredDirs.Length == 0;
            sb.AppendLine($"{indent}|-- {fileName}");
        }
        // �T�u�f�B���N�g��������
        for (int i = 0; i < filteredDirs.Length; i++)
        {
            var dirName = Path.GetFileName(filteredDirs[i]);
            bool isLast = i == filteredDirs.Length - 1;

            // �f�B���N�g������\��
            sb.AppendLine($"{indent}|-- {dirName}/");

            // �q�f�B���N�g����.gitignore���m�F
            string gitignorePath = Path.Combine(filteredDirs[i], ".gitignore");
            List<Regex> childIgnorePatterns = new List<Regex>(ignorePatterns);

            if (File.Exists(gitignorePath))
            {
                // �e�̖����p�^�[���ɉ����āA�q�f�B���N�g����.gitignore����̃p�^�[����ǉ�
                childIgnorePatterns.AddRange(ParseGitIgnore(gitignorePath, filteredDirs[i], rootPath));
            }

            if (recursive == false) continue;

            // �ċA�I�ɃT�u�f�B���N�g��������
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

    // �t�@�C����f�B���N�g���������Ώۂ��ǂ����𔻒�
    private static bool IsIgnored(string relativePath, List<Regex> ignorePatterns)
    {
        // .�Ŏn�܂�f�B���N�g���͖���
        if (relativePath.Split('/').Any(part => part.StartsWith(".") && part != "." && part != ".."))
            return true;

        // ���K�\���p�^�[���Ɉ�v���邩�`�F�b�N
        foreach (var pattern in ignorePatterns)
        {
            if (pattern.IsMatch(relativePath))
                return true;
        }

        return false;
    }

    // ���[�g�f�B���N�g����.gitignore��ǂݍ���
    private static List<Regex> LoadIgnorePatterns(string rootPath)
    {
        var patterns = new List<Regex>();

        // ���ʃe���v���[�g�̖����p�^�[����ǉ�
        patterns.AddRange(GetCommonIgnorePatterns());

        // ���[�g�f�B���N�g����.gitignore�t�@�C����ǂݍ���
        string gitignorePath = Path.Combine(rootPath, ".gitignore");
        if (File.Exists(gitignorePath))
        {
            patterns.AddRange(ParseGitIgnore(gitignorePath, rootPath, rootPath));
        }

        return patterns;
    }

    // .gitignore�t�@�C������͂��Đ��K�\���p�^�[���̃��X�g��Ԃ�
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

            // �R�����g�s���s���X�L�b�v
            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            // �������Ȃ��p�^�[���i!����n�܂�j�͌��݃T�|�[�g���Ă��Ȃ�
            if (trimmedLine.StartsWith("!"))
                continue;

            // �p�^�[���𐳋K�\���ɕϊ�
            string regexPattern = ConvertGitWildcardToRegex(trimmedLine, relativeDir);
            patterns.Add(new Regex(regexPattern, RegexOptions.IgnoreCase));
        }

        return patterns;
    }

    // Git�̃��C���h�J�[�h�p�^�[���𐳋K�\���ɕϊ�
    private static string ConvertGitWildcardToRegex(string pattern, string relativeDir)
    {
        string result;
        // �����̃X���b�V���������i�f�B���N�g���݂̂Ƀ}�b�`�j
        bool dirOnly = pattern.EndsWith("/");
        if (dirOnly)
            pattern = pattern.TrimEnd('/');

        // �p�^�[���� /�Ŏn�܂�ꍇ�́A���[�g����̐�΃p�X
        if (pattern.StartsWith("/"))
        {
            pattern = pattern.TrimStart('/');
            // ��΃p�X�̏ꍇ�ArelativeDir���܂߂Č����ȃp�X���\�z
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
            // ���΃p�X�̏ꍇ
            if (!pattern.Contains("/"))
            {
                // �X���b�V�����܂܂Ȃ��p�^�[���i�t�@�C������f�B���N�g�����̂݁j
                if (!string.IsNullOrEmpty(relativeDir))
                {
                    // relativeDir���w�肳��Ă���ꍇ�A���̃f�B���N�g�����̂ǂ�����T��
                    result = relativeDir + "/(?:.*/)?(" + pattern + ")";
                }
                else
                {
                    // relativeDir���Ȃ��ꍇ�́A�C�ӂ̃f�B���N�g���K�w�Ƀ}�b�`
                    result = "(?:.*/)?(" + pattern + ")";
                }
            }
            else
            {
                // �X���b�V�����܂ރp�^�[���i�p�X�w�肠��j
                if (!string.IsNullOrEmpty(relativeDir))
                {
                    // relativeDir���w�肳��Ă���ꍇ�A���̃f�B���N�g�����v���t�B�b�N�X�Ƃ��Ďg�p
                    result = relativeDir + "/" + pattern;
                }
                else
                {
                    // relativeDir���Ȃ��ꍇ�̓p�^�[�������̂܂܎g�p
                    result = pattern;
                }
            }
        }

        // ���C���h�J�[�h��Regex�ɕϊ�
        result = result
            .Replace(".", "\\.")
            .Replace("**/", "(?:.*/)?")
            .Replace("**", ".*")
            .Replace("*", "[^/]*")
            .Replace("?", "[^/]");

        // �f�B���N�g���݂̂Ƀ}�b�`������ꍇ�͖�����/��t���������ǉ�
        if (dirOnly)
        {
            result += "(?:/.*)?";
        }
        else
        {
            result += "(?:$|/.*)";
        }

        // �擪�Ɩ����̃}�b�`���O�𒲐�
        return @"^" + result + @"$";
    }

    // ���ʂŖ�������p�^�[�����擾
    private static List<Regex> GetCommonIgnorePatterns()
    {
        var patterns = new List<Regex>
        {
            // .git�t�H���_�𖳎�
            new Regex(@"^(?:.+/)?\.git(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?\.next(?:/.*)?$", RegexOptions.IgnoreCase),
            
            // �r���h�������𖳎�
            new Regex(@"^(?:.+/)?bin(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?obj(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?target(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?dist(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^(?:.+/)?lib(?:/.*)?$", RegexOptions.IgnoreCase),
            
            // Visual Studio�̃t�@�C���𖳎�
            new Regex(@"^(?:.+/)?\.vs(?:/.*)?$", RegexOptions.IgnoreCase),
            new Regex(@"^.*\.user$", RegexOptions.IgnoreCase),
            new Regex(@"^.*\.suo$", RegexOptions.IgnoreCase),
            
            // �L���b�V���t�@�C���𖳎�
            new Regex(@"^.*\.cache$", RegexOptions.IgnoreCase),
            
            // �o�b�N�A�b�v�t�@�C���𖳎�
            new Regex(@"^.*~$", RegexOptions.IgnoreCase),
            new Regex(@"^.*\.bak$", RegexOptions.IgnoreCase),
            
            // ���O�t�@�C���𖳎�
            new Regex(@"^.*\.log$", RegexOptions.IgnoreCase),
            
            // NuGet�p�b�P�[�W�𖳎�
            new Regex(@"^(?:.+/)?packages(?:/.*)?$", RegexOptions.IgnoreCase),
            
            // npm���W���[���𖳎�
            new Regex(@"^(?:.+/)?node_modules(?:/.*)?$", RegexOptions.IgnoreCase)
        };

        return patterns;
    }
}
