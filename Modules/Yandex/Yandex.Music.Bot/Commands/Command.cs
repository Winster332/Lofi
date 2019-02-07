using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Yandex.Music.Bot.Commands
{
  public abstract class Command
  {
    public string CommandName { get; set; }

    public abstract Task Perform(YandexApi api, ITelegramBotClient bot, Message message, string id);
  }
}