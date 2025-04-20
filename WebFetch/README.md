# WebFetch

[日本語版はこちら](README.ja.md)

The CSharpMcpServer WebFetch is a module that provides web content retrieval tools for the Model Context Protocol (MCP) server. This component enables fetching web pages and extracting their main content.

## Features
- **ExtractMainContentFromUrl**: Fetches URL content and extracts main text by removing ads, navigation, and scripts

## API Details

### ExtractMainContentFromUrl
```csharp
public static async Task<string> ExtractMainContentFromUrl(string url)
```
Extracts main content from a URL:
- **Description**: Fetches URL content and extracts main text by removing ads, navigation, and scripts. Uses semantic HTML elements to identify main content.
- **url**: The URL to fetch content from
- **Returns**: The main content text of the webpage with unwanted elements like ads and navigation removed

## Implementation Details

WebFetchTools extracts content through the following steps:

1. **HTML Fetching**: Retrieves HTML content from the specified URL (with User-Agent header)
2. **Unwanted Element Removal**: Removes noisy elements like ads, navigation, scripts, headers, footers
3. **Main Content Extraction**: Identifies main content with the following priority:
   - article elements
   - main elements
   - div elements with class/id names like "content" or "article"
   - div elements containing the longest text
   - entire body element (as last resort)
4. **Text Cleanup**: Removes consecutive whitespace, empty lines, and decodes HTML entities

## Usage

### Compilation and Building
- Requires dotnet 8.0 or higher
- Run the following command from the repository root directory:

```bash
dotnet build CSharpMcpServer/WebFetch
```

### Integration with Claude Desktop
To use with Claude Desktop, add the following configuration to your `claude_desktop_config.json`:

```json
{
    "mcpServers": {
        "WebFetch": {
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "absolute\\path\\to\\CSharpMCPServer\\WebFetch",
                "--no-build",
                "--"
            ]
        }
    }
}
```

**Important**: 
- Replace `absolute\\path\\to\\CSharpMCPServer\\WebFetch` with your actual project path

## Security

This component only supports HTTP and HTTPS protocols and validates input URLs:
- Rejects empty or null URLs
- Rejects malformed URL formats
- Rejects non-HTTP/HTTPS protocols

It accesses external websites with appropriate User-Agent headers and does not include any functionality to block specific websites.