# NetworkInfo

[English version here](README.md)

CSharpMcpServer NetworkInfoは、Model Context Protocol (MCP) サーバーのネットワーク情報取得機能を提供するモジュールです。このコンポーネントは、ネットワークアダプターとTCP接続に関する詳細情報を取得することを可能にします。

## 機能
- **GetNetworkInfo**: ネットワークアダプターおよびアクティブなTCP接続に関する包括的な情報を取得

## API詳細

### GetNetworkInfo
```csharp
public static string GetNetworkInfo()
```
ネットワーク情報を取得します：
- **説明**: ネットワーク情報を取得します
- **戻り値**: 以下の情報を含むJSONまたはYAML形式のネットワーク情報
  - **ネットワークアダプター**: 名前、説明、タイプ、速度、MACアドレス、IPv4アドレス
  - **TCP接続**: ローカルエンドポイント、リモートエンドポイント、接続状態（最大20接続まで表示）

## 実装詳細

NetworkInfoモジュールは、ネットワーク情報を取得するために以下の.NET APIを使用しています：

- ネットワークアダプター情報用の`System.Net.NetworkInformation`名前空間
- TCP接続情報用の`System.Net.IPGlobalProperties`
- ソケット関連データ用の`System.Net.Sockets`

収集された情報は、理解しやすく解析しやすいように構造化されたフォーマットで提供されます。

## 使用方法

### コンパイルとビルド
- dotnet 8.0以上が必要
- リポジトリのルートディレクトリから以下のコマンドを実行:

```bash
dotnet build CSharpMcpServer/NetworkInfo
```

### Claude Desktopとの連携
Claude Desktopで使用するには、以下の設定を`claude_desktop_config.json`に追加します:

```json
{
    "mcpServers": {
        "NetworkInfo": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\NetworkInfo",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\NetworkInfo`の部分を実際のプロジェクトパスに置き換えてください

## プライバシーに関する考慮事項

このモジュールはローカルネットワーク情報のみにアクセスし、データを送信することはありません。取得される情報には以下が含まれます：

- ネットワークアダプターの詳細（名前、MACアドレス、IPアドレス）
- アクティブなTCP接続（ローカルおよびリモートエンドポイント）

パケット検査やネットワークトラフィック分析は実行されません。