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

- [FileSystem](FileSystem/Servers/README.ja.md) - ファイルシステム操作機能を提供
  - ファイルの読み書き、編集、削除機能
  - ディレクトリの作成、フォルダ構造の取得機能
  - ファイルやディレクトリの移動機能
  - ZIP圧縮・解凍機能
  - ファイル/フォルダを規定アプリで開く機能
  - 指定アプリケーションでファイルを開く機能
  - ファイル関連付け情報の取得機能

- [Command](Command/Servers/README.ja.md) - シェルコマンド実行機能を提供
  - PowerShellでのコマンド実行
  - WSL Bashでのコマンド実行
  - タイムアウト管理機能付き

- [HardwareInfoRetriever](HardwareInfoRetriever/Servers/README.ja.md) - PC情報やネットワーク情報を取得
  - OS、CPU、GPU、メモリ、ディスク情報の取得（キャッシュ機能付き）
  - 特定コンポーネントの選択的な情報取得
  - キャッシュ情報の強制更新機能

- [Time](Time/Servers/README.ja.md) - 現在の時刻を取得
  - システムのローカル時間をフォーマット済みの文字列として提供

- [Web](Web/Servers/README.ja.md) - Webブラウザとの連携機能を提供
  - 規定のブラウザでURLを開く機能

- [WebFetch](WebFetch/Servers/README.ja.md) - URLからコンテンツを取得
  - ウェブページから広告やナビゲーションを除去したメインコンテンツを抽出
  - セマンティックHTML要素を利用したコンテンツ特定アルゴリズム

- [NetworkInfo](NetworkInfo/Servers/README.ja.md) - ネットワーク情報を取得
  - ネットワークアダプター情報の取得
  - TCP接続情報の取得

- [VisualStudio](VisualStudio/Servers/README.ja.md) - Visual Studio情報を取得
  - 実行中のVisual Studioインスタンス情報の取得
  - アクティブなソリューション情報の取得
  - 現在選択されているファイルの内容を取得
  - 開いているすべてのファイル情報の取得

- [Rss](Rss/Servers/README.ja.md) - RSSフィードを処理
  - 複数のRSSフィードを同時に処理
  - マークダウン形式のリンクとして出力

- [CreateMcpServer](CreateMcpServer/Servers/README.ja.md) - MCPサーバープロジェクトの作成ツール
  - 新規MCPサーバープロジェクトの自動生成（CreateMcpServerProject）
  - プロジェクト作成と設定に関するプロンプト機能（CreateMcpServerPrompts）
  - 必要なフォルダ構造とプロジェクトファイルの自動作成
  - ソリューションへの自動統合

## 使用方法

各ツールは独立したdotnetプロジェクトとして実装されており、それぞれをビルドして使用できます。詳細な使用方法は各ツールのServers/READMEを参照してください。

## ライセンス
このプロジェクトは[MITライセンス](LICENSE.txt)の下でライセンスされています。