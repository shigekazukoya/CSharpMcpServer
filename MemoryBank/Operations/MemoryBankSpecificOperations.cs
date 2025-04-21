using System.Text;
using MemoryBankTools.Models;
using MemoryBankTools.Utils;
using MemoryBankTools.Constants;
using MemoryBankTools.Validation;

namespace MemoryBankTools.Operations;

public static class MemoryBankSpecificOperations
{
    private static string RootPath => Environment.GetEnvironmentVariable("MEMORY_BANK_ROOT") ?? 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "MemoryBank");
    
    public static InitializeMemoryBankResponse InitializeProject(string projectName, string description, string? projectBriefContent = null)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        string projectPath = MemoryBankProjectOperations.EnsureProject(projectName, description);
        var initializedFiles = new List<string>();
        
        // Create all core files with templates if they don't exist
        foreach (var fileName in MemoryBankConstants.CoreMemoryBankFiles)
        {
            string filePath = Path.Combine(projectPath, fileName);
            
            if (!File.Exists(filePath))
            {
                string content = fileName == "projectbrief.md" && projectBriefContent != null 
                    ? projectBriefContent 
                    : MemoryBankConstants.CoreFileTemplates.GetValueOrDefault(fileName, $"# {fileName}\n\n");
                    
                File.WriteAllText(filePath, content);
                initializedFiles.Add(fileName);
            }
        }
        
        // Create decisions directory
        string decisionsPath = Path.Combine(projectPath, "decisions");
        if (!Directory.Exists(decisionsPath))
        {
            Directory.CreateDirectory(decisionsPath);
        }
        
        return new InitializeMemoryBankResponse
        {
            Success = true,
            Message = $"Memory Bank for project '{projectName}' initialized with {initializedFiles.Count} core files",
            InitializedFiles = initializedFiles
        };
    }
    
    public static void UpdateActiveContext(string projectName, string currentTask, string recentChanges, string nextSteps)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        string projectPath = MemoryBankProjectOperations.EnsureProject(projectName);
        string filePath = Path.Combine(projectPath, "activeContext.md");
        
        // Read existing content or use template
        string content;
        if (File.Exists(filePath))
        {
            content = File.ReadAllText(filePath);
        }
        else
        {
            content = MemoryBankConstants.CoreFileTemplates["activeContext.md"];
        }
        
        // Update specific sections
        string timestamp = MemoryBankUtils.CreateTimestamp();
        
        if (!string.IsNullOrEmpty(currentTask))
        {
            content = MemoryBankUtils.UpdateMarkdownSection(content, "Current Focus", $"Updated: {timestamp}\n\n{currentTask}");
        }
        
        if (!string.IsNullOrEmpty(recentChanges))
        {
            content = MemoryBankUtils.UpdateMarkdownSection(content, "Recent Changes", $"Updated: {timestamp}\n\n{recentChanges}");
        }
        
        if (!string.IsNullOrEmpty(nextSteps))
        {
            content = MemoryBankUtils.UpdateMarkdownSection(content, "Next Steps", $"Updated: {timestamp}\n\n{nextSteps}");
        }
        
        // Write updated content
        File.WriteAllText(filePath, content);
    }
    
    public static void UpdateProgress(string projectName, List<string> completed, List<string> inProgress, List<string> planned, List<string> issues)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        string projectPath = MemoryBankProjectOperations.EnsureProject(projectName);
        string filePath = Path.Combine(projectPath, "progress.md");
        
        // Read existing content or use template
        string content;
        if (File.Exists(filePath))
        {
            content = File.ReadAllText(filePath);
        }
        else
        {
            content = MemoryBankConstants.CoreFileTemplates["progress.md"];
        }
        
        // Update specific sections
        string timestamp = MemoryBankUtils.CreateTimestamp();
        
        if (completed.Count > 0)
        {
            string itemsList = string.Join("\n", completed.Select(item => $"- {item}"));
            content = MemoryBankUtils.UpdateMarkdownSection(content, "Completed", $"Updated: {timestamp}\n\n{itemsList}");
        }
        
        if (inProgress.Count > 0)
        {
            string itemsList = string.Join("\n", inProgress.Select(item => $"- {item}"));
            content = MemoryBankUtils.UpdateMarkdownSection(content, "In Progress", $"Updated: {timestamp}\n\n{itemsList}");
        }
        
        if (planned.Count > 0)
        {
            string itemsList = string.Join("\n", planned.Select(item => $"- {item}"));
            content = MemoryBankUtils.UpdateMarkdownSection(content, "Planned", $"Updated: {timestamp}\n\n{itemsList}");
        }
        
        if (issues.Count > 0)
        {
            string itemsList = string.Join("\n", issues.Select(item => $"- {item}"));
            content = MemoryBankUtils.UpdateMarkdownSection(content, "Known Issues", $"Updated: {timestamp}\n\n{itemsList}");
        }
        
        // Write updated content
        File.WriteAllText(filePath, content);
    }
    
    public static void RecordDecision(string projectName, string title, string description, string rationale, List<string> alternatives)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        string projectPath = MemoryBankProjectOperations.EnsureProject(projectName);
        
        // Ensure decisions directory exists
        string decisionsDirectory = Path.Combine(projectPath, "decisions");
        if (!Directory.Exists(decisionsDirectory))
        {
            Directory.CreateDirectory(decisionsDirectory);
        }
        
        // Generate safe filename
        string datePrefix = DateTime.Now.ToString("yyyy-MM-dd");
        string safeTitle = MemoryBankUtils.CreateSafeFileName(title);
        string fileName = $"{datePrefix}-{safeTitle}.md";
        string filePath = Path.Combine(decisionsDirectory, fileName);
        
        // Build decision document
        var sb = new StringBuilder();
        sb.AppendLine($"# Decision: {title}");
        sb.AppendLine();
        sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd}");
        sb.AppendLine();
        sb.AppendLine("## Description");
        sb.AppendLine();
        sb.AppendLine(description);
        sb.AppendLine();
        sb.AppendLine("## Rationale");
        sb.AppendLine();
        sb.AppendLine(rationale);
        sb.AppendLine();
        
        if (alternatives.Count > 0)
        {
            sb.AppendLine("## Alternatives Considered");
            sb.AppendLine();
            foreach (string alternative in alternatives)
            {
                sb.AppendLine($"- {alternative}");
            }
            sb.AppendLine();
        }
        
        // Write decision file
        File.WriteAllText(filePath, sb.ToString());
        
        // Update active context to reference this decision
        string activeContextPath = Path.Combine(projectPath, "activeContext.md");
        if (File.Exists(activeContextPath))
        {
            string content = File.ReadAllText(activeContextPath);
            string decision = $"- [{title}](decisions/{fileName}) - {DateTime.Now:yyyy-MM-dd}";
            content = MemoryBankUtils.UpdateMarkdownSection(content, "Active Decisions", decision, true);
            File.WriteAllText(activeContextPath, content);
        }
    }
    
    public static List<string> CheckMemoryBankIntegrity(string projectName)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        string projectPath = Path.Combine(RootPath, projectName);
        if (!Directory.Exists(projectPath))
        {
            return MemoryBankConstants.CoreMemoryBankFiles.ToList();
        }
        
        var missingFiles = new List<string>();
        
        foreach (var fileName in MemoryBankConstants.CoreMemoryBankFiles)
        {
            string filePath = Path.Combine(projectPath, fileName);
            if (!File.Exists(filePath))
            {
                missingFiles.Add(fileName);
            }
        }
        
        return missingFiles;
    }
    
    public static Dictionary<string, List<string>> FindIncompleteMemoryBanks()
    {
        var result = new Dictionary<string, List<string>>();
        
        foreach (var project in MemoryBankProjectOperations.GetProjects())
        {
            var missingFiles = CheckMemoryBankIntegrity(project.Name);
            if (missingFiles.Count > 0)
            {
                result[project.Name] = missingFiles;
            }
        }
        
        return result;
    }
    
    public static List<string> RepairMemoryBank(string projectName)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        var missingFiles = CheckMemoryBankIntegrity(projectName);
        if (missingFiles.Count == 0)
        {
            return new List<string>();
        }
        
        string projectPath = MemoryBankProjectOperations.EnsureProject(projectName);
        var createdFiles = new List<string>();
        
        foreach (var fileName in missingFiles)
        {
            string filePath = Path.Combine(projectPath, fileName);
            string content = MemoryBankConstants.CoreFileTemplates.GetValueOrDefault(fileName, $"# {fileName}\n\n");
            
            File.WriteAllText(filePath, content);
            createdFiles.Add(fileName);
        }
        
        return createdFiles;
    }
    
    public static Dictionary<string, List<string>> RepairAllMemoryBanks()
    {
        var result = new Dictionary<string, List<string>>();
        
        foreach (var project in MemoryBankProjectOperations.GetProjects())
        {
            var createdFiles = RepairMemoryBank(project.Name);
            if (createdFiles.Count > 0)
            {
                result[project.Name] = createdFiles;
            }
        }
        
        return result;
    }
    
    public static bool CreateClineRules(string projectName)
    {
        MemoryBankValidation.ValidateProjectName(projectName);
        
        string projectPath = MemoryBankProjectOperations.EnsureProject(projectName);
        string filePath = Path.Combine(projectPath, ".clinerules");
        
        if (File.Exists(filePath))
        {
            return false;
        }
        
        var sb = new StringBuilder();
        sb.AppendLine("# Cline Memory Bank Rules");
        sb.AppendLine();
        sb.AppendLine("This file contains rules and preferences for working with this project's memory bank.");
        sb.AppendLine();
        sb.AppendLine("## Core Files");
        sb.AppendLine();
        sb.AppendLine("The following files are part of the memory bank core structure:");
        sb.AppendLine();
        
        foreach (var fileName in MemoryBankConstants.CoreMemoryBankFiles)
        {
            sb.AppendLine($"- {fileName}");
        }
        
        sb.AppendLine();
        sb.AppendLine("## Project Preferences");
        sb.AppendLine();
        sb.AppendLine("<!-- Add project-specific preferences here -->");
        sb.AppendLine();
        sb.AppendLine("## Learning");
        sb.AppendLine();
        sb.AppendLine("<!-- Document key insights and patterns learned during development -->");
        
        try
        {
            File.WriteAllText(filePath, sb.ToString());
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
