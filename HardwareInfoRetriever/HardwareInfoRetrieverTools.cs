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
    [McpServerTool, Description("Retrieves hardware information")]
    public static string HardwareInfoRetriever() => 
        BuildYaml("hardware_info", builder => {
            // OS information
            AppendSection(builder, "os", sb => {
                sb.AppendLine($"    name: '{Environment.OSVersion.VersionString}'")
                  .AppendLine($"    platform: '{RuntimeInformation.OSDescription}'")
                  .AppendLine($"    is_64bit: {Environment.Is64BitOperatingSystem}")
                  .AppendLine($"    machine_name: '{Environment.MachineName}'")
                  .AppendLine($"    user_name: '{Environment.UserName}'")
                  .AppendLine($"    system_directory: '{Environment.SystemDirectory}'")
                  .AppendLine($"    processor_count: {Environment.ProcessorCount}");
            });

            // CPU information
            QueryWmiObjects(builder, "cpu", "Win32_Processor", obj => {
                builder.AppendLine($"    - name: '{obj["Name"]}'")
                       .AppendLine($"      manufacturer: '{obj["Manufacturer"]}'")
                       .AppendLine($"      cores: {obj["NumberOfCores"]}")
                       .AppendLine($"      logical_processors: {obj["NumberOfLogicalProcessors"]}")
                       .AppendLine($"      max_clock_speed: {obj["MaxClockSpeed"]} MHz");
            });

            // GPU information
            QueryWmiObjects(builder, "gpu", "Win32_VideoController", obj => {
                builder.AppendLine($"    - name: '{obj["Name"]}'")
                       .AppendLine($"      driver_version: '{obj["DriverVersion"]}'")
                       .AppendLine($"      adapter_ram: {Math.Round(Convert.ToDouble(obj["AdapterRAM"]) / (1024 * 1024 * 1024), 2)} GB")
                       .AppendLine($"      video_mode_description: '{obj["VideoModeDescription"]}'");
            });

            // Memory information
            AppendSection(builder, "memory", sb => {
                try {
                    using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                    var totalMemory = 0.0;
                    var memoryDevices = new List<string>();
                    
                    foreach (var obj in searcher.Get()) {
                        double capacity = Convert.ToDouble(obj["Capacity"]) / (1024 * 1024 * 1024);
                        totalMemory += capacity;
                        memoryDevices.Add($"      - type: '{obj["MemoryType"]}', capacity: {Math.Round(capacity, 2)} GB, manufacturer: '{obj["Manufacturer"]}'");
                    }
                    
                    sb.AppendLine($"    total: {Math.Round(totalMemory, 2)} GB")
                      .AppendLine("    devices:");
                    
                    foreach (var device in memoryDevices) {
                        sb.AppendLine(device);
                    }
                }
                catch (Exception ex) {
                    sb.AppendLine($"    error: '{ex.Message}'");
                }
            });

            // Disk capacity
            AppendSection(builder, "disks", sb => {
                try {
                    foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady)) {
                        sb.AppendLine($"    - name: '{drive.Name}'")
                          .AppendLine($"      label: '{drive.VolumeLabel}'")
                          .AppendLine($"      type: '{drive.DriveType}'")
                          .AppendLine($"      format: '{drive.DriveFormat}'")
                          .AppendLine($"      total_size: {Math.Round(drive.TotalSize / (1024.0 * 1024 * 1024), 2)} GB")
                          .AppendLine($"      free_space: {Math.Round(drive.AvailableFreeSpace / (1024.0 * 1024 * 1024), 2)} GB")
                          .AppendLine($"      used_space: {Math.Round((drive.TotalSize - drive.AvailableFreeSpace) / (1024.0 * 1024 * 1024), 2)} GB");
                    }
                }
                catch (Exception ex) {
                    sb.AppendLine($"    error: '{ex.Message}'");
                }
            });
        });
    
    [McpServerTool, Description("Retrieves network information")]
    public static string GetNetworkInfo() => 
        BuildYaml("network_info", builder => {
            // Network adapters
            AppendSection(builder, "adapters", sb => {
                try {
                    foreach (var nic in NetworkInterface.GetAllNetworkInterfaces().Where(n => n.OperationalStatus == OperationalStatus.Up)) {
                        var ipProps = nic.GetIPProperties();
                        var ipv4Addresses = ipProps.UnicastAddresses
                            .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            .Select(a => a.Address.ToString())
                            .ToList();
                        
                        sb.AppendLine($"    - name: '{nic.Name}'")
                          .AppendLine($"      description: '{nic.Description}'")
                          .AppendLine($"      type: '{nic.NetworkInterfaceType}'")
                          .AppendLine($"      speed: {Math.Round(nic.Speed / 1000000.0, 2)} Mbps")
                          .AppendLine($"      mac_address: '{nic.GetPhysicalAddress()}'")
                          .AppendLine($"      ipv4_addresses: [{string.Join(", ", ipv4Addresses)}]");
                    }
                }
                catch (Exception ex) {
                    sb.AppendLine($"    error: '{ex.Message}'");
                }
            });
            
            // TCP connection information
            AppendSection(builder, "tcp_connections", sb => {
                try {
                    var tcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
                    foreach (var connection in tcpConnections.Take(20)) {
                        sb.AppendLine($"    - local: '{connection.LocalEndPoint}'")
                          .AppendLine($"      remote: '{connection.RemoteEndPoint}'")
                          .AppendLine($"      state: '{connection.State}'");
                    }
                    
                    if (tcpConnections.Length > 20) {
                        sb.AppendLine($"    - note: '{tcpConnections.Length - 20} more connections not shown'");
                    }
                }
                catch (Exception ex) {
                    sb.AppendLine($"    error: '{ex.Message}'");
                }
            });
        });

    // Helper method to build YAML document
    private static string BuildYaml(string rootName, Action<StringBuilder> buildAction)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{rootName}:");
        buildAction(sb);
        return sb.ToString();
    }

    // Helper method to add YAML section
    private static void AppendSection(StringBuilder sb, string sectionName, Action<StringBuilder> sectionBuilder)
    {
        sb.AppendLine($"  {sectionName}:");
        sectionBuilder(sb);
    }

    // Helper method to retrieve and process WMI information
    private static void QueryWmiObjects(StringBuilder sb, string sectionName, string wmiClass, Action<ManagementBaseObject> processObject)
    {
        AppendSection(sb, sectionName, sectionSb => {
            try {
                using var searcher = new ManagementObjectSearcher($"SELECT * FROM {wmiClass}");
                foreach (var obj in searcher.Get()) {
                    processObject(obj);
                }
            }
            catch (Exception ex) {
                sectionSb.AppendLine($"    error: '{ex.Message}'");
            }
        });
    }
}
