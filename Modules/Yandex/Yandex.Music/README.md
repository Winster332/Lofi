Yandex.Music API (Unofficial) for .Net Core
====

[![Build Status](https://travis-ci.com/Winster332/Lofi.svg?token=9RFGGw1id2424svMxqyZ&branch=master)](https://travis-ci.com/Winster332/Lofi)

This is wrapper for the [Yandex.Music](http://music.yandex.ru/) API

Usage
-------

```C#
 var yandexApi = new LofiYandexMusicApi();
 
 yandexApi.Authorize("login", "password");
 // place code here
})
```

This library provides following functions:

#### Users

- Authorize
- SearchUsers

#### Music

- GetListFavorites
- ExtractTrackToFile
- ExtractStreamTrack
- ExtractDataTrack
- SearchTrack

#### Playlist

- GetPlaylistOfDay
- GetPlaylistDejaVu
- SearchPlaylist
- SearchArtist
- SearchAlbums
- GetAlbum
