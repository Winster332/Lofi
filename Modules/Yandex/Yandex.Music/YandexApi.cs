using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Yandex.Music
{
  public interface YandexApi
  {
    /// <summary>
    /// Authorize user to yandex
    /// </summary>
    /// <param name="username">User name</param>
    /// <param name="password">User password</param>
    /// <returns></returns>
    bool Authorize(string username, string password);
    
    /// <summary>
    /// Return list track favorites
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns></returns>
    List<Track> GetListFavorites(string userId = null);
    
    /// <summary>
    /// Save track to file
    /// </summary>
    /// <param name="track">Track instance</param>
    /// <param name="filder">Folder for save file</param>
    /// <returns></returns>
    bool ExtractTrackToFile(Track track, string filder = "data");
    
    /// <summary>
    /// Return track stram
    /// </summary>
    /// <param name="track">Track instance</param>
    /// <returns></returns>
    StreamTrack ExtractStreamTrack(Track track);
    
    /// <summary>
    /// Return bytes from track
    /// </summary>
    /// <param name="track">Track instance</param>
    /// <returns></returns>
    byte[] ExtractDataTrack(Track track);

    /// <summary>
    /// Search by tracks
    /// </summary>
    /// <param name="trackName">Track name</param>
    /// <param name="pageNumber">Page number</param>
    /// <returns></returns>
    List<Track> SearchTrack(string trackName, int pageNumber = 0);

    /// <summary>
    /// Search by artists
    /// </summary>
    /// <param name="artistName">Artist name</param>
    /// <param name="pageNumber">Page number</param>
    /// <returns></returns>
    List<Artist> SearchArtist(string artistName, int pageNumber = 0);

    /// <summary>
    /// Search by albums
    /// </summary>
    /// <param name="albumName">Album name</param>
    /// <param name="pageNumber">Page number</param>
    /// <returns></returns>
    List<Album> SearchAlbums(string albumName, int pageNumber = 0);

    /// <summary>
    /// Search by playlists
    /// </summary>
    /// <param name="playlistName">Playlist name</param>
    /// <param name="pageNumber">Page number</param>
    /// <returns></returns>
    List<Playlist> SearchPlaylist(string playlistName, int pageNumber = 0);

    /// <summary>
    /// Search by users
    /// </summary>
    /// <param name="userName">User name</param>
    /// <param name="pageNumber">Page number</param>
    /// <returns></returns>
    List<User> SearchUsers(string userName, int pageNumber = 0);

    /// <summary>
    /// Return best playlist of day
    /// </summary>
    /// <returns></returns>
    Playlist GetPlaylistOfDay();

    /// <summary>
    /// Return play list deja vu
    /// </summary>
    /// <returns></returns>
    Playlist GetPlaylistDejaVu();

    /// <summary>
    /// Return album from albumId
    /// </summary>
    /// <param name="albumId">Id album</param>
    /// <returns></returns>
    Album GetAlbum(string albumId);
  }
}