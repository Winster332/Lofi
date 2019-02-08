using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Yandex.Music.Bot.Common
{
  public class Session : ISession
  {
    public Guid Id { get; set; }
    public Message Message { get; set; }
    public YandexApi YandexApi { get; set; }
    public ITelegramBotClient Bot { get; set; }

    public Session(TelegramBotClient bot, YandexApi yandexApi)
    {
      Bot = bot;
      YandexApi = yandexApi;
    }
    
    public ISession Open(Message message)
    {
      Id = Guid.NewGuid();
      Message = message;

      return this;
    }
  }
}