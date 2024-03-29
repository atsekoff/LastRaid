﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LastRaid.Tod;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LastRaid;

public class Program
{
  public static async Task Main()
  {
    // enforce EU formatting regardless of where the bot is hosted
    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-NL");

    var config = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddEnvironmentVariables()
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
        .AddSingleton(x => new InteractionService(
          x.GetRequiredService<DiscordSocketClient>(),
          new InteractionServiceConfig { LogLevel = LogSeverity.Debug }))
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
    slashCommands.Log += OnCommandLog;
    client.GuildScheduledEventStarted += async (SocketGuildEvent e) => await OnEventStarted(e);
    client.GuildAvailable += async (SocketGuild g) => await slashCommands.RegisterCommandsToGuildAsync(g.Id, true);

#if DEBUG
    await client.LoginAsync(TokenType.Bot, config["DISCORD_DEBUG"]);
#else
    await client.LoginAsync(TokenType.Bot, config["DISCORD_RELEASE"]);
#endif
    await client.StartAsync();

    await Task.Delay(-1);
  }

  private static async Task OnEventStarted(SocketGuildEvent e)
  {
    ulong channelId = TimeEventTools.GetIdFromLocation(e, 0);
    SocketTextChannel channel = e.Guild.GetTextChannel(channelId);
    ulong msgId = TimeEventTools.GetIdFromLocation(e, 1);
    IMessage msg = await channel.GetMessageAsync(msgId);

    if (msg is not IUserMessage userMsg) return;

    string startDescription = e.Description.Split('\n').First();
    _ = userMsg.ReplyAsync($"**{e.Name}** {startDescription} @everyone");

    Console.WriteLine($"Event started: {e.Name}");
  }

  public static Task OnClientLog(LogMessage msg)
  {
    return Task.Run(() => Console.WriteLine(msg));
  }

  private static Task OnCommandLog(LogMessage msg)
  {
    return Task.Run(() => Console.WriteLine(msg));
  }
}