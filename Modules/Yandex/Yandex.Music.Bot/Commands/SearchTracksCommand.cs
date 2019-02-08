using System;
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
    public override async Task Perform(string id)
    {
      try
      {
        var tracks = Session.YandexApi.SearchTrack(Session.Message.Text);
        var infoMessage = await Session.Bot.SendTextMessageAsync(
          Session.Message.Chat.Id,
          $"Я пришлю вам 5 треков по запросу \"{Session.Message.Text}\", в ближайшее время");

        for (var i = 0; i < 5; i++)
        {
          if (i > tracks.Count)
            break;

          var track = Session.YandexApi.GetTrack(tracks[i].Id);
          var streamTrack = Session.YandexApi.ExtractStreamTrack(track);
          var artistName = track.Artists.FirstOrDefault()?.Name;

          Log.Information($"[SEND] {track.Title}");

          Task.WaitAll(streamTrack.Task);

          var inputStream = new InputOnlineFile(streamTrack, $"{artistName} - {track.Title}");

          Session.Bot.SendAudioAsync(
            Session.Message.Chat.Id,
            inputStream, disableNotification: true).GetAwaiter().GetResult();

          Log.Information($"[SUCCESS] {track.Title}");
        }
      }
      catch (Exception ex)
      {
        Log.Error($"Error message: {ex.Message}\nStackTrace: {ex.StackTrace}");
      }
    }
  }
}