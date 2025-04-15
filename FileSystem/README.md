# FileSystem

CSharpMcpServer FileSystemは、Model Context Protocol (MCP) サーバーのファイルシステム操作ツールを提供するモジュールです。このコンポーネントは、ファイルの読み込み、編集、削除、ディレクトリ構造の取得などの基本的なファイルシステム操作を可能にします。

## 機能
- **GetFileInfo**: ファイルの情報（パス、行数、内容）を取得
- **EditFile**: 特定の行番号から指定した行数を削除し、新しいコンテンツに置き換え
- **Delete**: ファイルまたはディレクトリを削除（ディレクトリの場合は再帰的削除オプションあり）
- **GetFolderStructure**: 指定されたディレクトリの階層構造をYAML形式で取得（.gitignoreに基づく除外処理付き）

## API詳細

### GetFileInfo
```csharp
public static string GetFileInfo(string filePath)
```
指定されたファイルの情報を取得します：
- **filePath**: 読み込むファイルの完全パス
- **戻り値**: ファイルパス、行数、内容を含むJSON形式の情報

### EditFile
```csharp
public static void EditFile(string path, int lineNumber, int linesToDelete, string content)
```
ファイルの特定の位置のコンテンツを編集します：
- **filePath**: 編集するファイルのパス
- **lineNumber**: 編集を開始する1ベースの行番号（ファイルの先頭から開始するには1を使用）
- **linesToDelete**: 開始行から削除する行数（完全なファイル置換には総行数を、削除なしの挿入には0を設定）
- **content**: 指定された位置に挿入するテキスト内容（完全なファイル置換の場合は新しい内容全体を提供）

### Delete
```csharp
public static void Delete(string path, bool recursive = false)
```
ファイルまたはディレクトリを削除します：
- **fullPath**: 削除するファイルまたはディレクトリの完全パス
- **recursive**: ディレクトリ内のすべてのコンテンツを削除するかどうか（ファイルの場合は無視、デフォルトはfalse）

### GetFolderStructure
```csharp
public static string GetFolderStructure(string fullPath, bool recursive = true)
```
指定されたディレクトリの階層構造をYAML形式で取得します：
- **fullPath**: フォルダ構造を取得するルートディレクトリの絶対パス
- **recursive**: フォルダ構造にサブディレクトリを再帰的に含めるかどうかを指定（trueの場合、すべてのネストされたディレクトリを走査、falseの場合はルートディレクトリの直接の子のみ含む、デフォルトはtrue）
- **戻り値**: YAML形式のフォルダ構造

## 使用方法

### コンパイルとビルド
- dotnet 8.0以上が必要
- リポジトリのルートディレクトリから以下のコマンドを実行:

```bash
dotnet build CSharpMcpServer/FileSystem
```

### Claude Desktopとの連携
Claude Desktopで使用するには、以下の設定を`claude_desktop_config.json`に追加します:

```json
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

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\FileSystem`の部分を実際のプロジェクトパスに置き換えてください
- 必要に応じて`/path/to/other/allowed/dir`にアクセスを許可したい追加のディレクトリを指定できます

## セキュリティ

このサーバーは指定されたディレクトリとその子ディレクトリのみにアクセスを制限します。

## .gitignore対応

GetFolderStructureには、.gitignoreファイルを解析する機能が含まれています：

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

---

# FileSystem (English)

The CSharpMcpServer FileSystem is a module that provides file system operation tools for the Model Context Protocol (MCP) server. This component enables basic file system operations such as reading files, editing, deletion, and retrieving directory structures.

## Features
- **GetFileInfo**: Get file information including path, line count, and content
- **EditFile**: Delete a specified number of lines from a specific line number and replace with new content
- **Delete**: Delete a file or directory (with recursive deletion option for directories)
- **GetFolderStructure**: Get the hierarchical structure of a specified directory in YAML format (with exclusion processing based on .gitignore)

## API Details

### GetFileInfo
```csharp
public static string GetFileInfo(string filePath)
```
Gets information about the specified file:
- **filePath**: The full path to the file to be read
- **Returns**: JSON-formatted information including file path, line count, and content

### EditFile
```csharp
public static void EditFile(string path, int lineNumber, int linesToDelete, string content)
```
Edits content at a specific position in a file:
- **filePath**: The full path to the file to edit
- **lineNumber**: The 1-based line number where the edit should start (use 1 to start from the beginning of the file)
- **linesToDelete**: The number of lines to delete from the start line (set to the total line count for complete file replacement, or 0 for pure insertion without deletion)
- **content**: The text content to insert at the specified position (for full file replacement, provide the entire new content)

### Delete
```csharp
public static void Delete(string path, bool recursive = false)
```
Deletes a file or directory:
- **fullPath**: The full path of the file or directory to delete
- **recursive**: Whether to delete all contents inside a directory (ignored for files, default is false)

### GetFolderStructure
```csharp
public static string GetFolderStructure(string fullPath, bool recursive = true)
```
Retrieves the hierarchical folder structure in YAML format:
- **fullPath**: Absolute path to the root directory whose folder structure should be retrieved
- **recursive**: Specifies whether to include subdirectories recursively in the folder structure (if true, the function will traverse through all nested directories; if false, only the immediate children of the root directory will be included, default is true)
- **Returns**: Folder structure in YAML format

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/FileSystem
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
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

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\FileSystem` with your actual project path
- You can specify additional allowed directories with `/path/to/other/allowed/dir` as needed

## Security

This server restricts access to only the specified directories and their subdirectories. 

## .gitignore Support

GetFolderStructure includes functionality to parse .gitignore files:

- Reads the .gitignore from the root directory
- Properly processes .gitignore files in subdirectories
- Supports absolute and relative path patterns
- Converts wildcards (*, **, ?)
- Handles directory-specific patterns (trailing /)

Additionally, the following common files/directories are automatically excluded:

- .git, .next directories
- Build outputs like bin, obj, target, dist
- Visual Studio files like .vs, *.user, *.suo
- Package directories like node_modules, packages
- Log files, backup files, cache files

