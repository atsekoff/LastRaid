using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace L2Calendar
{
  public class PrefixHandler
  {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IConfigurationRoot _config;

    public PrefixHandler(DiscordSocketClient client, CommandService commands, IConfigurationRoot config)
    {
      _client = client;
      _commands = commands;
      _config = config;
    }

    public Task InitializeAsync()
    {
      _client.MessageReceived += OnMessageReceived;
      return Task.CompletedTask;
    }

    public void AddModule<T>()
    {
      _commands.AddModuleAsync<T>(null);
    }

    private async Task OnMessageReceived(SocketMessage msg)
    {
      if (msg is not SocketUserMessage userMsg)
        return;

      if (userMsg.Author.IsBot)
        return;

      int argPos = 0;
      if (!userMsg.HasCharPrefix(_config["prefix"][0], ref argPos) &&
        !userMsg.HasMentionPrefix(_client.CurrentUser, ref argPos))
        return;

      var context = new SocketCommandContext(_client, userMsg);
      await _commands.ExecuteAsync(
        context: context,
        argPos: argPos,
        services: null);
    }
  }
}
