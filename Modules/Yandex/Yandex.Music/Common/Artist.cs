namespace Yandex.Music
{
  public class Artist
  {
    public string Id { get;set; }
    public string Name { get; set; }
    public bool Various { get; set; }
    public bool Composer { get; set; }
    public Cover Cover { get; set; }
    public string[] Genres { get; set; }
  }

  public class Cover
  {
    public string Type { get; set; }
    public string Prefix { get; set; }
    public string Url { get; set; }
  }
}