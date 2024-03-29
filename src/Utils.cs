﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LastRaid.Tod;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static LastRaid.Consts;

namespace LastRaid
{
  internal static class Utils
  {
    internal static string GetUrl(this IGuildScheduledEvent e)
    {
      return $"https://discord.com/events/{e.Guild.Id}/{e.Id}";
    }

    internal static bool TryGetUrl(this IMessage msg, [NotNullWhen(true)] out string? url)
    {
      url = null;
      ulong channelId = msg.Channel.Id;
      ulong? guildId = (msg.Channel as IGuildChannel)?.GuildId;

      if (guildId == null) return false;

      url = $"https://discord.com/channels/{guildId}/{channelId}/{msg.Id}";
      return true;
    }

    internal static bool TryGetMessage(this SocketInteraction interaction, [NotNullWhen(true)] out SocketUserMessage? msg)
    {
      msg = null;
      if (interaction is not SocketMessageComponent msgComponent)
        return false;

      msg = msgComponent.Message;
      return true;
    }

    internal static bool TryGetEvent(this IMessage msg, [NotNullWhen(true)] out IGuildScheduledEvent? e)
    {
      e = null;

      if (ulong.TryParse(msg.Embeds.First()?.Url.Split('/').Last(), out ulong eventId))
      {
        e = (msg.Channel as SocketTextChannel)?.Guild.GetEvent(eventId);
      }

      return e != null;
    }

    internal static bool TryGetEvent(this SocketInteractionContext context, BossNames bossName, [NotNullWhen(true)] out SocketGuildEvent? @event)
    {
      @event = context.Guild.Events.FirstOrDefault(e =>
        e.Name == bossName.ToString() &&
        e.Creator.Id == context.Client.CurrentUser.Id &&
        (e.Status == GuildScheduledEventStatus.Scheduled || e.Status == GuildScheduledEventStatus.Active));

      return @event != null;
    }

    internal static bool TryGetButtonWithLabel(this IUserMessage msg, string buttonLabel, [NotNullWhen(true)] out ButtonComponent? button)
    {
      button = null;
      foreach (ActionRowComponent comp in msg.Components)
      {
        foreach (ButtonComponent but in comp.Components)
        {
          if (but.Label == buttonLabel)
          {
            button = but;
            return true;
          }
        }
      }

      return button != null;
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

    internal static async Task RemoveAllComponentsAsync(this IUserMessage msg)
    {
      await msg.ModifyAsync(mp => mp.Components = new ComponentBuilder().Build());
    }
  }
}
