using Discord;

namespace LastRaid
{
  internal static class Utils
  {
    public static string GetUrl(this IGuildScheduledEvent e)
    {
      return $"https://discord.com/events/{e.Guild.Id}/{e.Id}";
    }
  }
}
