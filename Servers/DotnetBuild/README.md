# DotnetBuild

[日本語版はこちら](README.ja.md)

The CSharpMcpServer DotnetBuild is a tool module for building and analyzing .NET projects using the dotnet CLI. It provides functionality to build projects and analyze dependencies in solution files.

## Features

- **BuildProject**: Builds specified .NET projects using the dotnet CLI
  - Supports different configuration settings (Debug/Release)
  - Allows targeting specific frameworks
  - Provides verbose output options
  - Returns detailed build results including success status and output

- **GetProjectDependencies**: Analyzes and displays project dependencies in a solution file
  - Parses solution files to identify included projects
  - Analyzes project references between projects
  - Identifies package dependencies for each project
  - Constructs dependency tree relationships

## API Details

### BuildProject

```csharp
public static BuildResult BuildProject(string projectPath, string configuration = "Debug", string framework = "", bool verbose = false)
```

Builds a specified .NET project:
- **Description**: Builds the specified project or solution using the dotnet command
- **projectPath**: The path to the project or solution file to build
- **configuration**: The build configuration to use (Debug/Release)
- **framework**: The target framework (optional)
- **verbose**: Whether to output detailed build information
- **Returns**: A BuildResult object containing build outcome, output, and error information

### GetProjectDependencies

```csharp
public static ProjectDependencies GetProjectDependencies(string solutionPath)
```

Analyzes dependencies between projects in a solution:
- **Description**: Analyzes and displays project dependencies in a solution file (.sln)
- **solutionPath**: The path to the solution file to analyze
- **Returns**: A ProjectDependencies object containing the dependency analysis results

## Usage

```csharp
// Build a project with default settings
var result = BuildProject(@"C:\Projects\MyProject\MyProject.csproj");

// Build a project with Release configuration
var releaseResult = BuildProject(@"C:\Projects\MyProject\MyProject.csproj", "Release");

// Analyze dependencies in a solution
var dependencies = GetProjectDependencies(@"C:\Projects\MySolution.sln");
```

## Result Classes

### BuildResult

Contains the results of a build operation:
- **Success**: Boolean indicating if the build was successful
- **Output**: Standard output from the build process
- **ErrorOutput**: Error output from the build process
- **ExitCode**: The exit code returned by the build process

### ProjectDependencies

Contains the results of dependency analysis:
- **Success**: Boolean indicating if the analysis was successful
- **ErrorMessage**: Error message if the analysis failed
- **Projects**: List of ProjectInfo objects representing projects in the solution

### ProjectInfo

Represents information about a project:
- **ProjectName**: The name of the project
- **ProjectPath**: The full path to the project file
- **PackageReferences**: NuGet package dependencies
- **ProjectReferences**: Project references
- **Dependencies**: Names of dependent projects

### Compilation and Building

- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/DotnetBuild
```

### Integration with Claude Desktop

To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "DotnetBuild": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Servers\\DotnetBuild",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\path\to\CSharpMCPServer\DotnetBuild` with your actual project path
