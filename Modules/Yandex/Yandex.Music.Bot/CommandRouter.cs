using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Yandex.Music.Bot.Core;

namespace Yandex.Music.Bot
{
  public class CommandRouter
  {
    public Dictionary<string, Type> _commands;
    
    public CommandRouter()
    {
      _commands = new Dictionary<string, Type>();
    }
    
    public CommandResult Push(Message message, IServiceProvider provider)
    {
      if (message == null || message.Type != MessageType.Text) return null;
      
      CommandResult result = null;
      var command = message.Text.Split(' ').First().Replace("/", "").ToLower();

      foreach (var keyValuePair in _commands)
      {
        var commandType = keyValuePair.Value;
        var commandInstance = (Command) provider.GetService(commandType);

        if (commandInstance.RouteName.ToLower() == command)
        {
          result = commandInstance.Perform(message).GetAwaiter().GetResult();

          result.Command = commandInstance;
        }
      }

      return result;
    }
  }
}