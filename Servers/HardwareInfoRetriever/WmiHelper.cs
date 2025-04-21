using System.Management;
using System.Text;

namespace HardwareInfoProvider;

/// <summary>
/// Provides utilities for working with Windows Management Instrumentation (WMI).
/// </summary>
public static class WmiHelper
{
    /// <summary>
    /// Queries WMI objects and processes each object with the specified action.
    /// </summary>
    /// <param name="sb">The StringBuilder to append YAML content to.</param>
    /// <param name="sectionName">Name of the section.</param>
    /// <param name="wmiClass">WMI class to query.</param>
    /// <param name="processObject">Action to process each WMI object.</param>
    public static void QueryWmiObjects(StringBuilder sb, string sectionName, string wmiClass, Action<ManagementBaseObject> processObject)
    {
        YamlFormatter.AppendSection(sb, sectionName, sectionSb =>
        {
            try
            {
                using var searcher = new ManagementObjectSearcher($"SELECT * FROM {wmiClass}");
                foreach (var obj in searcher.Get())
                {
                    processObject(obj);
                }
            }
            catch (Exception ex)
            {
                sectionSb.AppendLine($"    error: '{ex.Message}'");
            }
        });
    }
}
