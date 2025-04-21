using System.Text;

namespace HardwareInfoProvider;

/// <summary>
/// Retrieves processor (CPU) information.
/// </summary>
public static class ProcessorInfoRetriever
{
    /// <summary>
    /// Retrieves processor information and appends it to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to append the information to.</param>
    public static void RetrieveCpuInfo(StringBuilder builder)
    {
        WmiHelper.QueryWmiObjects(builder, "cpu", "Win32_Processor", obj =>
        {
            builder.AppendLine($"    - name: '{obj["Name"]}'")
                   .AppendLine($"      manufacturer: '{obj["Manufacturer"]}'")
                   .AppendLine($"      cores: {obj["NumberOfCores"]}")
                   .AppendLine($"      logical_processors: {obj["NumberOfLogicalProcessors"]}")
                   .AppendLine($"      max_clock_speed: {obj["MaxClockSpeed"]} MHz");
        });
    }
}
