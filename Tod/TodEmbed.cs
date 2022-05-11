using Discord;
using System;
using static L2Calendar.EpicsData;

namespace L2Calendar.Tod
{
  internal class TodEmbed
  {
    internal static EmbedBuilder Create(BossNames bossName, DateTimeOffset tod, TimeSpan spawnTimer, TimeSpan spawnWindow, string thumbnailUrl, Color color, string link = "", string description = "")
    {
      return new EmbedBuilder()
        .WithColor(color)
        .WithDescription(description)
        .WithFooter($"Respawn: {spawnTimer} + {spawnWindow}")
        .WithThumbnailUrl(thumbnailUrl)
        .WithTitle(bossName.ToString())
        .WithUrl(link)
        .AddField(new EmbedFieldBuilder()
          .WithName("Time of death")
          .WithValue($"{TimestampTag.FromDateTimeOffset(tod, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(tod)})"))
        .AddField(new EmbedFieldBuilder()
          .WithName("Window start")
          .WithValue($"{TimestampTag.FromDateTimeOffset(tod + spawnTimer, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(tod + spawnTimer)})"))
        .AddField(new EmbedFieldBuilder()
          .WithName("Window end")
          .WithValue($"{TimestampTag.FromDateTimeOffset(tod + spawnTimer + spawnWindow, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(tod + spawnTimer + spawnWindow)})"));
    }
  }
}
