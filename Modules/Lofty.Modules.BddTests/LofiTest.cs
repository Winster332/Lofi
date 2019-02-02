using Xunit;
using Yandex.Music;

namespace Lofty.Modules.BddTests
{
  [CollectionDefinition("Lofi Test Harness")]
  public class LofiTestCollection : ICollectionFixture<LofiTestHarness>
  {
  }

  public class LofiTest 
  {
    public LofiYandexMusicApi Api { get; set; }
    public LofiTestHarness Fixture { get; set; }
    
    public LofiTest(LofiTestHarness fixture)
    {
      Fixture = fixture;
      
      Api = new LofiYandexMusicApi();
    }
  }
}
