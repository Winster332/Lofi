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
    public override async Task Perform(string trackId)
    {
      var track = Session.YandexApi.GetTrack(trackId);
      var streamTrack = Session.YandexApi.ExtractStreamTrack(track);

      var artistName = track.Artists.FirstOrDefault()?.Name;
      var infoMessage = await Session.Bot.SendTextMessageAsync(
        Session.Message.Chat.Id,
        $"Я пришлю вам трек {track.Title} в ближайшее время");

      streamTrack.Complated += (o, track1) =>
      {
        var inputStream = new InputOnlineFile(streamTrack, $"{artistName} - {track.Title}");

        Session.Bot.SendAudioAsync(
          Session.Message.Chat.Id,
          inputStream).GetAwaiter().GetResult();
        
        Session.Bot.DeleteMessageAsync(Session.Message.Chat.Id, infoMessage.MessageId);
      };
      
      Log.Information($"[SEND] {track.Title}");
    }
  }
}