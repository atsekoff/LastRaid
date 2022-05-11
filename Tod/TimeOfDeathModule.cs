using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using static L2Calendar.EpicsData;

namespace L2Calendar.Tod
{
  public class TimeOfDeathModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand("tod", "Creates an event reminder for next boss spawn.")]
    public async Task HandleBossTodCommand(
      BossNames bossName,
      [Summary("Last-known-time-of-death", "In format dd.mm.yyyy hh:mm. Or just hh:mm.")]
      DateTime lastKnownTod,
      [Summary("Heads-up-time", "Get notified ahead of the window start, in minutes. Default is 15.")]
      int headsUpTime = 15)
    {
      TimeSpan spawnTimer = TimeSpan.FromHours(timers[(int)bossName]);
      TimeSpan spawnWindow = TimeSpan.FromHours(windows[(int)bossName]);
      var lastKnownDeath = new DateTimeOffset(lastKnownTod);
      TimeSpan hoursSinceLastKnownTod = DateTimeOffset.UtcNow.Subtract(lastKnownDeath).Duration();

      if (spawnTimer >= hoursSinceLastKnownTod)
      {
        await HandleExactTod(bossName, lastKnownTod, spawnTimer, spawnWindow, TimeSpan.FromMinutes(headsUpTime));
        return;
      }

      if (hoursSinceLastKnownTod < spawnTimer + spawnWindow)
      {
        await HandleCurrentlyInWindow(bossName, lastKnownDeath, spawnTimer, spawnWindow, TimeSpan.FromMinutes(headsUpTime));
        return;
      }

      await HandleUnknownTod(bossName, hoursSinceLastKnownTod, spawnTimer, spawnWindow);
    }

    private async Task HandleUnknownTod(BossNames bossName, TimeSpan hoursSinceLastKnownTod, TimeSpan spawnTimer, TimeSpan spawnWindow)
    {
      var possibleSpawnsMsg = $"**{Math.Floor(hoursSinceLastKnownTod / spawnTimer)}** possible spawns missed";
      var embed = TodEmbed.Create(bossName, DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1), spawnTimer + spawnWindow, epicThumbs[(int)bossName], Color.Red, "https://discord.com/events/972265218862383124/973704427817033768", possibleSpawnsMsg);
      await RespondAsync(embed: embed.Build());
    }

    private async Task HandleCurrentlyInWindow(BossNames bossName, DateTimeOffset tod, TimeSpan spawnTimer, TimeSpan spawnWindow, TimeSpan headsupTime)
    {
      var e = await TodEvent.Create(Context, bossName, tod, spawnTimer, spawnWindow, headsupTime);
      var embedUrl = $"https://discord.com/events/{Context.Guild.Id}/{e.Id}";
      var embed = TodEmbed.Create(bossName, tod, spawnTimer, spawnWindow, epicThumbs[(int)bossName], Color.Orange, embedUrl, "Already in window!");
      await RespondAsync(embed: embed.Build());      
    }

    private async Task HandleExactTod(BossNames bossName, DateTimeOffset tod, TimeSpan spawnTimer, TimeSpan spawnWindow, TimeSpan headsupTime)
    {
      var e = await TodEvent.Create(Context, bossName, tod, spawnTimer, spawnWindow, headsupTime);
      var embedUrl = $"https://discord.com/events/{Context.Guild.Id}/{e.Id}";
      var embed = TodEmbed.Create(bossName, tod, spawnTimer, spawnWindow, epicThumbs[(int)bossName], Color.Green, embedUrl);

      //var components = new ComponentBuilder()
      //  .WithButton("PrimaryButtonNNNNNNNNNNNNN\nNNNNNNNNNNNNNNNNN\nBIGGGGGG", "primarybuttonid", ButtonStyle.Primary, row: 0)
      //  .WithButton("SecondaryButton", "secondarybuttonid", ButtonStyle.Secondary, row: 1)
      //  .WithButton("SuccessButton", "successbuttonid", ButtonStyle.Success, row: 1)
      //  .WithButton("DangerButton", "dangerbuttonid", ButtonStyle.Danger, row: 2)
      //  .WithButton("LinkButton", style: ButtonStyle.Link, url: "https://enhanced.l2reborn.org/", row: 2);

      await RespondAsync(embed: embed.Build());
    }
  }
}
