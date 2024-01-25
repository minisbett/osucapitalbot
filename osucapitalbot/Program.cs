using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using dotenv.net;
using osucapitalbot.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using osucapitalbot.Services;
using System.Globalization;

public class Program
{
  /// <summary>
  /// The version of the application.
  /// </summary>
  public const string VERSION = "1.0.0";

  /// <summary>
  /// The startup time of the application.
  /// </summary>
  public static readonly DateTime STARTUP_TIME;

  static Program()
  {
    STARTUP_TIME = DateTime.UtcNow;
  }

  public static async Task Main(string[] args)
  {
    // Run the host in a try-catch block to catch any unhandled exceptions.
    try
    {
      await MainAsync(args);
    }
    catch (Exception ex)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(ex);
      Console.ForegroundColor = ConsoleColor.Gray;
      Environment.ExitCode = 727;
    }
  }

  public static async Task MainAsync(string[] args)
  {
    // Load the .env file. (Only useful when debugging locally, not when running it via e.g. Docker)
    DotEnv.Load();

    // Ensure a consistent culture for parsing & formatting.
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

    // Build the generic host.
#pragma warning disable CS0618
    IHost host = Host.CreateDefaultBuilder()
      // Configure the host to use environment variables for the config.
      .ConfigureHostConfiguration(config => config.AddEnvironmentVariables())

      // Configure the logging to have timestamps.
      .ConfigureLogging(logging =>
      {
        logging.AddSimpleConsole(options =>
        {
          options.TimestampFormat = "[HH:mm:ss] ";
          options.UseUtcTimestamp = true;
          options.ColorBehavior = LoggerColorBehavior.Enabled;
        });

        // Exclude HttpClients and DB commands from logging, as they spam the logs.
        logging.AddFilter("System.Net.Http.HttpClient", LogLevel.None);
        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
      })

      // Configure the Discord host (bot token, log level, bot behavior etc.)
      .ConfigureDiscordHost((context, config) =>
      {
        config.SocketConfig = new DiscordSocketConfig()
        {
          LogLevel = LogSeverity.Verbose,
          AlwaysDownloadUsers = true,
          MessageCacheSize = 100,
          GatewayIntents = GatewayIntents.AllUnprivileged
        };

        config.Token = context.Configuration["BOT_TOKEN"]
          ?? throw new InvalidOperationException("The environment variable 'BOT_TOKEN' is not set.");
      })

      // Configure Discord.NET's interaction service.
      .UseInteractionService((context, config) =>
      {
        config.LogLevel = LogSeverity.Verbose;
        config.UseCompiledLambda = true;
      })

      // Configure further services necessary in the application's lifetime.
      .ConfigureServices((context, services) =>
      {
        // Add the handler for Discord interactions.
        services.AddHostedService<InteractionHandler>();

        // Add the UserWatcherService, responsible for the stock live-feed.
        services.AddHostedService<PlayerWatcherService>();

        // Add the osu! API service for communicating with the osu! API.
        services.AddSingleton<OsuApiService>();

        // Add the osu!capital API service for communicating with osu!capital.
        services.AddSingleton<OsuCapitalApiService>();

        // Register the persistence service, responsible for providing logic for accessing the persistence database.
        services.AddScoped<PersistenceService>();

        // Add the caching service.
        services.AddScoped<CachingService>();

        // Add an http client for communicating with the Huis API.
        services.AddHttpClient("capitalapi", client =>
        {
          client.BaseAddress = new Uri("https://osucapital.com/api/public/");
          client.DefaultRequestHeaders.Add("User-Agent", $"osucapitalbot/{VERSION}");
        });

        // Add an http client for communicating with the osu! API.
        services.AddHttpClient("osuapi", client =>
        {
          client.BaseAddress = new Uri("https://osu.ppy.sh/");
          client.DefaultRequestHeaders.Add("User-Agent", $"osucapitalbot/{VERSION}");
        });

        // Register our data context for accessing our database.
        services.AddDbContext<Database>(options =>
        {
          options.UseSqlite("Data Source=database.db");
          options.UseSnakeCaseNamingConvention();
        });
      })
      .Build();
#pragma warning restore CS0618

    // Run migrations on the database.
    await host.Services.GetRequiredService<Database>().Database.MigrateAsync();

    // Ensure that all APIs are available.
    OsuApiService osu = host.Services.GetRequiredService<OsuApiService>();
    OsuCapitalApiService capital = host.Services.GetRequiredService<OsuCapitalApiService>();
    if (!(await osu.CheckAvailableAsync()).IsSuccessful)
      throw new Exception("The osu! API v2 was deemed unavailable at startup.");
    if (!(await capital.CheckAvailableAsync()).IsSuccessful)
      throw new Exception("The osu!capital API was deemed unavailable at startup.");

    // Run the host.
    await host.RunAsync();
  }
}