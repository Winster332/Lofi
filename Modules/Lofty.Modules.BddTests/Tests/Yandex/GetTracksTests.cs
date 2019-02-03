﻿using System;
using System.Linq;
using FluentAssertions;
using Lofty.Modules.BddTests.Traits;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Lofty.Modules.BddTests.Tests.Yandex
{
  [Collection("Lofi Test Harness")]
  public class GetTracksTests : LofiTest
  {
    public GetTracksTests(LofiTestHarness fixture, ITestOutputHelper output) : base(fixture, output)
    {
      Api.Authorize("Winster332", "Stas32MP3tanki");
    }

    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetAlbum_Test()
    {
      var albumId = "3236027";
      var album = Api.GetAlbum(albumId);
      album.Should().NotBeNull();
      album.Id.Should().BeEquivalentTo(albumId);
      album.Volumes.First().Should().HaveCount(40);
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetPlayListDejaVu_Test()
    {
      var playlist = Api.GetPlaylistDejaVu();
      playlist.Should().NotBeNull();
      playlist.Tracks.Should().HaveCount(30);
      playlist.Title.Should().BeEquivalentTo("Дежавю");
      
      Log.Information("123");
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetPlayListOfDay_Test()
    {
      var playlist = Api.GetPlaylistOfDay();
      playlist.Should().NotBeNull();
      playlist.Tracks.Should().HaveCount(60);
      playlist.Title.Should().BeEquivalentTo("Плейлист дня");
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetUsers_Search_ReturnUsers()
    {
      var users = Api.SearchUsers("a", 0);
      users.Should().NotHaveCount(0);
      users.Should().HaveCount(1);
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetAlbums_Search_ReturnAlbums()
    {
      var tracks = Api.SearchAlbums("a", 0);
      tracks.Should().NotHaveCount(0);
      tracks.Should().HaveCount(10);
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetTracks_Search_ReturnTracks()
    {
      var tracks = Api.SearchTrack("a", 0);
      tracks.Should().NotHaveCount(0);
      tracks.Should().HaveCount(20);
    }
    
    [Fact, YandexTrait(TraitGroup.GetTracks)]
    public void GetPlaylists_Search_ReturnPlaylists()
    {
      var playlists = Api.SearchPlaylist("a", 0);
      playlists.Should().NotHaveCount(0);
      playlists.Should().HaveCount(10);
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