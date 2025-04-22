using System.Text.Json.Serialization;

namespace MemoryBankTools.Models;


public class ProjectInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [JsonPropertyName("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
    
    // Git関連の情報を追加
    [JsonPropertyName("gitEnabled")]
    public bool GitEnabled { get; set; } = true; // デフォルトでGit有効に
    
    [JsonPropertyName("lastCommitHash")]
    public string? LastCommitHash { get; set; }
    
    [JsonPropertyName("remoteUrl")]
    public string? RemoteUrl { get; set; }
}

public class FileInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; }
}



public class ListProjectFilesRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;
    
    [JsonPropertyName("includeCoreFilesOnly")]
    public bool IncludeCoreFilesOnly { get; set; } = false;
    
    [JsonPropertyName("includeSubdirectories")]
    public bool IncludeSubdirectories { get; set; } = true;
}

public class ReadFileRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;
}

public class WriteFileRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonPropertyName("createDirectories")]
    public bool CreateDirectories { get; set; } = true;
}

public class UpdateFileRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonPropertyName("createIfNotExist")]
    public bool CreateIfNotExist { get; set; } = false;
}

public class InitializeMemoryBankRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("projectBriefContent")]
    public string? ProjectBriefContent { get; set; }
}

public class ReadBulkFilesRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;
    
    [JsonPropertyName("fileNames")]
    public List<string> FileNames { get; set; } = new List<string>();
    
    [JsonPropertyName("readCoreFilesOnly")]
    public bool ReadCoreFilesOnly { get; set; } = false;
}

public class RecordDecisionRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("rationale")]
    public string Rationale { get; set; } = string.Empty;
    
    [JsonPropertyName("alternatives")]
    public List<string> Alternatives { get; set; } = new List<string>();
}

public class UpdateContextRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;
    
    [JsonPropertyName("currentTask")]
    public string CurrentTask { get; set; } = string.Empty;
    
    [JsonPropertyName("recentChanges")]
    public string RecentChanges { get; set; } = string.Empty;
    
    [JsonPropertyName("nextSteps")]
    public string NextSteps { get; set; } = string.Empty;
}

public class TrackProgressRequest
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;
    
    [JsonPropertyName("completed")]
    public List<string> Completed { get; set; } = new List<string>();
    
    [JsonPropertyName("inProgress")]
    public List<string> InProgress { get; set; } = new List<string>();
    
    [JsonPropertyName("planned")]
    public List<string> Planned { get; set; } = new List<string>();
    
    [JsonPropertyName("issues")]
    public List<string> Issues { get; set; } = new List<string>();
}



public class ListProjectsResponse
{
    [JsonPropertyName("projects")]
    public List<ProjectInfo> Projects { get; set; } = new List<ProjectInfo>();
}

public class ListProjectFilesResponse
{
    [JsonPropertyName("files")]
    public List<FileInfo> Files { get; set; } = new List<FileInfo>();
}

public class ReadFileResponse
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; }
}

public class WriteFileResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;
}

public class UpdateFileResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;
    
    [JsonPropertyName("created")]
    public bool Created { get; set; } = false;
}

public class InitializeMemoryBankResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("initializedFiles")]
    public List<string> InitializedFiles { get; set; } = new List<string>();
}

public class ReadBulkFilesResponse
{
    [JsonPropertyName("files")]
    public Dictionary<string, FileInfo> Files { get; set; } = new Dictionary<string, FileInfo>();
}

public class RecordDecisionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class UpdateContextResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class TrackProgressResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}


