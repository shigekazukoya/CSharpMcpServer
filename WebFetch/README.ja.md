# WebFetch

[English version here](README.md)

CSharpMcpServer WebFetch は、Model Context Protocol (MCP) サーバーのウェブコンテンツ取得ツールを提供するモジュールです。このコンポーネントは、ウェブページの取得とメインコンテンツの抽出を可能にします。

## 機能
- **ExtractMainContentFromUrl**: URLからコンテンツを取得し、広告、ナビゲーション、スクリプトなどを削除してメインテキストを抽出します

## API詳細

### ExtractMainContentFromUrl
```csharp
public static async Task<string> ExtractMainContentFromUrl(string url)
```
URLからメインコンテンツを抽出します：
- **説明**: URLコンテンツを取得し、広告、ナビゲーション、スクリプトを削除して意味的なHTML要素を使用してメインコンテンツを特定します
- **url**: コンテンツを取得するURLを指定します
- **戻り値**: 広告やナビゲーションなどの不要な要素を削除した、ウェブページのメインコンテンツテキスト

## 実装の詳細

WebFetchTools は以下のステップでコンテンツを抽出します：

1. **HTMLの取得**: 指定されたURLからHTMLコンテンツを取得します（User-Agentヘッダー付き）
2. **不要な要素の削除**: 広告、ナビゲーション、スクリプト、ヘッダー、フッターなどのノイズとなる要素を削除します
3. **メインコンテンツの抽出**: 以下の優先順位でメインコンテンツを特定します：
   - article要素
   - main要素
   - "content"や"article"などのクラス/ID名を持つdiv要素
   - 最も長いテキストを含むdiv要素
   - body要素全体（最終手段）
4. **テキストのクリーンアップ**: 連続する空白の削除、空の行の削除、HTMLエンティティのデコードを行います

## 使用方法

### コンパイルとビルド
- dotnet 8.0以上が必要
- リポジトリのルートディレクトリから以下のコマンドを実行:

```bash
dotnet build CSharpMcpServer/WebFetch
```

### Claude Desktopとの連携
Claude Desktopで使用するには、以下の設定を`claude_desktop_config.json`に追加します:

```json
{
    "mcpServers": {
        "WebFetch": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\WebFetch",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\WebFetch`の部分を実際のプロジェクトパスに置き換えてください

## セキュリティ

このコンポーネントはHTTPとHTTPSプロトコルのみをサポートし、入力URLの検証を行います：
- 空またはnullのURLを拒否
- 不正な形式のURLを拒否
- HTTP/HTTPS以外のプロトコルを拒否

また、外部ウェブサイトへのアクセスは適切なUser-Agentヘッダーを使用し、特定のウェブサイトをブロックするような機能はありません。