using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Yandex.Music.Extensions;

namespace Yandex.Music
{
  public class Track : ISearchable
  {
    public string Id { get; set; }
    public List<Album> Albums { get; set; }
    public string RealId { get; set; }
    public string Title { get; set; }
    public Major Major { get; set; }
    public bool? Available { get; set; }
    public bool? AvailableForPremiumUsers { get; set; }
    public int? DurationMS { get; set; }
    public string StorageDir { get; set; }
    public int? FileSize { get; set; }
    public List<Artist> Artists { get; set; }
    public string OgImage { get; set; }

    public static Track FromJson(JToken jTrack)
    {
      try
      {
        var track = new Track
        {
          Id = jTrack.GetString("id"),
          RealId = jTrack.GetString("realId"),
          Title = jTrack.GetString("title"),
          Major = Major.FromJson(jTrack.Contains("major")),
          Available = jTrack.GetBool("available"),
          AvailableForPremiumUsers = jTrack.GetBool("availableForPremiumUsers"),
          Albums = jTrack.ContainField("albums") ? Album.FromJsonArray(jTrack["albums"].ToObject<JArray>()) : null,
          DurationMS = jTrack["durationMs"].ToObject<int>(),
          StorageDir = jTrack.GetString("storageDir"),
          FileSize = jTrack.GetInt("fileSize"),
          Artists = Artist.FromJsonArray(jTrack["artists"].ToObject<JArray>()),
          OgImage = jTrack.GetString("ogImage")
        };
        return track;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }

      return null;
    }

    public static List<Track> FromJsonArray(JArray jTracks)
    {
      var list = new List<Track>();

      for (var i = 0; i < jTracks.Count; i++)
      {
        var jTrack = jTracks[i];
        var track = FromJson(jTrack);


        list.Add(track);
      }

      return list;
    }
  }
}