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
      var yandexApi = new LofiYandexMusicApi("Winster332", "Stas32MP3tanki");
      yandexApi.Authorize();
      var tracks = yandexApi.GetListFavorites();
//      tracks.ForEach(track => yandexApi.DownloadTrack(track));
        yandexApi.GetTrackStream(tracks.First(), (x) => { });

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