using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TextCommandFramework.Services;

namespace TextCommandFramework;

class Program
{
    public const string PathConfigFile = "config.json";

    // There is no need to implement IDisposable like before as we are
    // using dependency injection, which handles calling Dispose for us.
    static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

    public static IConfiguration _config;

    public async Task MainAsync()
    {
        // First of all save config variables
        _config = BuildConfig();
        // You should dispose a service provider created using ASP.NET
        // when you are finished using it, at the end of your app's lifetime.
        // If you use another dependency injection framework, you should inspect
        // its documentation for the best way to do this.
        using var services = ConfigureServices();
        var client = services.GetRequiredService<DiscordSocketClient>();

        client.Log += LogAsync;
        services.GetRequiredService<CommandService>().Log += LogAsync;

        // Tokens should be considered secret data and never hard-coded.
        // We can read from the environment variable to avoid hard coding.
        await client.LoginAsync(TokenType.Bot, _config["token"]);
        await client.StartAsync();

        // Here we initialize the logic required to register our commands.
        await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

        await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());

        return Task.CompletedTask;
    }

    private static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<HttpClient>()
            .AddSingleton<PictureService>()
            .BuildServiceProvider();
    }

    // Load config file
    public static IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Program.PathConfigFile)
            .Build();
    }
}
