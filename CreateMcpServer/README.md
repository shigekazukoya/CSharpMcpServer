# CreateMcpServer

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

1. `C:\Projects\MCPServer\CSharpMcpServer\MyFeature` ディレクトリの作成
2. `dotnet new console` コマンドによる新規コンソールプロジェクトの初期化
3. `CSharpMcpServer.Common` プロジェクト参照の追加
4. MCP機能が使える基本的な `Program.cs` の作成
5. `MyFeatureTools.cs` という機能実装用のテンプレートファイルの作成
6. 既存のソリューションファイルへのプロジェクト追加

## プロジェクト作成の自動化

CreateMcpServerを使用すると、MCP機能ごとに必要な標準的なボイラープレートコードを手動で記述する必要がなくなり、新機能の開発に集中できます。

## ソリューションとの統合

作成されたプロジェクトは自動的に既存のソリューションファイルに追加されるため、Visual Studioですぐに開発を開始できます。

---

# CreateMcpServer (English)

The CSharpMcpServer CreateMcpServer is a tool module for easily creating MCP (Model Context Protocol) server projects. It automates the creation and setup of new MCP server projects, streamlining the development process.

## Features

- **CreateMcpServerProject**: Creates a new MCP server project
  - Creates the necessary folder structure
  - Initializes a .NET Console project
  - Adds common library references
  - Creates a Program.cs file with MCP server configuration
  - Automatically generates a feature-specific Tools class file
  - Adds the project to the solution

- **CreateMcpServerPrompts**: Provides prompts related to MCP server projects
  - Prompts for creating new projects
  - Prompts for updating README.md files

## API Details

### CreateMcpServerProject

```csharp
public static string CreateMcpServerProject(string feature)
```

Creates a new MCP server project:
- **Description**: Create a new MCP Server project
- **feature**: The name of the feature for the project (forms the basis for folder and class names)
- **Returns**: A message describing the processing result

### CreateMcpServerPrompts

A class that provides prompts related to MCP server projects.

#### CreateMcpServerProjectPrompt

```csharp
public static string CreateMcpServerProjectPrompt(string feature)
```

- **Description**: Prompt for creating a new MCP server project
- **feature**: The name of the feature for the project
- **Returns**: Prompt string

#### UpdateReadMePrompt

```csharp
public static string UpdateReadMePrompt(string feature)
```

- **Description**: Prompt for updating project README.md files
- **feature**: The name of the feature for the project
- **Returns**: Prompt string

## Usage

```csharp
// Create a new MCP Server project
CreateMcpServerProject("MyFeature");
```

This performs the following operations:

1. Creates the `C:\Projects\MCPServer\CSharpMcpServer\MyFeature` directory
2. Initializes a new console project using the `dotnet new console` command
3. Adds a reference to the `CSharpMcpServer.Common` project
4. Creates a basic `Program.cs` with MCP functionality enabled
5. Creates a template file `MyFeatureTools.cs` for feature implementation
6. Adds the project to existing solution files

## Project Creation Automation

Using CreateMcpServer eliminates the need to manually write standard boilerplate code for each MCP feature, allowing you to focus on developing new functionality.

## Solution Integration

The created projects are automatically added to existing solution files, enabling you to start development immediately in Visual Studio.
