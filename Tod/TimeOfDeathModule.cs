using Discord;
using Discord.Interactions;
using Discord.Rest;
using System;
using System.Linq;
using System.Threading.Tasks;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  [Group("tod", "Commands for creating RB respawn window reminders.")]
  public class TimeOfDeathModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand("relative", "How long ago it died as HH:MM. Max 24 hours ago!")]
    public async Task HandleRelativeTodCommand(BossNames bossName,
      [Summary("relative-time", "How long since boss death in HH:MM (e.g. 00:25 -> 25 min ago)")]
      DateTime relativeTime,
      [Summary("Heads-up-time", "Get notified ahead of the window start, in minutes. Default is 30.")]
      int headsupTime = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = DateTimeOffset.Now.AddHours(-relativeTime.Hour).AddMinutes(-relativeTime.Minute);
      var headsup = TimeSpan.FromMinutes(headsupTime);

      await HandleTod(bossName, tod, headsup);
    }

    [SlashCommand("exact", "Exact date and time of death.")]
    public async Task HandleExactTodCommand(
      BossNames bossName,
      [Summary("Last-known-time-of-death", "DD.MM.YYYY HH:MM -> Use your local time.")]
      DateTime lastKnownTod,
      [Summary("Local-time", "DD.MM.YYYY HH:MM -> The date and time shown on your pc clock")]
      DateTime userTime,
      [Summary("Heads-up-time", "Get notified ahead of the window start, in minutes. Default is 30.")]
      int headsUpTime = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = new DateTimeOffset(lastKnownTod.ConvertToLocalDateTime(userTime));
      var headsup = TimeSpan.FromMinutes(headsUpTime);

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

    [ComponentInteraction(CANCEL_BUTTON_ID, true)]
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

    [ComponentInteraction(OURS_BUTTON_ID, true)]
    public async Task HandleOursButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Ours);

      await RespondAsync();
    }


    [ComponentInteraction(ENEMIES_BUTTON_ID, true)]
    public async Task HandleEnemiesButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Enemies);
      }

      await RespondAsync();
    }

    [ComponentInteraction(NODROP_BUTTON_ID, true)]
    public async Task HandleNoDropButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.UpdateTodMsgStateAsync(Utils.TodState.NoDrop);
      }

      await RespondAsync();
    }

    [ComponentInteraction(DEAD_BUTTON_ID, true)]
    public async Task HandleDeadButton()
    {
      try
      {
        if (Context.Interaction.TryGetMessage(out var msg))
        {
          await msg.UpdateTodMsgStateAsync(Utils.TodState.Dead);
          await RespondAsync($"**{msg.Embeds.First().Title}** is dead! @everyone");
        }
      }
      catch (Exception ex)
      {
        await RespondAsync(ex.Message, ephemeral: true);
      }
    }

    [ComponentInteraction(SPAWNED_BUTTON_ID, true)]
    public async Task HandleSpawnedButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Spawned);
        await RespondAsync($"**{msg.Embeds.First().Title}** UP UP UP! @everyone FULL DAVAI!");
      }
    }
  }
}
