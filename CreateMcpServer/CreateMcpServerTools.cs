﻿using CreateMcpServer;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;
namespace CreateMcpServerTools;

[McpServerToolType]
public static class CreateMcpServerTools
{
    [McpServerTool, Description("Create a new MCP Server project")]
    public static string CreateMcpServerProject(string feature)
    {
        var folderPath = Path.Combine(CreateMcpServerPath.RootFolderPath, feature);

        // フォルダが既に存在するかチェック
        if (Directory.Exists(folderPath))
        {
            return $"フォルダ '{folderPath}' は既に存在します。";
        }

        // フォルダを作成
        Directory.CreateDirectory(folderPath);

        // プロジェクトファイルを作成
        CreateConsoleProject(folderPath, feature);

        // Program.cs ファイルを作成
        CreateProgramCs(folderPath);

        // ToolsクラスファイルをFeatureNameTools.csという形で作成
        CreateToolsFile(folderPath, feature);

        // ソリューションファイルにプロジェクトを追加
        if(AddProjectToSolution(feature, out var errorMesssage))
        {
            return errorMesssage;
        }

        return $"{feature} プロジェクトの作成が完了しました。";
    }

    private static void CreateConsoleProject(string folderPath, string feature)
    {
        // dotnet new console コマンドを実行
        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "new console",
            WorkingDirectory = folderPath,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(processInfo))
        {
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception("コンソールプロジェクトの作成に失敗しました。");
            }
        }
        
        // CSharpMcpServer.Common プロジェクト参照を追加
        AddProjectReference(folderPath, Path.Combine(CreateMcpServerPath.RootFolderPath, "CSharpMcpServer.Common", "CSharpMcpServer.Common.csproj"));
    }
    
    private static void AddProjectReference(string projectPath, string referenceProjectPath)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"add reference {referenceProjectPath}",
            WorkingDirectory = projectPath,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(processInfo))
        {
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"プロジェクト参照 {referenceProjectPath} の追加に失敗しました。");
            }
        }
    }

    private static void CreateProgramCs(string folderPath)
    {
        string programPath = Path.Combine(folderPath, "Program.cs");
        string programContent = @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly();

var app = builder.Build();

await app.RunAsync();
";

        File.WriteAllText(programPath, programContent);
    }

    private static void CreateToolsFile(string folderPath, string feature)
    {
        string toolsFilePath = Path.Combine(folderPath, $"{feature}Tools.cs");
        string toolsFileContent = $@"using ModelContextProtocol.Server;
using System.ComponentModel;
namespace {feature}Tools;

[McpServerToolType]
public static class {feature}Tools
{{
    [McpServerTool, Description("""")]
    public static void {feature}()
    {{
        // ここに実装を追加
    }}
}}
";

        File.WriteAllText(toolsFilePath, toolsFileContent);
    }

    private static bool AddProjectToSolution(string feature, out string errorMessage)
    {
        errorMessage = string.Empty;
        // ルートフォルダにある .sln ファイルを検索
        string rootPath = CreateMcpServerPath.RootFolderPath;
        string[] slnFiles = Directory.GetFiles(rootPath, "*.sln");

        if (slnFiles.Length == 0)
        {
            errorMessage ="ソリューションファイルが見つかりませんでした。";
            return false;
        }

        foreach (string slnFile in slnFiles)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"sln {slnFile} add {feature}\\{feature}.csproj",
                WorkingDirectory = rootPath,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    errorMessage =  $"プロジェクトをソリューション {slnFile} に追加できませんでした。";
                    return false;
                }
            }
        }

        return true;
    }
}
