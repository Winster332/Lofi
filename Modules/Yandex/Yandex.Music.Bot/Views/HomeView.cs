using Serilog;
using Telegram.Bot.Types;

namespace Yandex.Music.Bot.Views
{
  public class HomeView : View
  {
    public override void Perform(CallbackQuery callbackQuery)
    {
      Log.Information("123");
    }
  }
}