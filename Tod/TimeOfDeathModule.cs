using Discord;
using Discord.Interactions;
using Discord.Rest;
using System;
using System.Linq;
using System.Threading.Tasks;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  [Group(COMMAND_NAME_TOD, COMMAND_DESCRIPTION_TOD)]
  public class TimeOfDeathModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand(COMMAND_NAME_RELATIVE, COMMAND_DESCRIPTION_RELATIVE)]
    public async Task HandleRelativeTodCommand(BossNames bossName,
      [Summary(PARAM_NAME_RELATIVE_TIME, PARAM_DESCRIPTION_RELATIVE_TIME)]
      DateTime relativeTime,
      [Summary(PARAM_NAME_HEADS_UP, PARAM_DESCRIPTION_HEADS_UP)]
      int headsUpMinutes = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = DateTimeOffset.Now.AddHours(-relativeTime.Hour).AddMinutes(-relativeTime.Minute);
      var headsup = TimeSpan.FromMinutes(headsUpMinutes);

      await HandleTod(bossName, tod, headsup);
    }

    [SlashCommand(COMMAND_NAME_EXACT, COMMAND_DESCRIPTION_EXACT)]
    public async Task HandleExactTodCommand(
      BossNames bossName,
      [Summary(PARAM_NAME_LAST_TOD, PARAM_DESCRIPTION_LAST_TOD)]
      DateTime lastKnownTod,
      [Summary(PARAM_NAME_USER_TIME, PARAM_DESCRIPTION_USER_TIME)]
      DateTime userTime,
      [Summary(PARAM_NAME_HEADS_UP, PARAM_DESCRIPTION_HEADS_UP)]
      int headsUpMinutes = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = new DateTimeOffset(lastKnownTod.ConvertToLocalDateTime(userTime));
      var headsup = TimeSpan.FromMinutes(headsUpMinutes);

      await HandleTod(bossName, tod, headsup);
    }

    private async Task HandleTod(BossNames bossName, DateTimeOffset tod, TimeSpan headsupTime)
    {
      if (Context.TryGetTodEvent(bossName, out var e))
      {
        await RespondAsync($"**{bossName}** reminder already exists: {e.GetUrl()}", ephemeral: true);
        return;
      }

      TimeSpan spawnTimer = TimeSpan.FromHours(DEATH_DURATIONS[(int)bossName]);
      TimeSpan spawnWindow = TimeSpan.FromHours(WINDOW_DURATIONS[(int)bossName]);
      TimeSpan hoursSinceLastKnownTod = DateTimeOffset.UtcNow.Subtract(tod).Duration();

      if (spawnTimer >= hoursSinceLastKnownTod)
      {
        await HandleExactTod(bossName, tod, spawnTimer, spawnWindow, headsupTime);
        return;
      }

      if (hoursSinceLastKnownTod < spawnTimer + spawnWindow)
      {
        await HandleCurrentlyInWindow(bossName, tod, spawnTimer, spawnWindow, headsupTime);
        return;
      }

      await HandleUnknownTod(bossName, hoursSinceLastKnownTod, spawnTimer, spawnWindow, headsupTime);
    }

    private async Task HandleUnknownTod(BossNames bossName, TimeSpan hoursSinceLastKnownTod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var newTod = DateTimeOffset.Now;
      var windowStartTime = newTod;
      var windowEndTime = newTod + deathDuration + windowDuration;
      var possibleSpawnsMsg = $"**{Math.Floor(hoursSinceLastKnownTod / deathDuration)}** possible spawns missed";
      await CreateTodReminder(bossName, newTod, windowStartTime, windowEndTime, headsupTime, possibleSpawnsMsg);
    }

    private async Task HandleCurrentlyInWindow(BossNames bossName, DateTimeOffset tod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var windowStartTime = tod + deathDuration;
      var windowEndTime = tod + deathDuration + windowDuration;
      string description = "Already in window!";
      await CreateTodReminder(bossName, tod, windowStartTime, windowEndTime, headsupTime, description);
    }

    private async Task HandleExactTod(BossNames bossName, DateTimeOffset tod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var windowStartTime = tod + deathDuration;
      var windowEndTime = tod + deathDuration + windowDuration;
      await CreateTodReminder(bossName, tod, windowStartTime, windowEndTime, headsupTime);
    }

    private async Task CreateTodReminder(BossNames bossName, DateTimeOffset tod, DateTimeOffset windowStartTime, DateTimeOffset windowEndTime, TimeSpan headsupTime, string desc = "")
    {
      try
      {
        var component = TodComponentTools.CreateInitialTodComponent();
        IGuildScheduledEvent? e = await TodEventTools.CreateTodEvent(Context, bossName, windowStartTime, windowEndTime, headsupTime);

        try
        {
          EmbedBuilder embed = TodEmbedTools.CreateTodEmbed(bossName, tod, windowStartTime, windowEndTime, e.GetUrl(), desc);
          await RespondAsync(components: component.Build(), embed: embed.Build());
          RestInteractionMessage msg = await Context.Interaction.GetOriginalResponseAsync();
          await e.ModifyAsync(ep => ep.Location = TodEventTools.BuildMetadata(Context.Channel.Id, msg.Id));
        }
        catch (Exception ex)
        {
          await RespondAsync($"Failed to create a reminder: {ex.Message}", ephemeral: true);
          await e.DeleteAsync();
        }
      }
      catch (Exception ex)
      {
        await RespondAsync($"Failed to create event: {ex.Message}", ephemeral: true);
      }
    }

    [ComponentInteraction(BUTTON_ID_CONFIRM, true)]
    public async Task HandleConfirmButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Confirm, Context);

      await RespondAsync();
    }

    [ComponentInteraction(BUTTON_ID_CANCEL, true)]
    public async Task HandleCancelButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        _ = msg.DeleteAsync();

        if (msg.TryGetTodEvent(out var e))
          _ = e.DeleteAsync();
      }

      await RespondAsync();
    }

    [ComponentInteraction(BUTTON_ID_OURS, true)]
    public async Task HandleOursButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Ours, Context);

      await RespondAsync();
    }


    [ComponentInteraction(BUTTON_ID_ENEMIES, true)]
    public async Task HandleEnemiesButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Enemies, Context);
      }

      await RespondAsync();
    }

    [ComponentInteraction(BUTTON_ID_NO_DROP, true)]
    public async Task HandleNoDropButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.UpdateTodMsgStateAsync(Utils.TodState.NoDrop, Context);
      }

      await RespondAsync();
    }
  }
}
