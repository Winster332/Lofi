using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yandex.Music
{
  public class LofiYandexMusicApi : YandexApi
  {
    private string _login;
    private string _password;
    private CookieContainer _cookies;
    
    public LofiYandexMusicApi()
    {
      NetworkChange.NetworkAvailabilityChanged += (sender, args) =>
      {
        Console.WriteLine($"Networking: {args.IsAvailable}");
      };
    }

    public bool Authorize(string login, string password)
    {
      _login = login;
      _password = password;
      
      var result = false;
      var _passportUri = new Uri("https://pda-passport.yandex.ru/passport?mode=auth");
      
      var request = GetRequest(_passportUri,
        new KeyValuePair<string, string>("login", _login),
        new KeyValuePair<string, string>("passwd", _password),
        new KeyValuePair<string, string>("twoweeks", "yes"),
        new KeyValuePair<string, string>("retpath", ""));

      try
      {
        using (var response = (HttpWebResponse) request.GetResponse())
        {
          _cookies.Add(response.Cookies);
          result = true;
          
          if (response.ResponseUri == _passportUri)
          {
            result = false;
          }
        }
      }
      catch (Exception ex)
      {
        result = false;
      }

      return result;
    }

    public List<Track> GetListFavorites(string login = null)
    {
      if (login == null)
        login = _login;
      
      var request = GetRequest(new Uri($"https://music.yandex.ru/handlers/library.jsx?owner={login}&filter=tracks&likeFilter=favorite&sort=&dir=&lang=ru&external-domain=music.yandex.ru&overembed=false&ncrnd=0.7506943983987266"));
      var tracks = new List<Track>();
      
      using (var response = (HttpWebResponse) request.GetResponse())
      {
        var data = GetDataFromResponse(response);
        var jTracks = (JArray) data["tracks"];

        tracks = Track.FromJsonArray(jTracks);

        _cookies.Add(response.Cookies);
      }

      return tracks;
    }

    public bool ExtractTrackToFile(Track track, string folder)
    {
      try
      {
        var trackDownloadUrl = GetURLDownloadTrack(track);
        var isNetworing = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

        using (var client = new WebClient())
        {
          client.DownloadProgressChanged += (sender, args) =>
          {
            Console.WriteLine(
              $"received: {args.BytesReceived} from: {args.TotalBytesToReceive} progress: {args.ProgressPercentage} state: {args.UserState}");
          };
          client.DownloadFile(trackDownloadUrl, $"{folder}/{track.Title}.mp3");
        }

        Console.WriteLine("Done");
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }

      return false;
    }

    public StreamTrack ExtractStreamTrack(Track track)
    {
      var trackDownloadUrl = GetURLDownloadTrack(track);
      Console.WriteLine($"track download url: {trackDownloadUrl}");

      var isNetworing = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

      Console.WriteLine($"Get {track.Title} track. Networking: {isNetworing}");

      return StreamTrack.Open(trackDownloadUrl, track.FileSize);
    }

    public byte[] ExtractDataTrack(Track track)
    {
      var trackDownloadUrl = GetURLDownloadTrack(track);
      byte[] bytes;
      
      using (var client = new WebClient())
      {
        bytes = client.DownloadData(trackDownloadUrl);
      }

      return bytes;
    }

    public List<Track> SearchTrack(string trackName, int pageNumber = 0)
    {
      var tracks = Search(trackName, SearchType.Tracks, pageNumber).Select(x => (Track)x).ToList();

      return tracks;
    }

    public List<Artist> SearchArtist(string artistName, int pageNumber = 0)
    {
      var artists = Search(artistName, SearchType.Artists, pageNumber).Select(x => (Artist)x).ToList();

      return artists;
    }

    public List<Playlist> SearchPlaylist(string playlistName, int pageNumber = 0)
    {
      var playlists = Search(playlistName, SearchType.Playlists, pageNumber).Select(x => (Playlist)x).ToList();

      return playlists;
    }

    public List<Album> SearchAlbums(string albumName, int pageNumber = 0)
    {
      var albums = Search(albumName, SearchType.Albums, pageNumber).Select(x => (Album)x).ToList();

      return albums;
    }
    
    public List<User> SearchUsers(string userName, int pageNumber = 0)
    {
      var users = Search(userName, SearchType.Users, pageNumber).Select(x => (User)x).ToList();

      return users;
    }

    private List<ISearchable> Search(string searchText, SearchType searchType, int page = 0)
    {
      var listResult = new List<ISearchable>();
      var searchTypeAsString = searchType.ToString();
      var urlSearch = new StringBuilder();
      urlSearch.Append($"https://music.yandex.ru/handlers/music-search.jsx?text={searchText}");
      urlSearch.Append($"&type={searchTypeAsString}");
      urlSearch.Append(
        $"&page={page}&ncrnd=0.7060701951464323&lang=ru&external-domain=music.yandex.ru&overembed=false");

      var request = GetRequest(new Uri(urlSearch.ToString()), WebRequestMethods.Http.Get);

      using (var response = (HttpWebResponse) request.GetResponse())
      {
        var json = GetDataFromResponse(response);
        var fieldName = searchType.ToString().ToLower();
        var jArray = (JArray) json[fieldName]["items"];
        
        if (searchType == SearchType.Tracks)
        {
          listResult = Track.FromJsonArray(jArray).Select(x => (ISearchable) x).ToList();
        } 
        else if (searchType == SearchType.Artists)
        {
          listResult = Artist.FromJsonArray(jArray).Select(x => (ISearchable) x).ToList();
        }
        else if (searchType == SearchType.Albums)
        {
          listResult = Album.FromJsonArray(jArray).Select(x => (ISearchable) x).ToList();
        }
        else if (searchType == SearchType.Playlists)
        {
          listResult = Playlist.FromJsonArray(jArray).Select(x => (ISearchable) x).ToList();
        }
        else if (searchType == SearchType.Users)
        {
          listResult = User.FromJsonArray(jArray).Select(x => (ISearchable) x).ToList();
        }
      }

      return listResult;
    }

    protected Uri GetURLDownloadTrack(Track track)
    {
      var downloadInfo = GetDownloadTrackInfo(track.StorageDir);
      var key = "";//downloadInfo.Path.Substring(1, downloadInfo.Path.Length - 1) + downloadInfo.S;

      using (var md5 = MD5.Create())
      {
        key = GetMdHesh(md5, $"XGRlBW9FXlekgbPrRHuSiA{downloadInfo.Path.Substring(1, downloadInfo.Path.Length - 1)}{downloadInfo.S}");
      }

      var trackDownloadUrl =
        String.Format("http://{0}/get-mp3/{1}/{2}{3}?track-id={4}&region=225&from=service-search",
          downloadInfo.Host, 
          key, 
          downloadInfo.Ts,
          downloadInfo.Path,
          track.Id);

      return new Uri(trackDownloadUrl);
    }

    protected string GetMdHesh(MD5 md5, string str)
    {
      var data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
      var sBuilder = new StringBuilder();

      for (var i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }

      return sBuilder.ToString();
    }

    protected TrackDownloadInfo GetDownloadTrackInfo(string storageDir)
    {
      var fileName = GetDownloadTrackInfoFileName(storageDir);
      var request = GetRequest(new Uri($"http://storage.music.yandex.ru/download-info/{storageDir}/{fileName}"));
      var trackDownloadInfo = new TrackDownloadInfo();

      using (var response = (HttpWebResponse) request.GetResponse())
      {
        using (var stream = response.GetResponseStream())
        {
          var reader = new StreamReader(stream);
          var sourceText = reader.ReadToEnd();
          
          var xElem = XDocument.Parse(sourceText).Root;
          var elements = new Dictionary<string, string>();
          for (var x = (XElement) xElem.FirstNode; x != null; x = (XElement) x.NextNode)
          {
            elements.Add(x.Name.LocalName, x.Value);
          }
          _cookies.Add(response.Cookies);

          trackDownloadInfo.Host = elements["host"];
          trackDownloadInfo.Path = elements["path"];
          trackDownloadInfo.Ts = elements["ts"];
          trackDownloadInfo.Region = elements["region"];
          trackDownloadInfo.S = elements["s"];
        }
      }

      return trackDownloadInfo;
    }

    protected string GetDownloadTrackInfoFileName(string storageDir)
    {
      var request = GetRequest(new Uri($"http://storage.music.yandex.ru/get/{storageDir}/2.xml"), WebRequestMethods.Http.Get);
      var fileName = "";
      var trackLength = 0;
      
      using (var response = (HttpWebResponse) request.GetResponse())
      {
        using (var stream = response.GetResponseStream())
        {
          var reader = new StreamReader(stream);
          var sourceText = reader.ReadLine();
          sourceText = reader.ReadLine();

          var xElem = XDocument.Parse(sourceText).Root;
          var attrs = xElem.Attributes()
            .Where(a => !a.IsNamespaceDeclaration)
            .Select(a => new XAttribute(a.Name.LocalName, a.Value))
            .ToList();

          _cookies.Add(response.Cookies);
          fileName = attrs.First().Value;
          trackLength = int.Parse(attrs.Last().Value.ToString());
        }
      }

      return fileName;
    }

    protected JToken GetDataFromResponse(HttpWebResponse response)
    {
      var result = "";

      using (var stream = response.GetResponseStream())
      {
        var reader = new StreamReader(stream);

        result = reader.ReadToEnd();
      }
      
      return JToken.Parse(result);
    }
    
    protected virtual HttpWebRequest GetRequest(Uri uri, string method)
    {
      var request = HttpWebRequest.CreateHttp(uri);
      request.Method = method;
      if (_cookies == null)
      {
        _cookies = new CookieContainer();
      }

      request.CookieContainer = _cookies;
      request.KeepAlive = true;
      request.Headers[HttpRequestHeader.AcceptCharset] = Encoding.UTF8.WebName;
      request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
      request.AutomaticDecompression = DecompressionMethods.GZip;

      return request;
    }

    protected virtual HttpWebRequest GetRequest(Uri uri, params KeyValuePair<string, string>[] headers)
    {
      var request = GetRequest(uri, WebRequestMethods.Http.Post);
      var data = new StringBuilder(1024);
      
      for (var i = 0; i < headers.Length - 1; i++)
      {
        data.AppendFormat("{0}={1}&",
          HttpUtility.HtmlEncode(headers[i].Key),
          HttpUtility.HtmlEncode(headers[i].Value));
      }

      if (headers.Length > 0)
      {
        data.AppendFormat("{0}={1}",
          HttpUtility.HtmlEncode(headers[headers.Length - 1].Key),
          HttpUtility.HtmlEncode(headers[headers.Length - 1].Value));
      }

      var rawData = Encoding.UTF8.GetBytes(data.ToString());
      request.ContentLength = rawData.Length;
      request.GetRequestStream().Write(rawData, 0, rawData.Length);

      return request;
    }
  }
}