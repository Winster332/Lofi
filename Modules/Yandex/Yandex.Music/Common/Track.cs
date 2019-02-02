using System.Collections.Generic;

namespace Yandex.Music
{
  public class Track
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
  }
}