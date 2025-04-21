using HardwareInfoProvider;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HardwareInfoRetrieverTools;

[McpServerToolType]
public static class HardwareInfoRetrieverTools
{
    private const string VERSION = "1.1.0";
    private const int CACHE_DURATION = 60;
    private static string? _cachedFullHardwareInfo;
    private static DateTime _lastCacheTime = DateTime.MinValue;
    
    [McpServerTool, Description("Retrieves comprehensive hardware information with caching support. Information is cached for a specific duration to improve performance for repeated calls.")]
    public static string HardwareInfoRetriever()
    {
        if (_cachedFullHardwareInfo != null && 
            (DateTime.Now - _lastCacheTime).TotalSeconds < CACHE_DURATION)
        {
            return _cachedFullHardwareInfo;
        }
        
        try
        {
            _cachedFullHardwareInfo = YamlFormatter.BuildYaml("hardware_info", builder => 
            {
                builder.AppendLine($"  version: '{VERSION}'");
                builder.AppendLine($"  timestamp: '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'");
                
                SystemInfoRetriever.RetrieveOsInfo(builder);
                ProcessorInfoRetriever.RetrieveCpuInfo(builder);
                GraphicsInfoRetriever.RetrieveGpuInfo(builder);
                MemoryInfoRetriever.RetrieveMemoryInfo(builder);
                StorageInfoRetriever.RetrieveDiskInfo(builder);
            });
            
            _lastCacheTime = DateTime.Now;
            return _cachedFullHardwareInfo;
        }
        catch (Exception ex)
        {
            return YamlFormatter.BuildYaml("hardware_info", builder => 
            {
                builder.AppendLine($"  error: 'Failed to retrieve hardware information: {ex.Message}'");
                builder.AppendLine($"  stack_trace: '{ex.StackTrace}'");
            });
        }
    }
    
    [McpServerTool, Description("Retrieves only the specified components of hardware information. Valid components: os, cpu, gpu, memory/ram, storage/disk.")]
    public static string SelectiveHardwareInfo(params string[] components)
    {
        try
        {
            return YamlFormatter.BuildYaml("hardware_info", builder => 
            {
                builder.AppendLine($"  version: '{VERSION}'");
                builder.AppendLine($"  timestamp: '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'");
                
                var requestedComponents = components.Select(c => c.ToLower()).ToArray();
                
                if (requestedComponents.Contains("os"))
                    SystemInfoRetriever.RetrieveOsInfo(builder);
                
                if (requestedComponents.Contains("cpu"))
                    ProcessorInfoRetriever.RetrieveCpuInfo(builder);
                
                if (requestedComponents.Contains("gpu"))
                    GraphicsInfoRetriever.RetrieveGpuInfo(builder);
                
                if (requestedComponents.Contains("memory") || requestedComponents.Contains("ram"))
                    MemoryInfoRetriever.RetrieveMemoryInfo(builder);
                
                if (requestedComponents.Contains("storage") || requestedComponents.Contains("disk"))
                    StorageInfoRetriever.RetrieveDiskInfo(builder);
                
                if (components.Length == 0 || !requestedComponents.Any(c => 
                    c == "os" || c == "cpu" || c == "gpu" || c == "memory" || 
                    c == "ram" || c == "storage" || c == "disk"))
                {
                    builder.AppendLine("  warning: 'No valid components specified. Valid components are: os, cpu, gpu, memory/ram, storage/disk'");
                }
            });
        }
        catch (Exception ex)
        {
            return YamlFormatter.BuildYaml("hardware_info", builder => 
            {
                builder.AppendLine($"  error: 'Failed to retrieve hardware information: {ex.Message}'");
                builder.AppendLine($"  stack_trace: '{ex.StackTrace}'");
            });
        }
    }
    
    [McpServerTool, Description("Forces a refresh of the cached hardware information, ignoring any existing cached data and retrieving the latest system information.")]
    public static string RefreshHardwareInfo()
    {
        _cachedFullHardwareInfo = null;
        return HardwareInfoRetriever();
    }
}
