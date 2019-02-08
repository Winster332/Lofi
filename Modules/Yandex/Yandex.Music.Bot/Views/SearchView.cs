using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Yandex.Music.Bot.Common;

namespace Yandex.Music.Bot.Views
{
  public class SearchView : View
  {
    public override void Perform(CallbackQuery callbackQuery)
    {
      var command = callbackQuery.Data.Split(".").First();
      var chatId = callbackQuery.Message.Chat.Id;

      try
      {
        Bot.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
        Log.Error(ex.ToString());
      }

      if (command == "home")
      {
        Bot.SendTextMessageAsync(chatId, "Что будете искать?",
            replyMarkup: KeyboardBuilder.CreateSearchKeyboardMarkup()).GetAwaiter()
          .GetResult();
      } 
      else if (command == "track")
      {
        Bot.SendTextMessageAsync(chatId, "Введите название трека").GetAwaiter()
          .GetResult();
        
        Bot.OnMessage += BotOnSearchTrack;
      }
      else if (command == "artist")
      {
        Bot.SendTextMessageAsync(chatId, "Введите название трека").GetAwaiter()
          .GetResult();
        
        Bot.OnMessage += BotOnSearchArtist;
      }
      else if (command == "back")
      {
        Bot.SendTextMessageAsync(chatId, "Главная",
            replyMarkup: KeyboardBuilder.CreateStartKeyboardMarkup()).GetAwaiter()
          .GetResult();
      }
    }
    
    private void BotOnSearchArtist(object sender, MessageEventArgs e)
    {
      Bot.OnMessage -= BotOnSearchArtist;

      Log.Information($"Search track: {e.Message.Text}");

      var message = e.Message;

      try
      {
        var artists = YandexApi.SearchArtist(message.Text);
        var infoMessage = Bot.SendTextMessageAsync(
          message.Chat.Id,
          $"Я пришлю вам 5 артистов по запросу \"{message.Text}\", в ближайшее время");

        for (var i = 0; i < artists.Count; i++)
        {
          Bot.SendTextMessageAsync(message.Chat.Id, $"Исполнитель: {artists[i].Name}", 
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
              InlineKeyboardButton.WithCallbackData("Подробнее...", "no") 
            }));

          Log.Information($"[SUCCESS] {artists[i].Name}");

        }

        Bot.SendTextMessageAsync(message.Chat.Id, "Еще что-то поищим?",
            replyMarkup: KeyboardBuilder.CreateSearchKeyboardMarkup()).GetAwaiter()
          .GetResult();
      }
      catch (Exception ex)
      {
        Log.Error($"Error message: {ex.Message}\nStackTrace: {ex.StackTrace}");
      }
    }

    private void BotOnSearchTrack(object sender, MessageEventArgs e)
    {
      Bot.OnMessage -= BotOnSearchTrack;

      Log.Information($"Search track: {e.Message.Text}");

      var message = e.Message;

      try
      {
        var tracks = YandexApi.SearchTrack(message.Text);
        var infoMessage = Bot.SendTextMessageAsync(
          message.Chat.Id,
          $"Я пришлю вам 5 треков по запросу \"{message.Text}\", в ближайшее время");

        for (var i = 0; i < 5; i++)
        {
          if (i > tracks.Count)
            break;

          var track = YandexApi.GetTrack(tracks[i].Id);
          var streamTrack = YandexApi.ExtractStreamTrack(track);
          var artistName = track.Artists.FirstOrDefault()?.Name;

          Log.Information($"[SEND] {track.Title}");

          Task.WaitAll(streamTrack.Task);

          var inputStream = new InputOnlineFile(streamTrack, $"{artistName} - {track.Title}");

          Bot.SendAudioAsync(
            message.Chat.Id,
            inputStream, disableNotification: true).GetAwaiter().GetResult();

          Log.Information($"[SUCCESS] {track.Title}");

        }

        Bot.SendTextMessageAsync(message.Chat.Id, "Еще что-то поищим?",
            replyMarkup: KeyboardBuilder.CreateSearchKeyboardMarkup()).GetAwaiter()
          .GetResult();
      }
      catch (Exception ex)
      {
        Log.Error($"Error message: {ex.Message}\nStackTrace: {ex.StackTrace}");
      }
    }
  }
}