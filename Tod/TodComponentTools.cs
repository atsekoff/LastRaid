using Discord;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod
{
  internal class TodComponentTools
  {
    internal static ComponentBuilder CreateInitialTodComponent()
    {
      return new ComponentBuilder()
        .WithButton(CreateConfirmButton())
        .WithButton(CreateCancelButton());
    }

    internal static ComponentBuilder CreateDropComponent()
    {
      return new ComponentBuilder()
        .WithButton(CreateOursButton())
        .WithButton(CreateEnemiesButton())
        .WithButton(CreateNoDropButton());
    }

    private static ButtonBuilder CreateConfirmButton()
    {
      return new ButtonBuilder(BUTTON_LABEL_CONFIRM, BUTTON_ID_CONFIRM, ButtonStyle.Success);
    }

    private static ButtonBuilder CreateOursButton()
    {
      return new ButtonBuilder(BUTTON_LABEL_OURS, BUTTON_ID_OURS, ButtonStyle.Success);
    }

    private static ButtonBuilder CreateEnemiesButton()
    {
      return new ButtonBuilder(BUTTON_LABEL_ENEMIES, BUTTON_ID_ENEMIES, ButtonStyle.Danger);
    }

    private static ButtonBuilder CreateNoDropButton()
    {
      return new ButtonBuilder(BUTTON_LABEL_NO_DROP, BUTTON_ID_NO_DROP, ButtonStyle.Primary);
    }

    private static ButtonBuilder CreateCancelButton()
    {
      return new ButtonBuilder(BUTTON_LABEL_CANCEL, BUTTON_ID_CANCEL, ButtonStyle.Secondary);
    }
  }
}
