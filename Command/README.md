# Command

[日本語版はこちら](README.ja.md)

The CSharpMcpServer Command is a module that provides shell command execution functionality for the Model Context Protocol (MCP) server. This component enables command execution in PowerShell and WSL Bash environments, with timeout management capability.

## Features
- **PowerShell**: Command execution in PowerShell
- **Bash**: Command execution in WSL Bash environment

## API Details

### PowerShell
```csharp
public static ShellResult PowerShell(ShellOptions options)
```
Executes a command in PowerShell:
- **Description**: Executes a command in PowerShell
- **options**: Shell command options
  - **Command**: The command to execute
  - **Description**: A brief description of the command (about 5-10 words)
  - **Timeout**: Timeout for command execution in milliseconds (maximum 600000)
- **Returns**: ShellResult object
  - **Stdout**: Standard output result of command execution
  - **Stderr**: Standard error output result of command execution
  - **Interrupted**: True if the command was interrupted by timeout etc.
  - **IsImage**: True if the output is image data
  - **Sandbox**: True if the command was executed in a sandbox environment

### Bash
```csharp
public static ShellResult Bash(ShellOptions options)
```
Executes a command in WSL Bash environment:
- **Description**: Executes a command in WSL Bash environment
- **options**: Shell command options
  - **Command**: The command to execute
  - **Description**: A brief description of the command (about 5-10 words)
  - **Timeout**: Timeout for command execution in milliseconds (maximum 600000)
- **Returns**: ShellResult object
  - **Stdout**: Standard output result of command execution
  - **Stderr**: Standard error output result of command execution
  - **Interrupted**: True if the command was interrupted by timeout etc.
  - **IsImage**: True if the output is image data
  - **Sandbox**: True if the command was executed in a sandbox environment

## Implementation Details

CommandTools implements the following features:

1. **Command Execution**: Flexible command execution in PowerShell and WSL Bash environments
2. **Output Stream Processing**: Asynchronous capture of standard output and standard error output
3. **Timeout Management**: Automatic termination of command execution exceeding specified time (maximum 10 minutes)
4. **Long Output Handling**: Automatic truncation of outputs exceeding 30,000 characters

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/Command
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "Command": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Command",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\Command` with your actual project path

## Security

This module involves security risks as it executes system commands. The following limitations are implemented:

1. **Timeout Limit**: Commands automatically terminate after a maximum of 10 minutes (or 30 minutes if no setting is provided)
2. **Output Limit**: Excessively long outputs (over 30,000 characters) are truncated