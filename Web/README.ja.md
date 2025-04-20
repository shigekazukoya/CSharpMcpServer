# Web

[English version here](README.md)

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