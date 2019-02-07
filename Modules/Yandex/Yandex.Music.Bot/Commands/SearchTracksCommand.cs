using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Yandex.Music.Bot.Commands
{
  public class SearchTracksCommand : Command
  {
    public override Task Perform(YandexApi api, ITelegramBotClient bot, Message message, string id)
    {
//      var tracks = api.SearchTrack(message.Text);
      
//      tracks.To
      return Task.CompletedTask;
    }
  }
}