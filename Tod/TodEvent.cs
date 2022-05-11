using Discord;
using Discord.Interactions;
using Discord.Rest;
using System;
using System.Threading.Tasks;
using static L2Calendar.EpicsData;

namespace L2Calendar.Tod
{
  internal class TodEvent
  {
    internal static async Task<RestGuildEvent> Create(SocketInteractionContext context, BossNames bossName, DateTimeOffset tod, TimeSpan spawnTimer, TimeSpan spawnWindow, TimeSpan headsupTime)
    {
      var windowStart = tod + spawnTimer;
      var timeTilWindowStart = windowStart - DateTimeOffset.Now;
      headsupTime = TimeSpan.FromSeconds(Math.Min(headsupTime.TotalSeconds, timeTilWindowStart.TotalSeconds - 1));
      var eventStart = windowStart - headsupTime;
      Console.WriteLine(
        $"timeNow: {DateTimeOffset.Now}\n" +
        $"windowStart: {windowStart}\n" +
        $"timeTilWindowStart: {timeTilWindowStart}\n" +
        $"headsup: {headsupTime}\n" +
        $"eventStart: {eventStart}" +
        $"");
      var eventEnd = tod + spawnTimer + spawnWindow;

      return await context.Guild.CreateEventAsync(
        name: bossName.ToString(),
        startTime: eventStart,
        type: GuildScheduledEventType.External,
        endTime: eventEnd,
        location: $"{context.Channel.Id},{headsupTime.Minutes}",
        description:
          $"Start: **{TimestampTag.FromDateTimeOffset(eventStart, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(eventStart)})\n" +
          $"End: **{TimestampTag.FromDateTimeOffset(eventEnd, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(eventEnd)})");
    }
  }
}
