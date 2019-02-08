using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
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
      services.AddTransient<ISession, Session>();
      services.UseRouter(defaultCommand: "SearchTracks");

      Container = services.BuildServiceProvider();

      var bot = Container.GetService<TelegramBotClient>();
      Container.GetService<YandexApi>().Authorize(Configuration.GetSection("Yandex").GetValue<string>("Username"),
        Configuration.GetSection("Yandex").GetValue<string>("Password"));
      
      var me = bot.GetMeAsync().Result;
      Console.Title = me.Username;

      bot.OnMessage += Container
        .GetService<Router>()
        .Create(Container.GetService<YandexApi>()
         // .UseWebProxy(new WebProxy("45.32.24.242", 3128))
          , Container.GetService<TelegramBotClient>())
        .BotOnMessageReceived;
      bot.StartReceiving(Array.Empty<UpdateType>());
      
      Log.Information($"Start listening for @{me.Username}");
      Log.Information($"Bot {me.Username} started");
    }


    public void Stop()
    {
      var bot = Container.GetService<TelegramBotClient>();
      
      bot.StopReceiving();
    }
  }
}