using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace MemoryBankTools.Utils;

public static class MemoryBankUtils
{
    public static string UpdateMarkdownSection(string content, string sectionName, string newContent, bool append = false)
    {
        // Try to find section with different header levels
        for (int headerLevel = 1; headerLevel <= 6; headerLevel++)
        {
            string headerPattern = new string('#', headerLevel) + $" {sectionName}";
            
            // Find the section
            int startIndex = content.IndexOf(headerPattern);
            if (startIndex < 0) continue;
            
            // Find the start of the section's content
            int contentStartIndex = content.IndexOf('\n', startIndex);
            if (contentStartIndex < 0) continue;
            contentStartIndex++;
            
            // Find the start of the next section (if any)
            int nextSectionIndex = -1;
            for (int nextHeaderLevel = 1; nextHeaderLevel <= headerLevel; nextHeaderLevel++)
            {
                string nextHeaderPattern = new string('#', nextHeaderLevel) + " ";
                int nextHeaderIndex = content.IndexOf(nextHeaderPattern, contentStartIndex);
                
                if (nextHeaderIndex >= 0 && (nextSectionIndex < 0 || nextHeaderIndex < nextSectionIndex))
                {
                    nextSectionIndex = nextHeaderIndex;
                }
            }
            
            // Handle case where this is the last section
            string updatedContent;
            if (nextSectionIndex < 0)
            {
                if (append)
                {
                    // Append to existing content
                    updatedContent = content.Substring(0, contentStartIndex) + 
                                    (content.Length > contentStartIndex ? content.Substring(contentStartIndex) : "") + 
                                    "\n" + newContent;
                }
                else
                {
                    // Replace existing content
                    updatedContent = content.Substring(0, contentStartIndex) + newContent;
                }
            }
            else
            {
                if (append)
                {
                    // Append to existing content
                    string existingContent = content.Substring(contentStartIndex, nextSectionIndex - contentStartIndex);
                    updatedContent = content.Substring(0, contentStartIndex) + 
                                   existingContent + 
                                   (existingContent.EndsWith("\n\n") ? "" : "\n\n") + 
                                   newContent + "\n\n" + 
                                   content.Substring(nextSectionIndex);
                }
                else
                {
                    // Replace existing content
                    updatedContent = content.Substring(0, contentStartIndex) + 
                                   newContent + "\n\n" + 
                                   content.Substring(nextSectionIndex);
                }
            }
            
            return updatedContent;
        }
        
        // Section not found, add it at the end
        var sb = new StringBuilder(content);
        
        if (!content.EndsWith("\n"))
        {
            sb.AppendLine();
        }
        if (!content.EndsWith("\n\n"))
        {
            sb.AppendLine();
        }
        
        sb.AppendLine($"## {sectionName}");
        sb.AppendLine();
        sb.Append(newContent);
        
        return sb.ToString();
    }
    
    public static string CreateSafeFileName(string title)
    {
        // Replace spaces with hyphens
        string fileName = title.Replace(' ', '-');
        
        // Remove invalid characters
        fileName = Regex.Replace(fileName, "[^a-zA-Z0-9-_.]", "");
        
        // Ensure it's not too long
        if (fileName.Length > 50)
        {
            fileName = fileName.Substring(0, 50);
        }
        
        return fileName;
    }
    
    public static string SanitizeFilePath(string path)
    {
        // Remove any relative path components
        path = path.Replace("..", "");
        
        // Remove any leading directory separators
        path = path.TrimStart('/', '\\');
        
        // Replace multiple directory separators with single ones
        path = Regex.Replace(path, @"[/\\]+", Path.DirectorySeparatorChar.ToString());
        
        return path;
    }
    
    public static string CreateTimestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
