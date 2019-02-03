using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Yandex.Music.Extensions;

namespace Yandex.Music
{
  public class User : ISearchable
  {
    public string Uid { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public List<string> Regions { get; set; }

    public static User FromJson(JToken jUser)
    {
      var user = new User
      {
        Uid = jUser.GetString("uid"),
        Login = jUser.GetString("login"),
        Name = jUser.GetString("name"),
        Regions = jUser.ContainField("regions") ? 
          jUser["regions"].ToObject<JArray>().Select(x => x.ToString()).ToList()
          : null
      };
      
      return user;
    }

    public static List<User> FromJsonArray(JToken jUsers)
    {
      return jUsers.Select(FromJson).ToList();
    }
  }
}