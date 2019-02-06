using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MihaZupan;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Yandex.Music.Bot.Extensions;
using Yandex.Music.Extensions;

namespace Yandex.Music.Bot
{
  public class YandexMusicBotService : MicroService, IMicroService
  {
    public static string Name => Assembly.GetEntryAssembly().GetName().Name;
    public static string Title => Assembly.GetEntryAssembly().GetTitle();
    public static string Description => Assembly.GetEntryAssembly().GetDescription();
    public static string Version => Assembly.GetEntryAssembly().GetVersion();
    
    public IConfigurationRoot Configuration { get; set; }
    public IServiceProvider Container { get; set; }

    public void Start()
    {
      Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false, true)
        .Build();

      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .CreateLogger();

      
      Log.Information($"Service {Name} v.{Version} initialized");

      Log.Information($"Name: {Name}");
      Log.Information($"Title: {Title}");
      Log.Information($"Description: {Description}");
      Log.Information($"Version: {Version}");
      Log.Information("Starting...");

      var services = new ServiceCollection();

      services.UseTelegramBot(Configuration);
      services.UseYandexMusicApi();
      services.UseRouter();

      Container = services.BuildServiceProvider();

      var bot = Container.GetService<TelegramBotClient>();
      
      var me = bot.GetMeAsync().Result;
      Console.Title = me.Username;

      bot.OnMessage += BotOnMessageReceived;
      bot.StartReceiving(Array.Empty<UpdateType>());
      
      bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
      {
        var message = ev.CallbackQuery.Message;
        if (ev.CallbackQuery.Data == "callback1")
        {
          // сюда то что тебе нужно сделать при нажатии на первую кнопку 
          Console.WriteLine("123");

          await bot.SendTextMessageAsync(
            message.Chat.Id,
            "Спасибо что выбрали one");
        }
        else
        if (ev.CallbackQuery.Data == "callback2")
        {
          // сюда то что нужно сделать при нажатии на вторую кнопку
          await bot.SendTextMessageAsync(
            message.Chat.Id,
            "Спасибо что выбрали two");
        }
      };
      
      Log.Information($"Start listening for @{me.Username}");
      Log.Information($"Bot {me.Username} started");
      
    }

    private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
    {
      var message = messageEventArgs.Message;

      if (message.Type != MessageType.Text)
      {
        return;
      }

      var command = message.Text.Split(" ").First();

      Log.Information($"GET[{message.From.Username}] > {message.Text}");

      var bot = Container.GetService<TelegramBotClient>();

      if (command == "/start")
      {
        await bot.SendTextMessageAsync(
          message.Chat.Id,
          $"Привет {message.From.FirstName} {message.From.LastName}. Я бот который работает с Яндекс.Музыкой. Просто пришли мне ссылку на песню, и я скину тебе её");
      }
      else
      {
        if (Uri.TryCreate(message.Text, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
          if (uriResult.DnsSafeHost == "music.yandex.ru" && uriResult.Segments.Contains("track/"))
          {
            var trackIdFromUrl = uriResult.Segments.LastOrDefault();

            if (trackIdFromUrl != null && long.TryParse(trackIdFromUrl, out var trackId))
            {
              var yandexApi = Container.GetService<YandexApi>();
              var track = yandexApi.GetTrack(trackId.ToString());

              var streamTrack = yandexApi.ExtractStreamTrack(track);
              await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"Я пришлю вам трек {track.Title} в ближайшее время");

              streamTrack.Complated += (o, track1) =>
              {
                var inputStream = new InputOnlineFile(streamTrack);

                bot.SendAudioAsync(
                  message.Chat.Id,
                  inputStream, track.Title).GetAwaiter().GetResult();
                Log.Information($"[SEND] {track.Title} {uriResult}");
              };
            }
          }
        }
      }
    }


    public void Stop()
    {
      var bot = Container.GetService<TelegramBotClient>();
      
      bot.StopReceiving();
    }
  }
}