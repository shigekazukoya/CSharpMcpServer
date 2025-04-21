using System.Text;

namespace NetworkInfoProvider;

/// <summary>
/// Provides utilities for formatting data as YAML.
/// </summary>
public static class YamlFormatter
{
    /// <summary>
    /// Builds a YAML document with the specified root name.
    /// </summary>
    /// <param name="rootName">Name of the root element.</param>
    /// <param name="buildAction">Action to build the YAML content.</param>
    /// <returns>A string containing the YAML document.</returns>
    public static string BuildYaml(string rootName, Action<StringBuilder> buildAction)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{rootName}:");
        buildAction(sb);
        return sb.ToString();
    }

    /// <summary>
    /// Appends a section to a YAML document.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="sectionName">Name of the section.</param>
    /// <param name="sectionBuilder">Action to build the section content.</param>
    public static void AppendSection(StringBuilder sb, string sectionName, Action<StringBuilder> sectionBuilder)
    {
        sb.AppendLine($"  {sectionName}:");
        sectionBuilder(sb);
    }
}
