using Newtonsoft.Json.Linq;
using Yandex.Music.Extensions;

namespace Yandex.Music
{
  public class Owner
  {
    public string Login { get; set; }
    public string Name { get; set; }
    public string Sex { get; set; }
    public string Uid { get; set; }
    public bool? Verified { get; set; }

    public static Owner FromJson(JToken jOwner)
    {
      var owner = new Owner
      {
        Login = jOwner.GetString("login"),
        Name = jOwner.GetString("name"),
        Sex = jOwner.GetString("sex"),
        Uid = jOwner.GetString("uid"),
        Verified = jOwner.GetBool("verified")
      };

      return owner;
    }
  }
}