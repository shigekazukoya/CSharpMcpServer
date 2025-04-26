using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;

namespace DotnetBuildTools;

[McpServerToolType]
public static class DotnetBuildTools
{
    [McpServerTool, Description("指定されたプロジェクトやソリューションをdotnetコマンドを使用してビルドします")]
    public static BuildResult BuildProject(string projectPath, string configuration = "Debug", string framework = "", bool verbose = false)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            return new BuildResult
            {
                Success = false,
                Output = "プロジェクトパスが指定されていません",
                ErrorOutput = "プロジェクトパスは必須パラメーターです"
            };
        }

        var arguments = $"build \"{projectPath}\" --configuration {configuration}";
        
        if (!string.IsNullOrEmpty(framework))
        {
            arguments += $" --framework {framework}";
        }

        if (verbose)
        {
            arguments += " -v:detailed";
        }

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process
        {
            StartInfo = processStartInfo
        };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return new BuildResult
        {
            Success = process.ExitCode == 0,
            Output = outputBuilder.ToString(),
            ErrorOutput = errorBuilder.ToString(),
            ExitCode = process.ExitCode
        };
    }

    [McpServerTool, Description("ソリューションファイル（.sln）に含まれるプロジェクト（.csproj）の依存関係を分析して表示します")]
    public static ProjectDependencies GetProjectDependencies(string solutionPath)
    {
        if (string.IsNullOrEmpty(solutionPath))
        {
            return new ProjectDependencies
            {
                Success = false,
                ErrorMessage = "ソリューションパスが指定されていません"
            };
        }

        if (!File.Exists(solutionPath))
        {
            return new ProjectDependencies
            {
                Success = false,
                ErrorMessage = $"指定されたパス '{solutionPath}' にソリューションファイルが見つかりません"
            };
        }

        var solutionDir = Path.GetDirectoryName(solutionPath);
        var projects = new List<ProjectInfo>();
        
        try
        {
            // ソリューションファイルを解析してプロジェクトを見つける
            var solutionContent = File.ReadAllLines(solutionPath);
            var projectPaths = new List<string>();
            
            foreach (var line in solutionContent)
            {
                if (line.StartsWith("Project("))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        var projectPath = parts[1].Trim().Trim('\"');
                        if (projectPath.EndsWith(".csproj"))
                        {
                            var fullPath = Path.Combine(solutionDir, projectPath);
                            if (File.Exists(fullPath))
                            {
                                projectPaths.Add(fullPath);
                            }
                        }
                    }
                }
            }
            
            // 各プロジェクトの依存関係を分析
            foreach (var projectPath in projectPaths)
            {
                var projectInfo = AnalyzeProject(projectPath);
                projects.Add(projectInfo);
            }
            
            // プロジェクト間の依存関係を構築
            foreach (var project in projects)
            {
                foreach (var dependency in project.ProjectReferences)
                {
                    var dependencyProject = projects.Find(p => p.ProjectPath.EndsWith(dependency));
                    if (dependencyProject != null)
                    {
                        project.Dependencies.Add(dependencyProject.ProjectName);
                    }
                }
            }
            
            return new ProjectDependencies
            {
                Success = true,
                Projects = projects
            };
        }
        catch (Exception ex)
        {
            return new ProjectDependencies
            {
                Success = false,
                ErrorMessage = $"依存関係の分析中にエラーが発生しました: {ex.Message}"
            };
        }
    }
    
    private static ProjectInfo AnalyzeProject(string projectPath)
    {
        var projectInfo = new ProjectInfo
        {
            ProjectName = Path.GetFileNameWithoutExtension(projectPath),
            ProjectPath = projectPath,
            PackageReferences = new List<string>(),
            ProjectReferences = new List<string>(),
            Dependencies = new List<string>()
        };
        
        try
        {
            var doc = XDocument.Load(projectPath);
            var ns = doc.Root.Name.Namespace;
            
            // PackageReference要素を探す
            var packageRefs = doc.Descendants(ns + "PackageReference");
            foreach (var packageRef in packageRefs)
            {
                var packageName = packageRef.Attribute("Include")?.Value;
                var version = packageRef.Attribute("Version")?.Value;
                if (!string.IsNullOrEmpty(packageName))
                {
                    projectInfo.PackageReferences.Add($"{packageName} ({version ?? "?"})");
                }
            }
            
            // ProjectReference要素を探す
            var projectRefs = doc.Descendants(ns + "ProjectReference");
            foreach (var projectRef in projectRefs)
            {
                var includePath = projectRef.Attribute("Include")?.Value;
                if (!string.IsNullOrEmpty(includePath))
                {
                    projectInfo.ProjectReferences.Add(Path.GetFileName(includePath));
                }
            }
        }
        catch
        {
            // XMLの解析に失敗した場合は空のプロジェクト情報を返す
        }
        
        return projectInfo;
    }
}

public class BuildResult
{
    public bool Success { get; set; }
    public string Output { get; set; }
    public string ErrorOutput { get; set; }
    public int ExitCode { get; set; }
}

public class ProjectDependencies
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public List<ProjectInfo> Projects { get; set; } = new List<ProjectInfo>();
}

public class ProjectInfo
{
    public string ProjectName { get; set; }
    public string ProjectPath { get; set; }
    public List<string> PackageReferences { get; set; }
    public List<string> ProjectReferences { get; set; }
    public List<string> Dependencies { get; set; }
}
