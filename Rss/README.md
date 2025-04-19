# Rss

CSharpMcpServer Rss は、Model Context Protocol (MCP) サーバーのRSSフィード処理ツールを提供するモジュールです。このコンポーネントは、複数のRSSフィードからアイテムを取得し、マークダウン形式のリンクとして出力する機能を提供します。

## 機能
- **ParseRssFeeds**: コマンドライン引数から複数のRSSフィードを処理し、その内容をマークダウン形式のリンクとして出力
- 各フィードの最初のアイテムをスキップする機能
- エラーハンドリング機能付き

## API詳細

### ParseRssFeeds
```csharp
public static async Task<string> ParseRssFeeds()
```
複数のRSSフィードを処理し、タイトルとURLをマークダウン形式のリンクとして出力します：
- **説明**: コマンドライン引数から複数のRSSフィードを処理し、その内容をマークダウン形式のリンクとして出力します（各フィードの最初のアイテムはスキップ）
- **引数**: コマンドライン引数を通じてRSS URLを指定
- **戻り値**: マークダウン形式のリンクリスト

### ExtractRssFeedItems
```csharp
static async Task<List<(string title, string url)>> ExtractRssFeedItems(string rssUrl)
```
RSS フィードからタイトルとURLを抽出します（最初のエントリーを除く）：
- **rssUrl**: 処理するRSSフィードのURL
- **戻り値**: フィードアイテムのタイトルとURLを含むタプルのリスト

### LoadSyndicationFeed
```csharp
private static bool LoadSyndicationFeed(string rssUrl, out SyndicationFeed feed)
```
エラーハンドリングを備えた指定URLからのシンジケーションフィード読み込み：
- **rssUrl**: 読み込むRSSフィードのURL
- **feed**: 成功時に出力されるシンジケーションフィードオブジェクト
- **戻り値**: フィードの読み込みが成功したかどうかを示すブール値

## 使用方法

### コンパイルとビルド
- dotnet 8.0以上が必要
- リポジトリのルートディレクトリから以下のコマンドを実行:

```bash
dotnet build CSharpMcpServer/Rss
```

### Claude Desktopとの連携
Claude Desktopで使用するには、以下の設定を`claude_desktop_config.json`に追加します:

```json
{
    "mcpServers": {
        "Rss": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Rss",
                "--no-build",
                "--",
                "https://example.com/rss"
            ]
        }
    }
}
```

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\Rss`の部分を実際のプロジェクトパスに置き換えてください
- URLの部分にRSSフィードのURLを指定できます（複数指定可能）

## セキュリティ

このサーバーは指定されたRSSフィードのみにアクセスし、最初のアイテムをスキップしてコンテンツを取得します。

---

# Rss (English)

The CSharpMcpServer Rss is a module that provides RSS feed processing tools for the Model Context Protocol (MCP) server. This component enables retrieval of items from multiple RSS feeds and outputs them as markdown-formatted links.

## Features
- **ParseRssFeeds**: Processes multiple RSS feeds from command line arguments and formats their content as markdown links
- Capability to skip the first item of each feed
- Error handling functionality

## API Details

### ParseRssFeeds
```csharp
public static async Task<string> ParseRssFeeds()
```
Processes multiple RSS feeds and outputs titles and URLs as markdown-formatted links:
- **Description**: Processes multiple RSS feeds from command line arguments and formats their content as markdown links, ignoring the first item of each feed
- **Arguments**: RSS URLs provided through command line arguments
- **Returns**: Markdown-formatted list of links

### ExtractRssFeedItems
```csharp
static async Task<List<(string title, string url)>> ExtractRssFeedItems(string rssUrl)
```
Extracts titles and URLs from an RSS feed, excluding the first entry:
- **rssUrl**: The URL of the RSS feed to process
- **Returns**: A list of tuples containing the title and URL of each feed item

### LoadSyndicationFeed
```csharp
private static bool LoadSyndicationFeed(string rssUrl, out SyndicationFeed feed)
```
Loads a syndication feed from a specified URL with error handling:
- **rssUrl**: The URL of the RSS feed to load
- **feed**: The output syndication feed object when successful
- **Returns**: True if the feed was successfully loaded, false if any errors occurred

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/Rss
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "Rss": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Rss",
                "--no-build",
                "--",
                "https://example.com/rss"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\Rss` with your actual project path
- You can specify one or more RSS feed URLs after the `--` argument

## Security

This server only accesses the specified RSS feeds and retrieves content while skipping the first item of each feed.
