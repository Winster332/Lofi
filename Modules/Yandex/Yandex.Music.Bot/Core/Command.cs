using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Yandex.Music.Bot.Core
{
  public abstract class Command
  {
    public string CommandName { get; set; }
    public string RouteName { get; set; }
    
    public Command SetCommandName(string name)
    {
      CommandName = name;
      return this;
    }

    public abstract Task<CommandResult> Perform(Message message);
  }
}