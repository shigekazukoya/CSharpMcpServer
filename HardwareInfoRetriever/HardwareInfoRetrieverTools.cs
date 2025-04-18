using HardwareInfoProvider;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HardwareInfoRetrieverTools;

/// <summary>
/// Provides tools for retrieving hardware information.
/// </summary>
[McpServerToolType]
public static class HardwareInfoRetrieverTools
{
    /// <summary>
    /// Retrieves comprehensive hardware information.
    /// </summary>
    /// <returns>YAML-formatted hardware information.</returns>
    [McpServerTool, Description("Retrieves hardware information")]
    public static string HardwareInfoRetriever() => 
        YamlFormatter.BuildYaml("hardware_info", builder => 
        {
            // OS information
            SystemInfoRetriever.RetrieveOsInfo(builder);

            // CPU information
            ProcessorInfoRetriever.RetrieveCpuInfo(builder);

            // GPU information
            GraphicsInfoRetriever.RetrieveGpuInfo(builder);

            // Memory information
            MemoryInfoRetriever.RetrieveMemoryInfo(builder);

            // Disk information
            StorageInfoRetriever.RetrieveDiskInfo(builder);
        });
}
