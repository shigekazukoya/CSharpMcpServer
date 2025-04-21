# HardwareInfoRetriever

[日本語版はこちら](README.ja.md)

The CSharpMcpServer HardwareInfoRetriever is a module that provides system hardware and network information retrieval tools for the Model Context Protocol (MCP) server. This component allows easy access to detailed information about OS, CPU, GPU, memory, disks, and network adapters in YAML format.

## Features
- **HardwareInfoRetriever**: Get hardware information including OS, CPU, GPU, memory, and disks
- **SelectiveHardwareInfo**: Get hardware information for only the specified components
- **RefreshHardwareInfo**: Force a refresh of cached hardware information
- **GetNetworkInfo**: Get information about network adapters and TCP connections

## API Details

### HardwareInfoRetriever
```csharp
public static string HardwareInfoRetriever()
```
Gets system hardware information in YAML format:
- **Description**: Retrieves comprehensive hardware information with caching support. Information is cached for a specific duration to improve performance for repeated calls.
- **Returns**: YAML-formatted hardware information including:
  - **OS information**: OS version, platform, 64-bit status, machine name, user name, etc.
  - **CPU information**: Name, manufacturer, cores, logical processors, max clock speed
  - **GPU information**: Name, driver version, adapter RAM, video mode
  - **Memory information**: Total capacity and memory device details (type, capacity, manufacturer)
  - **Disk information**: Name, label, type, format, total size, free space, used space

### SelectiveHardwareInfo
```csharp
public static string SelectiveHardwareInfo(params string[] components)
```
Gets hardware information for only the specified components:
- **Description**: Retrieves only the specified components of hardware information. Valid components: os, cpu, gpu, memory/ram, storage/disk.
- **components**: Array of hardware components to retrieve
- **Returns**: YAML-formatted hardware information for the specified components

### RefreshHardwareInfo
```csharp
public static string RefreshHardwareInfo()
```
Forces a refresh of cached hardware information:
- **Description**: Forces a refresh of the cached hardware information, ignoring any existing cached data and retrieving the latest system information.
- **Returns**: Updated YAML-formatted hardware information

### GetNetworkInfo
```csharp
public static string GetNetworkInfo()
```
Gets system network information in YAML format:
- **Description**: Retrieves network information
- **Returns**: YAML-formatted network information including:
  - **Network adapters**: Name, description, type, speed, MAC address, IPv4 addresses
  - **TCP connections**: Local endpoint, remote endpoint, connection state (showing up to 20 connections)

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/HardwareInfoRetriever
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "HardwareInfoRetriever": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Servers\\HardwareInfoRetriever",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\HardwareInfoRetriever` with your actual project path

## Implementation Details

The HardwareInfoRetriever uses the following features to collect system information:

- WMI (Windows Management Instrumentation) queries from the `System.Management` namespace
- Network information APIs from the `System.Net.NetworkInformation` namespace
- System information APIs from `System.Environment` and `System.Runtime.InteropServices`
- Disk information from the `System.IO.DriveInfo` class

The collected data is formatted in YAML and organized hierarchically. Each section has appropriate error handling implemented so that even if specific information cannot be retrieved, the available information is still returned.