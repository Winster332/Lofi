using System;
using PeterKottas.DotNetCore.WindowsService;
using Serilog;
using Telegram.Bot;

namespace Yandex.Music.Bot
{
  class Program
  {
    private static TelegramBotClient Bot;

    static void Main(string[] args)
    {
      if (!Console.IsOutputRedirected)
      {
        Console.Title = YandexMusicBotService.Title;
      }

      ServiceRunner<YandexMusicBotService>.Run(config =>
      {
        config.SetName(YandexMusicBotService.Name);
        config.SetDisplayName(YandexMusicBotService.Title);
        config.SetDescription(YandexMusicBotService.Description);

        var name = config.GetDefaultName();

        config.Service(serviceConfig =>
        {
          serviceConfig.ServiceFactory((extraArguments, controller) => { return new YandexMusicBotService(); });

          serviceConfig.OnStart((service, extraParams) => { service.Start(); });

          serviceConfig.OnStop(service => { service.Stop(); });

          serviceConfig.OnError(e =>
          {
            Log.Error($"Service {name} errored with exception: {e.Message}\n{e.StackTrace}");
          });
        });
      });
    }
  }
}
