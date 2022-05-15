using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  internal static class TodEventTools
  {
    private const char _separator = ',';
    internal static async Task<IGuildScheduledEvent> CreateTodEvent(SocketInteractionContext context, BossNames bossName, DateTimeOffset windowStartTime, DateTimeOffset windowEndTime, TimeSpan headsupTime)
    {
      bool isWindowStarted = windowStartTime <= DateTimeOffset.Now;
      // if window has started, time til window is 0
      var timeTilWindowStart = isWindowStarted ? TimeSpan.Zero : windowStartTime - DateTimeOffset.Now;
      // if there isnt enough time for the wanted heads up notification, notify sooner
      headsupTime = TimeSpan.FromSeconds(Math.Min(headsupTime.TotalSeconds, timeTilWindowStart.TotalSeconds));
      var eventStartTime = isWindowStarted ? DateTimeOffset.Now : windowStartTime - headsupTime;
      // give a little head room for the event to be created and launched properly
      eventStartTime += EVENT_HEADROOM;

      return await context.Guild.CreateEventAsync(
        name: bossName.ToString(),
        startTime: eventStartTime,
        type: GuildScheduledEventType.External,
        endTime: windowEndTime,
        location: $"{context.Channel.Id}",
        description:
          $"Start: **{TimestampTag.FromDateTimeOffset(windowStartTime, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(windowStartTime)})\n" +
          $"End: **{TimestampTag.FromDateTimeOffset(windowEndTime, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(windowEndTime)})");
    }

    internal static string BuildMetadata(ulong channelId, ulong msgId)
    {
      return $"{channelId}{_separator}{msgId}";
    }

    internal static ulong GetChannelId(SocketGuildEvent e)
    {
      return ulong.Parse(e.Location.Split(_separator)[0]);
    }

    internal static ulong GetMsgId(SocketGuildEvent e)
    {
      return ulong.Parse(e.Location.Split(_separator)[1]);
    }
  }
}
