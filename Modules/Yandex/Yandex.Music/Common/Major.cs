using System.Linq;
using Newtonsoft.Json.Linq;
using Yandex.Music.Extensions;

namespace Yandex.Music
{
  public class Major
  {
    public string Id { get; set; }
    public string Name { get; set; }

    public static Major FromJson(JToken jMajor)
    {
      if (!jMajor.Contains("major"))
      {
        return null;
      }

      var majot = new Major
      {
        Id = jMajor.GetString("id"),
        Name = jMajor.GetString("name")
      };
      
      return majot;
    }
  }
}