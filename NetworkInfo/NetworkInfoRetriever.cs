using System.Net.NetworkInformation;
using System.Text;

namespace NetworkInfoProvider;

/// <summary>
/// Retrieves network information.
/// </summary>
public static class NetworkInfoRetriever
{
    /// <summary>
    /// Retrieves network adapter information and appends it to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to append the information to.</param>
    public static void RetrieveNetworkAdapterInfo(StringBuilder builder)
    {
        YamlFormatter.AppendSection(builder, "adapters", sb =>
        {
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces().Where(n => n.OperationalStatus == OperationalStatus.Up))
                {
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
            catch (Exception ex)
            {
                sb.AppendLine($"    error: '{ex.Message}'");
            }
        });
    }

    /// <summary>
    /// Retrieves TCP connection information and appends it to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to append the information to.</param>
    public static void RetrieveTcpConnectionInfo(StringBuilder builder)
    {
        YamlFormatter.AppendSection(builder, "tcp_connections", sb =>
        {
            try
            {
                var tcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
                foreach (var connection in tcpConnections.Take(20))
                {
                    sb.AppendLine($"    - local: '{connection.LocalEndPoint}'")
                      .AppendLine($"      remote: '{connection.RemoteEndPoint}'")
                      .AppendLine($"      state: '{connection.State}'");
                }
                
                if (tcpConnections.Length > 20)
                {
                    sb.AppendLine($"    - note: '{tcpConnections.Length - 20} more connections not shown'");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"    error: '{ex.Message}'");
            }
        });
    }
}
