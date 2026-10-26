using System.Text.Json.Serialization;

namespace XboxIptvSmarters.Models;

public class XtreamAuth
{
    public string ServerUrl { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string? UserId { get; set; }
    public string? Status { get; set; }
    public string? ExpirationDate { get; set; }
    public double? TotalBandwidth { get; set; }
}

public class UserInfo
{
    [JsonPropertyName("auth")] public int Auth { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; } = "";
    [JsonPropertyName("password")] public string Password { get; set; } = "";
    [JsonPropertyName("message")] public string Message { get; set; } = "";
    [JsonPropertyName("status")] public string Status { get; set; } = "";
    [JsonPropertyName("exp_date")] public string ExpirationDate { get; set; } = "";
    [JsonPropertyName("license_plate")] public string LicensePlate { get; set; } = "";
    [JsonPropertyName("is_trial")] public string IsTrial { get; set; } = "0";
    [JsonPropertyName("active_cons")] public int ActiveCons { get; set; }
    [JsonPropertyName("created_at")] public string CreatedAt { get; set; } = "";
    [JsonPropertyName("display_icon")] public string DisplayIcon { get; set; } = "";
    [JsonPropertyName("url")] public string ServerUrl { get; set; } = "";
}

public class UserInfoResponse
{
    [JsonPropertyName("user_info")] public UserInfo UserInfo { get; set; } = new();
    [JsonPropertyName("server_info")] public ServerInfo? ServerInfo { get; set; }
}

public class ServerInfo
{
    [JsonPropertyName("url")] public string Url { get; set; } = "";
    [JsonPropertyName("port")] public int Port { get; set; }
    [JsonPropertyName("https_port")] public int HttpsPort { get; set; }
    [JsonPropertyName("server_protocol")] public string ServerProtocol { get; set; } = "";
    [JsonPropertyName("rtmp_port")] public int RtmpPort { get; set; }
    [JsonPropertyName("timezone")] public string Timezone { get; set; } = "";
    [JsonPropertyName("timestamp_now")] public long TimestampNow { get; set; }
    [JsonPropertyName("time_now")] public string TimeNow { get; set; } = "";
}

public class Category
{
    [JsonPropertyName("category_id")] public string CategoryId { get; set; } = "";
    [JsonPropertyName("category_name")] public string CategoryName { get; set; } = "";
    [JsonPropertyName("parent_id")] public int ParentId { get; set; }
}

public class LiveChannel
{
    [JsonPropertyName("num")] public int Num { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("stream_type")] public string StreamType { get; set; } = "live";
    [JsonPropertyName("stream_id")] public int StreamId { get; set; }
    [JsonPropertyName("stream_icon")] public string StreamIcon { get; set; } = "";
    [JsonPropertyName("epg_channel_id")] public string? EpgChannelId { get; set; }
    [JsonPropertyName("added")] public string Added { get; set; } = "";
    [JsonPropertyName("category_id")] public string CategoryId { get; set; } = "";
    [JsonPropertyName("custom_sid")] public string CustomSid { get; set; } = "";
    [JsonPropertyName("tv_archive")] public int TvArchive { get; set; }
    [JsonPropertyName("direct_source")] public string DirectSource { get; set; } = "";
    [JsonPropertyName("tv_archive_duration")] public int TvArchiveDuration { get; set; }
}

public class EpgProgramme
{
    [JsonPropertyName("channel_id")] public string ChannelId { get; set; } = "";
    [JsonPropertyName("title")] public string Title { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("icon")] public string Icon { get; set; } = "";
    [JsonPropertyName("start")] public string Start { get; set; } = "";
    [JsonPropertyName("stop")] public string Stop { get; set; } = "";
    [JsonPropertyName("rating")] public string Rating { get; set; } = "";
    [JsonPropertyName("series_id")] public string SeriesId { get; set; } = "";
    [JsonPropertyName("program_id")] public string ProgramId { get; set; } = "";
}

public class EpgResponse
{
    [JsonPropertyName("epg_list")] public List<EpgProgramme> EpgList { get; set; } = new();
}

public class VodCategory
{
    [JsonPropertyName("category_id")] public string CategoryId { get; set; } = "";
    [JsonPropertyName("category_name")] public string CategoryName { get; set; } = "";
    [JsonPropertyName("parent_id")] public int ParentId { get; set; }
    [JsonPropertyName("category_type")] public string CategoryType { get; set; } = "movie";
}

public class VodItem
{
    [JsonPropertyName("num")] public int Num { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("stream_type")] public string StreamType { get; set; } = "movie" | "series";
    [JsonPropertyName("stream_id")] public int StreamId { get; set; }
    [JsonPropertyName("stream_icon")] public string StreamIcon { get; set; } = "";
    [JsonPropertyName("rating")] public string Rating { get; set; } = "";
    [JsonPropertyName("rating_5based")] public double Rating5Based { get; set; }
    [JsonPropertyName("added")] public string Added { get; set; } = "";
    [JsonPropertyName("category_id")] public string CategoryId { get; set; } = "";
    [JsonPropertyName("container_extension")] public string ContainerExtension { get; set; } = "";
    [JsonPropertyName("custom_sid")] public string CustomSid { get; set; } = "";
    [JsonPropertyName("direct_source")] public string DirectSource { get; set; } = "";
    [JsonPropertyName("video_info")] public VideoInfo? VideoInfo { get; set; }
    [JsonPropertyName("series_info")] public SeriesInfo? SeriesInfo { get; set; }
}

public class VideoInfo
{
    [JsonPropertyName("releaseDate")] public string ReleaseDate { get; set; } = "";
    [JsonPropertyName("durationSec")] public int DurationSec { get; set; }
    [JsonPropertyName("overview")] public string Overview { get; set; } = "";
    [JsonPropertyName("director")] public string Director { get; set; } = "";
    [JsonPropertyName("cast")] public string Cast { get; set; } = "";
    [JsonPropertyName("youtube_trailer")] public string YoutubeTrailer { get; set; } = "";
    [JsonPropertyName("plot")] public string Plot { get; set; } = "";
    [JsonPropertyName("release_date")] public string ReleaseDate2 { get; set; } = "";
    [JsonPropertyName("duration")] public string Duration { get; set; } = "";
    [JsonPropertyName("container_extension")] public string ContainerExtension { get; set; } = "";
}

public class SeriesInfo
{
    [JsonPropertyName("last_episode")] public LastEpisode? LastEpisode { get; set; }
    [JsonPropertyName("releaseDate")] public string ReleaseDate { get; set; } = "";
    [JsonPropertyName("episode_run_time")] public string EpisodeRunTime { get; set; } = "";
    [JsonPropertyName("num_of_episodes")] public int NumOfEpisodes { get; set; }
    [JsonPropertyName("num_of_seasons")] public int NumOfSeasons { get; set; }
    [JsonPropertyName("overview")] public string Overview { get; set; } = "";
    [JsonPropertyName("cover")] public string Cover { get; set; } = "";
    [JsonPropertyName("series_id")] public int SeriesId { get; set; }
}

public class LastEpisode
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("season_num")] public int SeasonNum { get; set; }
    [JsonPropertyName("episode_num")] public int EpisodeNum { get; set; }
    [JsonPropertyName("added")] public string Added { get; set; } = "";
    [JsonPropertyName("category_id")] public string CategoryId { get; set; } = "";
    [JsonPropertyName("container_extension")] public string ContainerExtension { get; set; } = "";
    [JsonPropertyName("info")] public LastEpisodeInfo Info { get; set; } = new();
}

