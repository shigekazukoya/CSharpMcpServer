using System.Text.Json;
using MemoryBankTools.Models;
using MemoryBankTools.Validation;
using FileInfo = MemoryBankTools.Models.FileInfo;

namespace MemoryBankTools.Operations;

public static class MemoryBankFileOperations
{
    private static string RootPath => Environment.GetEnvironmentVariable("MEMORY_BANK_ROOT") ?? 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "MemoryBank");

    public static List<FileInfo> GetProjectFiles(string projectName, bool includeCoreFilesOnly = false, bool includeSubdirectories = true)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        string projectPath = Path.Combine(RootPath, projectName);
        
        if (!Directory.Exists(projectPath))
        {
            return new List<FileInfo>();
        }

        var files = new List<FileInfo>();
        SearchOption searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        
        foreach (string filePath in Directory.GetFiles(projectPath, "*.*", searchOption))
        {
            if (Path.GetFileName(filePath) == ".project-info.json")
                continue;
                
            string relativePath = filePath.Substring(projectPath.Length).TrimStart('\\', '/');
            
            if (includeCoreFilesOnly && !Constants.MemoryBankConstants.CoreMemoryBankFiles.Contains(relativePath))
                continue;
                
            var fileInfo = new FileInfo
            {
                Name = Path.GetFileName(filePath),
                Path = relativePath,
                LastModified = System.IO.File.GetLastWriteTime(filePath)
            };
            
            files.Add(fileInfo);
        }
        
        return files;
    }

    public static ReadFileResponse? ReadFile(string projectName, string filePath)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        MemoryBankValidation.ValidateFilePath(filePath);
        
        string fullPath = Path.Combine(RootPath, projectName, filePath);
        if (!File.Exists(fullPath))
        {
            return null;
        }

        return new ReadFileResponse
        {
            Content = File.ReadAllText(fullPath),
            LastModified = System.IO.File.GetLastWriteTime(fullPath)
        };
    }

    public static WriteFileResponse WriteFile(string projectName, string filePath, string content, bool createDirectories = true)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        MemoryBankValidation.ValidateFilePath(filePath);
        
        string projectPath = MemoryBankProjectOperations.EnsureProject(projectName);
        string fullPath = Path.Combine(projectPath, filePath);
        
        if (File.Exists(fullPath))
        {
            return new WriteFileResponse 
            { 
                Success = false, 
                Message = $"File '{filePath}' already exists in project '{projectName}'",
                FilePath = filePath
            };
        }
        
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            if (createDirectories)
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                return new WriteFileResponse
                {
                    Success = false,
                    Message = $"Directory '{Path.GetDirectoryName(filePath)}' does not exist in project '{projectName}'",
                    FilePath = filePath
                };
            }
        }
        
        File.WriteAllText(fullPath, content);
        
        // ファイル作成後にGitコミット
        var projectInfo = MemoryBankProjectOperations.GetProjectInfo(projectName);
        if (projectInfo != null && projectInfo.GitEnabled && GitOperations.IsGitRepository(projectPath))
        {
            if (GitOperations.CommitChanges(projectPath, $"Create file: {filePath}"))
            {
                // コミットハッシュを更新
                projectInfo.LastCommitHash = GitOperations.GetLatestCommitHash(projectPath);
                string metadataPath = Path.Combine(projectPath, ".project-info.json");
                File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }));
            }
        }
        
        return new WriteFileResponse 
        { 
            Success = true, 
            Message = $"File '{filePath}' created successfully in project '{projectName}'",
            FilePath = filePath
        };
    }

    public static UpdateFileResponse UpdateFile(string projectName, string filePath, string content, bool createIfNotExist = false)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        MemoryBankValidation.ValidateFilePath(filePath);
        
        string projectPath = Path.Combine(RootPath, projectName);
        if (!Directory.Exists(projectPath))
        {
            if (createIfNotExist)
            {
                projectPath = MemoryBankProjectOperations.EnsureProject(projectName);
            }
            else
            {
                return new UpdateFileResponse 
                { 
                    Success = false, 
                    Message = $"Project '{projectName}' does not exist",
                    FilePath = filePath
                };
            }
        }
        
        string fullPath = Path.Combine(projectPath, filePath);
        bool fileExists = File.Exists(fullPath);
        
        if (!fileExists && !createIfNotExist)
        {
            return new UpdateFileResponse 
            { 
                Success = false, 
                Message = $"File '{filePath}' not found in project '{projectName}'",
                FilePath = filePath
            };
        }
        
        if (!fileExists)
        {
            string? directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        File.WriteAllText(fullPath, content);
        
        // ファイル更新/作成後にGitコミット
        var projectInfo = MemoryBankProjectOperations.GetProjectInfo(projectName);
        if (projectInfo != null && projectInfo.GitEnabled && GitOperations.IsGitRepository(projectPath))
        {
            string commitMessage = fileExists 
                ? $"Update file: {filePath}" 
                : $"Create file: {filePath}";
                
            if (GitOperations.CommitChanges(projectPath, commitMessage))
            {
                // コミットハッシュを更新
                projectInfo.LastCommitHash = GitOperations.GetLatestCommitHash(projectPath);
                string metadataPath = Path.Combine(projectPath, ".project-info.json");
                File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }));
            }
        }
        
        return new UpdateFileResponse 
        { 
            Success = true, 
            Message = fileExists 
                ? $"File '{filePath}' updated successfully in project '{projectName}'" 
                : $"File '{filePath}' created successfully in project '{projectName}'",
            FilePath = filePath,
            Created = !fileExists
        };
    }
    
    public static Dictionary<string, FileInfo> ReadMultipleFiles(string projectName, IEnumerable<string> filePaths)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        var results = new Dictionary<string, FileInfo>();
        string projectPath = Path.Combine(RootPath, projectName);
        
        if (!Directory.Exists(projectPath))
            return results;
            
        foreach (string filePath in filePaths)
        {
            if (!MemoryBankValidation.IsValidFilePath(filePath))
                continue;
                
            string fullPath = Path.Combine(projectPath, filePath);
            if (!File.Exists(fullPath))
                continue;
                
            var fileInfo = new FileInfo
            {
                Name = Path.GetFileName(fullPath),
                Path = filePath,
                Content = File.ReadAllText(fullPath),
                LastModified = System.IO.File.GetLastWriteTime(fullPath)
            };
            
            results[filePath] = fileInfo;
        }
        
        return results;
    }
    
    public static bool DeleteFile(string projectName, string filePath)
    {
        if (!MemoryBankValidation.IsValidProjectName(projectName) || !MemoryBankValidation.IsValidFilePath(filePath))
            return false;
            
        string projectPath = Path.Combine(RootPath, projectName);
        string fullPath = Path.Combine(projectPath, filePath);
        if (!File.Exists(fullPath))
            return false;
            
        try
        {
            File.Delete(fullPath);
            
            // ファイル削除後にGitコミット
            var projectInfo = MemoryBankProjectOperations.GetProjectInfo(projectName);
            if (projectInfo != null && projectInfo.GitEnabled && GitOperations.IsGitRepository(projectPath))
            {
                if (GitOperations.CommitChanges(projectPath, $"Delete file: {filePath}"))
                {
                    // コミットハッシュを更新
                    projectInfo.LastCommitHash = GitOperations.GetLatestCommitHash(projectPath);
                    string metadataPath = Path.Combine(projectPath, ".project-info.json");
                    File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    }));
                }
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public static bool CopyFile(string sourceProjectName, string sourceFilePath, string targetProjectName, string targetFilePath, bool overwrite = false)
    {
        if (!MemoryBankValidation.IsValidProjectName(sourceProjectName) || 
            !MemoryBankValidation.IsValidFilePath(sourceFilePath) ||
            !MemoryBankValidation.IsValidProjectName(targetProjectName) ||
            !MemoryBankValidation.IsValidFilePath(targetFilePath))
            return false;
            
        string sourceFullPath = Path.Combine(RootPath, sourceProjectName, sourceFilePath);
        if (!File.Exists(sourceFullPath))
            return false;
            
        string targetFullPath = Path.Combine(RootPath, targetProjectName, targetFilePath);
        
        string? targetDirectory = Path.GetDirectoryName(targetFullPath);
        if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
        {
            try
            {
                Directory.CreateDirectory(targetDirectory);
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        try
        {
            File.Copy(sourceFullPath, targetFullPath, overwrite);
            
            // ファイルコピー後にGitコミット (ターゲットプロジェクトのみ)
            var projectInfo = MemoryBankProjectOperations.GetProjectInfo(targetProjectName);
            if (projectInfo != null && projectInfo.GitEnabled)
            {
                string targetProjectPath = Path.Combine(RootPath, targetProjectName);
                if (GitOperations.IsGitRepository(targetProjectPath))
                {
                    string commitMessage = $"Copy file from '{sourceProjectName}/{sourceFilePath}' to '{targetFilePath}'";
                    if (GitOperations.CommitChanges(targetProjectPath, commitMessage))
                    {
                        // コミットハッシュを更新
                        projectInfo.LastCommitHash = GitOperations.GetLatestCommitHash(targetProjectPath);
                        string metadataPath = Path.Combine(targetProjectPath, ".project-info.json");
                        File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
                        { 
                            WriteIndented = true 
                        }));
                    }
                }
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
