using FluentAssertions;
using Lofty.Modules.BddTests.Traits;
using Xunit;

namespace Lofty.Modules.BddTests.Tests.Yandex
{
  [Collection("Lofi Test Harness")]
  public class AuthorizeTest : LofiTest
  {
    public AuthorizeTest(LofiTestHarness fixture) : base(fixture)
    {
    }

    [Fact, YandexTrait(TraitGroup.Authorize)]
    public void Authorize_ValidData_GenerateTrue()
    {
      var isAuthorized = Api.Authorize("Winster332", "Stas32MP3tanki");
      
      isAuthorized.Should().BeTrue();
    }
    
    [Fact, YandexTrait(TraitGroup.Authorize)]
    public void Authorize_InvalidData_GenerateFalse()
    {
      var isAuthorized = Api.Authorize("Winster332-test", "Stas32MP3tanki");
      
      isAuthorized.Should().BeFalse();
    }
  }
}