﻿using Discord;
using System;
using static LastRaid.Consts;

namespace LastRaid.Tod
{
  internal class EmbedTools
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

    internal static EmbedBuilder CreateSiegeEmbed(Castle castleName, DateTimeOffset startTime, string link = "", string description = "")
    {
      return new EmbedBuilder()
        .WithColor(Color.Purple)
        .WithDescription(description)
        .WithTitle(castleName.ToString())
        .WithUrl(link)
        .AddField(new EmbedFieldBuilder()
          .WithName("Siege start")
          .WithValue($"{TimestampTag.FromDateTimeOffset(startTime, TimestampTagStyles.Relative)} ({TimestampTag.FromDateTimeOffset(startTime)})"));
    }
  }
}
