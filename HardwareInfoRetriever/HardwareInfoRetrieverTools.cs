using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Runtime.InteropServices;

namespace HardwareInfoRetrieverTools;

[McpServerToolType]
public static class HardwareInfoRetrieverTools
{
    [McpServerTool, Description("ハードウェア情報を取得します")]
    public static string HardwareInfoRetriever()
    {
        var sb = new StringBuilder();
        
        // YAMLフォーマットで出力
        sb.AppendLine("hardware_info:");
        
        // OS情報の取得
        sb.AppendLine("  os:");
        sb.AppendLine($"    name: '{Environment.OSVersion.VersionString}'");
        sb.AppendLine($"    platform: '{RuntimeInformation.OSDescription}'");
        sb.AppendLine($"    is_64bit: {Environment.Is64BitOperatingSystem}");
        sb.AppendLine($"    machine_name: '{Environment.MachineName}'");
        sb.AppendLine($"    user_name: '{Environment.UserName}'");
        sb.AppendLine($"    system_directory: '{Environment.SystemDirectory}'");
        sb.AppendLine($"    processor_count: {Environment.ProcessorCount}");

        // CPU情報
        sb.AppendLine("  cpu:");
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                sb.AppendLine($"    - name: '{obj["Name"]}'");
                sb.AppendLine($"      manufacturer: '{obj["Manufacturer"]}'");
                sb.AppendLine($"      cores: {obj["NumberOfCores"]}");
                sb.AppendLine($"      logical_processors: {obj["NumberOfLogicalProcessors"]}");
                sb.AppendLine($"      max_clock_speed: {obj["MaxClockSpeed"]} MHz");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"    error: '{ex.Message}'");
        }

        // GPU情報
        sb.AppendLine("  gpu:");
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (var obj in searcher.Get())
            {
                sb.AppendLine($"    - name: '{obj["Name"]}'");
                sb.AppendLine($"      driver_version: '{obj["DriverVersion"]}'");
                sb.AppendLine($"      adapter_ram: {Math.Round(Convert.ToDouble(obj["AdapterRAM"]) / (1024 * 1024 * 1024), 2)} GB");
                sb.AppendLine($"      video_mode_description: '{obj["VideoModeDescription"]}'");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"    error: '{ex.Message}'");
        }

        // メモリ情報
        sb.AppendLine("  memory:");
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            var totalMemory = 0.0;
            var memoryDevices = new List<string>();
            
            foreach (var obj in searcher.Get())
            {
                double capacity = Convert.ToDouble(obj["Capacity"]) / (1024 * 1024 * 1024);
                totalMemory += capacity;
                memoryDevices.Add($"      - type: '{obj["MemoryType"]}', capacity: {Math.Round(capacity, 2)} GB, manufacturer: '{obj["Manufacturer"]}'");
            }
            
            sb.AppendLine($"    total: {Math.Round(totalMemory, 2)} GB");
            sb.AppendLine("    devices:");
            foreach (var device in memoryDevices)
            {
                sb.AppendLine(device);
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"    error: '{ex.Message}'");
        }

        // ディスク容量
        sb.AppendLine("  disks:");
        try
        {
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                sb.AppendLine($"    - name: '{drive.Name}'");
                sb.AppendLine($"      label: '{drive.VolumeLabel}'");
                sb.AppendLine($"      type: '{drive.DriveType}'");
                sb.AppendLine($"      format: '{drive.DriveFormat}'");
                sb.AppendLine($"      total_size: {Math.Round(drive.TotalSize / (1024.0 * 1024 * 1024), 2)} GB");
                sb.AppendLine($"      free_space: {Math.Round(drive.AvailableFreeSpace / (1024.0 * 1024 * 1024), 2)} GB");
                sb.AppendLine($"      used_space: {Math.Round((drive.TotalSize - drive.AvailableFreeSpace) / (1024.0 * 1024 * 1024), 2)} GB");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"    error: '{ex.Message}'");
        }

        // USB機器一覧
        sb.AppendLine("  usb_devices:");
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice");
            var usbDevices = new List<string>();
            
            foreach (var obj in searcher.Get())
            {
                string deviceId = obj["Dependent"].ToString();
                if (!string.IsNullOrEmpty(deviceId))
                {
                    // デバイスIDから「DeviceID="」と末尾の「"」を削除
                    deviceId = deviceId.Substring(deviceId.IndexOf("DeviceID=\"") + 10);
                    deviceId = deviceId.Substring(0, deviceId.Length - 1);
                    
                    // デバイス情報を取得
                    using var deviceSearcher = new ManagementObjectSearcher($"SELECT * FROM Win32_PnPEntity WHERE DeviceID='{deviceId.Replace("\\", "\\\\")}'");
                    foreach (var device in deviceSearcher.Get())
                    {
                        if (device["Name"] != null)
                        {
                            usbDevices.Add($"    - name: '{device["Name"]}'");
                        }
                    }
                }
            }
            
            // 重複を除去してアルファベット順にソート
            foreach (var device in usbDevices.Distinct().OrderBy(d => d))
            {
                sb.AppendLine(device);
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"    error: '{ex.Message}'");
        }

        // ネットワークインターフェース一覧
        sb.AppendLine("  network:");
        
        // ネットワークアダプター
        sb.AppendLine("    adapters:");
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 有効なインターフェースのみ表示
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    var ipProps = nic.GetIPProperties();
                    var ipv4Addresses = ipProps.UnicastAddresses
                        .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(a => a.Address.ToString())
                        .ToList();
                    
                    sb.AppendLine($"      - name: '{nic.Name}'");
                    sb.AppendLine($"        description: '{nic.Description}'");
                    sb.AppendLine($"        type: '{nic.NetworkInterfaceType}'");
                    sb.AppendLine($"        speed: {Math.Round(nic.Speed / 1000000.0, 2)} Mbps");
                    sb.AppendLine($"        mac_address: '{nic.GetPhysicalAddress()}'");
                    sb.AppendLine($"        ipv4_addresses: [{string.Join(", ", ipv4Addresses)}]");
                }
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"        error: '{ex.Message}'");
        }
        
        // TCP接続情報
        sb.AppendLine("    tcp_connections:");
        try
        {
            var tcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            foreach (var connection in tcpConnections.Take(20)) // 接続数が多すぎる場合は制限
            {
                sb.AppendLine($"      - local: '{connection.LocalEndPoint}'");
                sb.AppendLine($"        remote: '{connection.RemoteEndPoint}'");
                sb.AppendLine($"        state: '{connection.State}'");
            }
            
            if (tcpConnections.Length > 20)
            {
                sb.AppendLine($"      - note: '{tcpConnections.Length - 20} more connections not shown'");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"        error: '{ex.Message}'");
        }

        return sb.ToString();
    }
}

