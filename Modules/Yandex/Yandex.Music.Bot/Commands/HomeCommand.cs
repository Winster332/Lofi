using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Yandex.Music.Bot.Core;

namespace Yandex.Music.Bot.Controllers
{
  public class HomeCommand : Command
  {
    private TelegramBotClient _bot;
    
    public HomeCommand(IServiceProvider provider)
    {
      _bot = (TelegramBotClient) provider.GetService(typeof(TelegramBotClient));
      RouteName = "Авторизоваться";
    }

    public override async Task<CommandResult> Perform(Message message)
    {
      ReplyKeyboardMarkup ReplyKeyboard = new[]
      {
        new[]
        {
          "Авторизоваться",
          "Главное",
        },
        new[]
        {
          "Жанры",
          "Рекомендации",
        },
        new[]
        {
          "Поиск",
          "Настройки",
        },
      };

      await _bot.SendTextMessageAsync(
        message.Chat.Id,
        "Choose",
        replyMarkup: ReplyKeyboard);
      
      return new CommandResult();
    }
  }
}