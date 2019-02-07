using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Yandex.Music.Bot.Commands
{
  public class TrackCommand : Command
  {
    public override async Task Perform(YandexApi yandexApi, ITelegramBotClient bot, Message message, string trackId)
    {
      var track = yandexApi.GetTrack(trackId);
      var streamTrack = yandexApi.ExtractStreamTrack(track);

      var artistName = track.Artists.FirstOrDefault()?.Name;
      var infoMessage = await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"Я пришлю вам трек {track.Title} в ближайшее время");

      streamTrack.Complated += (o, track1) =>
      {
        var inputStream = new InputOnlineFile(streamTrack, $"{artistName} - {track.Title}");

        bot.SendAudioAsync(
          message.Chat.Id,
          inputStream).GetAwaiter().GetResult();
        
        bot.DeleteMessageAsync(message.Chat.Id, infoMessage.MessageId);
      };
      
      Log.Information($"[SEND] {track.Title}");
    }
  }
}