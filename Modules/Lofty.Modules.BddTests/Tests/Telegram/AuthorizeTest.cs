using System;
using Telegram.Music;
using Xunit;

namespace Lofty.Modules.BddTests.Tests.Telegram
{
  public class AuthorizeTest
  {
    [Fact]
    public void AuthorizeWithPhoneNumberAndCode()
    {
      var api = new LofiTelegramApi();
      api.Login("+79500040940", () =>
      {
        var code = Console.ReadLine();
        return code;
      }).GetAwaiter().GetResult();
    }
  }
}