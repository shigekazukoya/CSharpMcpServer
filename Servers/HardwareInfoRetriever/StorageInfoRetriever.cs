using System.Text;

namespace HardwareInfoProvider;

/// <summary>
/// Retrieves storage device information.
/// </summary>
public static class StorageInfoRetriever
{
    /// <summary>
    /// Retrieves disk information and appends it to the provided StringBuilder.
    /// </summary>
    /// <param name="builder">The StringBuilder to append the information to.</param>
    public static void RetrieveDiskInfo(StringBuilder builder)
    {
        YamlFormatter.AppendSection(builder, "disks", sb =>
        {
            try
            {
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    sb.AppendLine($"    - name: '{drive.Name}'")
                      .AppendLine($"      label: '{drive.VolumeLabel}'")
                      .AppendLine($"      type: '{drive.DriveType}'")
                      .AppendLine($"      format: '{drive.DriveFormat}'")
                      .AppendLine($"      total_size: {Math.Round(drive.TotalSize / (1024.0 * 1024 * 1024), 2)} GB")
                      .AppendLine($"      free_space: {Math.Round(drive.AvailableFreeSpace / (1024.0 * 1024 * 1024), 2)} GB")
                      .AppendLine($"      used_space: {Math.Round((drive.TotalSize - drive.AvailableFreeSpace) / (1024.0 * 1024 * 1024), 2)} GB");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"    error: '{ex.Message}'");
            }
        });
    }
}
