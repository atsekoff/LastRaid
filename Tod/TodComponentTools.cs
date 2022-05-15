using Discord;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  internal class TodComponentTools
  {
    internal static ComponentBuilder CreateInitialTodComponent()
    {
      return new ComponentBuilder()
        .WithButton(CreateOursButton())
        .WithButton(CreateEnemiesButton())
        .WithButton(CreateNoDropButton())
        .WithButton(CreateCancelButton());
    }

    internal static ComponentBuilder CreateWindowStartedComponent()
    {
      return new ComponentBuilder()
        .WithButton(CreateSpawnedButton());
    }

    internal static ComponentBuilder CreateSpawnedComponent()
    {
      return new ComponentBuilder()
        .WithButton(CreateDeadButton());
    }

    private static ButtonBuilder CreateOursButton()
    {
      return new ButtonBuilder(OURS_BUTTON_LABEL, OURS_BUTTON_ID, ButtonStyle.Success);
    }

    private static ButtonBuilder CreateEnemiesButton()
    {
      return new ButtonBuilder(ENEMIES_BUTTON_LABEL, ENEMIES_BUTTON_ID, ButtonStyle.Danger);
    }

    private static ButtonBuilder CreateNoDropButton()
    {
      return new ButtonBuilder(NODROP_BUTTON_LABEL, NODROP_BUTTON_ID, ButtonStyle.Primary);
    }

    private static ButtonBuilder CreateCancelButton()
    {
      return new ButtonBuilder(CANCEL_BUTTON_LABEL, CANCEL_BUTTON_ID, ButtonStyle.Secondary);
    }

    private static ButtonBuilder CreateDeadButton()
    {
      return new ButtonBuilder(DEAD_BUTTON_LABEL, DEAD_BUTTON_ID, ButtonStyle.Primary);
    }

    private static ButtonBuilder CreateSpawnedButton()
    {
      return new ButtonBuilder(SPAWNED_BUTTON_LABEL, SPAWNED_BUTTON_ID, ButtonStyle.Success);
    }
  }
}
