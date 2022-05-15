using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static LastRaid.EpicsDataConst;

namespace LastRaid
{
  internal static class Utils
  {
    internal static string GetUrl(this IGuildScheduledEvent e)
    {
      return $"https://discord.com/events/{e.Guild.Id}/{e.Id}";
    }

    internal static bool TryGetUrl(this IMessage msg, [NotNullWhen(true)]out string? url)
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

    internal static bool TryGetTodEvent(this IMessage msg, [NotNullWhen(true)] out IGuildScheduledEvent? e)
    {
      e = null;

      if(ulong.TryParse(msg.Embeds.First()?.Url.Split('/').Last(), out ulong eventId))
      {
        e = (msg.Channel as SocketTextChannel)?.Guild.GetEvent(eventId);
      }

      return e != null;
    }

    internal static bool TryGetTodEvent(this SocketInteractionContext context, BossNames bossName, [NotNullWhen(true)] out IGuildScheduledEvent? @event)
    {
      @event = context.Guild.Events.FirstOrDefault(e =>
        e.Name == bossName.ToString() &&
        e.Creator.Id == context.Client.CurrentUser.Id &&
        (e.Status == GuildScheduledEventStatus.Scheduled || e.Status == GuildScheduledEventStatus.Active));

      return @event != null;
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
