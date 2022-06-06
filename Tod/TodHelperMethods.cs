using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  internal static class TodHelperMethods
  {
    internal static async Task HandleTod(this SocketInteractionContext context, BossNames bossName, DateTimeOffset tod, TimeSpan headsupTime)
    {
      if (context.TryGetTodEvent(bossName, out var e))
      {
        await context.HandleExistingEvent(e, bossName, tod, headsupTime);
        return;
      }

      TimeSpan spawnTimer = TimeSpan.FromHours(DEATH_DURATIONS[(int)bossName]);
      TimeSpan spawnWindow = TimeSpan.FromHours(WINDOW_DURATIONS[(int)bossName]);
      TimeSpan hoursSinceLastKnownTod = DateTimeOffset.UtcNow.Subtract(tod).Duration();

      if (spawnTimer >= hoursSinceLastKnownTod)
      {
        await context.HandleExactTod(bossName, tod, spawnTimer, spawnWindow, headsupTime);
        return;
      }

      if (hoursSinceLastKnownTod < spawnTimer + spawnWindow)
      {
        await context.HandleCurrentlyInWindow(bossName, tod, spawnTimer, spawnWindow, headsupTime);
        return;
      }

      await context.HandleUnknownTod(bossName, hoursSinceLastKnownTod, spawnTimer, spawnWindow, headsupTime);
    }

    internal static async Task HandleExistingEvent(this SocketInteractionContext context, SocketGuildEvent e, BossNames bossName, DateTimeOffset tod, TimeSpan headsupTime)
    {
      var comp = TodComponentTools.CreateExistingEventComponent(e.Id, bossName, tod, headsupTime);
      await context.Interaction.RespondAsync($"**{bossName}** reminder already exists: {e.GetUrl()}", ephemeral: true, components: comp.Build());
    }

    internal static async Task HandleUnknownTod(this SocketInteractionContext context, BossNames bossName, TimeSpan hoursSinceLastKnownTod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var newTod = DateTimeOffset.Now;
      var windowStartTime = newTod;
      var windowEndTime = newTod + deathDuration + windowDuration;
      var possibleSpawnsMsg = $"**{Math.Floor(hoursSinceLastKnownTod / deathDuration)}** possible spawns missed";
      await context.CreateTodReminder(bossName, newTod, windowStartTime, windowEndTime, headsupTime, possibleSpawnsMsg);
    }

    internal static async Task HandleCurrentlyInWindow(this SocketInteractionContext context, BossNames bossName, DateTimeOffset tod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var windowStartTime = tod + deathDuration;
      var windowEndTime = tod + deathDuration + windowDuration;
      string description = "Already in window!";
      await context.CreateTodReminder(bossName, tod, windowStartTime, windowEndTime, headsupTime, description);
    }

    internal static async Task HandleExactTod(this SocketInteractionContext context, BossNames bossName, DateTimeOffset tod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var windowStartTime = tod + deathDuration;
      var windowEndTime = tod + deathDuration + windowDuration;
      await context.CreateTodReminder(bossName, tod, windowStartTime, windowEndTime, headsupTime);
    }

    internal static async Task CreateTodReminder(this SocketInteractionContext context, BossNames bossName, DateTimeOffset tod, DateTimeOffset windowStartTime, DateTimeOffset windowEndTime, TimeSpan headsupTime, string desc = "")
    {
      try
      {
        var component = TodComponentTools.CreateDropComponent();
        IGuildScheduledEvent? e = await TodEventTools.CreateTodEvent(context, bossName, windowStartTime, windowEndTime, headsupTime);

        try
        {
          EmbedBuilder embed = TodEmbedTools.CreateTodEmbed(bossName, tod, windowStartTime, windowEndTime, e.GetUrl(), desc);
          await context.Interaction.RespondAsync(components: component.Build(), embed: embed.Build());
          RestInteractionMessage msg = await context.Interaction.GetOriginalResponseAsync();
          await e.ModifyAsync(ep => ep.Location = TodEventTools.BuildMetadata(context.Channel.Id, msg.Id));
        }
        catch (Exception ex)
        {
          await context.Interaction.RespondAsync($"Failed to create a reminder: {ex.Message}", ephemeral: true);
          await e.DeleteAsync();
        }
      }
      catch (Exception ex)
      {
        await context.Interaction.RespondAsync($"Failed to create event: {ex.Message}", ephemeral: true);
      }
    }
  }
}
