using System.Text;

namespace HardwareInfoProvider;

/// <summary>
/// Retrieves graphics processing unit (GPU) information.
/// </summary>
public static class GraphicsInfoRetriever
{
    /// <summary>
    /// Retrieves GPU information and appends it to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to append the information to.</param>
    public static void RetrieveGpuInfo(StringBuilder builder)
    {
        WmiHelper.QueryWmiObjects(builder, "gpu", "Win32_VideoController", obj =>
        {
            builder.AppendLine($"    - name: '{obj["Name"]}'")
                   .AppendLine($"      driver_version: '{obj["DriverVersion"]}'")
                   .AppendLine($"      adapter_ram: {Math.Round(Convert.ToDouble(obj["AdapterRAM"]) / (1024 * 1024 * 1024), 2)} GB")
                   .AppendLine($"      video_mode_description: '{obj["VideoModeDescription"]}'");
        });
    }
}
