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
  public class LofiYandexMusicApi
  {
    private string _login;
    private string _password;
    private CookieContainer _cookies;
    
    public LofiYandexMusicApi(string login, string password)
    {
      _login = login;
      _password = password;
      
      NetworkChange.NetworkAvailabilityChanged += (sender, args) =>
      {
        Console.WriteLine($"Networking: {args.IsAvailable}");
      };
    }

    public bool Authorize()
    {
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
            throw new Exception("Error auth");
          }
        }
      }
      catch (Exception ex)
      {
        result = false;
        throw new Exception("Fuck-fuck-fuck");
      }

      return result;
    }

    public List<Track> GetListFavorites()
    {
      var request = GetRequest(new Uri($"https://music.yandex.ru/handlers/library.jsx?owner={_login}&filter=tracks&likeFilter=favorite&sort=&dir=&lang=ru&external-domain=music.yandex.ru&overembed=false&ncrnd=0.7506943983987266"));
      var tracks = new List<Track>();
      
      using (var response = (HttpWebResponse) request.GetResponse())
      {
        var data = GetDataFromResponse(response);
        var jTracks = (JArray) data["tracks"];

        for (var i = 0; i < jTracks.Count; i++)
        {
          var jTrack = jTracks[i];
          var track = new Track
          {
            Id = jTrack["id"].ToObject<string>(),
            RealId = jTrack["realId"].ToObject<string>(),
            Title = jTrack["title"].ToObject<string>(),
            Major = new Major
            {
              Id = jTrack["major"]["id"].ToObject<string>(),
              Name = jTrack["major"]["name"].ToObject<string>()
            },
            Available = jTrack["available"].ToObject<bool>(),
            AvailableForPremiumUsers = jTrack["availableForPremiumUsers"].ToObject<bool>(),
            
            DurationMS = jTrack["durationMs"].ToObject<int>(),
            StorageDir = jTrack["storageDir"].ToObject<string>(),
            FileSize = jTrack["fileSize"].ToObject<int>(),
            Artists = jTrack["artists"].ToObject<JArray>()
              .Select(jArtist => new Artist
              {
                Id = jArtist["id"].ToObject<string>(),
                Name = jArtist["name"].ToObject<string>(),
                Various = jArtist["various"].ToObject<bool>(),
                Composer = jArtist["composer"].ToObject<bool>(),
                Cover = new Cover
                {
                  Type = jArtist["cover"]["type"].ToObject<string>(),
                  Prefix = jArtist["cover"]["prefix"].ToObject<string>(),
                  Url = jArtist["cover"]["uri"].ToObject<string>()
                },
                Genres = new string[] {}
              }).ToList(),
            OgImage = jTrack["ogImage"].ToObject<string>()
          };

          tracks.Add(track);
        }

        _cookies.Add(response.Cookies);
      }

      return tracks;
    }

    public void DownloadTrack(Track track)
    {
      var trackDownloadUrl = GetURLDownloadTrack(track);
      var isNetworing = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
      
      using (var client = new WebClient())
      {
        client.DownloadProgressChanged += (sender, args) =>
        {
          Console.WriteLine($"received: {args.BytesReceived} from: {args.TotalBytesToReceive} progress: {args.ProgressPercentage} state: {args.UserState}");
        };
        client.DownloadFile(trackDownloadUrl, $"data/{track.Title}.mp3");
      }
      Console.WriteLine("Done");
    }

    public StreamTrack GetTrackStream(Track track)
    {
      var trackDownloadUrl = GetURLDownloadTrack(track);
      Console.WriteLine($"track download url: {trackDownloadUrl}");

      var isNetworing = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

      Console.WriteLine($"Get {track.Title} track. Networking: {isNetworing}");

      return StreamTrack.Open(trackDownloadUrl, track.FileSize);
    }

    public byte[] GetDataTrack(Track track)
    {
      var trackDownloadUrl = GetURLDownloadTrack(track);
      byte[] bytes;
      
      using (var client = new WebClient())
      {
        bytes = client.DownloadData(trackDownloadUrl);
      }

      return bytes;
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