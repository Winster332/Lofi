using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yandex.Music.Bot.Common;

namespace Yandex.Music.Bot.Commands
{
  public abstract class Command
  {
    public string CommandName { get; set; }
    public ISession Session { get; set; }

    public abstract Task Perform(string id);
  }
}