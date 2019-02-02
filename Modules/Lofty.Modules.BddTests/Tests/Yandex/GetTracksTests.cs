using System;
using FluentAssertions;
using Lofty.Modules.BddTests.Traits;
using Xunit;

namespace Lofty.Modules.BddTests.Tests.Yandex
{
  [Collection("Lofi Test Harness")]
  public class GetTracksTests : LofiTest
  {
    public GetTracksTests(LofiTestHarness fixture) : base(fixture)
    {
      Api.Authorize("Winster332", "Stas32MP3tanki");
    }

    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetTracks_Search_ReturnTracks()
    {
      var tracks = Api.SearchTrack("a", 0);
      tracks.Should().NotHaveCount(0);
      tracks.Should().HaveCount(20);
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetArtists_Search_ReturnAllArtists()
    {
      var tracks = Api.SearchArtist("a", 0);
      tracks.Should().NotHaveCount(0);
      tracks.Should().HaveCount(10);
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetTracks_GetFavoritesTracks_ReturnTracks()
    {
      var list = Api.GetListFavorites();
      list.Count.Should().BeGreaterOrEqualTo(21);

      list.ForEach(track => track.Should().NotBeNull());
    }
  }
}