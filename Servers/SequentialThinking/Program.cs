using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.Threading.Tasks;

namespace SequentialThinking
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            
            // Configure console logging to stderr as per MCP standard
            builder.Logging.AddConsole(options =>
            {
                options.LogToStandardErrorThreshold = LogLevel.Trace;
            });
            
            // Register MCP server services
            builder.Services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly(); // This will find and register all tools marked with [McpServerToolType]
            
            var app = builder.Build();
            await app.RunAsync();
        }
    }
}