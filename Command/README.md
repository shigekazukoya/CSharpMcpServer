# Command

CSharpMcpServer Command は、Model Context Protocol (MCP) サーバーのシェルコマンド実行機能を提供するモジュールです。このコンポーネントは、PowerShellやWSL Bash環境でのコマンド実行を可能にし、タイムアウト管理機能も備えています。

## 機能
- **PowerShell**: PowerShellでコマンドを実行
- **Bash**: WSL Bash環境でコマンドを実行

## API詳細

### PowerShell
```csharp
public static ShellResult PowerShell(ShellOptions options)
```
PowerShellでコマンドを実行します：
- **説明**: PowerShellでコマンドを実行します
- **options**: シェルコマンドオプション
  - **Command**: 実行するコマンドを指定します
  - **Description**: コマンドの簡潔な説明（5-10語程度）を指定します
  - **Timeout**: コマンド実行のタイムアウト時間をミリ秒単位で指定します（最大600000）
- **戻り値**: ShellResult オブジェクト
  - **Stdout**: コマンド実行の標準出力結果
  - **Stderr**: コマンド実行の標準エラー出力結果
  - **Interrupted**: コマンドがタイムアウトなどで中断された場合はtrue
  - **IsImage**: 出力が画像データの場合はtrue
  - **Sandbox**: コマンドがサンドボックス環境で実行された場合はtrue

### Bash
```csharp
public static ShellResult Bash(ShellOptions options)
```
WSL Bash環境でコマンドを実行します：
- **説明**: WSL Bash環境でコマンドを実行します
- **options**: シェルコマンドオプション
  - **Command**: 実行するコマンドを指定します
  - **Description**: コマンドの簡潔な説明（5-10語程度）を指定します
  - **Timeout**: コマンド実行のタイムアウト時間をミリ秒単位で指定します（最大600000）
- **戻り値**: ShellResult オブジェクト
  - **Stdout**: コマンド実行の標準出力結果
  - **Stderr**: コマンド実行の標準エラー出力結果
  - **Interrupted**: コマンドがタイムアウトなどで中断された場合はtrue
  - **IsImage**: 出力が画像データの場合はtrue
  - **Sandbox**: コマンドがサンドボックス環境で実行された場合はtrue

## 実装の詳細

CommandTools は以下の機能を実装しています：

1. **コマンド実行**: PowerShellおよびWSL Bash環境での柔軟なコマンド実行
2. **出力ストリーム処理**: 標準出力と標準エラー出力の非同期キャプチャ
3. **タイムアウト管理**: 指定された時間を超えるコマンド実行の自動終了（最大10分）
4. **長い出力の処理**: 30,000文字を超える出力の自動切り捨て

## 使用方法

### コンパイルとビルド
- dotnet 8.0以上が必要
- リポジトリのルートディレクトリから以下のコマンドを実行:

```bash
dotnet build CSharpMcpServer/Command
```

### Claude Desktopとの連携
Claude Desktopで使用するには、以下の設定を`claude_desktop_config.json`に追加します:

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

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\Command`の部分を実際のプロジェクトパスに置き換えてください

## セキュリティ

このモジュールはシステムコマンドを実行するため、セキュリティ上のリスクを伴います。以下の制限が実装されています：

1. **タイムアウト制限**: コマンドは最大10分で自動的に終了します（設定がない場合は30分）
2. **出力制限**: 長すぎる出力（30,000文字超）は切り捨てられます

---

# Command (English)

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
