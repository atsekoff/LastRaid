using Discord.Interactions;
using System;
using System.Threading.Tasks;
using static LastRaid.EpicsDataConst;

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
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Confirm, Context);

      await RespondAsync();
    }

    [ComponentInteraction(BUTTON_ID_CANCEL, true)]
    public async Task HandleCancelButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        _ = msg.DeleteAsync();

        if (msg.TryGetTodEvent(out var e))
          _ = e.DeleteAsync();
      }

      await RespondAsync();
    }

    [ComponentInteraction(BUTTON_ID_OURS, true)]
    public async Task HandleOursButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Ours, Context);

      await RespondAsync();
    }


    [ComponentInteraction(BUTTON_ID_ENEMIES, true)]
    public async Task HandleEnemiesButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.UpdateTodMsgStateAsync(Utils.TodState.Enemies, Context);
      }

      await RespondAsync();
    }

    [ComponentInteraction(BUTTON_ID_NO_DROP, true)]
    public async Task HandleNoDropButton()
    {
      if (Context.Interaction.TryGetMessage(out var msg))
      {
        await msg.UpdateTodMsgStateAsync(Utils.TodState.NoDrop, Context);
      }

      await RespondAsync();
    }
  }
}
