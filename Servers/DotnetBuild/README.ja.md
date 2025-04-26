# DotnetBuild

[English version here](README.md)

CSharpMcpServer DotnetBuildは、dotnet CLIを使用して.NETプロジェクトをビルドおよび分析するためのツールモジュールです。プロジェクトのビルドとソリューションファイル内の依存関係を分析する機能を提供します。

## 機能

- **BuildProject**: dotnet CLIを使用して指定された.NETプロジェクトをビルドします
  - 異なる構成設定（Debug/Release）をサポート
  - 特定のフレームワークをターゲットにすることが可能
  - 詳細な出力オプションを提供
  - 成功ステータスと出力を含む詳細なビルド結果を返します

- **GetProjectDependencies**: ソリューションファイル内のプロジェクト依存関係を分析して表示します
  - ソリューションファイルを解析して含まれるプロジェクトを識別
  - プロジェクト間のプロジェクト参照を分析
  - 各プロジェクトのパッケージ依存関係を識別
  - 依存関係ツリーの関係を構築

## API詳細

### BuildProject

```csharp
public static BuildResult BuildProject(string projectPath, string configuration = "Debug", string framework = "", bool verbose = false)
```

指定された.NETプロジェクトをビルドします：
- **説明**: dotnetコマンドを使用して指定されたプロジェクトまたはソリューションをビルドします
- **projectPath**: ビルドするプロジェクトまたはソリューションファイルのパス
- **configuration**: 使用するビルド構成（Debug/Release）
- **framework**: ターゲットフレームワーク（オプション）
- **verbose**: 詳細なビルド情報を出力するかどうか
- **戻り値**: ビルド結果、出力、およびエラー情報を含むBuildResultオブジェクト

### GetProjectDependencies

```csharp
public static ProjectDependencies GetProjectDependencies(string solutionPath)
```

ソリューション内のプロジェクト間の依存関係を分析します：
- **説明**: ソリューションファイル（.sln）内のプロジェクト依存関係を分析して表示します
- **solutionPath**: 分析するソリューションファイルのパス
- **戻り値**: 依存関係分析結果を含むProjectDependenciesオブジェクト

## 使用例

```csharp
// デフォルト設定でプロジェクトをビルド
var result = BuildProject(@"C:\Projects\MyProject\MyProject.csproj");

// Release構成でプロジェクトをビルド
var releaseResult = BuildProject(@"C:\Projects\MyProject\MyProject.csproj", "Release");

// ソリューション内の依存関係を分析
var dependencies = GetProjectDependencies(@"C:\Projects\MySolution.sln");
```

## 結果クラス

### BuildResult

ビルド操作の結果を含みます：
- **Success**: ビルドが成功したかどうかを示すブール値
- **Output**: ビルドプロセスからの標準出力
- **ErrorOutput**: ビルドプロセスからのエラー出力
- **ExitCode**: ビルドプロセスから返された終了コード

### ProjectDependencies

依存関係分析の結果を含みます：
- **Success**: 分析が成功したかどうかを示すブール値
- **ErrorMessage**: 分析が失敗した場合のエラーメッセージ
- **Projects**: ソリューション内のプロジェクトを表すProjectInfoオブジェクトのリスト

### ProjectInfo

プロジェクトに関する情報を表します：
- **ProjectName**: プロジェクトの名前
- **ProjectPath**: プロジェクトファイルへのフルパス
- **PackageReferences**: NuGetパッケージの依存関係
- **ProjectReferences**: プロジェクト参照
- **Dependencies**: 依存プロジェクトの名前

### コンパイルとビルド

- dotnet 8.0以上が必要です
- リポジトリのルートディレクトリから以下のコマンドを実行します：

```bash
dotnet build CSharpMcpServer/DotnetBuild
```

### Claude Desktopとの統合

Claude Desktopで使用するには、`claude_desktop_config.json`に以下の設定を追加してください：

```json
{
    "mcpServers": {
        "DotnetBuild": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Servers\\DotnetBuild",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**重要**: 
- `absolute\path\to\CSharpMCPServer\DotnetBuild`を実際のプロジェクトパスに置き換えてください
