Yandex.Music API (Unofficial) for .Net Core
====

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
