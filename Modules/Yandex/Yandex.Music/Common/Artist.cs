using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Yandex.Music
{
  public class Artist : ISearchable
  {
    public string Id { get;set; }
    public string Name { get; set; }
    public bool Various { get; set; }
    public bool Composer { get; set; }
    public Cover Cover { get; set; }
    public string[] Genres { get; set; }

    public static Artist FromJson(JToken jArtist)
    {
      var artist = new Artist
      {
        Id = jArtist["id"].ToObject<string>(),
        Name = jArtist["name"].ToObject<string>(),
        Various = jArtist["various"].ToObject<bool>(),
        Composer = jArtist.Contains("composer") ? jArtist["composer"].ToObject<bool>() : false,
        Cover = jArtist.Contains("cover") ? new Cover
        {
          Type = jArtist["cover"]["type"].ToObject<string>(),
          Prefix = jArtist["cover"]["prefix"].ToObject<string>(),
          Url = jArtist["cover"]["uri"].ToObject<string>()
        } : null,
        Genres = new string[] { }
      };

      return artist;
    }

    public static List<Artist> FromJsonArray(JArray jArtists)
    {
      return jArtists.Select(FromJson).ToList();
    }
  }

  public class Cover
  {
    public string Type { get; set; }
    public string Prefix { get; set; }
    public string Url { get; set; }
  }
}