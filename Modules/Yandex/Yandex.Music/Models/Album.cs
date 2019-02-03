using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Yandex.Music.Extensions;

namespace Yandex.Music
{
  public class Album : ISearchable
  {
    public List<Artist> Artists { get; set; }
    public bool? Available { get; set; }
    public bool? AvailableForPremiumUsers { get; set; }
    public string CoverUri { get; set; }
    public string Genre { get; set; }
    public string Id { get; set; }
    public string OriginalReleaseYear { get; set; }
    public List<string> Regions { get; set; }
    public string StorageDir { get; set; }
    public string Title { get; set; }
    public int? TrackCount { get; set; }
    public string Year { get; set; }

    public static Album FromJson(JToken jAlbum)
    {
      var album = new Album
      {
        Artists = Artist.FromJsonArray(jAlbum["artists"].ToObject<JArray>()),
        Available = jAlbum.GetBool("available"),
        AvailableForPremiumUsers = jAlbum.GetBool("availableForPremiumUsers"),
        CoverUri = jAlbum.GetString("coverUri"),
        Genre = jAlbum.GetString("genre"),
        Id = jAlbum.GetString("id"),
        OriginalReleaseYear = jAlbum.GetString("originalReleaseYear"),
        Regions = jAlbum.Contains("regions") 
          ? jAlbum["regions"].ToObject<JArray>().Select(x => (string)x).ToList()
          : null,
        StorageDir = jAlbum.GetString("storageDir"),
        Title = jAlbum.GetString("title"),
        TrackCount = jAlbum.GetInt("trackCount"),
        Year = jAlbum.GetString("year")
      };

      return album;
    }
    
    public static List<Album> FromJsonArray(JArray jAlbums)
    {
      return jAlbums.Select(FromJson).ToList();
    }
  }
}