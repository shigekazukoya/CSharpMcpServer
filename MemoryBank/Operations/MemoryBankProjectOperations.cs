using System.Text.Json;
using MemoryBankTools.Models;
using MemoryBankTools.Validation;

namespace MemoryBankTools.Operations;

public static class MemoryBankProjectOperations
{
    private static string RootPath => Environment.GetEnvironmentVariable("MEMORY_BANK_ROOT") ?? 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "MemoryBank");
        
    public static bool ProjectExists(string projectName)
    {
        if (!MemoryBankValidation.ValidateProjectName(projectName))
            return false;
            
        return Directory.Exists(Path.Combine(RootPath, projectName));
    }

    public static string EnsureProject(string projectName, string description = "")
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        string projectPath = Path.Combine(RootPath, projectName);
        
        if (!Directory.Exists(projectPath))
        {
            Directory.CreateDirectory(projectPath);
            
            // Create project metadata
            var projectInfo = new ProjectInfo
            {
                Name = projectName,
                Description = description,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };
            
            string metadataPath = Path.Combine(projectPath, ".project-info.json");
            File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
        }
        else
        {
            // Update last access time
            string metadataPath = Path.Combine(projectPath, ".project-info.json");
            if (File.Exists(metadataPath))
            {
                try
                {
                    var projectInfo = JsonSerializer.Deserialize<ProjectInfo>(File.ReadAllText(metadataPath));
                    if (projectInfo != null)
                    {
                        projectInfo.LastUpdatedAt = DateTime.Now;
                        File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
                        { 
                            WriteIndented = true 
                        }));
                    }
                }
                catch (Exception)
                {
                    // Ignore deserialize errors
                }
            }
        }
        
        return projectPath;
    }

    public static List<ProjectInfo> GetProjects()
    {
        EnsureRootDirectory();
        
        List<ProjectInfo> projects = new();
        foreach (string directory in Directory.GetDirectories(RootPath))
        {
            string projectName = Path.GetFileName(directory);
            if (string.IsNullOrEmpty(projectName))
                continue;
                
            string metadataPath = Path.Combine(directory, ".project-info.json");
            ProjectInfo projectInfo;
            
            if (File.Exists(metadataPath))
            {
                try
                {
                    projectInfo = JsonSerializer.Deserialize<ProjectInfo>(File.ReadAllText(metadataPath)) 
                                  ?? new ProjectInfo { Name = projectName };
                }
                catch (Exception)
                {
                    projectInfo = new ProjectInfo { Name = projectName };
                }
            }
            else
            {
                projectInfo = new ProjectInfo { Name = projectName };
            }
            
            projects.Add(projectInfo);
        }
        
        return projects;
    }

    public static void EnsureRootDirectory()
    {
        if (!Directory.Exists(RootPath))
        {
            Directory.CreateDirectory(RootPath);
        }
    }
    
    public static bool DeleteProject(string projectName)
    {
        if (!MemoryBankValidation.ValidateProjectName(projectName))
            return false;
            
        string projectPath = Path.Combine(RootPath, projectName);
        if (!Directory.Exists(projectPath))
            return false;
            
        try
        {
            Directory.Delete(projectPath, true);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public static bool RenameProject(string oldName, string newName)
    {
        if (!MemoryBankValidation.ValidateProjectName(oldName) || !MemoryBankValidation.ValidateProjectName(newName))
            return false;
            
        string oldPath = Path.Combine(RootPath, oldName);
        string newPath = Path.Combine(RootPath, newName);
        
        if (!Directory.Exists(oldPath) || Directory.Exists(newPath))
            return false;
            
        try
        {
            Directory.Move(oldPath, newPath);
            
            // Update project metadata
            string metadataPath = Path.Combine(newPath, ".project-info.json");
            if (File.Exists(metadataPath))
            {
                try
                {
                    var projectInfo = JsonSerializer.Deserialize<ProjectInfo>(File.ReadAllText(metadataPath));
                    if (projectInfo != null)
                    {
                        projectInfo.Name = newName;
                        projectInfo.LastUpdatedAt = DateTime.Now;
                        File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
                        { 
                            WriteIndented = true 
                        }));
                    }
                }
                catch (Exception)
                {
                    // Ignore deserialize errors
                }
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public static ProjectInfo? GetProjectInfo(string projectName)
    {
        if (!MemoryBankValidation.ValidateProjectName(projectName))
            return null;
            
        string projectPath = Path.Combine(RootPath, projectName);
        if (!Directory.Exists(projectPath))
            return null;
            
        string metadataPath = Path.Combine(projectPath, ".project-info.json");
        if (!File.Exists(metadataPath))
        {
            // Create default metadata
            var projectInfo = new ProjectInfo
            {
                Name = projectName,
                CreatedAt = Directory.GetCreationTime(projectPath),
                LastUpdatedAt = DateTime.Now
            };
            
            File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
            
            return projectInfo;
        }
        
        try
        {
            return JsonSerializer.Deserialize<ProjectInfo>(File.ReadAllText(metadataPath));
        }
        catch (Exception)
        {
            return new ProjectInfo { Name = projectName };
        }
    }
    
    public static bool UpdateProjectMetadata(string projectName, string description)
    {
        if (!MemoryBankValidation.ValidateProjectName(projectName))
            return false;
            
        string projectPath = Path.Combine(RootPath, projectName);
        if (!Directory.Exists(projectPath))
            return false;
            
        string metadataPath = Path.Combine(projectPath, ".project-info.json");
        ProjectInfo projectInfo;
        
        if (File.Exists(metadataPath))
        {
            try
            {
                projectInfo = JsonSerializer.Deserialize<ProjectInfo>(File.ReadAllText(metadataPath)) 
                            ?? new ProjectInfo { Name = projectName };
            }
            catch (Exception)
            {
                projectInfo = new ProjectInfo { Name = projectName };
            }
        }
        else
        {
            projectInfo = new ProjectInfo 
            { 
                Name = projectName,
                CreatedAt = Directory.GetCreationTime(projectPath)
            };
        }
        
        projectInfo.Description = description;
        projectInfo.LastUpdatedAt = DateTime.Now;
        
        try
        {
            File.WriteAllText(metadataPath, JsonSerializer.Serialize(projectInfo, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
