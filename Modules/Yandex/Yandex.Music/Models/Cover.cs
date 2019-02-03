﻿using Newtonsoft.Json.Linq;
using Yandex.Music.Extensions;

namespace Yandex.Music
{
  public class Cover
  {
    public string Type { get; set; }
    public string Prefix { get; set; }
    public string Url { get; set; }
    public bool? Custom { get; set; }
    public string Dir { get; set; }
    public string Version { get; set; }

    public static Cover FromJson(JToken jCover)
    {
      var cover = new Cover
      {
        Type = jCover.GetString("type"),
        Prefix = jCover.GetString("prefix"),
        Url = jCover.GetString("uri"),
        Custom = jCover.GetBool("custom"),
        Dir = jCover.GetString("dir"),
        Version = jCover.GetString("version")
      };
      return cover;
    }
  }
}