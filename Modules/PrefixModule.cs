using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace L2Calendar.Modules
{
  public class PrefixModule : ModuleBase<SocketCommandContext>
  {
    [Command("ping")]
    public async Task HandlePingCommand()
    {
      await Context.Message.ReplyAsync("PING COMMAND!");
    }
  }
}
