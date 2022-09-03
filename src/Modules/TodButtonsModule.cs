using Discord;
using Discord.Interactions;
using System;
using System.Linq;
using System.Threading.Tasks;
using static LastRaid.Consts;

namespace LastRaid.Tod.Modules
{
  internal class TodButtonsModule : InteractionModuleBase<SocketInteractionContext>
  {
    [ComponentInteraction($"{BUTTON_ID_OVERRIDE}:*,*,*,*", true)]
    public async Task HandleOverrideButton(string eventString, string bossString, string todUnixSec, string headsupTicks)
    {
      ulong eventId = ulong.Parse(eventString);
      BossNames bossName = Enum.Parse<BossNames>(bossString);
      DateTimeOffset tod = DateTimeOffset.FromUnixTimeSeconds(long.Parse(todUnixSec));
      TimeSpan headsupTime = TimeSpan.FromTicks(long.Parse(headsupTicks));

      var e = await Context.Guild.GetEventAsync(eventId);
      if (e == null) return;

      await e!.DeleteAsync();
      await Context.HandleTod(bossName, tod, headsupTime);
    }

    [ComponentInteraction(BUTTON_ID_CONFIRM, true)]
    public async Task HandleConfirmButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
        await msg.RemoveAllComponentsAsync();
    }

    [ComponentInteraction(BUTTON_ID_CANCEL, true)]
    public async Task HandleCancelButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.DeleteAsync();

        if (msg.TryGetEvent(out var e))
          await e.DeleteAsync();
      }
    }

    [ComponentInteraction(BUTTON_ID_OURS, true)]
    public async Task HandleOursButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.ModifyAsync(mp =>
        {
          mp.Components = TodComponentTools.CreateConfirmTodComponent().Build();
          mp.Embed = msg.Embeds.First().ToEmbedBuilder()
            .WithDescription($"**Drop for us** (by {Context.User.Mention})")
            .WithColor(Color.Green).Build();
        });
      }
    }


    [ComponentInteraction(BUTTON_ID_ENEMIES, true)]
    public async Task HandleEnemiesButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.ModifyAsync(mp =>
        {
          mp.Components = TodComponentTools.CreateConfirmTodComponent().Build();
          mp.Embed = msg.Embeds.First().ToEmbedBuilder()
            .WithDescription($"**Drop for enemies** (by {Context.User.Mention})")
            .WithColor(Color.Red).Build();
        });
      }
    }

    [ComponentInteraction(BUTTON_ID_NO_DROP, true)]
    public async Task HandleNoDropButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.ModifyAsync(mp =>
        {
          mp.Components = TodComponentTools.CreateConfirmTodComponent().Build();
          mp.Embed = msg.Embeds.First().ToEmbedBuilder()
            .WithDescription($"**No drop** (by {Context.User.Mention})")
            .WithColor(Color.Blue).Build();
        });
      }
    }
  }
}
