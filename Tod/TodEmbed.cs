using Discord;
using System;
using static L2Calendar.EpicsDataConst;

namespace L2Calendar.Tod
{
  internal class TodEmbed
  {
    internal static EmbedBuilder Create(BossNames bossName, DateTimeOffset tod, TimeSpan deathDuration, TimeSpan windowDuration, string thumbnailUrl, Color color, string link = "", string description = "")
    {
      return new EmbedBuilder()
        .WithColor(color)
        .WithDescription(description)
        .WithFooter($"Respawn: {deathDuration} + {windowDuration}")
        .WithThumbnailUrl(thumbnailUrl)
        .WithTitle(bossName.ToString())
        .WithUrl(link)
        .AddField(new EmbedFieldBuilder()
          .WithName("Time of death")
          .WithValue($"{TimestampTag.FromDateTimeOffset(tod, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(tod)})"))
        .AddField(new EmbedFieldBuilder()
          .WithName("Window start")
          .WithValue($"{TimestampTag.FromDateTimeOffset(tod + deathDuration, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(tod + deathDuration)})"))
        .AddField(new EmbedFieldBuilder()
          .WithName("Window end")
          .WithValue($"{TimestampTag.FromDateTimeOffset(tod + deathDuration + windowDuration, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(tod + deathDuration + windowDuration)})"));
    }
  }
}
