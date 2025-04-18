# CSharpMcpServer

C#で実装されたModel Context Protocol (MCP) サーバーは、Claude Desktop APIの拡張機能を提供します。このプロジェクトは、ファイルシステム操作、ハードウェア情報取得、ウェブコンテンツ取得、時間取得などの様々なツールを提供します。

## Tools

- [FileSystem](FileSystem/) - ファイルシステム操作機能を提供
  - ファイルの読み書き、削除機能
  - フォルダ構造の取得機能
  - ZIP圧縮・解凍機能
  - ファイル/フォルダを規定アプリで開く機能

- [HardwareInfoRetriever](HardwareInfoRetriever/) - PC情報やネットワーク情報を取得
  - OS、CPU、GPU、メモリ、ディスク情報の取得
  - ネットワークアダプターとTCP接続情報の取得

- [Time](Time/) - 現在の時刻を取得
  - システムのローカル時間をフォーマット済みの文字列として提供

- [WebFetch](WebFetch/) - URLからコンテンツを取得
  - ウェブページから広告やナビゲーションを除去したメインコンテンツを抽出
  - セマンティックHTML要素を利用したコンテンツ特定アルゴリズム

- [NetworkInfo](NetworkInfo/) - ネットワーク情報を取得
  - ネットワークアダプター情報の取得
  - TCP接続情報の取得

## 使用方法

各ツールは独立したdotnetプロジェクトとして実装されており、それぞれをビルドして使用できます。詳細な使用方法は各ツールのREADMEを参照してください。

## ライセンス
このプロジェクトは[MITライセンス](LICENSE.txt)の下でライセンスされています。

---

# CSharpMcpServer (English)

The C# implementation of Model Context Protocol (MCP) servers provides extensions for the Claude Desktop API. This project offers various tools for file system operations, hardware information retrieval, web content fetching, and time retrieval.

## Tools

- [FileSystem](FileSystem/) - Provides file system operation functionality
  - File reading, writing, and deletion
  - Folder structure retrieval
  - ZIP compression and extraction
  - Opening files/folders with default applications

- [HardwareInfoRetriever](HardwareInfoRetriever/) - Retrieves PC and network information
  - OS, CPU, GPU, memory, and disk information
  - Network adapter and TCP connection information

- [Time](Time/) - Retrieves the current time
  - Provides the system's local time as a formatted string

- [WebFetch](WebFetch/) - Retrieves content from URLs
  - Extracts main content from web pages by removing ads and navigation
  - Content identification algorithm using semantic HTML elements

- [NetworkInfo](NetworkInfo/) - Retrieves network information
  - Network adapter information
  - TCP connection information

## Usage

Each tool is implemented as an independent dotnet project and can be built and used separately. For detailed usage instructions, please refer to the README of each tool.

## License
This project is licensed under the [MIT License](LICENSE.txt).
