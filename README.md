# CSharpMcpServer

[日本語版はこちら](README.ja.md)

The C# implementation of Model Context Protocol (MCP) servers provides extensions for the Claude Desktop API. This project offers various tools for file system operations, hardware information retrieval, web content fetching, and time retrieval.

## Tools

- [FileSystem](FileSystem/Servers/README.md) - Provides file system operation functionality
  - File reading, writing, editing, and deletion
  - Directory creation and folder structure retrieval
  - Moving files and directories
  - ZIP compression and extraction
  - Opening files/folders with default applications
  - Opening files with specific applications
  - Retrieving file association information

- [Command](Command/Servers/README.md) - Provides shell command execution functionality
  - Command execution in PowerShell
  - Command execution in WSL Bash
  - Timeout management capability

- [HardwareInfoRetriever](HardwareInfoRetriever/Servers/README.md) - Retrieves PC and network information
  - OS, CPU, GPU, memory, and disk information (with caching support)
  - Selective retrieval of specific hardware components
  - Forced refresh of cached hardware information

- [Time](Time/Servers/README.md) - Retrieves the current time
  - Provides the system's local time as a formatted string

- [Web](Web/Servers/README.md) - Provides web browser integration functionality
  - Opening URLs in the default browser

- [WebFetch](WebFetch/Servers/README.md) - Retrieves content from URLs
  - Extracts main content from web pages by removing ads and navigation
  - Content identification algorithm using semantic HTML elements

- [NetworkInfo](NetworkInfo/Servers/README.md) - Retrieves network information
  - Network adapter information
  - TCP connection information

- [VisualStudio](VisualStudio/Servers/README.md) - Retrieves Visual Studio information
  - Information about running Visual Studio instances
  - Active solution information
  - Content of currently selected files
  - Information about all open files

- [Rss](Rss/Servers/README.md) - Processes RSS feeds
  - Processes multiple RSS feeds simultaneously
  - Outputs content as markdown-formatted links

- [CreateMcpServer](CreateMcpServer/Servers/README.md) - Tool for creating MCP server projects
  - Automatic generation of new MCP server projects (CreateMcpServerProject)
  - Project creation and configuration prompts (CreateMcpServerPrompts)
  - Automated creation of necessary folder structures and project files
  - Automatic integration with solutions

## Utilities

- [McpInsight](CSharpMcpServer.Utility/McpInsight/Servers/README.md) - Debug and monitoring tool for MCP servers
  - Real-time monitoring of communication between MCP clients and servers
  - Interactive testing of MCP server commands
  - Message display in an easy-to-analyze format
  - Usable in all phases of stdio-based MCP server development

## Usage

Each tool is implemented as an independent dotnet project and can be built and used separately. For detailed usage instructions, please refer to the Servers/README of each tool.

## License
This project is licensed under the [MIT License](LICENSE.txt).