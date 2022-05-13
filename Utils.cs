using Discord;
using System;

namespace LastRaid
{
  internal static class Utils
  {
    internal static string GetUrl(this IGuildScheduledEvent e)
    {
      return $"https://discord.com/events/{e.Guild.Id}/{e.Id}";
    }

    internal static DateTimeOffset ConvertToLocalDateTimeOffset(this DateTime lastKnownTod, DateTime userTime)
    {
      var diff = DateTime.Now - userTime;
      return new DateTimeOffset(lastKnownTod) + diff;
    }

    internal static DateTime ConvertToLocalDateTime(this DateTime lastKnownTod, DateTime userTime)
    {
      var diff = DateTime.Now - userTime;
      return lastKnownTod + diff;
    }
  }
}
