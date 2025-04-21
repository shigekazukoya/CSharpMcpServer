# CreateMcpServer

[日本語版はこちら](README.ja.md)

The CSharpMcpServer CreateMcpServer is a tool module for easily creating MCP (Model Context Protocol) server projects. It automates the creation and setup of new MCP server projects, streamlining the development process.

## Features

- **CreateMcpServerProject**: Creates a new MCP server project
  - Creates the necessary folder structure
  - Initializes a .NET Console project
  - Adds common library references
  - Creates a Program.cs file with MCP server configuration
  - Automatically generates a feature-specific Tools class file
  - Adds the project to the solution

- **CreateMcpServerPrompts**: Provides prompts related to MCP server projects
  - Prompts for creating new projects
  - Prompts for updating README.md files

## API Details

### CreateMcpServerProject

```csharp
public static string CreateMcpServerProject(string feature)
```

Creates a new MCP server project:
- **Description**: Create a new MCP Server project
- **feature**: The name of the feature for the project (forms the basis for folder and class names)
- **Returns**: A message describing the processing result

### CreateMcpServerPrompts

A class that provides prompts related to MCP server projects.

#### CreateMcpServerProjectPrompt

```csharp
public static string CreateMcpServerProjectPrompt(string feature)
```

- **Description**: Prompt for creating a new MCP server project
- **feature**: The name of the feature for the project
- **Returns**: Prompt string

#### UpdateReadMePrompt

```csharp
public static string UpdateReadMePrompt(string feature)
```

- **Description**: Prompt for updating project README.md files
- **feature**: The name of the feature for the project
- **Returns**: Prompt string

## Usage

```csharp
// Create a new MCP Server project
CreateMcpServerProject("MyFeature");
```

This performs the following operations:

1. Creates the `C:\Projects\MCPServer\CSharpMcpServer\MyFeature` directory
2. Initializes a new console project using the `dotnet new console` command
3. Adds a reference to the `CSharpMcpServer.Common` project
4. Creates a basic `Program.cs` with MCP functionality enabled
5. Creates a template file `MyFeatureTools.cs` for feature implementation
6. Adds the project to existing solution files

## Project Creation Automation

Using CreateMcpServer eliminates the need to manually write standard boilerplate code for each MCP feature, allowing you to focus on developing new functionality.

## Solution Integration

The created projects are automatically added to existing solution files, enabling you to start development immediately in Visual Studio.