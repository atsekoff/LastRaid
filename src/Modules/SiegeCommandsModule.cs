using Discord;
using Discord.Interactions;
using Discord.Rest;
using System;
using System.Threading.Tasks;
using static LastRaid.Consts;

namespace LastRaid.Tod.Modules
{
  public class SiegeCommandsModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand(COMMAND_SIEGE_NAME, COMMAND_SIEGE_DESCRIPTION)]
    public async Task HandleSiegeCommand(
      Castle castleName,
      [Summary(PARAM_SIEGE_START_NAME, PARAM_SIEGE_START_DESCRIPTION)]
      DateTime startTime,
      [Summary(PARAM_USER_TIME_NAME, PARAM_USER_TIME_DESCRIPTION)]
      DateTime userTime,
      [Summary(PARAM_HEADS_UP_NAME, PARAM_HEADS_UP_DESCRIPTION)]
      int headsUpMinutes = DEFAULT_HEADSUP_MINUTES,
      [Summary(PARAM_EXTRA_INFO_NAME, PARAM_EXTRA_INFO_DESCRIPTION)]
      string extraInfo = "")
    {
      var time = new DateTimeOffset(startTime.ConvertToLocalDateTime(userTime));
      var headsup = TimeSpan.FromMinutes(headsUpMinutes);

      await CreateSiegeReminder(castleName, time, time + TimeSpan.FromHours(SIEGE_DURATION), headsup, extraInfo);
    }

    private async Task CreateSiegeReminder(Castle castleName, DateTimeOffset startTime, DateTimeOffset endTime, TimeSpan headsupTime, string desc = "")
    {
      try
      {
        IGuildScheduledEvent? e = await Context.CreateTimeEvent(castleName.ToString(), startTime, endTime, headsupTime);

        try
        {
          EmbedBuilder embed = EmbedTools.CreateSiegeEmbed(castleName, startTime, e.GetUrl(), desc);
          await Context.Interaction.RespondAsync(embed: embed.Build());
          RestInteractionMessage msg = await Context.Interaction.GetOriginalResponseAsync();
          await e.ModifyAsync(ep => ep.Location = TimeEventTools.BuildMetadata(Context.Channel.Id, msg.Id));
        }
        catch (Exception ex)
        {
          await Context.Interaction.RespondAsync($"Failed to create a reminder: {ex.Message}", ephemeral: true);
          await e.DeleteAsync();
        }
      }
      catch (Exception ex)
      {
        await Context.Interaction.RespondAsync($"Failed to create event: {ex.Message}", ephemeral: true);
      }
    }
  }
}
