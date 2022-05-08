using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace L2Calendar.Modules
{
  public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
  {
    public enum BossNames { QueenAnt, Core, Orfen, Zaken, Baium, Antharas, Valakas, Frintezza, TestBoss }
    private readonly int[] _timers = { 24, 30, 30, 40, 120, 192, 264, 48, 1 };
    private readonly int[] _windows = { 6, 6, 6, 8, 8, 8, 0, 2, 1 };

    [SlashCommand("tod", "Create a raid boss respawn window event.")]
    public async Task HandleTodCommand(BossNames bossName, uint deathOffsetMinutes, uint notificationOffsetMinutes = 20)
    {
      int respTime = _timers[(int)bossName];
      int window = _windows[(int)bossName];
      DateTimeOffset windowStart = DateTimeOffset.UtcNow.AddHours(respTime).AddMinutes(-deathOffsetMinutes);
      DateTimeOffset windowEnd = window == 0 ? windowStart.AddMinutes(1) : windowStart.AddHours(window);
      DateTimeOffset eventStart = windowStart.AddMinutes(-notificationOffsetMinutes);

      var e = await Context.Guild.CreateEventAsync(
        name: bossName.ToString(),
        startTime: eventStart,
        type: GuildScheduledEventType.External,
        endTime: windowEnd,
        location: $"{Context.Channel.Id}",
        description: $"respawn timers: **{respTime}h + {window}h**\n" +
          $"Start: **{TimestampTag.FromDateTimeOffset(windowStart, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(windowStart)})\n" +
          $"End: **{TimestampTag.FromDateTimeOffset(windowEnd, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(windowEnd)})");

      //await ReplyAsync($"https://discord.com/events/{e.Guild.Id}/{e.Id}");
      await RespondAsync($"https://discord.com/events/{e.Guild.Id}/{e.Id}");
      //await Task.Delay(5000);
      //await DeleteOriginalResponseAsync();
    }

    [SlashCommand("components", "Demonstrate buttons and select menus.")]
    public async Task HandleComponentsCommand()
    {
      var button = new ButtonBuilder()
      {
        Label = "ButtonLabel",
        CustomId = "custombuttonid",
        Style = ButtonStyle.Primary
      };

      var menu = new SelectMenuBuilder()
      {
        CustomId = "custommenuid",
        Placeholder = "Menu placeholder",
      }
      .AddOption("First option", "first")
      .AddOption("Second option", "second");

      var component = new ComponentBuilder()
        .WithButton(button)
        .WithSelectMenu(menu);

      await RespondAsync("testing components", components: component.Build());
    }

    [MessageCommand("Bookmark")]
    public async Task Bookmark(IMessage msg)
    {
      var component = new ComponentBuilder()
        .AddRow(new ActionRowBuilder())
        .Build();

      await RespondAsync(msg.Content);
    }

    [ComponentInteraction("custommenuid")]
    public async Task HandleMenuSelection(string[] args)
    {
      await RespondAsync(args[0]);
    }

    [ComponentInteraction("custombuttonid")]
    public async Task HandleButtonInput()
    {
      await RespondWithModalAsync<DemoModal>("demo_modal");
    }

    [ModalInteraction("demo_modal")]
    public async Task HandleModalInput(DemoModal modal)
    {
      string input = modal.Greeting;
      await RespondAsync(input);
    }
  }

  public class DemoModal : IModal
  {
    public string Title => "Demo modal";

    [InputLabel("Send a greeting!")]
    [ModalTextInput("greeting_input", TextInputStyle.Short, placeholder: "Be nice...", maxLength: 100)]
    public string Greeting { get; set; }
  }
}
