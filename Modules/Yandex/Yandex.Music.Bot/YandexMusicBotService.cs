using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Yandex.Music.Bot.Common;
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
          if (uriResult.DnsSafeHost == "music.yandex.ru")
          {
            var yandexApi = Container.GetService<YandexApi>();

            var router = Container.GetService<Router>();
            router.Create(yandexApi, bot).Push(uriResult, message);
          }
          else
          {
            var text = "Вы дали мне адрес, который не указывает на конкретный трек";
            bot.SendTextMessageAsync(
              message.Chat.Id,
              text).GetAwaiter().GetResult();

            Log.Information($"[SEND] {text}");
          }
        }
        else
        {
          var text = "Это не относится к адресу трека на Yandex.Music";
          
          bot.SendTextMessageAsync(
            message.Chat.Id,
            text).GetAwaiter().GetResult();
          
          Log.Information($"[SEND] {text}");
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