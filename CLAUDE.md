# CSharpMcpServer

## Project Overview

CSharpMcpServer is a C# implementation of Model Context Protocol (MCP) servers that provides extensions for the Claude Desktop API. This project offers various tools for file system operations, hardware information retrieval, web content fetching, time retrieval, and more.

The Model Context Protocol (MCP) allows AI assistants like Claude to interact with external tools and services through a standardized interface, enabling functionality beyond the AI's core capabilities.

## Technologies Used

- C# / .NET 8.0
- Model Context Protocol (MCP)
- Avalonia UI (for McpInsight tool)
- PowerShell integration
- WMI (Windows Management Instrumentation)
- ZIP compression/extraction
- RSS feed processing

## Project Structure

### Key Components:

- **Common**: Shared code and utilities used across the project
- **McpInsight**: Debug and monitoring tool for MCP servers
  - Real-time monitoring of communication between MCP clients and servers
  - Interactive testing of MCP server commands
  - Message display in an easy-to-analyze format

### Servers:
The project includes multiple specialized server modules:

- **FileSystem**: Provides file operations (read, write, edit, delete, zip/unzip)
- **PowerShell**: Secure interface for PowerShell command execution
- **HardwareInfoRetriever**: Gets OS, CPU, GPU, memory, and disk information
- **Time**: Retrieves current time information
- **Web**: Opens URLs in the default browser
- **WebFetch**: Extracts content from web pages by removing ads and navigation
- **NetworkInfo**: Retrieves network adapter and TCP connection information
- **VisualStudio**: Gets information about Visual Studio files
- **Rss**: Processes RSS feeds
- **CreateMcpServer**: Tool for creating new MCP server projects
- **Claude_md**: Creates and manages CLAUDE.md files for project documentation

## Development Workflow

1. **Build the project**:
   ```
   git clone https://github.com/shigekazukoya/CSharpMcpServer
   cd CSharpMcpServer
   dotnet build
   ```

2. **Use McpInsight for debugging and testing**:
   - Build and run the McpInsight tool for real-time monitoring
   - Test MCP server commands interactively

3. **Create new MCP servers**:
   - Use the CreateMcpServer tool to generate new server projects
   - Implement the required functionality following the MCP protocol

4. **Integration with Claude Desktop**:
   - Configure Claude Desktop to use these MCP servers via `claude_desktop_config.json`

## Common Commands

### Building the Project
```powershell
dotnet build
```

### Running a specific server
```powershell
dotnet run --project Servers/[ServerName]
```

### Integration with Claude Desktop
To use with Claude Desktop, add configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "FileSystem": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "path\\to\\CSharpMCPServer\\Servers\\FileSystem",
                "--no-build",
                "--",
                "/path/to/allowed/dir"
            ]
        }
    }
}
```

## Security Considerations

- FileSystem server restricts access to only specified directories
- PowerShell server uses an allowed_commands.json list for security
- All servers implement appropriate input validation

## API Documentation

Each server module has its own README.md file with detailed API documentation. Refer to these files for specific usage instructions and examples.
