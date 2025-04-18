using System.Runtime.InteropServices;
using System.Text;

namespace HardwareInfoProvider;

/// <summary>
/// Retrieves operating system information.
/// </summary>
public static class SystemInfoRetriever
{
    /// <summary>
    /// Retrieves operating system information and appends it to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to append the information to.</param>
    public static void RetrieveOsInfo(StringBuilder builder)
    {
        YamlFormatter.AppendSection(builder, "os", sb =>
        {
            sb.AppendLine($"    name: '{Environment.OSVersion.VersionString}'")
              .AppendLine($"    platform: '{RuntimeInformation.OSDescription}'")
              .AppendLine($"    is_64bit: {Environment.Is64BitOperatingSystem}")
              .AppendLine($"    machine_name: '{Environment.MachineName}'")
              .AppendLine($"    user_name: '{Environment.UserName}'")
              .AppendLine($"    system_directory: '{Environment.SystemDirectory}'")
              .AppendLine($"    processor_count: {Environment.ProcessorCount}");
        });
    }
}
