# FileSystem

CSharpMcpServer FileSystem は、Model Context Protocol (MCP) サーバーのファイルシステム操作ツールを提供するモジュールです。このコンポーネントは、ファイルの読み込み、編集、削除、ディレクトリ構造の取得などの基本的なファイルシステム操作を可能にします。

## 機能
- **GetFileInfo**: ファイルの情報（パス、行数、内容）を取得
- **WriteFile**: ファイルの内容を書き込み
- **EditFile**: 指定テキストを置換してファイルを編集
- **Delete**: ファイルまたはディレクトリを削除（ディレクトリの場合は再帰的削除オプションあり）
- **CreateDirectory**: ディレクトリを作成
- **Move**: ファイルまたはディレクトリを移動
- **GetFolderStructure**: 指定されたディレクトリの階層構造をYAML形式で取得（.gitignoreに基づく除外処理付き）
- **Zip**: ディレクトリまたはファイルを圧縮してZIPファイルを作成
- **Unzip**: ZIPファイルを展開
- **OpenWithApplication**: ファイルまたはフォルダを規定のアプリケーションで開く
- **OpenWithSpecificApplication**: 指定したプログラムでファイルを開く
- **GetFileAssociation**: ファイルに関連付けられたアプリケーション情報を取得

## API詳細

### GetFileInfo
```csharp
public static async Task<string> GetFileInfoAsync(string filePath, string encodingName = "utf-8", bool includeContent = true)
```
指定されたファイルの情報を取得します：
- **説明**: ファイル情報（パス、行数、内容など）を取得します
- **filePath**: 読み込むファイルの完全パス
- **encodingName**: 使用するエンコーディング（utf-8, shift-jis, etc.）、デフォルトはutf-8
- **includeContent**: 結果にファイル内容を含めるかどうか。大きいファイルの場合はfalseを推奨
- **戻り値**: ファイルパス、行数、内容を含むJSON形式の情報

### WriteFile
```csharp
public static string WriteFile(string filePath, string content, string encodingName = "utf-8")
```
ファイルに内容を書き込みます：
- **説明**: ファイルを書き込みます
- **filePath**: 編集するファイルのパス
- **content**: ファイルに書き込む内容
- **encodingName**: 使用するエンコーディング（utf-8, shift-jis, etc.）、デフォルトはutf-8
- **戻り値**: 処理結果のJSONメッセージ

### EditFile
```csharp
public static async Task<string> EditFileAsync(string filePath, string oldString, string newString, string encodingName = "utf-8", int replacementCount = 1)
```
指定テキストを置換してファイルを編集します：
- **説明**: 指定テキストを置換してファイルを編集します
- **filePath**: 編集するファイルのパス
- **oldString**: 置換対象のテキスト
- **newString**: 置換後のテキスト
- **encodingName**: 使用するエンコーディング（utf-8, shift-jis, etc.）、デフォルトはutf-8
- **replacementCount**: 実行する置換の回数（デフォルト: 1）
- **戻り値**: 処理結果のJSONメッセージ

### Delete
```csharp
public static string Delete(string fullPath, bool recursive = false)
```
ファイルまたはディレクトリを削除します：
- **説明**: ファイルまたはディレクトリをファイルシステムから削除します
- **fullPath**: 削除するファイルまたはディレクトリの完全パス
- **recursive**: ディレクトリ内のすべてのコンテンツを削除するかどうか（ファイルの場合は無視、デフォルトはfalse）
- **戻り値**: 処理結果のJSONメッセージ

### CreateDirectory
```csharp
public static string CreateDirectory(string directoryPath)
```
ディレクトリを作成します：
- **説明**: ディレクトリを作成します
- **directoryPath**: 作成するディレクトリのパス
- **戻り値**: 処理結果のJSONメッセージ

### Move
```csharp
public static string Move(string sourcePath, string destinationPath, bool overwrite = false)
```
ファイルまたはディレクトリを移動します：
- **説明**: ファイルまたはディレクトリを新しい場所に移動します
- **sourcePath**: 移動するファイルまたはディレクトリのパス
- **destinationPath**: 移動先のパス
- **overwrite**: 既存ファイルを上書きするかどうか（ディレクトリの場合は無視、デフォルトはfalse）
- **戻り値**: 処理結果のJSONメッセージ

