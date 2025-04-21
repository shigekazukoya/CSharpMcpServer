using System;
using System.IO;
using System.Linq;

namespace MemoryBankTools.Validation;

public static class MemoryBankValidation
{
    public static bool ValidateProjectName(string projectName)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentException("Project name cannot be empty");
        }
        
        if (projectName.Contains("..") || projectName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException($"Invalid project name: {projectName}");
        }
        
        return true;
    }
    
    public static bool ValidateFilePath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be empty");
        }
        
        // Allow subdirectories but check for dangerous patterns
        if (filePath.Contains("..") || 
            filePath.StartsWith("/") || 
            filePath.StartsWith("\\") ||
            Path.GetInvalidPathChars().Any(c => filePath.Contains(c)))
        {
            throw new ArgumentException($"Invalid file path: {filePath}");
        }
        
        return true;
    }
    
    public static bool IsValidFileName(string fileName)
    {
        return !string.IsNullOrEmpty(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }
    
    public static bool IsValidProjectName(string projectName)
    {
        return !string.IsNullOrEmpty(projectName) && 
               !projectName.Contains("..") && 
               projectName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }
    
    public static bool IsValidFilePath(string filePath)
    {
        return !string.IsNullOrEmpty(filePath) && 
               !filePath.Contains("..") && 
               !filePath.StartsWith("/") && 
               !filePath.StartsWith("\\") &&
               !Path.GetInvalidPathChars().Any(c => filePath.Contains(c));
    }
}
