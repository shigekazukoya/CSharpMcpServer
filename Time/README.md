# Time

CSharpMcpServer Timeは、Model Context Protocol (MCP) サーバーの時間関連機能を提供するモジュールです。このコンポーネントは、システム時刻の取得などの基本的な時間操作を可能にします。

## 機能
- **GetCurrentTime**: 現在のシステム時刻を取得し、フォーマットされた文字列として返す

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

## タイムゾーン

GetCurrentTimeは、サーバーが実行されているシステムのローカルタイムゾーンを使用します。UTC時間が必要な場合は、このモジュールの拡張が必要になる場合があります。

## フォーマット

時間は標準的な日時フォーマットで返されます。これにより、Claude AIによる時間情報の取得と処理が容易になります。