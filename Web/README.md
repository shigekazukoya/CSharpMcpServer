# Web

CSharpMcpServer Web は、ウェブブラウザとの連携機能を提供するMCP (Model Context Protocol) サーバーモジュールです。このモジュールにより、URLを規定のブラウザで開くなどのウェブ関連機能を簡単に実行できます。

## 機能

- **OpenUrlInDefaultBrowser**: 規定のブラウザでURLを開く
  - 指定したURLをシステムのデフォルトブラウザで開きます
  - エラーハンドリング機能付き

## API詳細

### OpenUrlInDefaultBrowser

```csharp
public static void OpenUrlInDefaultBrowser(string url)
```

指定したURLを規定のブラウザで開きます：
- **説明**: 規定のブラウザでURLを開きます
- **url**: 開きたいURLを指定します
- **戻り値**: なし（void）

## 使用方法

```csharp
// URLを規定のブラウザで開く
OpenUrlInDefaultBrowser("https://example.com");
```

これにより、以下の処理が実行されます：

1. 指定されたURLを、システムの規定のブラウザで開きます
2. 処理中にエラーが発生した場合は、コンソールにエラーメッセージを表示します

## 技術的実装

`OpenUrlInDefaultBrowser`メソッドは、.NET の`Process.Start`メソッドを使用して、システムのデフォルトブラウザでURLを開きます。`UseShellExecute = true`を設定することで、WindowsシェルのURLハンドリング機能を利用しています。

## エラーハンドリング

URLを開く際に問題が発生した場合、例外はキャッチされ、エラーメッセージがコンソールに出力されます。これにより、ユーザーエクスペリエンスを損なうことなく問題を診断できます。

---

# Web (English)

The CSharpMcpServer Web is an MCP (Model Context Protocol) server module that provides web browser integration functionality. This module allows you to easily perform web-related tasks such as opening URLs in the default browser.

## Features

- **OpenUrlInDefaultBrowser**: Opens URLs in the default browser
  - Opens specified URLs in the system's default browser
  - Includes error handling capabilities

## API Details

### OpenUrlInDefaultBrowser

```csharp
public static void OpenUrlInDefaultBrowser(string url)
```

Opens a specified URL in the default browser:
- **Description**: Opens URLs in the default browser
- **url**: The URL to open
- **Returns**: void (no return value)

## Usage

```csharp
// Open a URL in the default browser
OpenUrlInDefaultBrowser("https://example.com");
```

This performs the following operations:

1. Opens the specified URL in the system's default web browser
2. Displays error messages in the console if any issues occur during processing

## Technical Implementation

The `OpenUrlInDefaultBrowser` method uses the .NET `Process.Start` method to open URLs in the system's default browser. It sets `UseShellExecute = true` to leverage the Windows shell's URL handling capabilities.

## Error Handling

When problems occur while opening a URL, exceptions are caught and error messages are output to the console. This allows for problem diagnosis without compromising the user experience.
