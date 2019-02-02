using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Account;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Utils;

namespace Telegram.Music
{
  public class LofiTelegramApi
  {
    public TelegramClient Client { get; set; }
    public FileSessionStore Store { get; set; }
    public Session Session { get; set; }
    
    public LofiTelegramApi()
    {
      Store = new FileSessionStore();
      Client = new TelegramClient(614523, "584d46ed77ea333d584fa0744feaf4dd", Store);
      
      Session = Store.Load("session");
    }

    public async Task Login(string phone, Func<string> methodCode)
    {
      await Client.ConnectAsync();
      
      var isPhoneRegistred = await Client.IsPhoneRegisteredAsync(phone);
      var isAuth = Client.IsUserAuthorized();

//      if (!isPhoneRegistred && !isAuth)
//      {
        var hash = await Client.SendCodeRequestAsync(phone);
        var code = methodCode();
        var user = await Client.MakeAuthAsync(phone, hash, code);
//      }
      
    }

    public async Task SendAudio(int userId, List<string> files)
    {
      files = Directory.GetFiles("data").ToList();
      
      for (var i = 0; i < files.Count; i++)
      {
        var fileName = files[i];
        
        var n = fileName.Replace("data", "").Replace("\\", "");
        Console.WriteLine($"[{i+1}] - {n} [{Client.IsConnected}]");
        if (!Client.IsConnected)
        {
          Console.WriteLine("123");
          await Client.ConnectAsync();
        }

        try
        {
          var fileResult = await Client.UploadFile(n, new StreamReader(fileName));

          await Client.SendUploadedDocument(
            new TLInputPeerUser() {UserId = userId},
            fileResult,
            n,
            "audio/mpeg",
            new TLVector<TLAbsDocumentAttribute>());
        }
        catch (Exception ex)
        {
          i--;
          Console.WriteLine(ex.ToString());
        }
      }
      
    }

    public async Task UploadFiles(int userId, List<string> files)
    {
      files = Directory.GetFiles("data").ToList();
      
      for (var i = 0; i < files.Count; i++)
      {
        try
        {
          var fileName = files[i];
          var n = fileName.Replace("data", "").Replace("\\", "");
          var fileResult = await Client.UploadFile(n, new StreamReader(fileName));

          await Client.SendUploadedDocument(
            new TLInputPeerUser() {UserId = userId},
            fileResult,
            n,
            "application/media",
            new TLVector<TLAbsDocumentAttribute>());
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
    }
  }
}