using Discord;
using System;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  internal class TodEmbed
  {
    internal static EmbedBuilder Create(BossNames bossName, DateTimeOffset tod, DateTimeOffset windowStartTime, DateTimeOffset windowEndTime, string thumbnailUrl, Color color, string link = "", string description = "")
    {
      return new EmbedBuilder()
        .WithColor(color)
        .WithDescription(description)
        .WithFooter($"Respawn: {DEATH_DURATIONS[(int)bossName]} + {WINDOW_DURATIONS[(int)bossName]}")
        .WithThumbnailUrl(thumbnailUrl)
        .WithTitle(bossName.ToString())
        .WithUrl(link)
        .AddField(new EmbedFieldBuilder()
          .WithName("Time of death")
          .WithValue($"{TimestampTag.FromDateTimeOffset(tod, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(tod)})"))
        .AddField(new EmbedFieldBuilder()
          .WithName("Window start")
          .WithValue($"{TimestampTag.FromDateTimeOffset(windowStartTime, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(windowStartTime)})"))
        .AddField(new EmbedFieldBuilder()
          .WithName("Window end")
          .WithValue($"{TimestampTag.FromDateTimeOffset(windowEndTime, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(windowEndTime)})"));
    }
  }
}