### GetFolderStructure
```csharp
public static async Task<string> GetFolderStructureAsync(string fullPath, bool recursive = true, string format = "yaml", string excludePattern = "")
```
指定されたディレクトリの階層構造をYAML形式で取得します：
- **説明**: 指定されたディレクトリパスから階層的なフォルダ構造をYAML形式で取得します
- **fullPath**: フォルダ構造を取得するルートディレクトリの絶対パス
- **recursive**: フォルダ構造にサブディレクトリを再帰的に含めるかどうかを指定（trueの場合、すべてのネストされたディレクトリを走査、falseの場合はルートディレクトリの直接の子のみ含む、デフォルトはtrue）
- **format**: 出力形式（yaml または json）
- **excludePattern**: 除外するファイル/ディレクトリのパターン（正規表現）
- **戻り値**: YAMLまたはJSON形式のフォルダ構造

### Zip
```csharp
public static async Task<string> ZipAsync(string path, string outputPath = "", string compressionLevel = "Optimal")
```
指定されたディレクトリまたはファイルをZIP形式に圧縮します：
- **説明**: 圧縮ファイルを作成します
- **path**: 圧縮するディレクトリまたはファイルのパス
- **outputPath**: 出力するZIPファイルのパス（省略時は自動生成）
- **compressionLevel**: 圧縮レベル（Fastest, Optimal, NoCompression）、デフォルトは"Optimal"
- **戻り値**: 処理結果のJSONメッセージ

### Unzip
```csharp
public static async Task<string> UnzipAsync(string filePath, string outputPath = "", bool overwrite = false)
```
ZIPファイルを展開します：
- **説明**: 圧縮ファイルを展開します
- **filePath**: 展開するZIPファイルのパス
- **outputPath**: 展開先ディレクトリのパス（省略時は自動生成）
- **overwrite**: 既存ファイルを上書きするかどうか（デフォルトはfalse）
- **戻り値**: 処理結果のJSONメッセージ

### OpenWithApplication
```csharp
public static string OpenWithApplication(string path, string verb = "open")
```
ファイルまたはフォルダを規定のアプリケーションで開きます：
- **説明**: ファイルまたはフォルダを規定のアプリケーションで開く
- **path**: ファイルまたはフォルダのパス
- **verb**: 使用する動詞（open, edit, print など）、デフォルトは"open"
- **戻り値**: 処理結果のJSONメッセージ

### OpenWithSpecificApplication
```csharp
public static string OpenWithSpecificApplication(string filePath, string applicationPath, string arguments = "")
```
指定したプログラムでファイルを開きます：
- **説明**: 指定したプログラムでファイルを開きます
- **filePath**: 開くファイルのパス
- **applicationPath**: 使用するアプリケーションのパス
- **arguments**: 追加のコマンドライン引数
- **戻り値**: 処理結果のJSONメッセージ

