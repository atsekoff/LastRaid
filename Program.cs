using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using L2Calendar.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace L2Calendar;

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
          LogLevel = LogSeverity.Debug,
          AlwaysDownloadUsers = true
        }))
        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
        .AddSingleton<InteractionHandler>()
        .AddSingleton(x => new CommandService())
        .AddSingleton<PrefixHandler>()
        )
      .Build();
      
    await RunAsync(host, config);
  }

  public static async Task RunAsync(IHost host, IConfigurationRoot config)
  {
    using IServiceScope serviceScope = host.Services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var _slashCommands = provider.GetRequiredService<InteractionService>();
    await provider.GetRequiredService<InteractionHandler>().InitializeAsync(); // registers all available InteractionModules
    _slashCommands.Log += (LogMessage msg) =>
      {
        Console.WriteLine(msg.Message + "(" + msg.Exception.Message + ")");
        return Task.CompletedTask;
      };
   

    var _prefixCommands = provider.GetRequiredService<PrefixHandler>();
    await _prefixCommands.InitializeAsync(); // doesnt register PrefixModules automatically
    _prefixCommands.AddModule<PrefixModule>();

    var client = provider.GetRequiredService<DiscordSocketClient>();
    client.Log += OnClientLog;
    client.Ready += async () => await _slashCommands.RegisterCommandsGloballyAsync(true); await OnClientReady();
    client.GuildScheduledEventStarted += async (SocketGuildEvent e) => await OnEventStarted(e);
    client.GuildScheduledEventCompleted += async (SocketGuildEvent e) => await OnEventCompleted(e);

    await client.LoginAsync(TokenType.Bot, config["token"]);
    await client.StartAsync();

    await Task.Delay(-1);
  }

  private static async Task OnEventCompleted(SocketGuildEvent e)
  {
    string[] eventArgs = e.Location.Split(',');
    ulong channelId = ulong.Parse(eventArgs[0]);
    var channel = e.Guild.GetTextChannel(channelId);
    string msg = $"**{e.Name}** window **ended {TimestampTag.FromDateTimeOffset(DateTimeOffset.UtcNow, TimestampTagStyles.Relative)}**! @noone";

    await channel.SendMessageAsync(msg);
  }

  private static async Task OnEventStarted(SocketGuildEvent e)
  {
    string[] eventArgs = e.Location.Split(',');
    ulong channelId = ulong.Parse(eventArgs[0]);
    int headsUpMinutes = int.Parse(eventArgs[1]);
    var channel = e.Guild.GetTextChannel(channelId);
    string msg = $"**{e.Name}** window **starts {TimestampTag.FromDateTimeOffset(e.StartTime.AddMinutes(headsUpMinutes), TimestampTagStyles.Relative)}**! @noone";

    await channel.SendMessageAsync(msg);
  }

  public static Task OnClientLog(LogMessage msg)
  {
    return Task.CompletedTask.ContinueWith(_ => Console.WriteLine("OnClientLog: " + msg));
  }

  public static Task OnClientReady()
  {
    return Task.CompletedTask.ContinueWith(_ => Console.WriteLine("OnClientReady: Bot ready!"));
  }
}