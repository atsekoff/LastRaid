using Discord;
using Discord.Interactions;
using Discord.Rest;
using System;
using System.Threading.Tasks;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod.Modules
{
  [Group(COMMAND_TOD_NAME, COMMAND_TOD_DESCRIPTION)]
  public class TodCommandsModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand(COMMAND_RELATIVE_NAME, COMMAND_RELATIVE_DESCRIPTION)]
    public async Task HandleRelativeTodCommand(BossNames bossName,
      [Summary(PARAM_RELATIVE_TIME_NAME, PARAM_RELATIVE_TIME_DESCRIPTION)]
      DateTime relativeTime,
      [Summary(PARAM_HEADS_UP_NAME, PARAM_HEADS_UP_DESCRIPTION)]
      int headsUpMinutes = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = DateTimeOffset.Now.AddHours(-relativeTime.Hour).AddMinutes(-relativeTime.Minute);
      var headsup = TimeSpan.FromMinutes(headsUpMinutes);

      await Context.HandleTod(bossName, tod, headsup);
    }

    [SlashCommand(COMMAND_EXACT_NAME, COMMAND_EXACT_DESCRIPTION)]
    public async Task HandleExactTodCommand(
      BossNames bossName,
      [Summary(PARAM_LAST_TOD_NAME, PARAM_LAST_TOD_DESCRIPTION)]
      DateTime lastKnownTod,
      [Summary(PARAM_USER_TIME_NAME, PARAM_USER_TIME_DESCRIPTION)]
      DateTime userTime,
      [Summary(PARAM_HEADS_UP_NAME, PARAM_HEADS_UP_DESCRIPTION)]
      int headsUpMinutes = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = new DateTimeOffset(lastKnownTod.ConvertToLocalDateTime(userTime));
      var headsup = TimeSpan.FromMinutes(headsUpMinutes);

      await Context.HandleTod(bossName, tod, headsup);
    }
  }
}