### GetFileAssociation
```csharp
public static string GetFileAssociation(string path)
```
ファイルに関連付けられたアプリケーション情報を取得します：
- **説明**: ファイルに関連付けられたアプリケーション情報を取得します
- **path**: ファイルパス
- **戻り値**: 関連付け情報のJSONメッセージ
public static string Unzip(string filePath)
```
ZIPファイルを展開します：
- **filePath**: 展開するZIPファイルのパス
- **戻り値**: 展開結果のメッセージ

### Launch
```csharp
public static string Launch(string path)
```
ファイルまたはフォルダを規定のアプリケーションで開きます：
- **path**: ファイルまたはフォルダのパス
- **戻り値**: 実行結果のメッセージ

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
- **WriteFile**: Write content to a file
- **EditFile**: Edit a file by replacing specified text
- **Delete**: Delete a file or directory (with recursive deletion option for directories)
- **CreateDirectory**: Create a directory
- **Move**: Move a file or directory to a new location
- **GetFolderStructure**: Get the hierarchical structure of a specified directory in YAML format (with exclusion processing based on .gitignore)
- **Zip**: Compress a directory or file to create a ZIP file
- **Unzip**: Extract a ZIP file
- **OpenWithApplication**: Open a file or folder with the default application
- **OpenWithSpecificApplication**: Open a file with a specific application
- **GetFileAssociation**: Get information about applications associated with a file

## API Details

### GetFileInfo
```csharp
public static async Task<string> GetFileInfoAsync(string filePath, string encodingName = "utf-8", bool includeContent = true)
```
Gets information about the specified file:
- **Description**: Gets file information including path, line count and content.
- **filePath**: The full path to the file to be read
- **encodingName**: The encoding to use (utf-8, shift-jis, etc.). Default is utf-8
- **includeContent**: Whether to include file content in the result. For large files, setting this to false is recommended
- **Returns**: JSON-formatted information including file path, line count, and content

### WriteFile
```csharp
public static string WriteFile(string filePath, string content, string encodingName = "utf-8")
```
Writes content to a file:
- **Description**: Write file
- **filePath**: The path to the file to edit
- **content**: The content to write to the file
- **encodingName**: The encoding to use (utf-8, shift-jis, etc.). Default is utf-8
- **Returns**: JSON-formatted result message

### EditFile
```csharp
public static async Task<string> EditFileAsync(string filePath, string oldString, string newString, string encodingName = "utf-8", int replacementCount = 1)
```
Edits a file by replacing specified text:
- **Description**: Edit file by replacing specified text
- **filePath**: The path to the file to edit
- **oldString**: The text to replace
- **newString**: The text to replace it with
- **encodingName**: The encoding to use (utf-8, shift-jis, etc.). Default is utf-8
- **replacementCount**: The expected number of replacements to perform. Defaults to 1 if not specified.
- **Returns**: JSON-formatted result message

### Delete
```csharp
public static string Delete(string fullPath, bool recursive = false)
```
Deletes a file or directory:
- **Description**: Deletes a file or directory from the file system.
- **fullPath**: The full path of the file or directory to delete
- **recursive**: Whether to delete all contents inside a directory. Ignored for files. Default is false.
- **Returns**: JSON-formatted result message

### CreateDirectory
```csharp
public static string CreateDirectory(string directoryPath)
```
Creates a directory:
- **Description**: Creates a directory.
- **directoryPath**: The path of the directory to create.
- **Returns**: JSON-formatted result message

### Move
```csharp
public static string Move(string sourcePath, string destinationPath, bool overwrite = false)
```
Moves a file or directory:
- **Description**: Moves a file or directory to a new location.
- **sourcePath**: The path of the file or directory to move.
- **destinationPath**: The path to move the file or directory to.
- **overwrite**: Whether to overwrite an existing file. Ignored for directories. Default is false.
- **Returns**: JSON-formatted result message

### GetFolderStructure
```csharp
public static async Task<string> GetFolderStructureAsync(string fullPath, bool recursive = true, string format = "yaml", string excludePattern = "")
```
Retrieves the hierarchical folder structure in YAML format:
- **Description**: Retrieves the hierarchical folder structure in YAML format from a specified directory path.
- **fullPath**: Absolute path to the root directory whose folder structure should be retrieved
- **recursive**: Specifies whether to include subdirectories recursively in the folder structure. If set to true, the function will traverse through all nested directories. If false, only the immediate children of the root directory will be included.
- **format**: Output format (yaml or json)
- **excludePattern**: Regex pattern for files/directories to exclude
- **Returns**: Folder structure in YAML or JSON format

### Zip
```csharp
public static async Task<string> ZipAsync(string path, string outputPath = "", string compressionLevel = "Optimal")
```
Compresses the specified directory or file to ZIP format:
- **Description**: 圧縮ファイルを作成します (Creates a compressed file)
- **path**: The path to the directory or file to compress
- **outputPath**: The path for the output ZIP file (if omitted, automatically generated)
- **compressionLevel**: Compression level (Fastest, Optimal, NoCompression). Default is "Optimal"
- **Returns**: JSON-formatted result message

### Unzip
```csharp
public static async Task<string> UnzipAsync(string filePath, string outputPath = "", bool overwrite = false)
```
Extracts a ZIP file:
- **Description**: 圧縮ファイルを展開します (Extracts a compressed file)
- **filePath**: The path to the ZIP file to extract
- **outputPath**: The path for the output directory (if omitted, automatically generated)
- **overwrite**: Whether to overwrite existing files. Default is false.
- **Returns**: JSON-formatted result message

### OpenWithApplication
```csharp
public static string OpenWithApplication(string path, string verb = "open")
```
Opens a file or folder with the default application:
- **Description**: ファイルまたはフォルダを規定のアプリケーションで開く (Opens a file or folder with the default application)
- **path**: The path to the file or folder
- **verb**: The verb to use (open, edit, print, etc.). Default is "open"
- **Returns**: JSON-formatted result message

### OpenWithSpecificApplication
```csharp
public static string OpenWithSpecificApplication(string filePath, string applicationPath, string arguments = "")
```
Opens a file with a specific application:
- **Description**: 指定したプログラムでファイルを開きます (Opens a file with a specified program)
- **filePath**: The path to the file to open
- **applicationPath**: The path to the application to use
- **arguments**: Additional command line arguments
- **Returns**: JSON-formatted result message

### GetFileAssociation
```csharp
public static string GetFileAssociation(string path)
```
Gets information about applications associated with a file:
- **Description**: ファイルに関連付けられたアプリケーション情報を取得します (Gets application information associated with a file)
- **path**: The file path
- **Returns**: JSON-formatted association information Unzip(string filePath)
```
Extracts a ZIP file:
- **filePath**: The path to the ZIP file to extract
- **Returns**: Message about the extraction result

### Launch
```csharp
public static string Launch(string path)
```
Opens a file or folder with the default application:
- **path**: The path to the file or folder
- **Returns**: Message about the execution result

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
