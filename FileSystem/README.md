# FileSystem

CSharpMcpServer FileSystemは、Model Context Protocol (MCP) サーバーのファイルシステム操作ツールを提供するモジュールです。このコンポーネントは、ファイルの読み書き、削除、ディレクトリ構造の取得などの基本的なファイルシステム操作を可能にします。

## 機能
- **WriteFile**: ファイルにテキストを書き込み、必要に応じて親ディレクトリを作成
- **ReadFile**: ファイルからテキストを読み込み
- **Delete**: ファイルまたはディレクトリを削除（ディレクトリの場合は再帰的削除オプションあり）
- **GetFolderStructure**: 指定されたディレクトリの階層構造を取得（.gitignoreに基づく除外処理付き）

## Usage with Claude Desktop
- Add this to your claude_desktop_config.json
- dotnet 8.0以上が必要
- ビルドが必要

```
{
    "mcpServers": {
        "FileSystem": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\FileSystem",
                "--no-build",
                "--",
                "/path/to/other/allowed/dir"
            ]
        }
    }
}
```

## .gitignore対応

GetFolderStructureには、.gitignoreファイルを解析します：

- ルートディレクトリの.gitignoreを読み込み
- サブディレクトリの.gitignoreも適切に処理
- 絶対パスと相対パスのパターン対応
- ワイルドカード（*、**、？）の変換
- ディレクトリ専用パターン（末尾の/）対応

さらに、以下のような一般的なファイル/ディレクトリが自動的に除外されます：

- .git、.nextディレクトリ
- bin、obj、target、distなどのビルド出力
- .vs、*.user、*.suoなどのVisual Studioファイル
- node_modules、packagesなどのパッケージディレクトリ
- ログファイル、バックアップファイル、キャッシュファイル
