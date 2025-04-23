# CSharpMcpServer

[日本語版はこちら](README.ja.md)

The C# implementation of Model Context Protocol (MCP) servers provides extensions for the Claude Desktop API. This project offers various tools for file system operations, hardware information retrieval, web content fetching, and time retrieval.

## Usage

```Powershell
git clone https://github.com/shigekazukoya/CSharpMcpServer
cd CSharpMcpServer
dotnet build
```

Each McpServer tool is implemented as an independent dotnet project and can be built and used separately. For detailed usage instructions, please refer to the Servers/README of each tool.

## Utilities

- [McpInsight](McpInsight/README.md) - Debug and monitoring tool for MCP servers
  - Real-time monitoring of communication between MCP clients and servers
  - Interactive testing of MCP server commands
  - Message display in an easy-to-analyze format
  - Usable in all phases of stdio-based MCP server development

## Servers

- [FileSystem](Servers/FileSystem/README.md) - Provides file system operation functionality
  - File reading, writing, editing, and deletion
  - Directory creation and folder structure retrieval
  - ZIP compression and extraction
  - Opening files/folders with default applications

- [PowerShell](Servers/PowerShell/README.md) - Provides a secure interface for PowerShell command execution

- [HardwareInfoRetriever](Servers/HardwareInfoRetriever/README.md) - OS, CPU, GPU, memory, and disk information

- [Time](Servers/Time/README.md) - Retrieves the current time

- [Web](Servers/Web/README.md) - Opening URLs in the default browser

- [WebFetch](Servers/WebFetch/README.md) - Extracts main content from web pages by removing ads and navigation
  - Content identification algorithm using semantic HTML elements

- [NetworkInfo](Servers/NetworkInfo/README.md) - Retrieves network adapter and TCP connection information

- [VisualStudio](Servers/VisualStudio/README.md) - Retrieves Visual Studio information
  - Content of currently selected files
  - Information about all open files

- [Rss](Servers/Rss/README.md) - Processes RSS feeds

- [CreateMcpServer](Servers/CreateMcpServer/README.md) - Tool for creating MCP server projects

## License
This project is licensed under the [MIT License](LICENSE.txt).