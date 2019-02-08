using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Yandex.Music.Bot.Common
{
  public interface ISession
  {
    Guid Id { get; set; }
    Message Message { get; set; }
    YandexApi YandexApi { get; set; }
    ITelegramBotClient Bot { get; set; }

    ISession Open(Message message);
  }
}