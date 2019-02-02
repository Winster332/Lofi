using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Yandex.Music
{
  public class Track : ISearchable
  {
    public string Id { get; set; }
    public string RealId { get; set; }
    public string Title { get; set; }
    public Major Major { get; set; }
    public bool Available { get; set; }
    public bool AvailableForPremiumUsers { get; set; }
    public int DurationMS { get; set; }
    public string StorageDir { get; set; }
    public int FileSize { get; set; }
    public List<Artist> Artists { get; set; }
    public string OgImage { get; set; }

    public static Track FromJson(JToken jTrack)
    {
      try
      {
        var track = new Track
        {
          Id = jTrack["id"].ToObject<string>(),
          RealId = jTrack.Contains("realId") == true ? jTrack["realId"].ToObject<string>() : "",
          Title = jTrack["title"].ToObject<string>(),
          Major = jTrack.Contains("major") == true ? new Major
          {
            Id = jTrack["major"]["id"].ToObject<string>(),
            Name = jTrack["major"]["name"].ToObject<string>()
          } : null,
          Available = jTrack["available"].ToObject<bool>(),
          AvailableForPremiumUsers = jTrack["availableForPremiumUsers"].ToObject<bool>(),

          DurationMS = jTrack["durationMs"].ToObject<int>(),
          StorageDir = jTrack["storageDir"].ToObject<string>(),
          FileSize = jTrack.Contains("fileSize") ? jTrack["fileSize"].ToObject<int>() : -1,
          Artists = Artist.FromJsonArray(jTrack["artists"].ToObject<JArray>()),
          OgImage = jTrack.Contains("ogImage") ? jTrack["ogImage"].ToObject<string>() : ""
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