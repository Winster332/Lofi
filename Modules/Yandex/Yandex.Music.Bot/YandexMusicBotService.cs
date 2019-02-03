using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Telegram.Bot.Types.ReplyMarkups;
using Yandex.Music.Bot.Controllers;
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
      
      
      var result = Container.GetService<CommandRouter>().Push(message, Container);

      if (result == null)
      {
        Container.GetService<HomeCommand>().Perform(message).GetAwaiter().GetResult();
      }

      var bot = Container.GetService<TelegramBotClient>();
      
      Log.Information($"GET[{message.From.Username}] > {message.Text}");

      var command = message.Text.Split(' ').First();
      
            switch (command)
            {
                case "Авторизоваться": 
                  break;
                // send custom keyboard
                case "/photo":
                    await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string file = @"Files/tux.png";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await bot.SendPhotoAsync(
                            message.Chat.Id,
                            fileStream,
                            "Nice Picture");
                    }
                    break;

                // request location or contact
                case "/request":
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestLocation("Location"),
                        KeyboardButton.WithRequestContact("Contact"),
                    });

                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: RequestReplyKeyboard);
                    break;

//                default:
//                    const string usage = @"
//Usage:
///menu   - меню
///keyboard - send custom keyboard
///request  - request location or contact";

//                    await bot.SendTextMessageAsync(
//                        message.Chat.Id,
//                        usage,
//                        replyMarkup: new ReplyKeyboardRemove());
//                    break;
            }
    }
    

    public void Stop()
    {
      var bot = Container.GetService<TelegramBotClient>();
      
      bot.StopReceiving();
    }
  }
}