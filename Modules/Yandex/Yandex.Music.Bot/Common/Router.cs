using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
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
        var command = keyValuePair.Value;

        if (uri != null && uri.Segments.Contains($"{command.CommandName}/"))
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
  }
}