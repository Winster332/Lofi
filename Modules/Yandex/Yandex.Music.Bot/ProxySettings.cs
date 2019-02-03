using Microsoft.Extensions.Configuration;

namespace Yandex.Music.Bot
{
  public class ProxySettings
  {
    public string Socks5Hostname { get; set; }
    public int Sock5Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public ProxySettings(IConfiguration config)
    {
      Socks5Hostname = config.GetValue<string>("Socks5Hostname");
      Sock5Port = config.GetValue<int>("Socks5Port");
      Username = config.GetValue<string>("Username");
      Password = config.GetValue<string>("Password");
    }
  }
}