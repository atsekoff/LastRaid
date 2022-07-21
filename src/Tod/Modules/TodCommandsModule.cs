using Discord;
using Discord.Interactions;
using Discord.Rest;
using System;
using System.Threading.Tasks;
using static LastRaid.EpicsDataConst;

namespace LastRaid.Tod.Modules
{
  [Group(COMMAND_NAME_TOD, COMMAND_DESCRIPTION_TOD)]
  public class TodCommandsModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand(COMMAND_NAME_RELATIVE, COMMAND_DESCRIPTION_RELATIVE)]
    public async Task HandleRelativeTodCommand(BossNames bossName,
      [Summary(PARAM_NAME_RELATIVE_TIME, PARAM_DESCRIPTION_RELATIVE_TIME)]
      DateTime relativeTime,
      [Summary(PARAM_NAME_HEADS_UP, PARAM_DESCRIPTION_HEADS_UP)]
      int headsUpMinutes = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = DateTimeOffset.Now.AddHours(-relativeTime.Hour).AddMinutes(-relativeTime.Minute);
      var headsup = TimeSpan.FromMinutes(headsUpMinutes);

      await Context.HandleTod(bossName, tod, headsup);
    }

    [SlashCommand(COMMAND_NAME_EXACT, COMMAND_DESCRIPTION_EXACT)]
    public async Task HandleExactTodCommand(
      BossNames bossName,
      [Summary(PARAM_NAME_LAST_TOD, PARAM_DESCRIPTION_LAST_TOD)]
      DateTime lastKnownTod,
      [Summary(PARAM_NAME_USER_TIME, PARAM_DESCRIPTION_USER_TIME)]
      DateTime userTime,
      [Summary(PARAM_NAME_HEADS_UP, PARAM_DESCRIPTION_HEADS_UP)]
      int headsUpMinutes = DEFAULT_HEADSUP_MINUTES)
    {
      var tod = new DateTimeOffset(lastKnownTod.ConvertToLocalDateTime(userTime));
      var headsup = TimeSpan.FromMinutes(headsUpMinutes);

      await Context.HandleTod(bossName, tod, headsup);
    }
  }
}
