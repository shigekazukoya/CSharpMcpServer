using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace NetworkInfoProvider;

/// <summary>
/// Provides tools for retrieving network information.
/// </summary>
[McpServerToolType]
public static class NetworkInfoTools
{
    /// <summary>
    /// Retrieves network information.
    /// </summary>
    /// <returns>YAML-formatted network information.</returns>
    [McpServerTool, Description("Retrieves network information")]
    public static string GetNetworkInfo() => 
        YamlFormatter.BuildYaml("network_info", builder => 
        {
            // Network adapters
            NetworkInfoRetriever.RetrieveNetworkAdapterInfo(builder);
            
            // TCP connection information
            NetworkInfoRetriever.RetrieveTcpConnectionInfo(builder);
        });
}
