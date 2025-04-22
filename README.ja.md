# CSharpMcpServer

[English version here](README.md)

C#で実装されたModel Context Protocol (MCP) サーバーは、Claude Desktop APIの拡張機能を提供します。このプロジェクトは、ファイルシステム操作、ハードウェア情報取得、ウェブコンテンツ取得、時間取得などの様々なツールを提供します。

## Utilities

- [McpInsight](McpInsight/README.ja.md) - MCPサーバーのデバッグ・監視ツール
  - MCPクライアントとサーバー間の通信をリアルタイムに監視
  - MCPサーバーコマンドのインタラクティブなテスト
  - メッセージを分析しやすい形式で表示
  - stdioベースのMCPサーバー開発のあらゆるフェーズで利用可能

## Servers

- [FileSystem](Servers/FileSystem/README.ja.md) - ファイルシステム操作機能を提供
  - ファイルの読み書き、編集、削除機能
  - ディレクトリの作成、フォルダ構造の取得機能
  - ZIP圧縮・解凍機能
  - ファイル/フォルダを規定アプリで開く機能

- [PowerShell](Servers/PowerShell/README.ja.md) - PowerShellコマンド実行のためのセキュアなインターフェースを提供

- [HardwareInfoRetriever](Servers/HardwareInfoRetriever/README.ja.md) - OS、CPU、GPU、メモリ、ディスク情報の取得

- [Time](Servers/Time/README.ja.md) - 現在の時刻を取得

- [Web](Servers/Web/README.ja.md) - 規定のブラウザでURLを開く機能

- [WebFetch](Servers/WebFetch/README.ja.md) - ウェブページから広告やナビゲーションを除去したメインコンテンツを抽出
  - セマンティックHTML要素を利用したコンテンツ特定アルゴリズム

- [NetworkInfo](Servers/NetworkInfo/README.ja.md) - ネットワークアダプター情報やTCP接続情報の取得

- [VisualStudio](Servers/VisualStudio/README.ja.md) - Visual Studio情報を取得
  - 現在選択されているファイルの内容を取得
  - 開いているすべてのファイル情報の取得

- [Rss](Servers/Rss/README.ja.md) - RSSフィードを処理

- [CreateMcpServer](Servers/CreateMcpServer/README.ja.md) - MCPサーバープロジェクトの作成ツール

## 使用方法

各ツールは独立したdotnetプロジェクトとして実装されており、それぞれをビルドして使用できます。詳細な使用方法は各ツールのServers/READMEを参照してください。

## ライセンス
このプロジェクトは[MITライセンス](LICENSE.txt)の下でライセンスされています。