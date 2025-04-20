# Web

[日本語版はこちら](README.ja.md)

The CSharpMcpServer Web is an MCP (Model Context Protocol) server module that provides web browser integration functionality. This module allows you to easily perform web-related tasks such as opening URLs in the default browser.

## Features

- **OpenUrlInDefaultBrowser**: Opens URLs in the default browser
  - Opens specified URLs in the system's default browser
  - Includes error handling capabilities

## API Details

### OpenUrlInDefaultBrowser

```csharp
public static void OpenUrlInDefaultBrowser(string url)
```

Opens a specified URL in the default browser:
- **Description**: Opens URLs in the default browser
- **url**: The URL to open
- **Returns**: void (no return value)

## Usage

```csharp
// Open a URL in the default browser
OpenUrlInDefaultBrowser("https://example.com");
```

This performs the following operations:

1. Opens the specified URL in the system's default web browser
2. Displays error messages in the console if any issues occur during processing

## Technical Implementation

The `OpenUrlInDefaultBrowser` method uses the .NET `Process.Start` method to open URLs in the system's default browser. It sets `UseShellExecute = true` to leverage the Windows shell's URL handling capabilities.

## Error Handling

When problems occur while opening a URL, exceptions are caught and error messages are output to the console. This allows for problem diagnosis without compromising the user experience.