public class LastEpisodeInfo
{
    [JsonPropertyName("plot")] public string Plot { get; set; } = "";
    [JsonPropertyName("youtube_trailer")] public string YoutubeTrailer { get; set; } = "";
    [JsonPropertyName("episode_num")] public string EpisodeNum { get; set; } = "";
    [JsonPropertyName("added")] public string Added { get; set; } = "";
    [JsonPropertyName("direct_source")] public string DirectSource { get; set; } = "";
    [JsonPropertyName("title")] public string Title { get; set; } = "";
}

public class SeriesData
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("series_id")] public int SeriesId { get; set; }
    [JsonPropertyName("cover")] public string Cover { get; set; } = "";
    [JsonPropertyName("seasons")] public Dictionary<string, Season>? Seasons { get; set; }
    [JsonPropertyName("info")] public SeriesInfo Info { get; set; } = new();
}

public class Season
{
    [JsonPropertyName("title")] public string Title { get; set; } = "";
    [JsonPropertyName("episodes")] public List<Episode> Episodes { get; set; } = new();
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("cover")] public string Cover { get; set; } = "";
    [JsonPropertyName("plot")] public string Plot { get; set; } = "";
    [JsonPropertyName("season_number")] public int SeasonNumber { get; set; }
}

public class Episode
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("title")] public string Title { get; set; } = "";
    [JsonPropertyName("cover")] public string Cover { get; set; } = "";
    [JsonPropertyName("season_number")] public int SeasonNumber { get; set; }
    [JsonPropertyName("episode_number")] public int EpisodeNumber { get; set; }
    [JsonPropertyName("duration_sec")] public int DurationSec { get; set; }
    [JsonPropertyName("info")] public EpisodeInfo Info { get; set; } = new();
}

public class EpisodeInfo
{
    [JsonPropertyName("plot")] public string Plot { get; set; } = "";
    [JsonPropertyName("added")] public string Added { get; set; } = "";
    [JsonPropertyName("releaseDate")] public string ReleaseDate { get; set; } = "";
    [JsonPropertyName("director")] public string Director { get; set; } = "";
    [JsonPropertyName("cast")] public string Cast { get; set; } = "";
    [JsonPropertyName("rating")] public string Rating { get; set; } = "";
    [JsonPropertyName("video_info")] public VideoInfo? VideoInfo { get; set; }
}
