using Discord;
using System;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  internal class TodEmbedTools
  {
    internal static EmbedBuilder CreateTodEmbed(BossNames bossName, DateTimeOffset tod, DateTimeOffset windowStartTime, DateTimeOffset windowEndTime, string link = "", string description = "")
    {
      return new EmbedBuilder()
        .WithColor(Color.DarkerGrey)
        .WithDescription(description)
        .WithFooter($"Respawn: {DEATH_DURATIONS[(int)bossName]} + {WINDOW_DURATIONS[(int)bossName]}")
        .WithThumbnailUrl(EPIC_THUMBNAILS[(int)bossName])
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
