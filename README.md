# CSharpMcpServer

C#で実装されたModel Context Protocol (MCP) サーバーは、Claude Desktop APIの拡張機能を提供します。このプロジェクトは、ファイルシステム操作、ハードウェア情報取得、ウェブコンテンツ取得、時間取得などの様々なツールを提供します。

## Tools

- [FileSystem](FileSystem/) - ファイルシステム操作機能を提供
  - ファイルの読み書き、編集、削除機能
  - ディレクトリの作成、フォルダ構造の取得機能
  - ファイルやディレクトリの移動機能
  - ZIP圧縮・解凍機能
  - ファイル/フォルダを規定アプリで開く機能
  - 指定アプリケーションでファイルを開く機能
  - ファイル関連付け情報の取得機能

- [Command](Command/) - シェルコマンド実行機能を提供
  - PowerShellでのコマンド実行
  - WSL Bashでのコマンド実行
  - タイムアウト管理機能付き

- [HardwareInfoRetriever](HardwareInfoRetriever/) - PC情報やネットワーク情報を取得
  - OS、CPU、GPU、メモリ、ディスク情報の取得（キャッシュ機能付き）
  - 特定コンポーネントの選択的な情報取得
  - キャッシュ情報の強制更新機能

- [Time](Time/) - 現在の時刻を取得
  - システムのローカル時間をフォーマット済みの文字列として提供

- [WebFetch](WebFetch/) - URLからコンテンツを取得
  - ウェブページから広告やナビゲーションを除去したメインコンテンツを抽出
  - セマンティックHTML要素を利用したコンテンツ特定アルゴリズム

- [NetworkInfo](NetworkInfo/) - ネットワーク情報を取得
  - ネットワークアダプター情報の取得
  - TCP接続情報の取得

- [Rss](Rss/) - RSSフィードを処理
  - 複数のRSSフィードを同時に処理
  - マークダウン形式のリンクとして出力

## 使用方法

各ツールは独立したdotnetプロジェクトとして実装されており、それぞれをビルドして使用できます。詳細な使用方法は各ツールのREADMEを参照してください。

## ライセンス
このプロジェクトは[MITライセンス](LICENSE.txt)の下でライセンスされています。

---

# CSharpMcpServer (English)

The C# implementation of Model Context Protocol (MCP) servers provides extensions for the Claude Desktop API. This project offers various tools for file system operations, hardware information retrieval, web content fetching, and time retrieval.

## Tools

- [FileSystem](FileSystem/) - Provides file system operation functionality
  - File reading, writing, editing, and deletion
  - Directory creation and folder structure retrieval
  - Moving files and directories
  - ZIP compression and extraction
  - Opening files/folders with default applications
  - Opening files with specific applications
  - Retrieving file association information

- [Command](Command/) - Provides shell command execution functionality
  - Command execution in PowerShell
  - Command execution in WSL Bash
  - Timeout management capability

- [HardwareInfoRetriever](HardwareInfoRetriever/) - Retrieves PC and network information
  - OS, CPU, GPU, memory, and disk information (with caching support)
  - Selective retrieval of specific hardware components
  - Forced refresh of cached hardware information

- [Time](Time/) - Retrieves the current time
  - Provides the system's local time as a formatted string

- [WebFetch](WebFetch/) - Retrieves content from URLs
  - Extracts main content from web pages by removing ads and navigation
  - Content identification algorithm using semantic HTML elements

- [NetworkInfo](NetworkInfo/) - Retrieves network information
  - Network adapter information
  - TCP connection information

- [Rss](Rss/) - Processes RSS feeds
  - Processes multiple RSS feeds simultaneously
  - Outputs content as markdown-formatted links

## Usage

Each tool is implemented as an independent dotnet project and can be built and used separately. For detailed usage instructions, please refer to the README of each tool.

## License
This project is licensed under the [MIT License](LICENSE.txt).
