using System.Management;
using System.Text;

namespace HardwareInfoProvider;

/// <summary>
/// Retrieves physical memory information.
/// </summary>
public static class MemoryInfoRetriever
{
    /// <summary>
    /// Retrieves memory information and appends it to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to append the information to.</param>
    public static void RetrieveMemoryInfo(StringBuilder builder)
    {
        YamlFormatter.AppendSection(builder, "memory", sb =>
        {
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
                
                sb.AppendLine($"    total: {Math.Round(totalMemory, 2)} GB")
                  .AppendLine("    devices:");
                
                foreach (var device in memoryDevices)
                {
                    sb.AppendLine(device);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"    error: '{ex.Message}'");
            }
        });
    }
}
