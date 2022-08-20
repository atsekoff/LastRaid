using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LastRaid.Tod.Modules;
using System;
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
      await _commands.AddModuleAsync<TodButtonsModule>(_services);
      await _commands.AddModuleAsync<TodCommandsModule>(_services);
      _client.InteractionCreated += OnInteractionCreated;
      _commands.InteractionExecuted += OnInteractionExecuted;
    }

    private async Task OnInteractionCreated(SocketInteraction interaction)
    {
      try
      {
        var context = new SocketInteractionContext(_client, interaction);
        var result = await _commands.ExecuteCommandAsync(context, _services);
      }
      catch (Exception e)
      {
        await interaction.RespondAsync($":bangbang: {e.Message}", ephemeral: true);
        Console.WriteLine(e.Message);
      }
    }

    private async Task OnInteractionExecuted(ICommandInfo cmdInfo, IInteractionContext context, IResult result)
    {
      if (context.Interaction.HasResponded) return;

      if (result.IsSuccess)
        await context.Interaction.DeferAsync();
      else
        await context.Interaction.RespondAsync($":x: Command: {cmdInfo} :x:\n{result.ErrorReason}", ephemeral: true);
    }
  }
}
