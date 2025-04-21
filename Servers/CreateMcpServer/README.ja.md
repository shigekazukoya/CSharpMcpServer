# CreateMcpServer

[English version here](README.md)

CSharpMcpServer CreateMcpServer は、MCP (Model Context Protocol) サーバープロジェクトを簡単に作成するためのツールモジュールです。新しいMCPサーバープロジェクトの作成とセットアップを自動化し、開発を効率化します。

## 機能

- **CreateMcpServerProject**: 新しいMCPサーバープロジェクトを作成
  - 必要なフォルダ構造の作成
  - .NET Consoleプロジェクトの初期化
  - 共通ライブラリ参照の追加
  - Program.csファイルの作成（MCPサーバー構成付き）
  - 機能ごとのToolsクラスファイルの自動生成
  - ソリューションへのプロジェクト追加

- **CreateMcpServerPrompts**: MCPサーバープロジェクトに関連するプロンプトを提供
  - 新規プロジェクト作成のためのプロンプト
  - README.mdファイル更新のためのプロンプト

## API詳細

### CreateMcpServerProject

```csharp
public static string CreateMcpServerProject(string feature)
```

新しいMCPサーバープロジェクトを作成します：
- **説明**: Create a new MCP Server project
- **feature**: 作成するプロジェクトの機能名（フォルダ名やクラス名の基盤になります）
- **戻り値**: 処理結果のメッセージ

### CreateMcpServerPrompts

MCPサーバープロジェクトに関連するプロンプトを提供するクラスです。

#### CreateMcpServerProjectPrompt

```csharp
public static string CreateMcpServerProjectPrompt(string feature)
```

- **説明**: 新しいMCPサーバープロジェクトを作成するためのプロンプト
- **feature**: プロジェクトの機能名
- **戻り値**: プロンプト文字列

#### UpdateReadMePrompt

```csharp
public static string UpdateReadMePrompt(string feature)
```

- **説明**: プロジェクトのREADME.mdファイルを更新するためのプロンプト
- **feature**: プロジェクトの機能名
- **戻り値**: プロンプト文字列

## 使用方法

```csharp
// 新しいMcpServerプロジェクトの作成
CreateMcpServerProject("MyFeature");
```

これにより、以下の処理が実行されます：

1. `C:\Projects\MCPServer\CSharpMcpServer\Servers\MyFeature` ディレクトリの作成
2. `dotnet new console` コマンドによる新規コンソールプロジェクトの初期化
3. `CSharpMcpServer.Common` プロジェクト参照の追加
4. MCP機能が使える基本的な `Program.cs` の作成
5. `MyFeatureTools.cs` という機能実装用のテンプレートファイルの作成
6. 既存のソリューションファイルへのプロジェクト追加

## プロジェクト作成の自動化

CreateMcpServerを使用すると、MCP機能ごとに必要な標準的なボイラープレートコードを手動で記述する必要がなくなり、新機能の開発に集中できます。

## ソリューションとの統合

作成されたプロジェクトは自動的に既存のソリューションファイルに追加されるため、Visual Studioですぐに開発を開始できます。