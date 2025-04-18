# HardwareInfoRetriever

CSharpMcpServer HardwareInfoRetrieverは、Model Context Protocol (MCP) サーバーのシステムハードウェアおよびネットワーク情報の取得ツールを提供するモジュールです。このコンポーネントは、OS、CPU、GPU、メモリ、ディスク、ネットワークアダプターに関する詳細情報をYAML形式で簡単に取得できるようにします。

## 機能
- **HardwareInfoRetriever**: OS、CPU、GPU、メモリ、ディスクなどのハードウェア情報を取得
- **GetNetworkInfo**: ネットワークアダプターおよびTCP接続に関する情報を取得

## API詳細

### HardwareInfoRetriever
```csharp
public static string HardwareInfoRetriever()
```
システムのハードウェア情報をYAML形式で取得します：
- **戻り値**: 以下の情報を含むYAML形式のハードウェア情報
  - **OS情報**: OSのバージョン、プラットフォーム、64ビット有無、マシン名、ユーザー名など
  - **CPU情報**: 名前、製造元、コア数、論理プロセッサ数、最大クロック速度
  - **GPU情報**: 名前、ドライバーバージョン、アダプターRAM、ビデオモード
  - **メモリ情報**: 合計容量およびメモリデバイスの詳細（タイプ、容量、製造元）
  - **ディスク情報**: 名前、ラベル、タイプ、フォーマット、合計サイズ、空き容量、使用容量

### GetNetworkInfo
```csharp
public static string GetNetworkInfo()
```
システムのネットワーク情報をYAML形式で取得します：
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
                "absolute\\path\\to\\CSharpMCPServer\\HardwareInfoRetriever",
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

---

# HardwareInfoRetriever (English)

The CSharpMcpServer HardwareInfoRetriever is a module that provides system hardware and network information retrieval tools for the Model Context Protocol (MCP) server. This component allows easy access to detailed information about OS, CPU, GPU, memory, disks, and network adapters in YAML format.

## Features
- **HardwareInfoRetriever**: Get hardware information including OS, CPU, GPU, memory, and disks
- **GetNetworkInfo**: Get information about network adapters and TCP connections

## API Details

### HardwareInfoRetriever
```csharp
public static string HardwareInfoRetriever()
```
Gets system hardware information in YAML format:
- **Returns**: YAML-formatted hardware information including:
  - **OS information**: OS version, platform, 64-bit status, machine name, user name, etc.
  - **CPU information**: Name, manufacturer, cores, logical processors, max clock speed
  - **GPU information**: Name, driver version, adapter RAM, video mode
  - **Memory information**: Total capacity and memory device details (type, capacity, manufacturer)
  - **Disk information**: Name, label, type, format, total size, free space, used space

### GetNetworkInfo
```csharp
public static string GetNetworkInfo()
```
Gets system network information in YAML format:
- **Returns**: YAML-formatted network information including:
  - **Network adapters**: Name, description, type, speed, MAC address, IPv4 addresses
  - **TCP connections**: Local endpoint, remote endpoint, connection state (showing up to 20 connections)

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/HardwareInfoRetriever
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "HardwareInfoRetriever": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\HardwareInfoRetriever",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\HardwareInfoRetriever` with your actual project path

## Implementation Details

The HardwareInfoRetriever uses the following features to collect system information:

- WMI (Windows Management Instrumentation) queries from the `System.Management` namespace
- Network information APIs from the `System.Net.NetworkInformation` namespace
- System information APIs from `System.Environment` and `System.Runtime.InteropServices`
- Disk information from the `System.IO.DriveInfo` class

The collected data is formatted in YAML and organized hierarchically. Each section has appropriate error handling implemented so that even if specific information cannot be retrieved, the available information is still returned.
