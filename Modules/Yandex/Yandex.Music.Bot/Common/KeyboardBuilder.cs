using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace Yandex.Music.Bot.Common
{
  public class KeyboardBuilder
  {
    public static ReplyKeyboardMarkup Create(string[][] buttons)
    {
      var keyboard = buttons.Select(x => x.Select(f => new KeyboardButton(f)).ToArray()).ToArray();
      var replyKeyboardMarkup = new ReplyKeyboardMarkup();
      replyKeyboardMarkup.Keyboard = keyboard;

      return replyKeyboardMarkup;
    }

    public static ReplyKeyboardMarkup Create(KeyboardButton[][] buttons)
    {
      var replyKeyboardMarkup = new ReplyKeyboardMarkup
      {
        Keyboard = buttons
      };

      return replyKeyboardMarkup;
    }
  }
}