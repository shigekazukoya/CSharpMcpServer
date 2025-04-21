using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HardwareInfoProvider;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateEmptyApplicationBuilder(settings: null);

        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        var app = builder.Build();

        await app.RunAsync();
    }
}
