﻿using System.Threading.Tasks;
using Telegram.Bot.Types;
using Yandex.Music.Bot.Core;

namespace Yandex.Music.Bot.Controllers
{
  public class SettingsCommand : Command
  {
    public SettingsCommand()
    {
      RouteName = "Настройки";
    }

    public override async Task<CommandResult> Perform(Message message)
    {
      return new CommandResult();
    }
  }
}