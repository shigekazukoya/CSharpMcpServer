# VisualStudio

[日本語版はこちら](README.ja.md)

The CSharpMcpServer VisualStudio module provides tools for retrieving information and interacting with Visual Studio instances. This component allows access to information about running Visual Studio instances, open files, active solutions, and more.

## Features

- **GetAllVSInfo**: Get information about all running Visual Studio instances
- **GetActiveSolution**: Get information about the active solution
- **GetActiveFileContent**: Get the content of the currently selected file
- **GetOpenFiles**: Get information about all files open in Visual Studio

## API Details

### GetAllVSInfo

```csharp
public string GetAllVSInfo()
```

Gets information about all running Visual Studio instances:
- **Description**: Gets information about all running Visual Studio instances
- **Returns**: JSON-formatted Visual Studio instance information

### GetActiveSolution

```csharp
public string GetActiveSolution()
```

Gets information about the active solution:
- **Description**: Gets information about the active solution
- **Returns**: JSON-formatted solution information, or an error message if no running Visual Studio instance or solution is found

### GetActiveFileContent

```csharp
public string GetActiveFileContent()
```

Gets the content of the currently selected file:
- **Description**: Gets the content of the currently selected file
- **Returns**: Path and content of the selected file, or an error message if no running Visual Studio instance is found

### GetOpenFiles

```csharp
public string GetOpenFiles()
```

Gets information about all files open in Visual Studio:
- **Description**: Gets information about all files open in Visual Studio
- **Returns**: JSON-formatted open file information, or an error message if no instance is found

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/VisualStudio
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "VisualStudio": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\VisualStudio",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\VisualStudio` with your actual project path

## Security

This server only accesses and retrieves information from running Visual Studio instances. File content retrieval is limited to files that are open in Visual Studio.