# NetworkInfo

[日本語版はこちら](README.ja.md)

The CSharpMcpServer NetworkInfo is a module that provides network information retrieval functionality for the Model Context Protocol (MCP) server. This component enables retrieving detailed information about network adapters and TCP connections.

## Features
- **GetNetworkInfo**: Retrieves comprehensive information about network adapters and active TCP connections

## API Details

### GetNetworkInfo
```csharp
public static string GetNetworkInfo()
```
Retrieves network information:
- **Description**: Retrieves information about network adapters and TCP connections
- **Returns**: JSON or YAML formatted information including:
  - **Network adapters**: Name, description, type, speed, MAC address, IPv4 addresses
  - **TCP connections**: Local endpoint, remote endpoint, connection state (showing up to 20 connections)

## Implementation Details

The NetworkInfo module uses the following .NET APIs to retrieve network information:

- `System.Net.NetworkInformation` namespace for network adapter information
- `System.Net.IPGlobalProperties` for TCP connection information
- `System.Net.Sockets` for socket-related data

The collected information is formatted in a structured way to make it easy to understand and parse.

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/NetworkInfo
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "NetworkInfo": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\NetworkInfo",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\NetworkInfo` with your actual project path

## Privacy Considerations

This module only accesses local network information and does not transmit any data. The information retrieved includes:

- Network adapter details (names, MAC addresses, IP addresses)
- Active TCP connections (local and remote endpoints)

No packet inspection or network traffic analysis is performed.