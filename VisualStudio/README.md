# VisualStudio

CSharpMcpServer VisualStudio モジュールは、Visual Studio インスタンスの情報取得や操作を行うためのツールを提供します。このコンポーネントにより、実行中のVisual Studioの情報、開いているファイル、アクティブなソリューションなどの情報にアクセスできます。

## 機能

- **GetAllVSInfo**: すべての実行中のVisual Studioインスタンス情報を取得
- **GetActiveSolution**: アクティブなソリューションの情報を取得
- **GetActiveFileContent**: 現在選択されているファイルの内容を取得
- **GetOpenFiles**: Visual Studioで開いているすべてのファイル情報を取得

## API詳細

### GetAllVSInfo

```csharp
public string GetAllVSInfo()
```

実行中のすべてのVisual Studioインスタンス情報を取得します：
- **説明**: 実行中のすべてのVisual Studioインスタンス情報を取得します
- **戻り値**: JSON形式のVisual Studioインスタンス情報

### GetActiveSolution

```csharp
public string GetActiveSolution()
```

アクティブなソリューションの情報を取得します：
- **説明**: アクティブなソリューションの情報を取得します
- **戻り値**: JSON形式のソリューション情報、または実行中のVisual Studioインスタンスやソリューションが見つからない場合はエラーメッセージ

### GetActiveFileContent

```csharp
public string GetActiveFileContent()
```

現在選択されているファイルの内容を取得します：
- **説明**: 現在選択されているファイルの内容を取得します
- **戻り値**: 選択されているファイルのパスと内容、または実行中のVisual Studioインスタンスが見つからない場合はエラーメッセージ

### GetOpenFiles

```csharp
public string GetOpenFiles()
```

Visual Studioで開いているすべてのファイル情報を取得します：
- **説明**: Visual Studioで開いているすべてのファイル情報を取得します
- **戻り値**: JSON形式の開いているファイル情報、またはインスタンスが見つからない場合はエラーメッセージ

## 使用方法

### コンパイルとビルド
- dotnet 8.0以上が必要
- リポジトリのルートディレクトリから以下のコマンドを実行:

```bash
dotnet build CSharpMcpServer/VisualStudio
```

### Claude Desktopとの連携
Claude Desktopで使用するには、以下の設定を`claude_desktop_config.json`に追加します:

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

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\VisualStudio`の部分を実際のプロジェクトパスに置き換えてください

## セキュリティ

このサーバーは、実行中のVisual Studioインスタンスのみにアクセスし、情報を取得します。ファイルの内容の取得は、Visual Studioで開かれているファイルに制限されます。

---

# VisualStudio (English)

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
