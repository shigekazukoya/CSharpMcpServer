# HardwareInfoRetriever

[English version here](README.md)

CSharpMcpServer HardwareInfoRetrieverは、Model Context Protocol (MCP) サーバーのシステムハードウェアおよびネットワーク情報の取得ツールを提供するモジュールです。このコンポーネントは、OS、CPU、GPU、メモリ、ディスク、ネットワークアダプターに関する詳細情報をYAML形式で簡単に取得できるようにします。

## 機能
- **HardwareInfoRetriever**: OS、CPU、GPU、メモリ、ディスクなどのハードウェア情報を取得
- **SelectiveHardwareInfo**: 指定したコンポーネントのハードウェア情報のみを取得
- **RefreshHardwareInfo**: キャッシュされたハードウェア情報を強制的に更新
- **GetNetworkInfo**: ネットワークアダプターおよびTCP接続に関する情報を取得

## API詳細

### HardwareInfoRetriever
```csharp
public static string HardwareInfoRetriever()
```
システムのハードウェア情報をYAML形式で取得します：
- **説明**: キャッシュサポート付きの包括的なハードウェア情報を取得します。パフォーマンス向上のために情報は特定の期間キャッシュされます。
- **戻り値**: 以下の情報を含むYAML形式のハードウェア情報
  - **OS情報**: OSのバージョン、プラットフォーム、64ビット有無、マシン名、ユーザー名など
  - **CPU情報**: 名前、製造元、コア数、論理プロセッサ数、最大クロック速度
  - **GPU情報**: 名前、ドライバーバージョン、アダプターRAM、ビデオモード
  - **メモリ情報**: 合計容量およびメモリデバイスの詳細（タイプ、容量、製造元）
  - **ディスク情報**: 名前、ラベル、タイプ、フォーマット、合計サイズ、空き容量、使用容量

### SelectiveHardwareInfo
```csharp
public static string SelectiveHardwareInfo(params string[] components)
```
指定したコンポーネントのハードウェア情報のみを取得します：
- **説明**: 指定したコンポーネントのハードウェア情報のみを取得します。有効なコンポーネント: os, cpu, gpu, memory/ram, storage/disk
- **components**: 取得するハードウェアコンポーネントの配列
- **戻り値**: 指定したコンポーネントの情報を含むYAML形式のハードウェア情報

### RefreshHardwareInfo
```csharp
public static string RefreshHardwareInfo()
```
キャッシュされたハードウェア情報を強制的に更新します：
- **説明**: 既存のキャッシュデータを無視して、最新のシステム情報を取得し、キャッシュを更新します
- **戻り値**: 更新されたYAML形式のハードウェア情報

### GetNetworkInfo
```csharp
public static string GetNetworkInfo()
```
システムのネットワーク情報をYAML形式で取得します：
- **説明**: ネットワーク情報を取得します
- **戻り値**: 以下の情報を含むYAML形式のネットワーク情報
  - **ネットワークアダプター**: 名前、説明、タイプ、速度、MACアドレス、IPv4アドレス
  - **TCP接続**: ローカルエンドポイント、リモートエンドポイント、接続状態（最大20接続まで表示）

## 使用方法

### コンパイルとビルド
- dotnet 8.0以上が必要
- リポジトリのルートディレクトリから以下のコマンドを実行:

```bash
dotnet build CSharpMcpServer/HardwareInfoRetriever
```

### Claude Desktopとの連携
Claude Desktopで使用するには、以下の設定を`claude_desktop_config.json`に追加します:

```json
{
    "mcpServers": {
        "HardwareInfoRetriever": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\Servers\\HardwareInfoRetriever",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**重要**: 
- `absolute\\path\\to\\CSharpMCPServer\\HardwareInfoRetriever`の部分を実際のプロジェクトパスに置き換えてください

## 実装詳細

HardwareInfoRetrieverは、以下の機能を利用してシステム情報を取得します：

- `System.Management`名前空間のWMI（Windows Management Instrumentation）クエリ
- `System.Net.NetworkInformation`名前空間のネットワーク情報取得API
- `System.Environment`と`System.Runtime.InteropServices`のシステム情報取得API
- `System.IO.DriveInfo`クラスのディスク情報取得API

取得したデータはYAML形式でフォーマットされ、階層的に整理されます。各セクションには適切なエラーハンドリングが実装されており、特定の情報を取得できない場合でも、利用可能な情報は返されます。