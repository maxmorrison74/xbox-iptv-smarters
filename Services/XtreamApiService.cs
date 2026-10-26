using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XboxIptvSmarters.Models;

namespace XboxIptvSmarters.Services;

public class XtreamApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public XtreamApiService(XtreamAuth auth)
    {
        _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _baseUrl = auth.ServerUrl.TrimEnd('/');
    }

    public async Task<UserInfoResponse?> GetUserAsync()
    {
        var separator = _baseUrl.Contains("?") ? "&" : "?";
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var doc = JsonDocument.Parse(response);
            var userDoc = doc.RootElement.GetProperty("user_info");
            var serverDoc = doc.RootElement.GetProperty("server_info");

            return new UserInfoResponse
            {
                UserInfo = new UserInfo
                {
                    Username = userDoc.GetProperty("username").GetString() ?? "",
                    Password = userDoc.GetProperty("password").GetString() ?? "",
                    Message = userDoc.GetProperty("message").GetString() ?? "",
                    Status = userDoc.GetProperty("status").GetString() ?? "",
                    ExpirationDate = userDoc.GetProperty("exp_date").GetString() ?? "",
                    LicensePlate = userDoc.GetProperty("license_plate").GetString() ?? "",
                    IsTrial = userDoc.GetProperty("is_trial").GetString() ?? "0",
                    ActiveCons = userDoc.GetProperty("active_cons").GetInt32(),
                    CreatedAt = userDoc.GetProperty("created_at").GetString() ?? "",
                    DisplayIcon = userDoc.GetProperty("display_icon").GetString() ?? "",
                    ServerUrl = userDoc.GetProperty("url").GetString() ?? ""
                },
                ServerInfo = new ServerInfo
                {
                    Url = serverDoc.GetProperty("url").GetString() ?? "",
                    Port = serverDoc.GetProperty("port").GetInt32(),
                    HttpsPort = serverDoc.GetProperty("https_port").GetInt32(),
                    ServerProtocol = serverDoc.GetProperty("server_protocol").GetString() ?? "",
                    RtmpPort = serverDoc.GetProperty("rtmp_port").GetInt32(),
                    Timezone = serverDoc.GetProperty("timezone").GetString() ?? "",
                    TimestampNow = serverDoc.GetProperty("timestamp_now").GetInt64(),
                    TimeNow = serverDoc.GetProperty("time_now").GetString() ?? ""
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore GetUserAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<List<LiveChannel>> GetLiveStreamsAsync()
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_live_streams";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var channels = new List<LiveChannel>();
            
            foreach (var el in arr.EnumerateArray())
            {
                channels.Add(new LiveChannel
                {
                    Num = el.GetProperty("num").GetInt32(),
                    Name = el.GetProperty("name").GetString() ?? "",
                    StreamType = el.GetProperty("stream_type").GetString() ?? "live",
                    StreamId = el.GetProperty("stream_id").GetInt32(),
                    StreamIcon = el.TryGetProperty("stream_icon", out var icon) ? icon.GetString() ?? "" : "",
                    EpgChannelId = el.TryGetProperty("epg_channel_id", out var epgId) ? epgId.GetString() : null,
                    Added = el.GetProperty("added").GetString() ?? "",
                    CategoryId = el.GetProperty("category_id").GetString() ?? "",
                    CustomSid = el.GetProperty("custom_sid").GetString() ?? "",
                    TvArchive = el.GetProperty("tv_archive").GetInt32(),
                    DirectSource = el.TryGetProperty("direct_source", out var ds) ? ds.GetString() ?? "" : "",
                    TvArchiveDuration = el.TryGetProperty("tv_archive_duration", out var ad) ? ad.GetInt32() : 0
                });
            }
            return channels;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore GetLiveStreamsAsync: {ex.Message}");
            return new List<LiveChannel>();
        }
    }

    public async Task<List<LiveChannel>> GetLiveStreamsByCategoryAsync(string categoryId)
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_live_streams&category_id={categoryId}";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var channels = new List<LiveChannel>();
            
            foreach (var el in arr.EnumerateArray())
            {
                channels.Add(new LiveChannel
                {
                    Num = el.GetProperty("num").GetInt32(),
                    Name = el.GetProperty("name").GetString() ?? "",
                    StreamId = el.GetProperty("stream_id").GetInt32(),
                    StreamIcon = el.TryGetProperty("stream_icon", out var icon) ? icon.GetString() ?? "" : "",
                    EpgChannelId = el.TryGetProperty("epg_channel_id", out var epgId) ? epgId.GetString() : null,
                    CategoryId = el.GetProperty("category_id").GetString() ?? ""
                });
            }
            return channels;
        }
        catch
        {
            return new List<LiveChannel>();
        }
    }

    public async Task<string> GetLiveStreamUrl(int streamId)
    {
        var separator = _baseUrl.Contains("?") ? "&" : "?";
        return $"{_baseUrl}/live/{Uri.EscapeDataString(App.CurrentAuth!.Username)}/{Uri.EscapeDataString(App.CurrentAuth.Password)}/{streamId}.m3u8";
    }

    public async Task<List<Category>> GetLiveCategoriesAsync()
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_live_categories";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var categories = new List<Category>();
            
            foreach (var el in arr.EnumerateArray())
            {
                categories.Add(new Category
                {
                    CategoryId = el.GetProperty("category_id").GetString() ?? "",
                    CategoryName = el.GetProperty("category_name").GetString() ?? "",
                    ParentId = el.GetProperty("parent_id").GetInt32()
                });
            }
            return categories;
        }
        catch
        {
            return new List<Category>();
        }
    }

    public async Task<List<VodItem>> GetVodStreamsAsync()
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_vod_streams";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var streams = new List<VodItem>();
            
            foreach (var el in arr.EnumerateArray())
            {
                var vod = new VodItem
                {
                    Num = el.GetProperty("num").GetInt32(),
                    Name = el.GetProperty("name").GetString() ?? "",
                    StreamType = el.GetProperty("stream_type").GetString() ?? "movie",
                    StreamId = el.GetProperty("stream_id").GetInt32(),
                    StreamIcon = el.TryGetProperty("stream_icon", out var icon) ? icon.GetString() ?? "" : "",
                    Rating = el.TryGetProperty("rating", out var r) ? r.GetString() ?? "" : "",
                    Rating5Based = el.TryGetProperty("rating_5based", out var r5) ? r5.GetDouble() : 0,
                    Added = el.GetProperty("added").GetString() ?? "",
                    CategoryId = el.GetProperty("category_id").GetString() ?? "",
                    ContainerExtension = el.TryGetProperty("container_extension", out var ce) ? ce.GetString() ?? "" : "mp4",
                    CustomSid = el.GetProperty("custom_sid").GetString() ?? "",
                    DirectSource = el.TryGetProperty("direct_source", out var ds) ? ds.GetString() ?? "" : ""
                };
                
                if (el.TryGetProperty("video_info", out var info))
                {
                    vod.VideoInfo = new VideoInfo
                    {
                        ReleaseDate = info.TryGetProperty("releaseDate", out var rd) ? rd.GetString() ?? "" : "",
                        DurationSec = info.TryGetProperty("durationSec", out var ds2) ? ds2.GetInt32() : 0,
                        Overview = info.TryGetProperty("overview", out var ov) ? ov.GetString() ?? "" : "",
                        Director = info.TryGetProperty("director", out var d) ? d.GetString() ?? "" : "",
                        Cast = info.TryGetProperty("cast", out var c) ? c.GetString() ?? "" : "",
                        YoutubeTrailer = info.TryGetProperty("youtube_trailer", out var yt) ? yt.GetString() ?? "" : "",
                        Plot = info.TryGetProperty("plot", out var p) ? p.GetString() ?? "" : "",
                        ReleaseDate2 = info.TryGetProperty("release_date", out var rd2) ? rd2.GetString() ?? "" : "",
                        Duration = info.TryGetProperty("duration", out var d2) ? d2.GetString() ?? "" : "",
                        ContainerExtension = info.TryGetProperty("container_extension", out var ce2) ? ce2.GetString() ?? "" : ""
                    };
                }
                streams.Add(vod);
            }
            return streams;
        }
        catch
        {
            return new List<VodItem>();
        }
    }

    public async Task<List<VodItem>> GetVodStreamsByCategoryAsync(string categoryId)
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_vod_streams&category_id={categoryId}";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var streams = new List<VodItem>();
            
            foreach (var el in arr.EnumerateArray())
            {
                streams.Add(new VodItem
                {
                    Num = el.GetProperty("num").GetInt32(),
                    Name = el.GetProperty("name").GetString() ?? "",
                    StreamId = el.GetProperty("stream_id").GetInt32(),
                    StreamIcon = el.TryGetProperty("stream_icon", out var icon) ? icon.GetString() ?? "" : "",
                    CategoryId = el.GetProperty("category_id").GetString() ?? ""
                });
            }
            return streams;
        }
        catch
        {
            return new List<VodItem>();
        }
    }

    public async Task<string> GetVodStreamUrl(int streamId)
    {
        var separator = _baseUrl.Contains("?") ? "&" : "?";
        return $"{_baseUrl}/movie/{Uri.EscapeDataString(App.CurrentAuth!.Username)}/{Uri.EscapeDataString(App.CurrentAuth.Password)}/{streamId}.mp4";
    }

    public async Task<List<SeriesData>> GetSeriesAsync()
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_series";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var seriesList = new List<SeriesData>();
            
            foreach (var el in arr.EnumerateArray())
            {
                var series = new SeriesData
                {
                    Name = el.GetProperty("name").GetString() ?? "",
                    SeriesId = el.GetProperty("series_id").GetInt32(),
                    Cover = el.TryGetProperty("cover", out var c) ? c.GetString() ?? "" : ""
                };
                
                var seasonsObj = el.TryGetProperty("seasons", out var so) ? so : null;
                var seasonsDict = new Dictionary<string, Season>();
                
                if (seasonsObj != null && seasonsObj.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in seasonsObj.EnumerateObject())
                    {
                        var season = prop.Value;
                        var seasonData = new Season
                        {
                            Title = season.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "",
                            Id = season.TryGetProperty("id", out var i) ? i.GetInt32() : 0,
                            Cover = season.TryGetProperty("cover", out var sC) ? sC.GetString() ?? "" : "",
                            Plot = season.TryGetProperty("plot", out var sP) ? sP.GetString() ?? "" : "",
                            SeasonNumber = season.TryGetProperty("season_number", out var sn) ? sn.GetInt32() : 0
                        };
                        
                        var episodes = new List<Episode>();
                        var epArray = season.TryGetProperty("episodes", out var ea) && ea.ValueKind == JsonValueKind.Array ? ea : null;
                        if (epArray != null)
                        {
                            foreach (var ep in epArray.EnumerateArray())
                            {
                                var episode = new Episode
                                {
                                    Id = ep.TryGetProperty("id", out var eId) ? eId.GetInt32() : 0,
                                    Name = ep.TryGetProperty("name", out var eN) ? eN.GetString() ?? "" : "",
                                    Title = ep.TryGetProperty("title", out var eT) ? eT.GetString() ?? "" : "",
                                    Cover = ep.TryGetProperty("cover", out var eC) ? eC.GetString() ?? "" : "",
                                    SeasonNumber = ep.TryGetProperty("season_number", out var esn) ? esn.GetInt32() : 0,
                                    EpisodeNumber = ep.TryGetProperty("episode_number", out var en) ? en.GetInt32() : 0,
                                    DurationSec = ep.TryGetProperty("duration_sec", out var d) ? d.GetInt32() : 0
                                };
                                
                                if (ep.TryGetProperty("info", out var eI))
                                {
                                    episode.Info = new EpisodeInfo
                                    {
                                        Plot = eI.TryGetProperty("plot", out var iP) ? iP.GetString() ?? "" : "",
                                        Added = eI.TryGetProperty("added", out var a) ? a.GetString() ?? "" : "",
                                        ReleaseDate = eI.TryGetProperty("releaseDate", out var rD) ? rD.GetString() ?? "" : "",
                                        Director = eI.TryGetProperty("director", out var d2) ? d2.GetString() ?? "" : "",
                                        Cast = eI.TryGetProperty("cast", out var c) ? c.GetString() ?? "" : "",
                                        Rating = eI.TryGetProperty("rating", out var g) ? g.GetString() ?? "" : ""
                                    };
                                }
                                episodes.Add(episode);
                            }
                        }
                        seasonData.Episodes = episodes;
                        seasonsDict[prop.Name] = seasonData;
                    }
                }
                series.Seasons = seasonsDict;
                seriesList.Add(series);
            }
            return seriesList;
        }
        catch
        {
            return new List<SeriesData>();
        }
    }

    public async Task<string> GetSeriesStreamUrl(int seasonNum, int episodeNum, int streamId)
    {
        return $"{_baseUrl}/series/{Uri.EscapeDataString(App.CurrentAuth!.Username)}/{Uri.EscapeDataString(App.CurrentAuth.Password)}/{streamId}/{seasonNum}.mp4";
    }

    public async Task<List<VodCategory>> GetVodCategoriesAsync()
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_vod_categories";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var categories = new List<VodCategory>();
            
            foreach (var el in arr.EnumerateArray())
            {
                categories.Add(new VodCategory
                {
                    CategoryId = el.GetProperty("category_id").GetString() ?? "",
                    CategoryName = el.GetProperty("category_name").GetString() ?? "",
                    ParentId = el.GetProperty("parent_id").GetInt32(),
                    CategoryType = el.TryGetProperty("category_type", out var ct) ? ct.GetString() ?? "movie" : "movie"
                });
            }
            return categories;
        }
        catch
        {
            return new List<VodCategory>();
        }
    }

    public async Task<List<SeriesData>> GetSeriesByCategoryAsync(string categoryId)
    {
        var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_series&category_id={categoryId}";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var arr = JsonDocument.Parse(response).RootElement;
            var seriesList = new List<SeriesData>();
            
            foreach (var el in arr.EnumerateArray())
            {
                seriesList.Add(new SeriesData
                {
                    Name = el.GetProperty("name").GetString() ?? "",
                    SeriesId = el.GetProperty("series_id").GetInt32(),
                    Cover = el.TryGetProperty("cover", out var c) ? c.GetString() ?? "" : ""
                });
            }
            return seriesList;
        }
        catch
        {
            return new List<SeriesData>();
        }
    }

    public async Task<Dictionary<string, List<EpgProgramme>>> GetShortEpgAsync(List<string> channelIds, int maxRecordsPerChannel = 10)
    {
        var result = new Dictionary<string, List<EpgProgramme>>();
        
        foreach (var channelId in channelIds)
        {
            var url = $"{_baseUrl}/player_api.php?username={Uri.EscapeDataString(App.CurrentAuth!.Username)}&password={Uri.EscapeDataString(App.CurrentAuth.Password)}&action=get_short_epg&channel_id={channelId}&limit=10";
            
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var arr = JsonDocument.Parse(response).RootElement.GetProperty("epg_list").EnumerateArray();
                var programmes = new List<EpgProgramme>();
                
                foreach (var el in arr)
                {
                    programmes.Add(new EpgProgramme
                    {
                        ChannelId = el.GetProperty("channel_id").GetString() ?? "",
                        Title = el.GetProperty("title").GetString() ?? "",
                        Description = el.GetProperty("description").GetString() ?? "",
                        Icon = el.TryGetProperty("icon", out var i) ? i.GetString() ?? "" : "",
                        Start = el.GetProperty("start").GetString() ?? "",
                        Stop = el.GetProperty("stop").GetString() ?? "",
                        Rating = el.TryGetProperty("rating", out var r) ? r.GetString() ?? "" : "",
                        SeriesId = el.TryGetProperty("series_id", out var sId) ? sId.GetString() ?? "" : "",
                        ProgramId = el.TryGetProperty("program_id", out var pId) ? pId.GetString() ?? "" : ""
                    });
                }
                result[channelId] = programmes;
            }
            catch
            {
                result[channelId] = new List<EpgProgramme>();
            }
        }
        return result;
    }

    public static bool ValidateCredentials(string serverUrl, string username, string password, out string errorMessage)
    {
        errorMessage = "";
        
        // Rimuovi slash finale
        serverUrl = serverUrl.TrimEnd('/');
        
        if (string.IsNullOrEmpty(serverUrl) || !serverUrl.Contains("://"))
        {
            errorMessage = "URL del server non valido. Formato: http://o https://";
            return false;
        }
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorMessage = "Username e password sono obbligatori";
            return false;
        }
        
        return true;
    }
}
