using System.Collections.Generic;

namespace Yandex.Music
{
  public class User
  {
    public string Uid { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public List<string> Regions { get; set; }

  }
}