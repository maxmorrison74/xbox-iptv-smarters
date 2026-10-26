using System;
using System.Timers;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace XboxIptvSmarters.Services;

public class VideoPlayerService
{
    private MediaPlayer? _player;
    private System.Timers.Timer? _keepAliveTimer;
    
    public MediaPlayer Player => _player ??= CreateNewPlayer();
    
    private MediaPlayer CreateNewPlayer()
    {
        var player = new MediaPlayer
        {
            Source = null,
            IsLoopingEnabled = false,
            ShouldAutoplay = true,
            ImageSource = null
        };
        
        player.MediaFailed += (s, e) => {
            Console.WriteLine($"MediaPlayer error: {e.ErrorException?.Message}");
        };
        
        player.MediaOpened += (s, e) => {
            Console.WriteLine("Media aperto con successo");
        };
        
        player.MediaEnded += (s, e) => {
            Console.WriteLine("Media terminato");
        };
        
        SetupKeepAlive(player);
        
        return player;
    }

    public async Task<bool> PlayAsync(string url, string? title = null, ImageSource? thumbnail = null, bool loop = false)
    {
        try
        {
            var player = Player;
            playerShouldAutoplay = true;
            
            // Set source
            var source = MediaSource.CreateFromUri(new Uri(url));
            player.Source = source;
            player.ShouldAutoplay = true;
            
            // Setup image if provided
            if (thumbnail != null)
            {
                player.ImageSource = thumbnail;
            }
            
            player.Play();
            
            // Monitor start
            _ = Task.Run(async () => {
                await Task.Delay(5000);
                if (player.CurrentState == MediaPlayerState.Playing)
                {
                    Console.WriteLine("Playback avviato con successo");
                    return true;
                }
                Console.WriteLine("Playback fallito - stato: " + player.CurrentState);
                return false;
            });
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore PlayAsync: {ex.Message}");
            return false;
        }
    }

    public void Stop()
    {
        try
        {
            Player.Pause();
            Player.Source = null;
        }
        catch { }
    }

    public void Pause()
    {
        try { Player.Pause(); } catch { }
    }

    public void Resume()
    {
        try { Player.Play(); } catch { }
    }

    public bool IsPlaying => Player.CurrentState == MediaPlayerState.Playing;

    public void SetupKeepAlive(MediaPlayer player)
    {
        _keepAliveTimer?.Stop();
        _keepAliveTimer?.Dispose();
        
        _keepAliveTimer = new System.Timers.Timer(60000);
        _keepAliveTimer.Elapsed += (s, e) => {
            // Keep alive check - può servire per streaming live
            if (player.CurrentState == MediaPlayerState.Playing)
            {
                var position = player.Position;
                Console.WriteLine($"Playback live - posizione: {position}");
            }
        };
        _keepAliveTimer.AutoReset = true;
        _keepAliveTimer.Start();
    }

    public void Dispose()
    {
        Stop();
        _keepAliveTimer?.Stop();
        _keepAliveTimer?.Dispose();
        _player?.Dispose();
    }

    public event EventHandler<bool>? PlaybackStatusChanged;
}
