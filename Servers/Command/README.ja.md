# Command

[English version here](README.md)

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
                "absolute\\path\\to\\CSharpMCPServer\\Servers\\Command",
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