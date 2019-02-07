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
      var telegramApi = new LofiTelegramApi();
      telegramApi.Login("+79500040940", () =>
      {
        var code = Console.ReadLine();
        return code;
      }).GetAwaiter().GetResult();
    }
  }
}