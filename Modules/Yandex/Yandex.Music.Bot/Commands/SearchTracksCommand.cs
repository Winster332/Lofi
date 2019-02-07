using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Yandex.Music.Bot.Commands
{
  public class SearchTracksCommand : Command
  {
    public override async Task Perform(YandexApi yandexApi, ITelegramBotClient bot, Message message, string id)
    {
      var tracks = yandexApi.SearchTrack(message.Text);
        var infoMessage = await bot.SendTextMessageAsync(
          message.Chat.Id,
          $"Я пришлю вам 5 треков по запросу \"{message.Text}\", в ближайшее время");

      for (var i = 0; i < 5; i++)
      {
        if (i > tracks.Count)
          break;

        var track = yandexApi.GetTrack(tracks[i].Id);
        var streamTrack = yandexApi.ExtractStreamTrack(track);
        var artistName = track.Artists.FirstOrDefault()?.Name;

        Log.Information($"[SEND] {track.Title}");

        Task.WaitAll(streamTrack.Task);

        var inputStream = new InputOnlineFile(streamTrack, $"{artistName} - {track.Title}");

        bot.SendAudioAsync(
          message.Chat.Id,
          inputStream, disableNotification: true).GetAwaiter().GetResult();

        Log.Information($"[SUCCESS] {track.Title}");
      }
    }
  }
}