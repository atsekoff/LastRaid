using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace LastRaid;

public class Program
{
  public static async Task Main()
  {
    var config = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("config.json")
      .Build();

    using IHost host = Host.CreateDefaultBuilder()
      .ConfigureServices((_, services) =>
        services
        .AddSingleton(config)
        .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
        {
#if DEBUG
          LogLevel = LogSeverity.Debug,
#else
          LogLevel = LogSeverity.Info,
#endif
          GatewayIntents = GatewayIntents.GuildScheduledEvents | GatewayIntents.Guilds,
          LogGatewayIntentWarnings = true,
        }))
        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
        .AddSingleton<InteractionHandler>()
        )
      .Build();

    await RunAsync(host, config);
  }

  public static async Task RunAsync(IHost host, IConfigurationRoot config)
  {
    using IServiceScope serviceScope = host.Services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var slashCommands = provider.GetRequiredService<InteractionService>();
    await provider.GetRequiredService<InteractionHandler>().InitializeAsync(); // registers all available InteractionModules

    var client = provider.GetRequiredService<DiscordSocketClient>();
    client.Log += OnClientLog;
    client.GuildScheduledEventStarted += async (SocketGuildEvent e) => await OnEventStarted(e);
    client.GuildScheduledEventCompleted += async (SocketGuildEvent e) => await OnEventCompleted(e);
    client.GuildAvailable += async (SocketGuild g) => await slashCommands.RegisterCommandsToGuildAsync(g.Id, true);

#if DEBUG
    await client.LoginAsync(TokenType.Bot, config["token:debug"]);
#else
    await client.LoginAsync(TokenType.Bot, config["token:release"]);
#endif
    await client.StartAsync();

    await Task.Delay(-1);
  }

  private static async Task OnEventCompleted(SocketGuildEvent e)
  {
    string[] eventArgs = e.Location.Split(',');
    ulong channelId = ulong.Parse(eventArgs[0]);
    var channel = e.Guild.GetTextChannel(channelId);
    string msg = $"**{e.Name}** window **ended {TimestampTag.FromDateTimeOffset(DateTimeOffset.UtcNow, TimestampTagStyles.Relative)}**! @LastRaiders";

    await channel.SendMessageAsync(msg);
  }

  private static async Task OnEventStarted(SocketGuildEvent e)
  {
    ulong channelId = ulong.Parse(e.Location);
    var channel = e.Guild.GetTextChannel(channelId);
    string msg = $"**{e.Name}** window reminder @LastRaiders\n{e.GetUrl()}";

    await channel.SendMessageAsync(msg);
  }

  public static Task OnClientLog(LogMessage msg)
  {
    return Task.Run(() => Console.WriteLine(msg));
  }
}