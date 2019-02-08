using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Yandex.Music.Bot.Commands;

namespace Yandex.Music.Bot.Common
{
  public class Router
  {
    private Dictionary<string, Command> _commands { get; set; }
    private YandexApi _yandexApi;
    private ITelegramBotClient _bot;
    public string DefaultCommandName { get; set; }

    public Router()
    {
      _commands = new Dictionary<string, Command>();
    }
    
    public Router UseCommand(string name, Command command)
    {
      _commands.Add(name, command);
      return this;
    }

    public Router Create(YandexApi yandexApi, ITelegramBotClient bot)
    {
      _yandexApi = yandexApi;
      _bot = bot;

      return this;
    }

    public void Push(Uri uri, Message message)
    {
      var isPushDefault = true;
      
      foreach (var keyValuePair in _commands)
      {
        if (uri == null)
          continue;
        if (uri.Segments.Length - 2 <= 0)
          continue;
        
        var command = keyValuePair.Value;
        var segmentName = uri.Segments[uri.Segments.Length - 2];

        if (uri != null && segmentName == $"{command.CommandName}/")
        {
          var trackIdFromUrl = uri.Segments.LastOrDefault();

          if (trackIdFromUrl != null && long.TryParse(trackIdFromUrl, out var lastId))
          {
            command.Perform(_yandexApi, _bot, message, lastId.ToString()).GetAwaiter().GetResult();
            isPushDefault = false;
          }
          
          break;
        }
      }

      if (isPushDefault)
      {
        var command = _commands[DefaultCommandName.ToLower()];
        
        command.Perform(_yandexApi, _bot, message, null).GetAwaiter().GetResult();
      }
    }

    public async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
    {
      new Thread(() =>
      {
        var message = messageEventArgs.Message;

        if (message.Type != MessageType.Text)
        {
          return;
        }

        var command = message.Text.Split(" ").First();

        Log.Information($"GET[{message.From.Username}] > {message.Text}");

        if (command == "/start")
        {
          _bot.SendTextMessageAsync(
            message.Chat.Id,
            $"Привет {message.From.FirstName} {message.From.LastName}. Я бот который работает с Яндекс.Музыкой. Просто пришли мне ссылку на песню, и я скину тебе её").GetAwaiter().GetResult();
        }
        else
        {
          if (Uri.TryCreate(message.Text, UriKind.Absolute, out var uriResult)
              && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
          {
            if (uriResult.DnsSafeHost == "music.yandex.ru")
            {
              Push(uriResult, message);
            }
            else
            {
              var text =
                "Вы дали мне адрес, который не указывает на яндекс трек, альбом, плейлист, артиста или пользователя";
              _bot.SendTextMessageAsync(
                message.Chat.Id,
                text).GetAwaiter().GetResult();

              Log.Information($"[SEND] {text}");
            }
          }
          else
          {
            Log.Information($"[SEARCH] search by {message.Text}");

            Push(uriResult, message);
          }
        }
      }).Start();
    }
  }
}