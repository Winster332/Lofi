using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Telegram.Bot;
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
    
    public static InlineKeyboardMarkup CreateSearchKeyboardMarkup()
    {
      var replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Треки", "track.search"),
          InlineKeyboardButton.WithCallbackData("Исполнители", "artist.search"),
          InlineKeyboardButton.WithCallbackData("Альбомы", "album.search")
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Плейлисты", "playlist.search"),
          InlineKeyboardButton.WithCallbackData("Пользователи", "user.search"),
          InlineKeyboardButton.WithCallbackData("Вернуться домой", "back.search")
        }
      });

      return replyKeyboardMarkup;
    }
    
    public static InlineKeyboardMarkup CreateStartKeyboardMarkup()
    {
      var replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Поиск", "home.search"),
          InlineKeyboardButton.WithCallbackData("Новые релизы", "home.newReleases")
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Жанры", "home.genres"),
          InlineKeyboardButton.WithCallbackData("Чарт", "home.chart")
        }
      });

      return replyKeyboardMarkup;
    }
  }
}