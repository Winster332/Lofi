using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Yandex.Music.Bot
{
  class Program
  {
    private static TelegramBotClient Bot;

    static void Main(string[] args)
    {
      try
      {

        Console.WriteLine("starting...");
        var proxy = new HttpToSocks5Proxy("d2a5e5.reconnect.rocks", 1080, "3559738", "11a1fcc2");

//            proxy.ResolveHostnamesLocally = true;

        Bot = new TelegramBotClient("791758747:AAG5ivLkOomhhlTGQ-Pp5uGigMDXCPKx6d8", proxy);
        var me = Bot.GetMeAsync().Result;
        Console.Title = me.Username;

        Bot.OnMessage += BotOnMessageReceived;
        Console.WriteLine("started");
        Bot.StartReceiving(Array.Empty<UpdateType>());
        Console.WriteLine($"Start listening for @{me.Username}");
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }

      Console.ReadKey();
      Bot.StopReceiving();
    }

    private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
    {
      var message = messageEventArgs.Message;
      
      if (message == null || message.Type != MessageType.Text) return;

      var commands = message.Text.Split(' ').First();
      
            switch (commands)
            {
                // send inline keyboard
                case "/inline":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                    await Task.Delay(500); // simulate longer running task

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("1.1"),
                            InlineKeyboardButton.WithCallbackData("1.2"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("2.1"),
                            InlineKeyboardButton.WithCallbackData("2.2"),
                        }
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: inlineKeyboard);
                    break;

                // send custom keyboard
                case "/keyboard":
                    ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "1.1", "1.2" },
                        new[] { "2.1", "2.2" },
                    };

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: ReplyKeyboard);
                    break;

                // send a photo
                case "/photo":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string file = @"Files/tux.png";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await Bot.SendPhotoAsync(
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

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: RequestReplyKeyboard);
                    break;

                default:
                    const string usage = @"
Usage:
/inline   - send inline keyboard
/keyboard - send custom keyboard
/photo    - send a photo
/request  - request location or contact";

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
      
      await Console.Out.WriteLineAsync(message.Text);
    }
  }
}