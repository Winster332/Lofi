using System;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yandex.Music.Bot.Common;
using Yandex.Music.Bot.Views;

namespace Yandex.Music.Bot.Extensions
{
  public static class TelegramBotClientExtensions
  {
    public static void AddRouting(this TelegramBotClient bot, IServiceProvider container, IWebProxy webProxy = null)
    {
      var yandexApi = container.GetService<YandexApi>();
      
      if (webProxy != null)
      {
        yandexApi.UseWebProxy(webProxy);
      }

      var router = container.GetService<Router>()
        .Create(yandexApi, container.GetService<TelegramBotClient>());
      
      bot.OnMessage += router.BotOnMessageReceived;
    }

    public static User TestConnection(this TelegramBotClient bot)
    {
      try
      {
        var me = bot.GetMeAsync().Result;
        Console.Title = me.Username;
        
        return me;
      }
      catch (Exception ex)
      {
        Log.Error($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
      }

      return null;
    }

    public static TelegramBotClient InitialTelegramBotClient(this IServiceProvider container, IConfigurationRoot Configuration)
    {
      var bot = container.GetService<TelegramBotClient>();
      container.GetService<YandexApi>().Authorize(Configuration.GetSection("Yandex").GetValue<string>("Username"),
        Configuration.GetSection("Yandex").GetValue<string>("Password"));

      return bot;
    }
    
    public static void AddViews(this TelegramBotClient bot, IServiceProvider container)
    {
      var yandexApi = container.GetService<YandexApi>();
      
      bot.OnCallbackQuery += (sender, args) =>
      {
        var callbackQuery = args.CallbackQuery;
        var commands = args.CallbackQuery.Data.Split(".");
        var commandName = commands.LastOrDefault();

        if (commandName != null)
        {
          var assembly = Assembly.GetEntryAssembly();
          var listViews = assembly.GetExportedTypes().Where(t => t.BaseType == typeof(View)).ToList();
          
          for (var i = 0; i < listViews.Count; i++)
          {
            var viewType = listViews[i];
            var viewName = viewType.Name.Replace("View", "").ToLower();

            if (commandName == viewName)
            {
              var viewInstance = (View) Activator.CreateInstance(viewType);
              viewInstance.Bot = bot;
              viewInstance.YandexApi = yandexApi;
              
              viewInstance.Perform(callbackQuery);
              
              break;
            }
          }
        }
      };
    }
  }
}