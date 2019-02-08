using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Yandex.Music.Bot.Commands
{
  public class AlbumCommand : Command
  {
    public override async Task Perform(string albumId)
    {
      var album = Session.YandexApi.GetAlbum(albumId);
      var artistName = album.Artists.First().Name;
      var tracks = album.Volumes.First();

      var infoMessage = Session.Bot.SendTextMessageAsync(
        Session.Message.Chat.Id,
        $"В ближайшее время я пришлю вам {tracks.Count} треков из альбома {album.Title} этого артиста: {artistName}");

      var trackFailed = new List<YandexTrack>();

      tracks.ForEach(track =>
      {
        try
        {
          var streamTrack = Session.YandexApi.ExtractStreamTrack(track);

          streamTrack.Complated += (o, track1) =>
          {
            var inputStream = new InputOnlineFile(streamTrack, $"{artistName} - {track.Title}");

            Session.Bot.SendAudioAsync(
              Session.Message.Chat.Id,
              inputStream).GetAwaiter().GetResult();

            Log.Information($"[SUCCESS] {track.Title}");
          };

          Log.Information($"[SEND] {track.Title}");
          Thread.Sleep(5000);
        }
        catch (Exception ex)
        {
          trackFailed.Add(track);
          Log.Error($"Error send track from album\n{ex.Message}\nStackTrace: \n{ex.StackTrace}");
        }
      });
      
      Log.Information($"Failed tracks: {trackFailed.Count}");
      trackFailed.ForEach(track =>
      {
        var streamTrack = Session.YandexApi.ExtractStreamTrack(track);

        streamTrack.Complated += (o, track1) =>
        {
          var inputStream = new InputOnlineFile(streamTrack, track.Title);

          Session.Bot.SendAudioAsync(
            Session.Message.Chat.Id,
            inputStream).GetAwaiter().GetResult();

        };
        Log.Information($"[SEND] {track.Title}");
      });
    }
  }
}