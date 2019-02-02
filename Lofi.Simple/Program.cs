using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Telegram.Music;
using Yandex.Music;

namespace Lofi.Simple
{
  class Program
  {
    static void Main(string[] args)
    {
      var yandexApi = new LofiYandexMusicApi();
      yandexApi.Authorize("Winster332", "Stas32MP3tanki");
      var tracks = yandexApi.GetListFavorites();
      var currentTrack = tracks.First();
//      tracks.ForEach(track => yandexApi.DownloadTrack(track));
      
//      var memoryStream = yandexApi.GetTrackStream(currentTrack);
//      memoryStream.Complated += (sender, track) => 
//      {
//          Console.WriteLine("Complated");
//      };

//      System.Threading.Thread.Sleep(5000);
//      memoryStream.SaveToFile(currentTrack.Title);

      var trackBytes = yandexApi.ExtractDataTrack(currentTrack);

      using (var stream = new FileStream(currentTrack.Title, FileMode.Create))
      {
        stream.Write(trackBytes, 0, trackBytes.Length);
      }

      Console.WriteLine("End");
      Console.ReadKey();

//      var telegramApi = new LofiTelegramApi();
//      telegramApi.Login("+79500040940", () =>
//      {
//        var code = Console.ReadLine();
//        return code;
//      }).GetAwaiter().GetResult();
//     telegramApi.UploadFiles(telegramApi.Session.TLUser.Id, tracks.Select(x => $"{x.Title}.mp3").ToList());
//      telegramApi.SendAudio(telegramApi.Session.TLUser.Id, new List<string>())
//        .GetAwaiter().GetResult();

    }
  }
}