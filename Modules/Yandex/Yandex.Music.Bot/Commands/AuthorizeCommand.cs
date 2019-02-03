using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yandex.Music.Bot.Core;

namespace Yandex.Music.Bot.Controllers
{
  public class AuthorizeCommand : Command
  {
    private TelegramBotClient _bot;
    
    public AuthorizeCommand(IServiceProvider provider)
    {
      _bot = (TelegramBotClient) provider.GetService(typeof(TelegramBotClient));
      RouteName = "Авторизоваться";
    }

    public override async Task<CommandResult> Perform(Message message)
    {
      await _bot.SendTextMessageAsync(
        message.Chat.Id,
        "В данный момент авторизация не поддерживается");
      
      return new CommandResult();
    }
  }
}