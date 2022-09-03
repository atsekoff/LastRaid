using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using static LastRaid.Consts;

namespace LastRaid.Tod
{
  internal static class TimeEventTools
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "I like it like this.")]
    private const char _SEPARATOR = ',';

    internal static async Task<IGuildScheduledEvent> CreateTimeEvent(this SocketInteractionContext context, string eventName, DateTimeOffset eventStartTime, DateTimeOffset eventEndTime, TimeSpan headsupTime, TimeSpan? lingerTime = null)
    {
      bool isAlreadyStarted = eventStartTime <= DateTimeOffset.Now;
      // if window has started, time til window is 0
      var timeTilWindowStart = isAlreadyStarted ? TimeSpan.Zero : eventStartTime - DateTimeOffset.Now;
      // if there isnt enough time for the wanted heads up notification, notify sooner
      headsupTime = TimeSpan.FromSeconds(Math.Min(headsupTime.TotalSeconds, timeTilWindowStart.TotalSeconds));
      var actualStartTime = isAlreadyStarted ? DateTimeOffset.Now : eventStartTime - headsupTime;
      // give a little head room for the event to be created and launched properly
      actualStartTime += EVENT_HEADROOM;

      return await context.Guild.CreateEventAsync(
        name: eventName,
        startTime: actualStartTime,
        type: GuildScheduledEventType.External,
        endTime: eventEndTime + (lingerTime ?? TimeSpan.Zero),
        location: $"{context.Channel.Id}",
        description:
          $"Start: **{TimestampTag.FromDateTimeOffset(eventStartTime, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(eventStartTime)})\n" +
          $"End: **{TimestampTag.FromDateTimeOffset(eventEndTime, TimestampTagStyles.Relative)}** " +
          $"({TimestampTag.FromDateTimeOffset(eventEndTime)})");
    }

    internal static string BuildMetadata(ulong channelId, ulong msgId)
    {
      return $"{channelId}{_SEPARATOR}{msgId}";
    }

    /// <summary>
    /// This requires that discord IDs are written down in the 'location' property of the event.
    /// </summary>
    /// <param name="e">The discord guild event.</param>
    /// <param name="index">The index of the id written in the Location property</param>
    /// <returns>Discord Id</returns>
    internal static ulong GetIdFromLocation(SocketGuildEvent e, int index, char separator = _SEPARATOR)
    {
      return ulong.Parse(e.Location.Split(separator)[index]);
    }
  }
}
