using Telegram.Bot;
using Telegram.Bot.Types;

namespace Yandex.Music.Bot.Views
{
  public abstract class View
  {
    public YandexApi YandexApi { get; set; }
    public ITelegramBotClient Bot { get; set; }
    public string QueryData { get; set; }

    public abstract void Perform(CallbackQuery callbackQuery);
  }
}