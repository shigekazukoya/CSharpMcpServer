# Time

CSharpMcpServer Timeは、Model Context Protocol (MCP) サーバーの時間関連機能を提供するモジュールです。このコンポーネントは、システム時刻の取得などの基本的な時間操作を可能にします。

## 機能
- **GetCurrentTime**: 現在のシステム時刻を取得し、フォーマットされた文字列として返す

## API詳細

### GetCurrentTime
```csharp
public static string GetCurrentTime()
```
現在のシステム時刻を取得し、フォーマットされた文字列として返します：
- **戻り値**: 一般的な日時形式でフォーマットされた現在のシステム時刻（"G"形式）

## Usage with Claude Desktop
- Add this to your claude_desktop_config.json
- dotnet 8.0以上が必要
- ビルドが必要

```
{
    "mcpServers": {
        "Time": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Time",
                "--no-build"
            ]
        }
    }
}
```

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\Time`の部分を実際のプロジェクトパスに置き換えてください

## タイムゾーン

GetCurrentTimeは、サーバーが実行されているシステムのローカルタイムゾーンを使用します。UTC時間が必要な場合は、このモジュールの拡張が必要になる場合があります。

## フォーマット

時間は標準的な日時フォーマットで返されます。これにより、Claude AIによる時間情報の取得と処理が容易になります。

---

# Time (English)

The CSharpMcpServer Time is a module that provides time-related functionality for the Model Context Protocol (MCP) server. This component enables basic time operations such as retrieving the current system time.

## Features
- **GetCurrentTime**: Gets the current system time and returns it as a formatted string

## API Details

### GetCurrentTime
```csharp
public static string GetCurrentTime()
```
Gets the current system time and returns it as a formatted string:
- **Returns**: The current system time formatted in the general date/time format ("G" format)

## Usage with Claude Desktop
- Add this to your claude_desktop_config.json
- Requires dotnet 8.0 or higher
- Build is required

```
{
    "mcpServers": {
        "Time": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Time",
                "--no-build"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\Time` with your actual project path

## Time Zone

GetCurrentTime uses the local time zone of the system where the server is running. If you need UTC time, you may need to extend this module.

## Format

The time is returned in a standard date/time format. This makes it easy for Claude AI to retrieve and process time information.
