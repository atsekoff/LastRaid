using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LastRaid
{
  public class InteractionHandler
  {
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
    {
      _client = client;
      _commands = commands;
      _services = services;
    }

    public async Task InitializeAsync()
    {
      await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
      _client.InteractionCreated += OnInteractionCreated;
    }

    private async Task OnInteractionCreated(SocketInteraction interaction)
    {
      try
      {
        var context = new SocketInteractionContext(_client, interaction);
        var result = await _commands.ExecuteCommandAsync(context, _services);
        if (!result.IsSuccess)
          await interaction.RespondAsync($":x: {result.ErrorReason}", ephemeral: true);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
  }
}
