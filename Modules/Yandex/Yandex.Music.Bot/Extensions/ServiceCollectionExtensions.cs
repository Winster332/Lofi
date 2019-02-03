using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MihaZupan;
using Telegram.Bot;
using Yandex.Music.Bot.Core;

namespace Yandex.Music.Bot.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static void UseTelegramBot(this ServiceCollection services, IConfiguration config)
    {
      var proxySettings = new ProxySettings(config.GetSection("Proxy"));
      var proxy = new HttpToSocks5Proxy(proxySettings.Socks5Hostname, proxySettings.Sock5Port, proxySettings.Username, proxySettings.Password);
      
      services.AddSingleton(x =>
        new TelegramBotClient(config.GetSection("TelegramBot").GetValue<string>("Token"), proxy));
    }

    public static void UseYandexMusicApi(this ServiceCollection services)
    {
      services.AddSingleton<YandexApi, LofiYandexMusicApi>();
    }

    public static void UseRouter(this ServiceCollection services)
    {
      var router = new CommandRouter();
      var assembly = Assembly.GetEntryAssembly();
      var list = assembly.GetExportedTypes().Where(t => t.BaseType == typeof(Command)).ToList();

      list.ForEach(c =>
      {
        var name = c.Name.Replace("Command", "");
        
        services.AddTransient(c);
        router._commands.Add(name, c);
      });
      
      services.AddSingleton(router);
    }
  }
}
