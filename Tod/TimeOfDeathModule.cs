using Discord;
using Discord.Interactions;
using System;
using System.Threading.Tasks;
using static L2Calendar.EpicsDataConst;

namespace L2Calendar.Tod
{
  public class TimeOfDeathModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand("tod", "Creates an event reminder for next boss spawn.")]
    public async Task HandleBossTodCommand(
      BossNames bossName,
      [Summary("Last-known-time-of-death", "In format dd.mm.yyyy hh:mm. Or just hh:mm.")]
      DateTime lastKnownTod,
      [Summary("Heads-up-time", "Get notified ahead of the window start, in minutes. Default is 15.")]
      int headsUpTime = 15)
    {
      TimeSpan spawnTimer = TimeSpan.FromHours(DEATH_DURATIONS[(int)bossName]);
      TimeSpan spawnWindow = TimeSpan.FromHours(WINDOW_DURATIONS[(int)bossName]);
      var lastKnownDeath = new DateTimeOffset(lastKnownTod);
      TimeSpan hoursSinceLastKnownTod = DateTimeOffset.UtcNow.Subtract(lastKnownDeath).Duration();

      if (spawnTimer >= hoursSinceLastKnownTod)
      {
        await HandleExactTod(bossName, lastKnownTod, spawnTimer, spawnWindow, TimeSpan.FromMinutes(headsUpTime));
        return;
      }

      if (hoursSinceLastKnownTod < spawnTimer + spawnWindow)
      {
        await HandleCurrentlyInWindow(bossName, lastKnownDeath, spawnTimer, spawnWindow, TimeSpan.FromMinutes(headsUpTime));
        return;
      }

      await HandleUnknownTod(bossName, hoursSinceLastKnownTod, spawnTimer, spawnWindow, TimeSpan.FromMinutes(headsUpTime));
    }

    private async Task HandleUnknownTod(BossNames bossName, TimeSpan hoursSinceLastKnownTod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var newTod = DateTimeOffset.Now;
      var windowStartTime = newTod;
      var windowEndTime = newTod + deathDuration + windowDuration;
      var possibleSpawnsMsg = $"**{Math.Floor(hoursSinceLastKnownTod / deathDuration)}** possible spawns missed";

      var e = await TodEvent.Create(Context, bossName, windowStartTime, windowEndTime, headsupTime);
      var embedUrl = $"https://discord.com/events/{Context.Guild.Id}/{e.Id}";

      var embed = TodEmbed.Create(bossName, newTod, deathDuration, windowDuration, EPIC_THUMBNAILS[(int)bossName], Color.Red, embedUrl, possibleSpawnsMsg);

      await RespondAsync(embed: embed.Build());
    }

    private async Task HandleCurrentlyInWindow(BossNames bossName, DateTimeOffset tod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var windowStartTime = tod + deathDuration;
      var windowEndTime = tod + deathDuration + windowDuration;
      var e = await TodEvent.Create(Context, bossName, windowStartTime, windowEndTime, headsupTime);
      var embedUrl = $"https://discord.com/events/{Context.Guild.Id}/{e.Id}";
      var embed = TodEmbed.Create(bossName, tod, deathDuration, windowDuration, EPIC_THUMBNAILS[(int)bossName], Color.Orange, embedUrl, "Already in window!");
      await RespondAsync(embed: embed.Build());
    }

    private async Task HandleExactTod(BossNames bossName, DateTimeOffset tod, TimeSpan deathDuration, TimeSpan windowDuration, TimeSpan headsupTime)
    {
      var windowStartTime = tod + deathDuration;
      var windowEndTime = tod + deathDuration + windowDuration;
      var e = await TodEvent.Create(Context, bossName, windowStartTime, windowEndTime, headsupTime);
      var embedUrl = $"https://discord.com/events/{Context.Guild.Id}/{e.Id}";
      var embed = TodEmbed.Create(bossName, tod, deathDuration, windowDuration, EPIC_THUMBNAILS[(int)bossName], Color.Green, embedUrl);

      await RespondAsync(embed: embed.Build());
    }
  }
}
