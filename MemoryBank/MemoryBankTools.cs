using ModelContextProtocol.Server;
using System.ComponentModel;
using MemoryBankTools.Models;
using MemoryBankTools.Operations;
using MemoryBankTools.Constants;

namespace MemoryBankTools;

[McpServerToolType]
public static class MemoryBankTools
{

    private static readonly string[] CoreMemoryBankFiles = MemoryBankConstants.CoreMemoryBankFiles;

    [McpServerTool]
    [Description("List all available memory bank projects")]
    public static ListProjectsResponse ListProjects()
    {
        var projects = MemoryBankProjectOperations.GetProjects();
        return new ListProjectsResponse { Projects = projects };
    }

    [McpServerTool]
    [Description("List all files in a specific memory bank project")]
    public static ListProjectFilesResponse ListProjectFiles(ListProjectFilesRequest request)
    {
        var files = MemoryBankFileOperations.GetProjectFiles(
            request.ProjectName, 
            request.IncludeCoreFilesOnly, 
            request.IncludeSubdirectories);
            
        return new ListProjectFilesResponse { Files = files };
    }

    [McpServerTool]
    [Description("Read a file from a memory bank project")]
    public static ReadFileResponse ReadMemoryBankFile(ReadFileRequest request)
    {
        var fileResponse = MemoryBankFileOperations.ReadFile(request.ProjectName, request.FilePath);
        
        if (fileResponse == null)
        {
            throw new FileNotFoundException($"File '{request.FilePath}' not found in project '{request.ProjectName}'");
        }
        
        return fileResponse;
    }

    [McpServerTool]
    [Description("Write a new file to a memory bank project")]
    public static WriteFileResponse WriteMemoryBankFile(WriteFileRequest request)
    {
        return MemoryBankFileOperations.WriteFile(
            request.ProjectName, 
            request.FilePath, 
            request.Content, 
            request.CreateDirectories);
    }

    [McpServerTool]
    [Description("Update an existing file in a memory bank project")]
    public static UpdateFileResponse UpdateMemoryBankFile(UpdateFileRequest request)
    {
        return MemoryBankFileOperations.UpdateFile(
            request.ProjectName, 
            request.FilePath, 
            request.Content, 
            request.CreateIfNotExist);
    }
    
    [McpServerTool]
    [Description("Initialize a new memory bank project with core files")]
    public static InitializeMemoryBankResponse InitializeMemoryBank(InitializeMemoryBankRequest request)
    {
        return MemoryBankSpecificOperations.InitializeProject(
            request.ProjectName, 
            request.Description, 
            request.ProjectBriefContent);
    }
    
    [McpServerTool]
    [Description("Read multiple files from a memory bank project")]
    public static ReadBulkFilesResponse ReadBulkFiles(ReadBulkFilesRequest request)
    {
        var files = request.ReadCoreFilesOnly
            ? MemoryBankFileOperations.ReadMultipleFiles(request.ProjectName, CoreMemoryBankFiles)
            : MemoryBankFileOperations.ReadMultipleFiles(request.ProjectName, request.FileNames);
            
        return new ReadBulkFilesResponse { Files = files };
    }
    
    [McpServerTool]
    [Description("Record a decision in the memory bank")]
    public static RecordDecisionResponse RecordDecision(RecordDecisionRequest request)
    {
        try
        {
            MemoryBankSpecificOperations.RecordDecision(
                request.ProjectName,
                request.Title,
                request.Description,
                request.Rationale,
                request.Alternatives);
                
            return new RecordDecisionResponse
            {
                Success = true,
                Message = $"Decision '{request.Title}' recorded successfully in project '{request.ProjectName}'"
            };
        }
        catch (Exception ex)
        {
            return new RecordDecisionResponse
            {
                Success = false,
                Message = $"Failed to record decision: {ex.Message}"
            };
        }
    }
    
    [McpServerTool]
    [Description("Update the active context in the memory bank")]
    public static UpdateContextResponse UpdateContext(UpdateContextRequest request)
    {
        try
        {
            MemoryBankSpecificOperations.UpdateActiveContext(
                request.ProjectName,
                request.CurrentTask,
                request.RecentChanges,
                request.NextSteps);
                
            return new UpdateContextResponse
            {
                Success = true,
                Message = $"Active context updated successfully in project '{request.ProjectName}'"
            };
        }
        catch (Exception ex)
        {
            return new UpdateContextResponse
            {
                Success = false,
                Message = $"Failed to update active context: {ex.Message}"
            };
        }
    }
    
    [McpServerTool]
    [Description("Update progress tracking in the memory bank")]
    public static TrackProgressResponse TrackProgress(TrackProgressRequest request)
    {
        try
        {
            MemoryBankSpecificOperations.UpdateProgress(
                request.ProjectName,
                request.Completed,
                request.InProgress,
                request.Planned,
                request.Issues);
                
            return new TrackProgressResponse
            {
                Success = true,
                Message = $"Progress tracking updated successfully in project '{request.ProjectName}'"
            };
        }
        catch (Exception ex)
        {
            return new TrackProgressResponse
            {
                Success = false,
                Message = $"Failed to update progress tracking: {ex.Message}"
            };
        }
    }


}
